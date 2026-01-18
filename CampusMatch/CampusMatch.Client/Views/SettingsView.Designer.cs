#nullable enable

using CampusMatch.Client.Helpers;
using System.Drawing.Drawing2D;

namespace CampusMatch.Client.Views;

partial class SettingsView
{
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        pnlHeader = new Panel();
        lblTitle = new Label();
        pnlDiscovery = new Panel();
        lblDiscoveryTitle = new Label();
        lblAgeRange = new Label();
        numMinAge = new NumericUpDown();
        lblTo = new Label();
        numMaxAge = new NumericUpDown();
        pnlNotifications = new Panel();
        lblNotifTitle = new Label();
        chkNotifyMatches = new CheckBox();
        chkNotifyMessages = new CheckBox();
        pnlPrivacy = new Panel();
        lblPrivacyTitle = new Label();
        chkShowOnline = new CheckBox();
        chkShowDistance = new CheckBox();
        btnSaveSettings = new Button();
        lblStatus = new Label();
        pnlAccount = new Panel();
        lblAccountTitle = new Label();
        btnLogout = new Button();
        btnDeleteAccount = new Button();
        pnlLinks = new Panel();
        lnkPrivacy = new LinkLabel();
        lnkTerms = new LinkLabel();
        lblVersion = new Label();
        pnlHeader.SuspendLayout();
        pnlDiscovery.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numMinAge).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numMaxAge).BeginInit();
        pnlNotifications.SuspendLayout();
        pnlPrivacy.SuspendLayout();
        pnlAccount.SuspendLayout();
        pnlLinks.SuspendLayout();
        SuspendLayout();
        // 
        // pnlHeader
        // 
        pnlHeader.BackColor = Color.FromArgb(35, 30, 70);
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Margin = new Padding(3, 4, 3, 4);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(629, 80);
        pnlHeader.TabIndex = 0;
        pnlHeader.Paint += PnlHeader_Paint;
        // 
        // lblTitle
        // 
        lblTitle.BackColor = Color.Transparent;
        lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(17, 16);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(229, 53);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "⚙️ Settings";
        // 
        // pnlDiscovery
        // 
        pnlDiscovery.BackColor = Color.Transparent;
        pnlDiscovery.Controls.Add(lblDiscoveryTitle);
        pnlDiscovery.Controls.Add(lblAgeRange);
        pnlDiscovery.Controls.Add(numMinAge);
        pnlDiscovery.Controls.Add(lblTo);
        pnlDiscovery.Controls.Add(numMaxAge);
        pnlDiscovery.Location = new Point(17, 93);
        pnlDiscovery.Margin = new Padding(3, 4, 3, 4);
        pnlDiscovery.Name = "pnlDiscovery";
        pnlDiscovery.Size = new Size(594, 133);
        pnlDiscovery.TabIndex = 1;
        pnlDiscovery.Paint += PnlSection_Paint;
        // 
        // lblDiscoveryTitle
        // 
        lblDiscoveryTitle.BackColor = Color.Transparent;
        lblDiscoveryTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblDiscoveryTitle.ForeColor = Color.White;
        lblDiscoveryTitle.Location = new Point(17, 13);
        lblDiscoveryTitle.Name = "lblDiscoveryTitle";
        lblDiscoveryTitle.Size = new Size(229, 33);
        lblDiscoveryTitle.TabIndex = 0;
        lblDiscoveryTitle.Text = "Discovery Settings";
        // 
        // lblAgeRange
        // 
        lblAgeRange.BackColor = Color.Transparent;
        lblAgeRange.Font = new Font("Segoe UI", 10F);
        lblAgeRange.ForeColor = Color.White;
        lblAgeRange.Location = new Point(22, 65);
        lblAgeRange.Name = "lblAgeRange";
        lblAgeRange.Size = new Size(74, 27);
        lblAgeRange.TabIndex = 1;
        lblAgeRange.Text = "Age Range";
        // 
        // numMinAge
        // 
        numMinAge.BackColor = Color.FromArgb(60, 55, 105);
        numMinAge.ForeColor = Color.White;
        numMinAge.Location = new Point(102, 65);
        numMinAge.Margin = new Padding(3, 4, 3, 4);
        numMinAge.Minimum = new decimal(new int[] { 18, 0, 0, 0 });
        numMinAge.Name = "numMinAge";
        numMinAge.Size = new Size(69, 27);
        numMinAge.TabIndex = 2;
        numMinAge.Value = new decimal(new int[] { 18, 0, 0, 0 });
        // 
        // lblTo
        // 
        lblTo.BackColor = Color.Transparent;
        lblTo.Font = new Font("Segoe UI", 10F);
        lblTo.ForeColor = Color.White;
        lblTo.Location = new Point(184, 65);
        lblTo.Name = "lblTo";
        lblTo.Size = new Size(29, 27);
        lblTo.TabIndex = 3;
        lblTo.Text = "to";
        // 
        // numMaxAge
        // 
        numMaxAge.BackColor = Color.FromArgb(60, 55, 105);
        numMaxAge.ForeColor = Color.White;
        numMaxAge.Location = new Point(229, 65);
        numMaxAge.Margin = new Padding(3, 4, 3, 4);
        numMaxAge.Minimum = new decimal(new int[] { 18, 0, 0, 0 });
        numMaxAge.Name = "numMaxAge";
        numMaxAge.Size = new Size(69, 27);
        numMaxAge.TabIndex = 4;
        numMaxAge.Value = new decimal(new int[] { 30, 0, 0, 0 });
        // 
        // pnlNotifications
        // 
        pnlNotifications.BackColor = Color.Transparent;
        pnlNotifications.Controls.Add(lblNotifTitle);
        pnlNotifications.Controls.Add(chkNotifyMatches);
        pnlNotifications.Controls.Add(chkNotifyMessages);
        pnlNotifications.Location = new Point(17, 240);
        pnlNotifications.Margin = new Padding(3, 4, 3, 4);
        pnlNotifications.Name = "pnlNotifications";
        pnlNotifications.Size = new Size(594, 113);
        pnlNotifications.TabIndex = 2;
        pnlNotifications.Paint += PnlSection_Paint;
        // 
        // lblNotifTitle
        // 
        lblNotifTitle.BackColor = Color.Transparent;
        lblNotifTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblNotifTitle.ForeColor = Color.White;
        lblNotifTitle.Location = new Point(17, 13);
        lblNotifTitle.Name = "lblNotifTitle";
        lblNotifTitle.Size = new Size(229, 33);
        lblNotifTitle.TabIndex = 0;
        lblNotifTitle.Text = "🔔 Notifications";
        // 
        // chkNotifyMatches
        // 
        chkNotifyMatches.BackColor = Color.Transparent;
        chkNotifyMatches.Checked = true;
        chkNotifyMatches.CheckState = CheckState.Checked;
        chkNotifyMatches.Font = new Font("Segoe UI", 10F);
        chkNotifyMatches.ForeColor = Color.White;
        chkNotifyMatches.Location = new Point(17, 47);
        chkNotifyMatches.Margin = new Padding(3, 4, 3, 4);
        chkNotifyMatches.Name = "chkNotifyMatches";
        chkNotifyMatches.Size = new Size(171, 29);
        chkNotifyMatches.TabIndex = 1;
        chkNotifyMatches.Text = "New matches";
        chkNotifyMatches.UseVisualStyleBackColor = false;
        // 
        // chkNotifyMessages
        // 
        chkNotifyMessages.BackColor = Color.Transparent;
        chkNotifyMessages.Checked = true;
        chkNotifyMessages.CheckState = CheckState.Checked;
        chkNotifyMessages.Font = new Font("Segoe UI", 10F);
        chkNotifyMessages.ForeColor = Color.White;
        chkNotifyMessages.Location = new Point(17, 77);
        chkNotifyMessages.Margin = new Padding(3, 4, 3, 4);
        chkNotifyMessages.Name = "chkNotifyMessages";
        chkNotifyMessages.Size = new Size(171, 29);
        chkNotifyMessages.TabIndex = 2;
        chkNotifyMessages.Text = "New messages";
        chkNotifyMessages.UseVisualStyleBackColor = false;
        // 
        // pnlPrivacy
        // 
        pnlPrivacy.BackColor = Color.Transparent;
        pnlPrivacy.Controls.Add(lblPrivacyTitle);
        pnlPrivacy.Controls.Add(chkShowOnline);
        pnlPrivacy.Controls.Add(chkShowDistance);
        pnlPrivacy.Location = new Point(17, 367);
        pnlPrivacy.Margin = new Padding(3, 4, 3, 4);
        pnlPrivacy.Name = "pnlPrivacy";
        pnlPrivacy.Size = new Size(594, 113);
        pnlPrivacy.TabIndex = 3;
        pnlPrivacy.Paint += PnlSection_Paint;
        // 
        // lblPrivacyTitle
        // 
        lblPrivacyTitle.BackColor = Color.Transparent;
        lblPrivacyTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblPrivacyTitle.ForeColor = Color.White;
        lblPrivacyTitle.Location = new Point(17, 13);
        lblPrivacyTitle.Name = "lblPrivacyTitle";
        lblPrivacyTitle.Size = new Size(229, 33);
        lblPrivacyTitle.TabIndex = 0;
        lblPrivacyTitle.Text = "🔒 Privacy";
        // 
        // chkShowOnline
        // 
        chkShowOnline.BackColor = Color.Transparent;
        chkShowOnline.Checked = true;
        chkShowOnline.CheckState = CheckState.Checked;
        chkShowOnline.Font = new Font("Segoe UI", 10F);
        chkShowOnline.ForeColor = Color.White;
        chkShowOnline.Location = new Point(17, 47);
        chkShowOnline.Margin = new Padding(3, 4, 3, 4);
        chkShowOnline.Name = "chkShowOnline";
        chkShowOnline.Size = new Size(229, 29);
        chkShowOnline.TabIndex = 1;
        chkShowOnline.Text = "Show online status";
        chkShowOnline.UseVisualStyleBackColor = false;
        // 
        // chkShowDistance
        // 
        chkShowDistance.BackColor = Color.Transparent;
        chkShowDistance.Font = new Font("Segoe UI", 10F);
        chkShowDistance.ForeColor = Color.White;
        chkShowDistance.Location = new Point(17, 77);
        chkShowDistance.Margin = new Padding(3, 4, 3, 4);
        chkShowDistance.Name = "chkShowDistance";
        chkShowDistance.Size = new Size(229, 29);
        chkShowDistance.TabIndex = 2;
        chkShowDistance.Text = "Show distance on profile";
        chkShowDistance.UseVisualStyleBackColor = false;
        // 
        // btnSaveSettings
        // 
        btnSaveSettings.BackColor = Color.FromArgb(124, 58, 237);
        btnSaveSettings.Cursor = Cursors.Hand;
        btnSaveSettings.FlatAppearance.BorderSize = 0;
        btnSaveSettings.FlatStyle = FlatStyle.Flat;
        btnSaveSettings.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnSaveSettings.ForeColor = Color.White;
        btnSaveSettings.Location = new Point(17, 493);
        btnSaveSettings.Margin = new Padding(3, 4, 3, 4);
        btnSaveSettings.Name = "btnSaveSettings";
        btnSaveSettings.Size = new Size(171, 53);
        btnSaveSettings.TabIndex = 4;
        btnSaveSettings.Text = "💾 Save Settings";
        btnSaveSettings.UseVisualStyleBackColor = false;
        btnSaveSettings.Click += BtnSaveSettings_Click;
        btnSaveSettings.Paint += BtnSaveSettings_Paint;
        // 
        // lblStatus
        // 
        lblStatus.Font = new Font("Segoe UI", 10F);
        lblStatus.ForeColor = Color.FromArgb(34, 197, 94);
        lblStatus.Location = new Point(206, 504);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(171, 33);
        lblStatus.TabIndex = 5;
        lblStatus.Visible = false;
        // 
        // pnlAccount
        // 
        pnlAccount.BackColor = Color.Transparent;
        pnlAccount.Controls.Add(lblAccountTitle);
        pnlAccount.Controls.Add(btnLogout);
        pnlAccount.Controls.Add(btnDeleteAccount);
        pnlAccount.Location = new Point(17, 560);
        pnlAccount.Margin = new Padding(3, 4, 3, 4);
        pnlAccount.Name = "pnlAccount";
        pnlAccount.Size = new Size(594, 120);
        pnlAccount.TabIndex = 6;
        pnlAccount.Paint += PnlSection_Paint;
        // 
        // lblAccountTitle
        // 
        lblAccountTitle.BackColor = Color.Transparent;
        lblAccountTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblAccountTitle.ForeColor = Color.White;
        lblAccountTitle.Location = new Point(17, 13);
        lblAccountTitle.Name = "lblAccountTitle";
        lblAccountTitle.Size = new Size(229, 33);
        lblAccountTitle.TabIndex = 0;
        lblAccountTitle.Text = "Account";
        // 
        // btnLogout
        // 
        btnLogout.BackColor = Color.FromArgb(60, 55, 105);
        btnLogout.Cursor = Cursors.Hand;
        btnLogout.FlatAppearance.BorderSize = 0;
        btnLogout.FlatStyle = FlatStyle.Flat;
        btnLogout.Font = new Font("Segoe UI", 10F);
        btnLogout.ForeColor = Color.White;
        btnLogout.Location = new Point(17, 60);
        btnLogout.Margin = new Padding(3, 4, 3, 4);
        btnLogout.Name = "btnLogout";
        btnLogout.Size = new Size(114, 47);
        btnLogout.TabIndex = 1;
        btnLogout.Text = "🚪 Logout";
        btnLogout.UseVisualStyleBackColor = false;
        btnLogout.Click += BtnLogout_Click;
        btnLogout.Paint += BtnLogout_Paint;
        // 
        // btnDeleteAccount
        // 
        btnDeleteAccount.BackColor = Color.FromArgb(127, 29, 29);
        btnDeleteAccount.Cursor = Cursors.Hand;
        btnDeleteAccount.FlatAppearance.BorderSize = 0;
        btnDeleteAccount.FlatStyle = FlatStyle.Flat;
        btnDeleteAccount.Font = new Font("Segoe UI", 10F);
        btnDeleteAccount.ForeColor = Color.White;
        btnDeleteAccount.Location = new Point(149, 60);
        btnDeleteAccount.Margin = new Padding(3, 4, 3, 4);
        btnDeleteAccount.Name = "btnDeleteAccount";
        btnDeleteAccount.Size = new Size(160, 47);
        btnDeleteAccount.TabIndex = 2;
        btnDeleteAccount.Text = "🗑️ Delete Account";
        btnDeleteAccount.UseVisualStyleBackColor = false;
        btnDeleteAccount.Click += BtnDeleteAccount_Click;
        btnDeleteAccount.Paint += BtnDeleteAccount_Paint;
        // 
        // pnlLinks
        // 
        pnlLinks.Controls.Add(lnkPrivacy);
        pnlLinks.Controls.Add(lnkTerms);
        pnlLinks.Controls.Add(lblVersion);
        pnlLinks.Location = new Point(17, 693);
        pnlLinks.Margin = new Padding(3, 4, 3, 4);
        pnlLinks.Name = "pnlLinks";
        pnlLinks.Size = new Size(594, 53);
        pnlLinks.TabIndex = 7;
        // 
        // lnkPrivacy
        // 
        lnkPrivacy.ActiveLinkColor = Color.FromArgb(236, 72, 153);
        lnkPrivacy.Font = new Font("Segoe UI", 9F);
        lnkPrivacy.LinkColor = Color.FromArgb(124, 58, 237);
        lnkPrivacy.Location = new Point(0, 13);
        lnkPrivacy.Name = "lnkPrivacy";
        lnkPrivacy.Size = new Size(114, 27);
        lnkPrivacy.TabIndex = 0;
        lnkPrivacy.TabStop = true;
        lnkPrivacy.Text = "Privacy Policy";
        lnkPrivacy.Click += LnkPrivacy_Click;
        // 
        // lnkTerms
        // 
        lnkTerms.ActiveLinkColor = Color.FromArgb(236, 72, 153);
        lnkTerms.Font = new Font("Segoe UI", 9F);
        lnkTerms.LinkColor = Color.FromArgb(124, 58, 237);
        lnkTerms.Location = new Point(126, 13);
        lnkTerms.Name = "lnkTerms";
        lnkTerms.Size = new Size(114, 27);
        lnkTerms.TabIndex = 1;
        lnkTerms.TabStop = true;
        lnkTerms.Text = "Terms of Service";
        lnkTerms.Click += LnkTerms_Click;
        // 
        // lblVersion
        // 
        lblVersion.Font = new Font("Segoe UI", 9F);
        lblVersion.ForeColor = Color.FromArgb(100, 100, 120);
        lblVersion.Location = new Point(457, 13);
        lblVersion.Name = "lblVersion";
        lblVersion.Size = new Size(114, 27);
        lblVersion.TabIndex = 2;
        lblVersion.Text = "v1.0.0";
        lblVersion.TextAlign = ContentAlignment.MiddleRight;
        // 
        // SettingsView
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(30, 27, 75);
        Controls.Add(pnlHeader);
        Controls.Add(pnlDiscovery);
        Controls.Add(pnlNotifications);
        Controls.Add(pnlPrivacy);
        Controls.Add(btnSaveSettings);
        Controls.Add(lblStatus);
        Controls.Add(pnlAccount);
        Controls.Add(pnlLinks);
        Margin = new Padding(3, 4, 3, 4);
        Name = "SettingsView";
        Size = new Size(629, 773);
        pnlHeader.ResumeLayout(false);
        pnlDiscovery.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)numMinAge).EndInit();
        ((System.ComponentModel.ISupportInitialize)numMaxAge).EndInit();
        pnlNotifications.ResumeLayout(false);
        pnlPrivacy.ResumeLayout(false);
        pnlAccount.ResumeLayout(false);
        pnlLinks.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Panel pnlHeader;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.Panel pnlDiscovery;
    private System.Windows.Forms.Label lblDiscoveryTitle;
    private System.Windows.Forms.Label lblAgeRange;
    private System.Windows.Forms.NumericUpDown numMinAge;
    private System.Windows.Forms.Label lblTo;
    private System.Windows.Forms.NumericUpDown numMaxAge;
    private System.Windows.Forms.Panel pnlNotifications;
    private System.Windows.Forms.Label lblNotifTitle;
    private System.Windows.Forms.CheckBox chkNotifyMatches;
    private System.Windows.Forms.CheckBox chkNotifyMessages;
    private System.Windows.Forms.Panel pnlPrivacy;
    private System.Windows.Forms.Label lblPrivacyTitle;
    private System.Windows.Forms.CheckBox chkShowOnline;
    private System.Windows.Forms.CheckBox chkShowDistance;
    private System.Windows.Forms.Button btnSaveSettings;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.Panel pnlAccount;
    private System.Windows.Forms.Label lblAccountTitle;
    private System.Windows.Forms.Button btnLogout;
    private System.Windows.Forms.Button btnDeleteAccount;
    private System.Windows.Forms.Panel pnlLinks;
    private System.Windows.Forms.LinkLabel lnkPrivacy;
    private System.Windows.Forms.LinkLabel lnkTerms;
    private System.Windows.Forms.Label lblVersion;

    private static System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        var path = new System.Drawing.Drawing2D.GraphicsPath();
        path.AddArc(bounds.X, bounds.Y, radius * 2, radius * 2, 180, 90);
        path.AddArc(bounds.Right - radius * 2, bounds.Y, radius * 2, radius * 2, 270, 90);
        path.AddArc(bounds.Right - radius * 2, bounds.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
        path.CloseFigure();
        return path;
    }

    private void PnlSection_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Panel panel) return;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        // Draw shadow
        var shadowRect = new Rectangle(3, 3, panel.Width - 4, panel.Height - 4);
        using var shadowPath = RoundedRect(shadowRect, 15);
        using var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0));
        e.Graphics.FillPath(shadowBrush, shadowPath);
        
        // Draw panel
        var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
        using var path = RoundedRect(rect, 15);
        using var brush = new SolidBrush(UIHelper.CardBackground);
        e.Graphics.FillPath(brush, path);
        
        // Subtle gradient overlay for depth
        using var overlayBrush = new LinearGradientBrush(
            rect,
            Color.FromArgb(10, 255, 255, 255),
            Color.FromArgb(0, 255, 255, 255),
            LinearGradientMode.Vertical);
        e.Graphics.FillPath(overlayBrush, path);
    }

    private void PnlHeader_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Panel panel) return;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        // Solid purple header
        using var brush = new SolidBrush(UIHelper.PrimaryPurple);
        e.Graphics.FillRectangle(brush, panel.ClientRectangle);
    }

    private void BtnSaveSettings_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Button btn) return;
        var rect = new Rectangle(0, 0, btn.Width, btn.Height);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        using var path = RoundedRect(rect, 10);
        using var brush = new LinearGradientBrush(
            rect,
            UIHelper.PrimaryPurple,
            Color.FromArgb(168, 85, 247),
            LinearGradientMode.Horizontal);
        
        e.Graphics.FillPath(brush, path);
        
        using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        e.Graphics.DrawString(btn.Text, btn.Font, Brushes.White, rect, sf);
        
        btn.Region = new Region(path);
    }

    private void BtnLogout_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Button btn) return;
        var rect = new Rectangle(0, 0, btn.Width, btn.Height);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        using var path = RoundedRect(rect, 8);
        using var brush = new SolidBrush(UIHelper.InputBackground);
        e.Graphics.FillPath(brush, path);
        
        // Subtle border
        using var pen = new Pen(Color.FromArgb(80, 255, 255, 255), 1);
        e.Graphics.DrawPath(pen, path);
        
        using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        e.Graphics.DrawString(btn.Text, btn.Font, Brushes.White, rect, sf);
        
        btn.Region = new Region(path);
    }

    private void BtnDeleteAccount_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Button btn) return;
        var rect = new Rectangle(0, 0, btn.Width, btn.Height);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        using var path = RoundedRect(rect, 8);
        using var brush = new LinearGradientBrush(
            rect,
            Color.FromArgb(185, 28, 28), // Dark red
            Color.FromArgb(127, 29, 29), // Darker red
            LinearGradientMode.Vertical);
        e.Graphics.FillPath(brush, path);
        
        using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        e.Graphics.DrawString(btn.Text, btn.Font, Brushes.White, rect, sf);
        
        btn.Region = new Region(path);
    }
}
