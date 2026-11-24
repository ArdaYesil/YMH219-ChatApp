namespace ChatClientApp;

partial class ClientForm
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
        grpConnection = new GroupBox();
        lblHost = new Label();
        txtHost = new TextBox();
        lblPort = new Label();
        nudPort = new NumericUpDown();
        lblUsername = new Label();
        txtUsername = new TextBox();
        btnConnect = new Button();
        btnDisconnect = new Button();
        lblStatus = new Label();
        rtbMessages = new RichTextBox();
        grpUsers = new GroupBox();
        lstUsers = new ListBox();
        lblUsersHint = new Label();
        lblTarget = new Label();
        cmbTarget = new ComboBox();
        txtMessage = new TextBox();
        btnSend = new Button();
        btnSaveHistory = new Button();
        btnClearHistory = new Button();
        grpConnection.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)nudPort).BeginInit();
        grpUsers.SuspendLayout();
        SuspendLayout();
        //
        // grpConnection
        //
        grpConnection.Controls.Add(lblHost);
        grpConnection.Controls.Add(txtHost);
        grpConnection.Controls.Add(lblPort);
        grpConnection.Controls.Add(nudPort);
        grpConnection.Controls.Add(lblUsername);
        grpConnection.Controls.Add(txtUsername);
        grpConnection.Controls.Add(btnConnect);
        grpConnection.Controls.Add(btnDisconnect);
        grpConnection.Location = new Point(12, 8);
        grpConnection.Name = "grpConnection";
        grpConnection.Size = new Size(796, 62);
        grpConnection.Text = "Bağlantı";
        //
        // lblHost
        //
        lblHost.AutoSize = true;
        lblHost.Location = new Point(10, 28);
        lblHost.Name = "lblHost";
        lblHost.Text = "Sunucu IP:";
        //
        // txtHost
        //
        txtHost.Location = new Point(78, 24);
        txtHost.Name = "txtHost";
        txtHost.Size = new Size(120, 23);
        txtHost.Text = "127.0.0.1";
        //
        // lblPort
        //
        lblPort.AutoSize = true;
        lblPort.Location = new Point(210, 28);
        lblPort.Name = "lblPort";
        lblPort.Text = "Port:";
        //
        // nudPort
        //
        nudPort.Location = new Point(246, 24);
        nudPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
        nudPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        nudPort.Name = "nudPort";
        nudPort.Size = new Size(70, 23);
        nudPort.Value = new decimal(new int[] { 5000, 0, 0, 0 });
        //
        // lblUsername
        //
        lblUsername.AutoSize = true;
        lblUsername.Location = new Point(328, 28);
        lblUsername.Name = "lblUsername";
        lblUsername.Text = "Kullanıcı adı:";
        //
        // txtUsername
        //
        txtUsername.Location = new Point(410, 24);
        txtUsername.MaxLength = 20;
        txtUsername.Name = "txtUsername";
        txtUsername.Size = new Size(130, 23);
        //
        // btnConnect
        //
        btnConnect.BackColor = Color.FromArgb(46, 125, 50);
        btnConnect.FlatStyle = FlatStyle.Flat;
        btnConnect.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnConnect.ForeColor = Color.White;
        btnConnect.Location = new Point(552, 20);
        btnConnect.Name = "btnConnect";
        btnConnect.Size = new Size(110, 30);
        btnConnect.Text = "Bağlan";
        btnConnect.UseVisualStyleBackColor = false;
        btnConnect.Click += BtnConnect_Click;
        //
        // btnDisconnect
        //
        btnDisconnect.BackColor = Color.FromArgb(198, 40, 40);
        btnDisconnect.FlatStyle = FlatStyle.Flat;
        btnDisconnect.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnDisconnect.ForeColor = Color.White;
        btnDisconnect.Location = new Point(668, 20);
        btnDisconnect.Name = "btnDisconnect";
        btnDisconnect.Size = new Size(118, 30);
        btnDisconnect.Text = "Bağlantıyı Kes";
        btnDisconnect.UseVisualStyleBackColor = false;
        btnDisconnect.Click += BtnDisconnect_Click;
        //
        // lblStatus
        //
        lblStatus.BackColor = Color.DimGray;
        lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblStatus.ForeColor = Color.White;
        lblStatus.Location = new Point(12, 76);
        lblStatus.Name = "lblStatus";
        lblStatus.Padding = new Padding(8, 0, 0, 0);
        lblStatus.Size = new Size(796, 26);
        lblStatus.Text = "● Bağlı değil";
        lblStatus.TextAlign = ContentAlignment.MiddleLeft;
        //
        // rtbMessages
        //
        rtbMessages.BackColor = Color.White;
        rtbMessages.Font = new Font("Segoe UI", 10F);
        rtbMessages.Location = new Point(12, 110);
        rtbMessages.Name = "rtbMessages";
        rtbMessages.ReadOnly = true;
        rtbMessages.Size = new Size(586, 370);
        rtbMessages.TabStop = false;
        rtbMessages.Text = "";
        //
        // grpUsers
        //
        grpUsers.Controls.Add(lstUsers);
        grpUsers.Controls.Add(lblUsersHint);
        grpUsers.Location = new Point(608, 104);
        grpUsers.Name = "grpUsers";
        grpUsers.Size = new Size(200, 376);
        grpUsers.Text = "Çevrimiçi (0)";
        //
        // lstUsers
        //
        lstUsers.Font = new Font("Segoe UI", 10F);
        lstUsers.IntegralHeight = false;
        lstUsers.Location = new Point(10, 22);
        lstUsers.Name = "lstUsers";
        lstUsers.Size = new Size(180, 304);
        lstUsers.DoubleClick += LstUsers_DoubleClick;
        //
        // lblUsersHint
        //
        lblUsersHint.ForeColor = Color.DimGray;
        lblUsersHint.Location = new Point(10, 330);
        lblUsersHint.Name = "lblUsersHint";
        lblUsersHint.Size = new Size(180, 40);
        lblUsersHint.Text = "Özel mesaj için kullanıcıya çift tıklayın.";
        //
        // lblTarget
        //
        lblTarget.AutoSize = true;
        lblTarget.Location = new Point(12, 494);
        lblTarget.Name = "lblTarget";
        lblTarget.Text = "Alıcı:";
        //
        // cmbTarget
        //
        cmbTarget.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbTarget.Location = new Point(52, 490);
        cmbTarget.Name = "cmbTarget";
        cmbTarget.Size = new Size(140, 23);
        //
        // txtMessage
        //
        txtMessage.Font = new Font("Segoe UI", 10F);
        txtMessage.Location = new Point(200, 489);
        txtMessage.MaxLength = 500;
        txtMessage.Name = "txtMessage";
        txtMessage.PlaceholderText = "Mesajınızı yazın ve Enter'a basın...";
        txtMessage.Size = new Size(490, 25);
        //
        // btnSend
        //
        btnSend.BackColor = Color.FromArgb(25, 118, 210);
        btnSend.FlatStyle = FlatStyle.Flat;
        btnSend.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnSend.ForeColor = Color.White;
        btnSend.Location = new Point(698, 486);
        btnSend.Name = "btnSend";
        btnSend.Size = new Size(110, 30);
        btnSend.Text = "Mesaj Gönder";
        btnSend.UseVisualStyleBackColor = false;
        btnSend.Click += BtnSend_Click;
        //
        // btnSaveHistory
        //
        btnSaveHistory.Location = new Point(12, 526);
        btnSaveHistory.Name = "btnSaveHistory";
        btnSaveHistory.Size = new Size(170, 30);
        btnSaveHistory.Text = "Mesaj Geçmişini Kaydet";
        btnSaveHistory.Click += BtnSaveHistory_Click;
        //
        // btnClearHistory
        //
        btnClearHistory.Location = new Point(188, 526);
        btnClearHistory.Name = "btnClearHistory";
        btnClearHistory.Size = new Size(120, 30);
        btnClearHistory.Text = "Ekranı Temizle";
        btnClearHistory.Click += BtnClearHistory_Click;
        //
        // ClientForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(820, 566);
        Controls.Add(grpConnection);
        Controls.Add(lblStatus);
        Controls.Add(rtbMessages);
        Controls.Add(grpUsers);
        Controls.Add(lblTarget);
        Controls.Add(cmbTarget);
        Controls.Add(txtMessage);
        Controls.Add(btnSend);
        Controls.Add(btnSaveHistory);
        Controls.Add(btnClearHistory);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "ClientForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "YMH219 - Chat İstemcisi";
        FormClosing += ClientForm_FormClosing;
        grpConnection.ResumeLayout(false);
        grpConnection.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)nudPort).EndInit();
        grpUsers.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private GroupBox grpConnection;
    private Label lblHost;
    private TextBox txtHost;
    private Label lblPort;
    private NumericUpDown nudPort;
    private Label lblUsername;
    private TextBox txtUsername;
    private Button btnConnect;
    private Button btnDisconnect;
    private Label lblStatus;
    private RichTextBox rtbMessages;
    private GroupBox grpUsers;
    private ListBox lstUsers;
    private Label lblUsersHint;
    private Label lblTarget;
    private ComboBox cmbTarget;
    private TextBox txtMessage;
    private Button btnSend;
    private Button btnSaveHistory;
    private Button btnClearHistory;
}
