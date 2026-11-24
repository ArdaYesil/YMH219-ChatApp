using ChatCommon;

namespace ChatClientApp;

/// <summary>
/// İstemci penceresi: bağlanma, mesaj gönderme/alma, çevrimiçi kullanıcılar ve geçmiş.
/// Ağ işlerini ChatClient sınıfı yapar; form yalnızca onun metotlarını ve olaylarını kullanır.
/// </summary>
public partial class ClientForm : Form
{
    private const string EveryoneTarget = "Herkese";

    private static readonly Color OwnColor = Color.FromArgb(25, 118, 210);
    private static readonly Color OtherColor = Color.FromArgb(33, 33, 33);
    private static readonly Color PrivateColor = Color.FromArgb(142, 36, 170);
    private static readonly Color SystemColor = Color.Gray;
    private static readonly Color ErrorColor = Color.Firebrick;

    private readonly ChatClient _client = new();

    public ClientForm()
    {
        InitializeComponent();

        _client.MessageReceived += (s, msg) => RunOnUi(() => HandleMessage(msg));
        _client.ConnectionLost += (s, reason) => RunOnUi(() => HandleConnectionLost(reason));

        UpdateUserList(Array.Empty<string>());
        SetConnectedUi(false);
        ActiveControl = txtUsername;
    }

    // ------------------------------------------------------------------
    // Bağlan / Bağlantıyı Kes
    // ------------------------------------------------------------------

