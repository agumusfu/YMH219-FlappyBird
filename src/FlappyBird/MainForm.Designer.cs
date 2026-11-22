namespace FlappyBird;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.Timer gameTimer;
    private System.Windows.Forms.Panel topPanel;
    private System.Windows.Forms.Button btnNewGame;
    private System.Windows.Forms.Button btnPause;
    private System.Windows.Forms.ComboBox cmbDifficulty;
    private System.Windows.Forms.Label lblDifficulty;
    private System.Windows.Forms.CheckBox chkSound;
    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.ToolStripStatusLabel lblStatusInfo;
    private System.Windows.Forms.ToolStripStatusLabel lblCurrentScore;
    private System.Windows.Forms.ToolStripStatusLabel lblHighScore;
    private GameCanvas gameCanvas;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        gameTimer = new System.Windows.Forms.Timer(components);
        topPanel = new System.Windows.Forms.Panel();
        btnNewGame = new System.Windows.Forms.Button();
        btnPause = new System.Windows.Forms.Button();
        lblDifficulty = new System.Windows.Forms.Label();
        cmbDifficulty = new System.Windows.Forms.ComboBox();
        chkSound = new System.Windows.Forms.CheckBox();
        statusStrip = new System.Windows.Forms.StatusStrip();
        lblStatusInfo = new System.Windows.Forms.ToolStripStatusLabel();
        lblCurrentScore = new System.Windows.Forms.ToolStripStatusLabel();
        lblHighScore = new System.Windows.Forms.ToolStripStatusLabel();
        gameCanvas = new GameCanvas();
        topPanel.SuspendLayout();
        statusStrip.SuspendLayout();
        SuspendLayout();
        // 
        // gameTimer
        // 
        gameTimer.Interval = 20;
        gameTimer.Tick += GameTimer_Tick;
        // 
        // topPanel
        // 
        topPanel.BackColor = System.Drawing.Color.FromArgb(45, 52, 54);
        topPanel.Controls.Add(btnNewGame);
        topPanel.Controls.Add(btnPause);
        topPanel.Controls.Add(lblDifficulty);
        topPanel.Controls.Add(cmbDifficulty);
        topPanel.Controls.Add(chkSound);
        topPanel.Dock = System.Windows.Forms.DockStyle.Top;
        topPanel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        topPanel.ForeColor = System.Drawing.Color.White;
        topPanel.Location = new System.Drawing.Point(0, 0);
        topPanel.Name = "topPanel";
        topPanel.Padding = new System.Windows.Forms.Padding(6);
        topPanel.Size = new System.Drawing.Size(500, 48);
        topPanel.TabIndex = 0;
        // 
        // btnNewGame
        // 
        btnNewGame.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
        btnNewGame.Cursor = System.Windows.Forms.Cursors.Hand;
        btnNewGame.FlatAppearance.BorderSize = 0;
        btnNewGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnNewGame.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        btnNewGame.ForeColor = System.Drawing.Color.White;
        btnNewGame.Location = new System.Drawing.Point(8, 8);
        btnNewGame.Name = "btnNewGame";
        btnNewGame.Size = new System.Drawing.Size(95, 32);
        btnNewGame.TabIndex = 0;
        btnNewGame.TabStop = false;
        btnNewGame.Text = "Yeni Oyun (R)";
        btnNewGame.UseVisualStyleBackColor = false;
        btnNewGame.Click += BtnNewGame_Click;
        // 
        // btnPause
        // 
        btnPause.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
        btnPause.Cursor = System.Windows.Forms.Cursors.Hand;
        btnPause.FlatAppearance.BorderSize = 0;
        btnPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnPause.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        btnPause.ForeColor = System.Drawing.Color.White;
        btnPause.Location = new System.Drawing.Point(109, 8);
        btnPause.Name = "btnPause";
        btnPause.Size = new System.Drawing.Size(90, 32);
        btnPause.TabIndex = 1;
        btnPause.TabStop = false;
        btnPause.Text = "Duraklat (P)";
        btnPause.UseVisualStyleBackColor = false;
        btnPause.Click += BtnPause_Click;
        // 
        // lblDifficulty
        // 
        lblDifficulty.AutoSize = true;
        lblDifficulty.ForeColor = System.Drawing.Color.FromArgb(223, 230, 233);
        lblDifficulty.Location = new System.Drawing.Point(210, 16);
        lblDifficulty.Name = "lblDifficulty";
        lblDifficulty.Size = new System.Drawing.Size(46, 15);
        lblDifficulty.TabIndex = 2;
        lblDifficulty.Text = "Zorluk:";
        // 
        // cmbDifficulty
        // 
        cmbDifficulty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        cmbDifficulty.FormattingEnabled = true;
        cmbDifficulty.Items.AddRange(new object[] { "Kolay", "Normal", "Zor" });
        cmbDifficulty.Location = new System.Drawing.Point(260, 12);
        cmbDifficulty.Name = "cmbDifficulty";
        cmbDifficulty.Size = new System.Drawing.Size(90, 23);
        cmbDifficulty.TabIndex = 3;
        cmbDifficulty.TabStop = false;
        cmbDifficulty.SelectedIndexChanged += CmbDifficulty_SelectedIndexChanged;
        // 
        // chkSound
        // 
        chkSound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        chkSound.AutoSize = true;
        chkSound.Checked = true;
        chkSound.CheckState = System.Windows.Forms.CheckState.Checked;
        chkSound.Cursor = System.Windows.Forms.Cursors.Hand;
        chkSound.ForeColor = System.Drawing.Color.FromArgb(223, 230, 233);
        chkSound.Location = new System.Drawing.Point(405, 15);
        chkSound.Name = "chkSound";
        chkSound.Size = new System.Drawing.Size(77, 19);
        chkSound.TabIndex = 4;
        chkSound.TabStop = false;
        chkSound.Text = "Ses Açık";
        chkSound.UseVisualStyleBackColor = true;
        chkSound.CheckedChanged += ChkSound_CheckedChanged;
        // 
        // statusStrip
        // 
        statusStrip.BackColor = System.Drawing.Color.FromArgb(45, 52, 54);
        statusStrip.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            lblStatusInfo,
            lblCurrentScore,
            lblHighScore
        });
        statusStrip.Location = new System.Drawing.Point(0, 620);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new System.Drawing.Size(500, 24);
        statusStrip.TabIndex = 2;
        // 
        // lblStatusInfo
        // 
        lblStatusInfo.ForeColor = System.Drawing.Color.FromArgb(223, 230, 233);
        lblStatusInfo.Name = "lblStatusInfo";
        lblStatusInfo.Size = new System.Drawing.Size(185, 19);
        lblStatusInfo.Spring = true;
        lblStatusInfo.Text = "Boşluk: Zıpla | P: Duraklat | R: Sıfırla";
        lblStatusInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // lblCurrentScore
        // 
        lblCurrentScore.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        lblCurrentScore.ForeColor = System.Drawing.Color.FromArgb(241, 196, 15);
        lblCurrentScore.Name = "lblCurrentScore";
        lblCurrentScore.Size = new System.Drawing.Size(58, 19);
        lblCurrentScore.Text = "Skor: 0";
        // 
        // lblHighScore
        // 
        lblHighScore.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        lblHighScore.ForeColor = System.Drawing.Color.FromArgb(46, 204, 113);
        lblHighScore.Margin = new System.Windows.Forms.Padding(12, 3, 10, 2);
        lblHighScore.Name = "lblHighScore";
        lblHighScore.Size = new System.Drawing.Size(89, 19);
        lblHighScore.Text = "En Yüksek: 0";
        // 
        // gameCanvas
        // 
        gameCanvas.BackColor = System.Drawing.Color.FromArgb(112, 197, 206);
        gameCanvas.Cursor = System.Windows.Forms.Cursors.Hand;
        gameCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
        gameCanvas.Location = new System.Drawing.Point(0, 48);
        gameCanvas.Name = "gameCanvas";
        gameCanvas.Size = new System.Drawing.Size(500, 572);
        gameCanvas.TabIndex = 1;
        gameCanvas.Paint += GameCanvas_Paint;
        gameCanvas.MouseDown += GameCanvas_MouseDown;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(500, 644);
        Controls.Add(gameCanvas);
        Controls.Add(statusStrip);
        Controls.Add(topPanel);
        DoubleBuffered = true;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        KeyPreview = true;
        MaximizeBox = false;
        Name = "MainForm";
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        Text = "YMH219 - Flappy Bird Masaüstü Oyunu";
        FormClosing += MainForm_FormClosing;
        KeyDown += MainForm_KeyDown;
        topPanel.ResumeLayout(false);
        topPanel.PerformLayout();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
