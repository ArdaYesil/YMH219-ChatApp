using System.Net;
using System.Net.Sockets;
using ChatCommon;

namespace ChatServerApp;

/// <summary>
/// Sunucu penceresi: sunucuyu başlatır/durdurur, bağlı istemcileri ve günlüğü gösterir.
/// Ağ işlerini ChatServer sınıfı yapar; form yalnızca olaylarını dinler.
/// </summary>
public partial class ServerForm : Form
{
    private readonly ChatServer _server = new();

    public ServerForm()
    {
        InitializeComponent();

        _server.LogMessage += (s, text) => RunOnUi(() => AddLog(text));
        _server.ClientsChanged += (s, e) => RunOnUi(RefreshClientList);

        lblAddresses.Text = "Bu bilgisayarın IP adresleri: " + string.Join("  ·  ", GetLocalAddresses());
        UpdateUi();
        ActiveControl = btnStart;
    }

    // ------------------------------------------------------------------
    // Butonlar
    // ------------------------------------------------------------------

    private void BtnStart_Click(object? sender, EventArgs e)
    {
        try
        {
            _server.Start((int)nudPort.Value);
        }
        catch (SocketException ex)
        {
            string reason = ex.SocketErrorCode == SocketError.AddressAlreadyInUse
                ? $"{nudPort.Value} portu başka bir uygulama tarafından kullanılıyor. Farklı bir port deneyin."
                : ex.Message;

            AddLog("HATA: Sunucu başlatılamadı. " + reason);
            MessageBox.Show(reason, "Sunucu başlatılamadı", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        UpdateUi();
    }

    private async void BtnStop_Click(object? sender, EventArgs e)
    {
        btnStop.Enabled = false;
        await _server.StopAsync();
        UpdateUi();
        RefreshClientList();
    }

    private void BtnSaveLog_Click(object? sender, EventArgs e)
    {
        if (lstLog.Items.Count == 0)
        {
            MessageBox.Show("Günlükte kaydedilecek satır yok.", "Bilgi",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialog = new SaveFileDialog
        {
            Filter = "Metin dosyası (*.txt)|*.txt",
            FileName = $"sunucu_gunlugu_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            HistoryFileService.Save(dialog.FileName, "YMH219 - Chat Sunucusu Günlüğü",
                lstLog.Items.Cast<string>());
            MessageBox.Show("Günlük kaydedildi:\n" + dialog.FileName, "Kaydedildi",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show("Dosya kaydedilemedi:\n" + ex.Message, "Hata",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnClearLog_Click(object? sender, EventArgs e) => lstLog.Items.Clear();

    private async void ServerForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (!_server.IsRunning) return;

        // Önce istemcilere "Sunucu kapatıldı" mesajı gitsin, sonra pencere kapansın
        e.Cancel = true;
        Enabled = false;
        await _server.StopAsync();
        Close();
    }

    // ------------------------------------------------------------------
    // Arayüz güncelleme
    // ------------------------------------------------------------------

    private void UpdateUi()
    {
        bool running = _server.IsRunning;

        btnStart.Enabled = !running;
        btnStop.Enabled = running;
        nudPort.Enabled = !running;
        btnStart.BackColor = running ? Color.Gray : Color.FromArgb(46, 125, 50);
        btnStop.BackColor = running ? Color.FromArgb(198, 40, 40) : Color.Gray;

        lblStatus.Text = running ? $"● Sunucu çalışıyor (port {_server.Port})" : "● Sunucu kapalı";
        lblStatus.BackColor = running ? Color.FromArgb(46, 125, 50) : Color.DimGray;
    }

    /// <summary>Bağlı istemci tablosunu baştan doldurur.</summary>
    private void RefreshClientList()
    {
        var clients = _server.GetClients();

        lvClients.BeginUpdate();
        lvClients.Items.Clear();
        foreach (var client in clients)
        {
            var item = new ListViewItem(client.Username);
            item.SubItems.Add(client.EndPoint);
            item.SubItems.Add(client.ConnectedAt.ToString("dd.MM.yyyy HH:mm:ss"));
            item.SubItems.Add(client.MessageCount.ToString());
            lvClients.Items.Add(item);
        }
        lvClients.EndUpdate();

        grpClients.Text = $"Bağlı İstemciler ({clients.Count})";
    }

    private void AddLog(string text)
    {
        lstLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] {text}");
        lstLog.TopIndex = lstLog.Items.Count - 1;
    }

    /// <summary>
    /// Sunucu olayları arka plan iş parçacığından gelebilir. Kontroller yalnızca
    /// kendi (UI) iş parçacığından güncellenebildiği için gerekirse BeginInvoke kullanılır.
    /// </summary>
    private void RunOnUi(Action action)
    {
        if (IsDisposed) return;
        if (InvokeRequired) BeginInvoke(action);
        else action();
    }

    private static IEnumerable<string> GetLocalAddresses()
    {
        var addresses = new List<string> { "127.0.0.1 (aynı bilgisayar)" };
        try
        {
            addresses.AddRange(Dns.GetHostAddresses(Dns.GetHostName())
                .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                .Select(a => a + " (yerel ağ)"));
        }
        catch (SocketException)
        {
            // Ağ bilgisi alınamazsa yalnızca 127.0.0.1 gösterilir
        }
        return addresses;
    }
}
