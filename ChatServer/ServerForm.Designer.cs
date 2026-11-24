namespace ChatServerApp;

partial class ServerForm
{
    private System.ComponentModel.IContainer components = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        lblPort = new Label();
        nudPort = new NumericUpDown();
        btnStart = new Button();
        btnStop = new Button();
        lblStatus = new Label();
        lblAddresses = new Label();
        grpClients = new GroupBox();
        lvClients = new ListView();
        colUser = new ColumnHeader();
        colAddress = new ColumnHeader();
        colTime = new ColumnHeader();
        colCount = new ColumnHeader();
        grpLog = new GroupBox();
        lstLog = new ListBox();
        btnSaveLog = new Button();
        btnClearLog = new Button();
        ((System.ComponentModel.ISupportInitialize)nudPort).BeginInit();
        grpClients.SuspendLayout();
        grpLog.SuspendLayout();
        SuspendLayout();
        //
        // lblPort
        //
        lblPort.AutoSize = true;
        lblPort.Location = new Point(14, 21);
        lblPort.Name = "lblPort";
        lblPort.Text = "Port:";
        //
        // nudPort
        //
        nudPort.Location = new Point(52, 17);
        nudPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
        nudPort.Minimum = new decimal(new int[] { 1024, 0, 0, 0 });
        nudPort.Name = "nudPort";
        nudPort.Size = new Size(80, 23);
        nudPort.Value = new decimal(new int[] { 5000, 0, 0, 0 });
        //
        // btnStart
        //
        btnStart.BackColor = Color.FromArgb(46, 125, 50);
        btnStart.FlatStyle = FlatStyle.Flat;
        btnStart.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
        btnStart.ForeColor = Color.White;
        btnStart.Location = new Point(146, 11);
        btnStart.Name = "btnStart";
        btnStart.Size = new Size(165, 34);
        btnStart.Text = "▶ Sunucuyu Başlat";
        btnStart.UseVisualStyleBackColor = false;
        btnStart.Click += BtnStart_Click;
        //
        // btnStop
        //
        btnStop.BackColor = Color.FromArgb(198, 40, 40);
        btnStop.FlatStyle = FlatStyle.Flat;
        btnStop.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
        btnStop.ForeColor = Color.White;
        btnStop.Location = new Point(317, 11);
        btnStop.Name = "btnStop";
        btnStop.Size = new Size(165, 34);
        btnStop.Text = "■ Sunucuyu Durdur";
        btnStop.UseVisualStyleBackColor = false;
        btnStop.Click += BtnStop_Click;
        //
        // lblStatus
        //
        lblStatus.BackColor = Color.DimGray;
        lblStatus.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
        lblStatus.ForeColor = Color.White;
        lblStatus.Location = new Point(490, 11);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(316, 34);
        lblStatus.Text = "● Sunucu kapalı";
        lblStatus.TextAlign = ContentAlignment.MiddleCenter;
        //
        // lblAddresses
        //
        lblAddresses.ForeColor = Color.DimGray;
        lblAddresses.Location = new Point(14, 52);
        lblAddresses.Name = "lblAddresses";
        lblAddresses.Size = new Size(792, 20);
        lblAddresses.Text = "Bu bilgisayarın IP adresleri:";
        //
        // grpClients
        //
        grpClients.Controls.Add(lvClients);
        grpClients.Location = new Point(14, 78);
        grpClients.Name = "grpClients";
        grpClients.Size = new Size(792, 180);
        grpClients.Text = "Bağlı İstemciler (0)";
        //
        // lvClients
        //
        lvClients.Columns.AddRange(new ColumnHeader[] { colUser, colAddress, colTime, colCount });
        lvClients.FullRowSelect = true;
        lvClients.GridLines = true;
        lvClients.Location = new Point(10, 22);
        lvClients.Name = "lvClients";
        lvClients.Size = new Size(770, 148);
        lvClients.View = View.Details;
        //
        // columns
        //
        colUser.Text = "Kullanıcı Adı";
        colUser.Width = 200;
        colAddress.Text = "IP Adresi : Port";
        colAddress.Width = 220;
        colTime.Text = "Bağlanma Zamanı";
        colTime.Width = 200;
        colCount.Text = "Mesaj Sayısı";
        colCount.Width = 120;
        //
        // grpLog
        //
        grpLog.Controls.Add(lstLog);
        grpLog.Location = new Point(14, 266);
        grpLog.Name = "grpLog";
        grpLog.Size = new Size(792, 238);
        grpLog.Text = "Sunucu Günlüğü";
        //
        // lstLog
        //
        lstLog.Font = new Font("Consolas", 9F);
        lstLog.HorizontalScrollbar = true;
        lstLog.IntegralHeight = false;
        lstLog.Location = new Point(10, 22);
        lstLog.Name = "lstLog";
        lstLog.Size = new Size(770, 206);
        //
        // btnSaveLog
        //
        btnSaveLog.Location = new Point(14, 512);
        btnSaveLog.Name = "btnSaveLog";
        btnSaveLog.Size = new Size(160, 32);
        btnSaveLog.Text = "Günlüğü .txt Kaydet";
        btnSaveLog.Click += BtnSaveLog_Click;
        //
        // btnClearLog
        //
        btnClearLog.Location = new Point(180, 512);
        btnClearLog.Name = "btnClearLog";
        btnClearLog.Size = new Size(130, 32);
        btnClearLog.Text = "Günlüğü Temizle";
        btnClearLog.Click += BtnClearLog_Click;
        //
        // ServerForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(820, 556);
        Controls.Add(lblPort);
        Controls.Add(nudPort);
        Controls.Add(btnStart);
        Controls.Add(btnStop);
        Controls.Add(lblStatus);
        Controls.Add(lblAddresses);
        Controls.Add(grpClients);
        Controls.Add(grpLog);
        Controls.Add(btnSaveLog);
        Controls.Add(btnClearLog);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "ServerForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "YMH219 - Chat Sunucusu";
        FormClosing += ServerForm_FormClosing;
        ((System.ComponentModel.ISupportInitialize)nudPort).EndInit();
        grpClients.ResumeLayout(false);
        grpLog.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblPort;
    private NumericUpDown nudPort;
    private Button btnStart;
    private Button btnStop;
    private Label lblStatus;
    private Label lblAddresses;
    private GroupBox grpClients;
    private ListView lvClients;
    private ColumnHeader colUser;
    private ColumnHeader colAddress;
    private ColumnHeader colTime;
    private ColumnHeader colCount;
    private GroupBox grpLog;
    private ListBox lstLog;
    private Button btnSaveLog;
    private Button btnClearLog;
}