    private async void BtnConnect_Click(object? sender, EventArgs e)
    {
        string host = txtHost.Text.Trim();
        string username = txtUsername.Text.Trim();

        // Önce yerelde kontrol: hatalı girişi sunucuya hiç göndermeyelim
        string? error = string.IsNullOrEmpty(host)
            ? "Sunucu IP adresi boş olamaz."
            : ChatProtocol.ValidateUsername(username);
        if (error != null)
        {
            MessageBox.Show(error, "Eksik / hatalı bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        SetStatus($"● Bağlanıyor: {host}:{nudPort.Value} ...", Color.DarkGoldenrod);
        grpConnection.Enabled = false;

        try
        {
            await _client.ConnectAsync(host, (int)nudPort.Value, username);
            AppendSystem($"{host}:{nudPort.Value} sunucusuna '{username}' olarak bağlandınız.");
            SetConnectedUi(true);
            txtMessage.Focus();
        }
        catch (ChatConnectionException ex)
        {
            SetConnectedUi(false);
            AppendLine($"[{DateTime.Now:HH:mm:ss}] Bağlantı hatası: {ex.Message}", ErrorColor, FontStyle.Bold);
            MessageBox.Show(ex.Message, "Bağlanılamadı", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            grpConnection.Enabled = true;
        }
    }

    private async void BtnDisconnect_Click(object? sender, EventArgs e)
    {
        await _client.DisconnectAsync();
        AppendSystem("Bağlantıyı kestiniz.");
        UpdateUserList(Array.Empty<string>());
        SetConnectedUi(false);
    }

    private void HandleConnectionLost(string reason)
    {
        AppendLine($"[{DateTime.Now:HH:mm:ss}] {reason}", ErrorColor, FontStyle.Bold);
        UpdateUserList(Array.Empty<string>());
        SetConnectedUi(false);
        MessageBox.Show(reason, "Bağlantı koptu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    // ------------------------------------------------------------------
    // Mesaj gönderme / alma
    // ------------------------------------------------------------------

    private async void BtnSend_Click(object? sender, EventArgs e)
    {
        string text = ChatProtocol.NormalizeText(txtMessage.Text);
        if (text.Length == 0 || !_client.IsConnected) return;

        string target = cmbTarget.SelectedItem as string ?? EveryoneTarget;

        try
        {
            if (target == EveryoneTarget)
            {
                await _client.SendChatAsync(text);
                AppendChat(DateTime.Now, "Ben", text, OwnColor);
            }
            else
            {
                await _client.SendPrivateAsync(target, text);
                AppendChat(DateTime.Now, $"Ben → {target} (özel)", text, PrivateColor);
            }

            txtMessage.Clear();
        }
        catch (ChatConnectionException ex)
        {
            AppendLine($"[{DateTime.Now:HH:mm:ss}] {ex.Message}", ErrorColor, FontStyle.Bold);
        }

        txtMessage.Focus();
    }

    /// <summary>Sunucudan gelen mesajı türüne göre ekrana yansıtır.</summary>
    private void HandleMessage(ChatMessage msg)
    {
        switch (msg.Type)
        {
            case MessageType.Chat:
                AppendChat(msg.Time, msg.From, msg.Text, OtherColor);
                break;
            case MessageType.Private:
                AppendChat(msg.Time, $"{msg.From} → Ben (özel)", msg.Text, PrivateColor);
                break;
            case MessageType.System:
                AppendSystem(msg.Text, msg.Time);
                break;
            case MessageType.UserList:
                UpdateUserList(msg.Users);
                break;
        }
    }

    private void LstUsers_DoubleClick(object? sender, EventArgs e)
    {
        if (lstUsers.SelectedItem is string user && user != _client.Username)
        {
            cmbTarget.SelectedItem = user;
            txtMessage.Focus();
        }
    }

    // ------------------------------------------------------------------
    // Geçmiş
    // ------------------------------------------------------------------

    private void BtnSaveHistory_Click(object? sender, EventArgs e)
    {
        if (rtbMessages.TextLength == 0)
        {
            MessageBox.Show("Kaydedilecek mesaj yok.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialog = new SaveFileDialog
        {
            Filter = "Metin dosyası (*.txt)|*.txt",
            FileName = $"sohbet_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            string title = $"YMH219 - Chat Mesaj Geçmişi ({(_client.IsConnected ? _client.Username : "bağlı değil")})";
            HistoryFileService.Save(dialog.FileName, title, rtbMessages.Lines.Where(l => l.Length > 0));
            MessageBox.Show("Mesaj geçmişi kaydedildi:\n" + dialog.FileName, "Kaydedildi",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show("Dosya kaydedilemedi:\n" + ex.Message, "Hata",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnClearHistory_Click(object? sender, EventArgs e) => rtbMessages.Clear();

    private async void ClientForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (!_client.IsConnected) return;

        // Önce sunucuya "ayrılıyorum" de, sonra kapan
        e.Cancel = true;
        Enabled = false;
        await _client.DisconnectAsync();
        Close();
    }

    // ------------------------------------------------------------------
    // Arayüz yardımcıları
    // ------------------------------------------------------------------

    private void SetConnectedUi(bool connected)
    {
        txtHost.Enabled = !connected;
        nudPort.Enabled = !connected;
        txtUsername.Enabled = !connected;
        btnConnect.Enabled = !connected;
        btnDisconnect.Enabled = connected;
        btnConnect.BackColor = connected ? Color.Gray : Color.FromArgb(46, 125, 50);
        btnDisconnect.BackColor = connected ? Color.FromArgb(198, 40, 40) : Color.Gray;

        txtMessage.Enabled = connected;
        btnSend.Enabled = connected;
        cmbTarget.Enabled = connected;

        // Enter tuşu: bağlı değilken "Bağlan", bağlıyken "Mesaj Gönder" butonunu tetikler
        AcceptButton = connected ? btnSend : btnConnect;

        if (connected)
        {
            SetStatus($"● Bağlı: {_client.Username} @ {txtHost.Text}:{nudPort.Value}", Color.FromArgb(46, 125, 50));
            Text = $"YMH219 - Chat İstemcisi — {_client.Username}";
        }
        else
        {
            SetStatus("● Bağlı değil", Color.DimGray);
            Text = "YMH219 - Chat İstemcisi";
        }
    }

    private void SetStatus(string text, Color color)
    {
        lblStatus.Text = text;
        lblStatus.BackColor = color;
    }

    private void UpdateUserList(IReadOnlyCollection<string> users)
    {
        string? previousTarget = cmbTarget.SelectedItem as string;

        lstUsers.BeginUpdate();
        lstUsers.Items.Clear();
        foreach (string user in users)
        {
            lstUsers.Items.Add(user);
        }
        lstUsers.EndUpdate();
        grpUsers.Text = $"Çevrimiçi ({users.Count})";

        // Alıcı listesi: "Herkese" + kendisi dışındaki kullanıcılar
        cmbTarget.BeginUpdate();
        cmbTarget.Items.Clear();
        cmbTarget.Items.Add(EveryoneTarget);
        foreach (string user in users.Where(u => u != _client.Username))
        {
            cmbTarget.Items.Add(user);
        }
        cmbTarget.EndUpdate();

        // Önceki alıcı hâlâ çevrimiçiyse seçili kalsın, değilse "Herkese"
        cmbTarget.SelectedItem = previousTarget != null && cmbTarget.Items.Contains(previousTarget)
            ? previousTarget
            : EveryoneTarget;
    }

    private void AppendChat(DateTime time, string from, string text, Color color)
    {
        AppendText($"[{time:HH:mm:ss}] ", Color.Gray, FontStyle.Regular);
        AppendText($"{from}: ", color, FontStyle.Bold);
        AppendLine(text, color, FontStyle.Regular);
    }

    private void AppendSystem(string text, DateTime? time = null) =>
        AppendLine($"[{time ?? DateTime.Now:HH:mm:ss}] *** {text}", SystemColor, FontStyle.Italic);

    private void AppendLine(string text, Color color, FontStyle style) =>
        AppendText(text + Environment.NewLine, color, style);

    /// <summary>RichTextBox'a renkli/stilli metin ekler ve en alta kaydırır.</summary>
    private void AppendText(string text, Color color, FontStyle style)
    {
        rtbMessages.SelectionStart = rtbMessages.TextLength;
        rtbMessages.SelectionLength = 0;
        rtbMessages.SelectionColor = color;
        rtbMessages.SelectionFont = new Font(rtbMessages.Font, style);
        rtbMessages.AppendText(text);
        rtbMessages.ScrollToCaret();
    }

    /// <summary>
    /// Ağ olayları arka plan iş parçacığından gelebilir; kontroller yalnızca
    /// UI iş parçacığından güncellenebildiği için gerekirse BeginInvoke kullanılır.
    /// </summary>
    private void RunOnUi(Action action)
    {
        if (IsDisposed) return;
        if (InvokeRequired) BeginInvoke(action);
        else action();
    }
}
