#nullable enable


namespace CampusMatch.Client.Forms;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        pnlNav = new Panel();
        btnDiscover = new Button();
        btnMatches = new Button();
        btnProfile = new Button();
        btnSettings = new Button();
        btnLogout = new Button();
        pnlContent = new Panel();
        pnlNav.SuspendLayout();
        SuspendLayout();
        // 
        // pnlNav
        // 
        pnlNav.BackColor = Color.FromArgb(20, 17, 55);
        pnlNav.Controls.Add(btnDiscover);
        pnlNav.Controls.Add(btnMatches);
        pnlNav.Controls.Add(btnProfile);
        pnlNav.Controls.Add(btnSettings);
        pnlNav.Controls.Add(btnLogout);
        pnlNav.Dock = DockStyle.Left;
        pnlNav.Location = new Point(0, 0);
        pnlNav.Name = "pnlNav";
        pnlNav.Size = new Size(80, 850);
        pnlNav.TabIndex = 0;
        // 
        // btnDiscover
        // 
        btnDiscover.BackColor = Color.FromArgb(124, 58, 237);
        btnDiscover.Cursor = Cursors.Hand;
        btnDiscover.FlatAppearance.BorderSize = 0;
        btnDiscover.FlatAppearance.MouseOverBackColor = Color.FromArgb(139, 92, 246);
        btnDiscover.FlatStyle = FlatStyle.Flat;
        btnDiscover.Font = new Font("Segoe UI Emoji", 24F);
        btnDiscover.ForeColor = Color.White;
        btnDiscover.Location = new Point(10, 21);
        btnDiscover.Name = "btnDiscover";
        btnDiscover.Size = new Size(60, 60);
        btnDiscover.TabIndex = 0;
        btnDiscover.Text = "🔥";
        btnDiscover.UseVisualStyleBackColor = false;
        btnDiscover.Click += BtnDiscover_Click;
        btnDiscover.Paint += BtnNav_Paint;
        // 
        // btnMatches
        // 
        btnMatches.BackColor = Color.Transparent;
        btnMatches.Cursor = Cursors.Hand;
        btnMatches.FlatAppearance.BorderSize = 0;
        btnMatches.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 40, 90);
        btnMatches.FlatStyle = FlatStyle.Flat;
        btnMatches.Font = new Font("Segoe UI Emoji", 24F);
        btnMatches.ForeColor = Color.White;
        btnMatches.Location = new Point(10, 100);
        btnMatches.Name = "btnMatches";
        btnMatches.Size = new Size(60, 60);
        btnMatches.TabIndex = 1;
        btnMatches.Text = "💬";
        btnMatches.UseVisualStyleBackColor = false;
        btnMatches.Click += BtnMatches_Click;
        btnMatches.Paint += BtnNav_Paint;
        // 
        // btnProfile
        // 
        btnProfile.BackColor = Color.Transparent;
        btnProfile.Cursor = Cursors.Hand;
        btnProfile.FlatAppearance.BorderSize = 0;
        btnProfile.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 40, 90);
        btnProfile.FlatStyle = FlatStyle.Flat;
        btnProfile.Font = new Font("Segoe UI Emoji", 24F);
        btnProfile.ForeColor = Color.White;
        btnProfile.Location = new Point(10, 180);
        btnProfile.Name = "btnProfile";
        btnProfile.Size = new Size(60, 60);
        btnProfile.TabIndex = 2;
        btnProfile.Text = "👤";
        btnProfile.UseVisualStyleBackColor = false;
        btnProfile.Click += BtnProfile_Click;
        btnProfile.Paint += BtnNav_Paint;
        // 
        // btnSettings
        // 
        btnSettings.BackColor = Color.Transparent;
        btnSettings.Cursor = Cursors.Hand;
        btnSettings.FlatAppearance.BorderSize = 0;
        btnSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 40, 90);
        btnSettings.FlatStyle = FlatStyle.Flat;
        btnSettings.Font = new Font("Segoe UI Emoji", 24F);
        btnSettings.ForeColor = Color.White;
        btnSettings.Location = new Point(10, 260);
        btnSettings.Name = "btnSettings";
        btnSettings.Size = new Size(60, 60);
        btnSettings.TabIndex = 3;
        btnSettings.Text = "⚙️";
        btnSettings.UseVisualStyleBackColor = false;
        btnSettings.Click += BtnSettings_Click;
        btnSettings.Paint += BtnNav_Paint;
        // 
        // btnLogout
        // 
        btnLogout.BackColor = Color.Transparent;
        btnLogout.Cursor = Cursors.Hand;
        btnLogout.FlatAppearance.BorderSize = 0;
        btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(127, 29, 29);
        btnLogout.FlatStyle = FlatStyle.Flat;
        btnLogout.Font = new Font("Segoe UI Emoji", 24F);
        btnLogout.ForeColor = Color.White;
        btnLogout.Location = new Point(10, 770);
        btnLogout.Name = "btnLogout";
        btnLogout.Size = new Size(60, 60);
        btnLogout.TabIndex = 4;
        btnLogout.Text = "🚪";
        btnLogout.UseVisualStyleBackColor = false;
        btnLogout.Click += BtnLogout_Click;
        btnLogout.Paint += BtnNav_Paint;
        // 
        // pnlContent
        // 
        pnlContent.AutoScroll = true;
        pnlContent.BackColor = Color.FromArgb(30, 27, 75);
        pnlContent.Dock = DockStyle.Fill;
        pnlContent.Location = new Point(80, 0);
        pnlContent.Name = "pnlContent";
        pnlContent.Padding = new Padding(20);
        pnlContent.Size = new Size(804, 850);
        pnlContent.TabIndex = 1;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(30, 27, 75);
        ClientSize = new Size(884, 850);
        Controls.Add(pnlContent);
        Controls.Add(pnlNav);
        DoubleBuffered = true;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "CampusMatch - Dashboard";
        pnlNav.ResumeLayout(false);
        ResumeLayout(false);
    }

    private void BtnNav_Paint(object? sender, PaintEventArgs e)
    {
        // Create rounded corners for navigation buttons
        if (sender is Button btn)
        {
            var radius = 12;
            var rect = new Rectangle(0, 0, btn.Width, btn.Height);
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            btn.Region = new Region(path);
        }
    }

    #endregion

    private System.Windows.Forms.Panel pnlNav;
    private System.Windows.Forms.Button btnDiscover;
    private System.Windows.Forms.Button btnMatches;
    private System.Windows.Forms.Button btnProfile;
    private System.Windows.Forms.Button btnSettings;
    private System.Windows.Forms.Button btnLogout;
    private System.Windows.Forms.Panel pnlContent;
}
