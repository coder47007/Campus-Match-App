#nullable enable

namespace CampusMatch.Client.Forms;

partial class LoginForm
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
        pnlCard = new Panel();
        lblFormTitle = new Label();
        lblFormSubtitle = new Label();
        pnlNameField = new Panel();
        lblNameIcon = new Label();
        txtName = new TextBox();
        pnlEmailField = new Panel();
        lblEmailIcon = new Label();
        txtEmail = new TextBox();
        pnlPasswordField = new Panel();
        lblPasswordIcon = new Label();
        txtPassword = new TextBox();
        pnlPhoneField = new Panel();
        lblPhoneIcon = new Label();
        txtPhone = new TextBox();
        pnlInstagramField = new Panel();
        lblInstagramIcon = new Label();
        txtInstagram = new TextBox();
        lnkForgot = new LinkLabel();
        btnLogin = new Button();
        lblOrDivider = new Label();
        btnRegister = new Button();
        lblTitle = new Label();
        lblSubtitle = new Label();
        pnlCard.SuspendLayout();
        pnlNameField.SuspendLayout();
        pnlEmailField.SuspendLayout();
        pnlPasswordField.SuspendLayout();
        pnlPhoneField.SuspendLayout();
        pnlInstagramField.SuspendLayout();
        SuspendLayout();
        // 
        // pnlCard
        // 
        pnlCard.BackColor = Color.White;
        pnlCard.Controls.Add(lblFormTitle);
        pnlCard.Controls.Add(lblFormSubtitle);
        pnlCard.Controls.Add(pnlNameField);
        pnlCard.Controls.Add(pnlEmailField);
        pnlCard.Controls.Add(pnlPasswordField);
        pnlCard.Controls.Add(pnlPhoneField);
        pnlCard.Controls.Add(pnlInstagramField);
        pnlCard.Controls.Add(lnkForgot);
        pnlCard.Controls.Add(btnLogin);
        pnlCard.Controls.Add(lblOrDivider);
        pnlCard.Controls.Add(btnRegister);
        pnlCard.Location = new Point(46, 173);
        pnlCard.Margin = new Padding(3, 4, 3, 4);
        pnlCard.Name = "pnlCard";
        pnlCard.Padding = new Padding(23, 27, 23, 27);
        pnlCard.Size = new Size(457, 640);
        pnlCard.TabIndex = 0;
        pnlCard.Paint += PnlCard_Paint;
        // 
        // lblFormTitle
        // 
        lblFormTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblFormTitle.ForeColor = Color.FromArgb(30, 27, 75);
        lblFormTitle.Location = new Point(29, 33);
        lblFormTitle.Name = "lblFormTitle";
        lblFormTitle.Size = new Size(400, 47);
        lblFormTitle.TabIndex = 0;
        lblFormTitle.Text = "Welcome Back! 👋";
        lblFormTitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblFormSubtitle
        // 
        lblFormSubtitle.Font = new Font("Segoe UI", 10F);
        lblFormSubtitle.ForeColor = Color.FromArgb(120, 120, 140);
        lblFormSubtitle.Location = new Point(29, 80);
        lblFormSubtitle.Name = "lblFormSubtitle";
        lblFormSubtitle.Size = new Size(400, 27);
        lblFormSubtitle.TabIndex = 0;
        lblFormSubtitle.Text = "Sign in to continue to CampusMatch";
        lblFormSubtitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // pnlNameField
        // 
        pnlNameField.BackColor = Color.FromArgb(248, 248, 252);
        pnlNameField.Controls.Add(lblNameIcon);
        pnlNameField.Controls.Add(txtName);
        pnlNameField.Location = new Point(29, 127);
        pnlNameField.Margin = new Padding(3, 4, 3, 4);
        pnlNameField.Name = "pnlNameField";
        pnlNameField.Size = new Size(400, 67);
        pnlNameField.TabIndex = 1;
        pnlNameField.Visible = false;
        pnlNameField.Paint += PnlField_Paint;
        // 
        // lblNameIcon
        // 
        lblNameIcon.Font = new Font("Segoe UI Emoji", 14F);
        lblNameIcon.ForeColor = Color.FromArgb(124, 58, 237);
        lblNameIcon.Location = new Point(14, 13);
        lblNameIcon.Name = "lblNameIcon";
        lblNameIcon.Size = new Size(34, 40);
        lblNameIcon.TabIndex = 0;
        lblNameIcon.Text = "👤";
        // 
        // txtName
        // 
        txtName.BackColor = Color.FromArgb(248, 248, 252);
        txtName.BorderStyle = BorderStyle.None;
        txtName.Font = new Font("Segoe UI", 12F);
        txtName.ForeColor = Color.FromArgb(60, 60, 80);
        txtName.Location = new Point(55, 19);
        txtName.Margin = new Padding(3, 4, 3, 4);
        txtName.Name = "txtName";
        txtName.PlaceholderText = "Full Name";
        txtName.Size = new Size(331, 27);
        txtName.TabIndex = 0;
        // 
        // pnlEmailField
        // 
        pnlEmailField.BackColor = Color.FromArgb(248, 248, 252);
        pnlEmailField.Controls.Add(lblEmailIcon);
        pnlEmailField.Controls.Add(txtEmail);
        pnlEmailField.Location = new Point(29, 127);
        pnlEmailField.Margin = new Padding(3, 4, 3, 4);
        pnlEmailField.Name = "pnlEmailField";
        pnlEmailField.Size = new Size(400, 67);
        pnlEmailField.TabIndex = 2;
        pnlEmailField.Paint += PnlField_Paint;
        // 
        // lblEmailIcon
        // 
        lblEmailIcon.Font = new Font("Segoe UI Emoji", 14F);
        lblEmailIcon.ForeColor = Color.FromArgb(124, 58, 237);
        lblEmailIcon.Location = new Point(14, 13);
        lblEmailIcon.Name = "lblEmailIcon";
        lblEmailIcon.Size = new Size(34, 40);
        lblEmailIcon.TabIndex = 0;
        lblEmailIcon.Text = "📧";
        // 
        // txtEmail
        // 
        txtEmail.BackColor = Color.FromArgb(248, 248, 252);
        txtEmail.BorderStyle = BorderStyle.None;
        txtEmail.Font = new Font("Segoe UI", 12F);
        txtEmail.ForeColor = Color.FromArgb(60, 60, 80);
        txtEmail.Location = new Point(55, 19);
        txtEmail.Margin = new Padding(3, 4, 3, 4);
        txtEmail.Name = "txtEmail";
        txtEmail.PlaceholderText = "Email address (@mybvc.ca)";
        txtEmail.Size = new Size(331, 27);
        txtEmail.TabIndex = 1;
        // 
        // pnlPasswordField
        // 
        pnlPasswordField.BackColor = Color.FromArgb(248, 248, 252);
        pnlPasswordField.Controls.Add(lblPasswordIcon);
        pnlPasswordField.Controls.Add(txtPassword);
        pnlPasswordField.Location = new Point(29, 207);
        pnlPasswordField.Margin = new Padding(3, 4, 3, 4);
        pnlPasswordField.Name = "pnlPasswordField";
        pnlPasswordField.Size = new Size(400, 67);
        pnlPasswordField.TabIndex = 3;
        pnlPasswordField.Paint += PnlField_Paint;
        // 
        // lblPasswordIcon
        // 
        lblPasswordIcon.Font = new Font("Segoe UI Emoji", 14F);
        lblPasswordIcon.ForeColor = Color.FromArgb(124, 58, 237);
        lblPasswordIcon.Location = new Point(14, 13);
        lblPasswordIcon.Name = "lblPasswordIcon";
        lblPasswordIcon.Size = new Size(34, 40);
        lblPasswordIcon.TabIndex = 0;
        lblPasswordIcon.Text = "🔒";
        // 
        // txtPassword
        // 
        txtPassword.BackColor = Color.FromArgb(248, 248, 252);
        txtPassword.BorderStyle = BorderStyle.None;
        txtPassword.Font = new Font("Segoe UI", 12F);
        txtPassword.ForeColor = Color.FromArgb(60, 60, 80);
        txtPassword.Location = new Point(55, 19);
        txtPassword.Margin = new Padding(3, 4, 3, 4);
        txtPassword.Name = "txtPassword";
        txtPassword.PlaceholderText = "Password";
        txtPassword.Size = new Size(331, 27);
        txtPassword.TabIndex = 2;
        txtPassword.UseSystemPasswordChar = true;
        // 
        // pnlPhoneField
        // 
        pnlPhoneField.BackColor = Color.FromArgb(248, 248, 252);
        pnlPhoneField.Controls.Add(lblPhoneIcon);
        pnlPhoneField.Controls.Add(txtPhone);
        pnlPhoneField.Location = new Point(29, 287);
        pnlPhoneField.Margin = new Padding(3, 4, 3, 4);
        pnlPhoneField.Name = "pnlPhoneField";
        pnlPhoneField.Size = new Size(400, 67);
        pnlPhoneField.TabIndex = 4;
        pnlPhoneField.Visible = false;
        pnlPhoneField.Paint += PnlField_Paint;
        // 
        // lblPhoneIcon
        // 
        lblPhoneIcon.Font = new Font("Segoe UI Emoji", 14F);
        lblPhoneIcon.ForeColor = Color.FromArgb(124, 58, 237);
        lblPhoneIcon.Location = new Point(14, 13);
        lblPhoneIcon.Name = "lblPhoneIcon";
        lblPhoneIcon.Size = new Size(34, 40);
        lblPhoneIcon.TabIndex = 0;
        lblPhoneIcon.Text = "📱";
        // 
        // txtPhone
        // 
        txtPhone.BackColor = Color.FromArgb(248, 248, 252);
        txtPhone.BorderStyle = BorderStyle.None;
        txtPhone.Font = new Font("Segoe UI", 12F);
        txtPhone.ForeColor = Color.FromArgb(60, 60, 80);
        txtPhone.Location = new Point(55, 19);
        txtPhone.Margin = new Padding(3, 4, 3, 4);
        txtPhone.Name = "txtPhone";
        txtPhone.PlaceholderText = "Phone Number";
        txtPhone.Size = new Size(331, 27);
        txtPhone.TabIndex = 3;
        // 
        // pnlInstagramField
        // 
        pnlInstagramField.BackColor = Color.FromArgb(248, 248, 252);
        pnlInstagramField.Controls.Add(lblInstagramIcon);
        pnlInstagramField.Controls.Add(txtInstagram);
        pnlInstagramField.Location = new Point(29, 367);
        pnlInstagramField.Margin = new Padding(3, 4, 3, 4);
        pnlInstagramField.Name = "pnlInstagramField";
        pnlInstagramField.Size = new Size(400, 67);
        pnlInstagramField.TabIndex = 5;
        pnlInstagramField.Visible = false;
        pnlInstagramField.Paint += PnlField_Paint;
        // 
        // lblInstagramIcon
        // 
        lblInstagramIcon.Font = new Font("Segoe UI Emoji", 14F);
        lblInstagramIcon.ForeColor = Color.FromArgb(124, 58, 237);
        lblInstagramIcon.Location = new Point(14, 13);
        lblInstagramIcon.Name = "lblInstagramIcon";
        lblInstagramIcon.Size = new Size(34, 40);
        lblInstagramIcon.TabIndex = 0;
        lblInstagramIcon.Text = "📸";
        // 
        // txtInstagram
        // 
        txtInstagram.BackColor = Color.FromArgb(248, 248, 252);
        txtInstagram.BorderStyle = BorderStyle.None;
        txtInstagram.Font = new Font("Segoe UI", 12F);
        txtInstagram.ForeColor = Color.FromArgb(60, 60, 80);
        txtInstagram.Location = new Point(55, 19);
        txtInstagram.Margin = new Padding(3, 4, 3, 4);
        txtInstagram.Name = "txtInstagram";
        txtInstagram.PlaceholderText = "@instagram (optional)";
        txtInstagram.Size = new Size(331, 27);
        txtInstagram.TabIndex = 4;
        // 
        // lnkForgot
        // 
        lnkForgot.ActiveLinkColor = Color.FromArgb(236, 72, 153);
        lnkForgot.AutoSize = true;
        lnkForgot.Font = new Font("Segoe UI", 9F);
        lnkForgot.LinkColor = Color.FromArgb(124, 58, 237);
        lnkForgot.Location = new Point(286, 287);
        lnkForgot.Name = "lnkForgot";
        lnkForgot.Size = new Size(127, 20);
        lnkForgot.TabIndex = 6;
        lnkForgot.TabStop = true;
        lnkForgot.Text = "Forgot password?";
        lnkForgot.LinkClicked += LnkForgot_LinkClicked;
        // 
        // btnLogin
        // 
        btnLogin.BackColor = Color.FromArgb(124, 58, 237);
        btnLogin.Cursor = Cursors.Hand;
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(139, 92, 246);
        btnLogin.FlatStyle = FlatStyle.Flat;
        btnLogin.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        btnLogin.ForeColor = Color.White;
        btnLogin.Location = new Point(29, 333);
        btnLogin.Margin = new Padding(3, 4, 3, 4);
        btnLogin.Name = "btnLogin";
        btnLogin.Size = new Size(400, 73);
        btnLogin.TabIndex = 7;
        btnLogin.Text = "Sign In  →";
        btnLogin.UseVisualStyleBackColor = false;
        btnLogin.Click += BtnLogin_Click;
        btnLogin.Paint += BtnLogin_Paint;
        // 
        // lblOrDivider
        // 
        lblOrDivider.Font = new Font("Segoe UI", 9F);
        lblOrDivider.ForeColor = Color.FromArgb(150, 150, 160);
        lblOrDivider.Location = new Point(29, 427);
        lblOrDivider.Name = "lblOrDivider";
        lblOrDivider.Size = new Size(400, 27);
        lblOrDivider.TabIndex = 0;
        lblOrDivider.Text = "─────────────  or  ─────────────";
        lblOrDivider.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // btnRegister
        // 
        btnRegister.BackColor = Color.Transparent;
        btnRegister.Cursor = Cursors.Hand;
        btnRegister.FlatAppearance.BorderColor = Color.FromArgb(124, 58, 237);
        btnRegister.FlatAppearance.BorderSize = 2;
        btnRegister.FlatAppearance.MouseOverBackColor = Color.FromArgb(250, 245, 255);
        btnRegister.FlatStyle = FlatStyle.Flat;
        btnRegister.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnRegister.ForeColor = Color.FromArgb(124, 58, 237);
        btnRegister.Location = new Point(29, 473);
        btnRegister.Margin = new Padding(3, 4, 3, 4);
        btnRegister.Name = "btnRegister";
        btnRegister.Size = new Size(400, 67);
        btnRegister.TabIndex = 8;
        btnRegister.Text = "Create New Account";
        btnRegister.UseVisualStyleBackColor = false;
        btnRegister.Click += BtnRegister_Click;
        // 
        // lblTitle
        // 
        lblTitle.BackColor = Color.Transparent;
        lblTitle.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(46, 33);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(457, 73);
        lblTitle.TabIndex = 1;
        lblTitle.Text = "💜 CampusMatch";
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblSubtitle
        // 
        lblSubtitle.BackColor = Color.Transparent;
        lblSubtitle.Font = new Font("Segoe UI", 13F);
        lblSubtitle.ForeColor = Color.FromArgb(230, 255, 255, 255);
        lblSubtitle.Location = new Point(46, 107);
        lblSubtitle.Name = "lblSubtitle";
        lblSubtitle.Size = new Size(457, 40);
        lblSubtitle.TabIndex = 2;
        lblSubtitle.Text = "Find your perfect campus connection ✨";
        lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // LoginForm
        // 
        AcceptButton = btnLogin;
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(30, 27, 75);
        ClientSize = new Size(549, 907);
        Controls.Add(lblTitle);
        Controls.Add(lblSubtitle);
        Controls.Add(pnlCard);
        DoubleBuffered = true;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Margin = new Padding(3, 4, 3, 4);
        MaximizeBox = false;
        Name = "LoginForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "CampusMatch - Welcome";
        Paint += LoginForm_Paint;
        pnlCard.ResumeLayout(false);
        pnlCard.PerformLayout();
        pnlNameField.ResumeLayout(false);
        pnlNameField.PerformLayout();
        pnlEmailField.ResumeLayout(false);
        pnlEmailField.PerformLayout();
        pnlPasswordField.ResumeLayout(false);
        pnlPasswordField.PerformLayout();
        pnlPhoneField.ResumeLayout(false);
        pnlPhoneField.PerformLayout();
        pnlInstagramField.ResumeLayout(false);
        pnlInstagramField.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Panel pnlCard;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.Label lblSubtitle;
    private System.Windows.Forms.Label lblFormTitle;
    private System.Windows.Forms.Label lblFormSubtitle;
    private System.Windows.Forms.Panel pnlNameField;
    private System.Windows.Forms.Label lblNameIcon;
    private System.Windows.Forms.TextBox txtName;
    private System.Windows.Forms.Panel pnlEmailField;
    private System.Windows.Forms.Label lblEmailIcon;
    private System.Windows.Forms.TextBox txtEmail;
    private System.Windows.Forms.Panel pnlPasswordField;
    private System.Windows.Forms.Label lblPasswordIcon;
    private System.Windows.Forms.TextBox txtPassword;
    private System.Windows.Forms.Panel pnlPhoneField;
    private System.Windows.Forms.Label lblPhoneIcon;
    private System.Windows.Forms.TextBox txtPhone;
    private System.Windows.Forms.Panel pnlInstagramField;
    private System.Windows.Forms.Label lblInstagramIcon;
    private System.Windows.Forms.TextBox txtInstagram;
    private System.Windows.Forms.LinkLabel lnkForgot;
    private System.Windows.Forms.Button btnLogin;
    private System.Windows.Forms.Label lblOrDivider;
    private System.Windows.Forms.Button btnRegister;

    private void LoginForm_Paint(object? sender, PaintEventArgs e)
    {
        using var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
            this.ClientRectangle,
            Color.FromArgb(124, 58, 237),
            Color.FromArgb(236, 72, 153),
            System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal);
        e.Graphics.FillRectangle(brush, this.ClientRectangle);
    }

    private void PnlCard_Paint(object? sender, PaintEventArgs e)
    {
        var rect = new Rectangle(0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
        using var path = RoundedRect(rect, 25);
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        using var shadow = new SolidBrush(Color.FromArgb(30, 0, 0, 0));
        e.Graphics.TranslateTransform(3, 3);
        e.Graphics.FillPath(shadow, path);
        e.Graphics.ResetTransform();
        using var brush = new SolidBrush(Color.White);
        e.Graphics.FillPath(brush, path);
    }

    private void PnlField_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Panel panel) return;
        var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
        using var path = RoundedRect(rect, 12);
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        using var brush = new SolidBrush(Color.FromArgb(248, 248, 252));
        e.Graphics.FillPath(brush, path);
        using var pen = new Pen(Color.FromArgb(230, 230, 240), 1);
        e.Graphics.DrawPath(pen, path);
    }

    private void BtnLogin_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Button btn) return;
        var rect = new Rectangle(0, 0, btn.Width, btn.Height);
        using var path = RoundedRect(rect, 12);
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        using var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
            rect,
            Color.FromArgb(124, 58, 237),
            Color.FromArgb(168, 85, 247),
            System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
        e.Graphics.FillPath(brush, path);
        
        using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        e.Graphics.DrawString(btn.Text, btn.Font, Brushes.White, rect, sf);
    }
}
