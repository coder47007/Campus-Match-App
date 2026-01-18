#nullable enable

using CampusMatch.Client.Helpers;
using CampusMatch.Client.Controls;
using System.Drawing.Drawing2D;

namespace CampusMatch.Client.Views;

partial class ProfileView
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
        pnlCompletion = new Panel();
        lblCompletion = new Label();
        pnlProgressBar = new Panel();
        lblTips = new Label();
        pnlPhoto = new Panel();
        pnlPhotoPlaceholder = new Panel();
        lblPhotoInitials = new Label();
        picPreview = new PictureBox();
        lblAddPhoto = new Label();
        pnlFields = new Panel();
        lblName = new Label();
        txtName = new TextBox();
        lblAge = new Label();
        txtAge = new TextBox();
        lblMajor = new Label();
        txtMajor = new TextBox();
        lblYear = new Label();
        txtYear = new TextBox();
        lblBioLabel = new Label();
        txtBio = new TextBox();
        lblPhotoUrl = new Label();
        txtPhotoUrl = new TextBox();
        lblGender = new Label();
        cboGender = new ComboBox();
        lblPref = new Label();
        cboPrefGender = new ComboBox();
        btnSave = new Button();
        lblSaveStatus = new Label();
        lblPhotosHeader = new Label();
        pnlPhotoGallery = new FlowLayoutPanel();
        lblInterestsHeader = new Label();
        lblInterestCount = new Label();
        pnlInterests = new FlowLayoutPanel();
        lblPromptsHeader = new Label();
        btnAddPrompt = new Button();
        pnlPrompts = new FlowLayoutPanel();
        loadingOverlay = new LoadingOverlay();
        pnlHeader.SuspendLayout();
        pnlCompletion.SuspendLayout();
        pnlPhoto.SuspendLayout();
        pnlPhotoPlaceholder.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
        pnlFields.SuspendLayout();
        SuspendLayout();
        // 
        // pnlHeader
        // 
        pnlHeader.BackColor = Color.FromArgb(35, 30, 70);
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(547, 60);
        pnlHeader.TabIndex = 0;
        pnlHeader.Paint += PnlHeader_Paint;
        // 
        // lblTitle
        // 
        lblTitle.BackColor = Color.Transparent;
        lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(15, 12);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(200, 40);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "👤 Edit Profile";
        // 
        // pnlCompletion
        // 
        pnlCompletion.BackColor = Color.Transparent;
        pnlCompletion.Controls.Add(lblCompletion);
        pnlCompletion.Controls.Add(pnlProgressBar);
        pnlCompletion.Controls.Add(lblTips);
        pnlCompletion.Location = new Point(15, 70);
        pnlCompletion.Name = "pnlCompletion";
        pnlCompletion.Size = new Size(520, 70);
        pnlCompletion.TabIndex = 1;
        pnlCompletion.Paint += PnlSection_Paint;
        // 
        // lblCompletion
        // 
        lblCompletion.BackColor = Color.Transparent;
        lblCompletion.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblCompletion.ForeColor = Color.White;
        lblCompletion.Location = new Point(15, 10);
        lblCompletion.Name = "lblCompletion";
        lblCompletion.Size = new Size(150, 20);
        lblCompletion.TabIndex = 0;
        lblCompletion.Text = "0% Complete";
        // 
        // pnlProgressBar
        // 
        pnlProgressBar.BackColor = Color.FromArgb(60, 55, 105);
        pnlProgressBar.Location = new Point(15, 35);
        pnlProgressBar.Name = "pnlProgressBar";
        pnlProgressBar.Size = new Size(490, 8);
        pnlProgressBar.TabIndex = 1;
        pnlProgressBar.Paint += PnlProgressBar_Paint;
        // 
        // lblTips
        // 
        lblTips.Font = new Font("Segoe UI", 9F);
        lblTips.ForeColor = Color.FromArgb(180, 180, 200);
        lblTips.Location = new Point(15, 48);
        lblTips.Name = "lblTips";
        lblTips.Size = new Size(490, 18);
        lblTips.TabIndex = 2;
        lblTips.Text = "💡 Add a photo, Write a bio";
        // 
        // pnlPhoto
        // 
        pnlPhoto.Controls.Add(pnlPhotoPlaceholder);
        pnlPhoto.Controls.Add(lblAddPhoto);
        pnlPhoto.Location = new Point(15, 150);
        pnlPhoto.Name = "pnlPhoto";
        pnlPhoto.Size = new Size(130, 150);
        pnlPhoto.TabIndex = 2;
        // 
        // pnlPhotoPlaceholder
        // 
        pnlPhotoPlaceholder.Controls.Add(lblPhotoInitials);
        pnlPhotoPlaceholder.Controls.Add(picPreview);
        pnlPhotoPlaceholder.Cursor = Cursors.Hand;
        pnlPhotoPlaceholder.Location = new Point(0, 0);
        pnlPhotoPlaceholder.Name = "pnlPhotoPlaceholder";
        pnlPhotoPlaceholder.Size = new Size(130, 130);
        pnlPhotoPlaceholder.TabIndex = 1;
        pnlPhotoPlaceholder.Click += LblAddPhoto_Click;
        pnlPhotoPlaceholder.Paint += PnlPhotoPlaceholder_Paint;
        // 
        // lblPhotoInitials
        // 
        lblPhotoInitials.Dock = DockStyle.Fill;
        lblPhotoInitials.Font = new Font("Segoe UI", 36F, FontStyle.Bold);
        lblPhotoInitials.ForeColor = Color.White;
        lblPhotoInitials.Location = new Point(0, 0);
        lblPhotoInitials.Name = "lblPhotoInitials";
        lblPhotoInitials.Size = new Size(130, 130);
        lblPhotoInitials.TabIndex = 0;
        lblPhotoInitials.Text = "JD";
        lblPhotoInitials.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // picPreview
        // 
        picPreview.BackColor = Color.Transparent;
        picPreview.Dock = DockStyle.Fill;
        picPreview.Location = new Point(0, 0);
        picPreview.Name = "picPreview";
        picPreview.Size = new Size(130, 130);
        picPreview.SizeMode = PictureBoxSizeMode.Zoom;
        picPreview.TabIndex = 1;
        picPreview.TabStop = false;
        picPreview.Visible = false;
        // 
        // lblAddPhoto
        // 
        lblAddPhoto.Cursor = Cursors.Hand;
        lblAddPhoto.Font = new Font("Segoe UI", 9F);
        lblAddPhoto.ForeColor = Color.FromArgb(124, 58, 237);
        lblAddPhoto.Location = new Point(3, 133);
        lblAddPhoto.Name = "lblAddPhoto";
        lblAddPhoto.Size = new Size(130, 17);
        lblAddPhoto.TabIndex = 2;
        lblAddPhoto.Text = "📷 Change Photo";
        lblAddPhoto.TextAlign = ContentAlignment.MiddleCenter;
        lblAddPhoto.Click += LblAddPhoto_Click;
        // 
        // pnlFields
        // 
        pnlFields.Controls.Add(lblName);
        pnlFields.Controls.Add(txtName);
        pnlFields.Controls.Add(lblAge);
        pnlFields.Controls.Add(txtAge);
        pnlFields.Controls.Add(lblMajor);
        pnlFields.Controls.Add(txtMajor);
        pnlFields.Controls.Add(lblYear);
        pnlFields.Controls.Add(txtYear);
        pnlFields.Controls.Add(lblBioLabel);
        pnlFields.Controls.Add(txtBio);
        pnlFields.Controls.Add(lblPhotoUrl);
        pnlFields.Controls.Add(txtPhotoUrl);
        pnlFields.Controls.Add(lblGender);
        pnlFields.Controls.Add(cboGender);
        pnlFields.Controls.Add(lblPref);
        pnlFields.Controls.Add(cboPrefGender);
        pnlFields.Location = new Point(193, 150);
        pnlFields.Name = "pnlFields";
        pnlFields.Size = new Size(347, 308);
        pnlFields.TabIndex = 3;
        // 
        // lblName
        // 
        lblName.Font = new Font("Segoe UI", 10F);
        lblName.ForeColor = Color.White;
        lblName.Location = new Point(0, 5);
        lblName.Name = "lblName";
        lblName.Size = new Size(80, 20);
        lblName.TabIndex = 0;
        lblName.Text = "Name";
        // 
        // txtName
        // 
        txtName.BackColor = Color.FromArgb(60, 55, 105);
        txtName.BorderStyle = BorderStyle.FixedSingle;
        txtName.Font = new Font("Segoe UI", 11F);
        txtName.ForeColor = Color.White;
        txtName.Location = new Point(85, 2);
        txtName.Name = "txtName";
        txtName.Size = new Size(257, 27);
        txtName.TabIndex = 1;
        // 
        // lblAge
        // 
        lblAge.Font = new Font("Segoe UI", 10F);
        lblAge.ForeColor = Color.White;
        lblAge.Location = new Point(0, 40);
        lblAge.Name = "lblAge";
        lblAge.Size = new Size(80, 20);
        lblAge.TabIndex = 2;
        lblAge.Text = "Age";
        // 
        // txtAge
        // 
        txtAge.BackColor = Color.FromArgb(60, 55, 105);
        txtAge.BorderStyle = BorderStyle.FixedSingle;
        txtAge.Font = new Font("Segoe UI", 11F);
        txtAge.ForeColor = Color.White;
        txtAge.Location = new Point(85, 37);
        txtAge.Name = "txtAge";
        txtAge.Size = new Size(80, 27);
        txtAge.TabIndex = 3;
        // 
        // lblMajor
        // 
        lblMajor.Font = new Font("Segoe UI", 10F);
        lblMajor.ForeColor = Color.White;
        lblMajor.Location = new Point(0, 75);
        lblMajor.Name = "lblMajor";
        lblMajor.Size = new Size(80, 20);
        lblMajor.TabIndex = 4;
        lblMajor.Text = "Major";
        // 
        // txtMajor
        // 
        txtMajor.BackColor = Color.FromArgb(60, 55, 105);
        txtMajor.BorderStyle = BorderStyle.FixedSingle;
        txtMajor.Font = new Font("Segoe UI", 11F);
        txtMajor.ForeColor = Color.White;
        txtMajor.Location = new Point(85, 72);
        txtMajor.Name = "txtMajor";
        txtMajor.Size = new Size(257, 27);
        txtMajor.TabIndex = 5;
        // 
        // lblYear
        // 
        lblYear.Font = new Font("Segoe UI", 10F);
        lblYear.ForeColor = Color.White;
        lblYear.Location = new Point(0, 110);
        lblYear.Name = "lblYear";
        lblYear.Size = new Size(80, 20);
        lblYear.TabIndex = 6;
        lblYear.Text = "Year";
        // 
        // txtYear
        // 
        txtYear.BackColor = Color.FromArgb(60, 55, 105);
        txtYear.BorderStyle = BorderStyle.FixedSingle;
        txtYear.Font = new Font("Segoe UI", 11F);
        txtYear.ForeColor = Color.White;
        txtYear.Location = new Point(85, 107);
        txtYear.Name = "txtYear";
        txtYear.PlaceholderText = "e.g. Junior, Senior";
        txtYear.Size = new Size(180, 27);
        txtYear.TabIndex = 7;
        // 
        // lblBioLabel
        // 
        lblBioLabel.Font = new Font("Segoe UI", 10F);
        lblBioLabel.ForeColor = Color.White;
        lblBioLabel.Location = new Point(0, 145);
        lblBioLabel.Name = "lblBioLabel";
        lblBioLabel.Size = new Size(80, 20);
        lblBioLabel.TabIndex = 8;
        lblBioLabel.Text = "Bio";
        // 
        // txtBio
        // 
        txtBio.BackColor = Color.FromArgb(60, 55, 105);
        txtBio.BorderStyle = BorderStyle.FixedSingle;
        txtBio.Font = new Font("Segoe UI", 10F);
        txtBio.ForeColor = Color.White;
        txtBio.Location = new Point(85, 142);
        txtBio.Multiline = true;
        txtBio.Name = "txtBio";
        txtBio.PlaceholderText = "Tell others about yourself...";
        txtBio.Size = new Size(257, 55);
        txtBio.TabIndex = 9;
        // 
        // lblPhotoUrl
        // 
        lblPhotoUrl.Font = new Font("Segoe UI", 10F);
        lblPhotoUrl.ForeColor = Color.White;
        lblPhotoUrl.Location = new Point(0, 210);
        lblPhotoUrl.Name = "lblPhotoUrl";
        lblPhotoUrl.Size = new Size(80, 20);
        lblPhotoUrl.TabIndex = 10;
        lblPhotoUrl.Text = "Photo URL";
        // 
        // txtPhotoUrl
        // 
        txtPhotoUrl.BackColor = Color.FromArgb(60, 55, 105);
        txtPhotoUrl.BorderStyle = BorderStyle.FixedSingle;
        txtPhotoUrl.Font = new Font("Segoe UI", 10F);
        txtPhotoUrl.ForeColor = Color.White;
        txtPhotoUrl.Location = new Point(85, 207);
        txtPhotoUrl.Name = "txtPhotoUrl";
        txtPhotoUrl.PlaceholderText = "https://...";
        txtPhotoUrl.Size = new Size(257, 25);
        txtPhotoUrl.TabIndex = 11;
        txtPhotoUrl.TextChanged += TxtPhotoUrl_TextChanged;
        // 
        // lblGender
        // 
        lblGender.Font = new Font("Segoe UI", 10F);
        lblGender.ForeColor = Color.White;
        lblGender.Location = new Point(0, 250);
        lblGender.Name = "lblGender";
        lblGender.Size = new Size(80, 20);
        lblGender.TabIndex = 12;
        lblGender.Text = "Gender";
        // 
        // cboGender
        // 
        cboGender.BackColor = Color.FromArgb(60, 55, 105);
        cboGender.DropDownStyle = ComboBoxStyle.DropDownList;
        cboGender.ForeColor = Color.White;
        cboGender.FormattingEnabled = true;
        cboGender.Items.AddRange(new object[] { "Male", "Female", "Other" });
        cboGender.Location = new Point(85, 247);
        cboGender.Name = "cboGender";
        cboGender.Size = new Size(130, 23);
        cboGender.TabIndex = 13;
        // 
        // lblPref
        // 
        lblPref.Font = new Font("Segoe UI", 10F);
        lblPref.ForeColor = Color.White;
        lblPref.Location = new Point(0, 285);
        lblPref.Name = "lblPref";
        lblPref.Size = new Size(80, 20);
        lblPref.TabIndex = 14;
        lblPref.Text = "Show me";
        // 
        // cboPrefGender
        // 
        cboPrefGender.BackColor = Color.FromArgb(60, 55, 105);
        cboPrefGender.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPrefGender.ForeColor = Color.White;
        cboPrefGender.FormattingEnabled = true;
        cboPrefGender.Items.AddRange(new object[] { "Male", "Female", "Everyone" });
        cboPrefGender.Location = new Point(85, 282);
        cboPrefGender.Name = "cboPrefGender";
        cboPrefGender.Size = new Size(130, 23);
        cboPrefGender.TabIndex = 15;
        // 
        // btnSave
        // 
        btnSave.BackColor = Color.FromArgb(124, 58, 237);
        btnSave.Cursor = Cursors.Hand;
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(139, 92, 246);
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        btnSave.ForeColor = Color.White;
        btnSave.Location = new Point(81, 854);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(180, 50);
        btnSave.TabIndex = 4;
        btnSave.Text = "💾 Save Profile";
        btnSave.UseVisualStyleBackColor = false;
        btnSave.Click += BtnSave_Click;
        btnSave.Paint += BtnSave_Paint;
        // 
        // lblSaveStatus
        // 
        lblSaveStatus.Font = new Font("Segoe UI", 10F);
        lblSaveStatus.ForeColor = Color.FromArgb(34, 197, 94);
        lblSaveStatus.Location = new Point(278, 854);
        lblSaveStatus.Name = "lblSaveStatus";
        lblSaveStatus.Size = new Size(150, 25);
        lblSaveStatus.TabIndex = 5;
        lblSaveStatus.Text = "✓ Profile saved!";
        lblSaveStatus.Visible = false;
        // 
        // lblPhotosHeader
        // 
        lblPhotosHeader.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblPhotosHeader.ForeColor = Color.White;
        lblPhotosHeader.Location = new Point(15, 310);
        lblPhotosHeader.Name = "lblPhotosHeader";
        lblPhotosHeader.Size = new Size(200, 25);
        lblPhotosHeader.TabIndex = 6;
        lblPhotosHeader.Text = "📷 My Photos";
        // 
        // pnlPhotoGallery
        // 
        pnlPhotoGallery.AutoScroll = true;
        pnlPhotoGallery.BackColor = Color.Transparent;
        pnlPhotoGallery.Location = new Point(10, 340);
        pnlPhotoGallery.Name = "pnlPhotoGallery";
        pnlPhotoGallery.Size = new Size(177, 179);
        pnlPhotoGallery.TabIndex = 7;
        // 
        // lblInterestsHeader
        // 
        lblInterestsHeader.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblInterestsHeader.ForeColor = Color.White;
        lblInterestsHeader.Location = new Point(18, 522);
        lblInterestsHeader.Name = "lblInterestsHeader";
        lblInterestsHeader.Size = new Size(200, 25);
        lblInterestsHeader.TabIndex = 8;
        lblInterestsHeader.Text = "🏷️ Interests";
        // 
        // lblInterestCount
        // 
        lblInterestCount.Font = new Font("Segoe UI", 9F);
        lblInterestCount.ForeColor = Color.FromArgb(180, 180, 200);
        lblInterestCount.Location = new Point(430, 438);
        lblInterestCount.Name = "lblInterestCount";
        lblInterestCount.Size = new Size(100, 20);
        lblInterestCount.TabIndex = 9;
        lblInterestCount.Text = "0/10 selected";
        lblInterestCount.TextAlign = ContentAlignment.TopRight;
        // 
        // pnlInterests
        // 
        pnlInterests.AutoScroll = true;
        pnlInterests.BackColor = Color.Transparent;
        pnlInterests.Location = new Point(15, 550);
        pnlInterests.Name = "pnlInterests";
        pnlInterests.Size = new Size(520, 90);
        pnlInterests.TabIndex = 10;
        // 
        // lblPromptsHeader
        // 
        lblPromptsHeader.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblPromptsHeader.ForeColor = Color.White;
        lblPromptsHeader.Location = new Point(10, 664);
        lblPromptsHeader.Name = "lblPromptsHeader";
        lblPromptsHeader.Size = new Size(250, 25);
        lblPromptsHeader.TabIndex = 11;
        lblPromptsHeader.Text = "💬 Profile Prompts (0/3)";
        // 
        // btnAddPrompt
        // 
        btnAddPrompt.BackColor = Color.FromArgb(60, 55, 105);
        btnAddPrompt.Cursor = Cursors.Hand;
        btnAddPrompt.FlatAppearance.BorderColor = Color.FromArgb(124, 58, 237);
        btnAddPrompt.FlatStyle = FlatStyle.Flat;
        btnAddPrompt.Font = new Font("Segoe UI", 9F);
        btnAddPrompt.ForeColor = Color.FromArgb(124, 58, 237);
        btnAddPrompt.Location = new Point(430, 658);
        btnAddPrompt.Name = "btnAddPrompt";
        btnAddPrompt.Size = new Size(100, 28);
        btnAddPrompt.TabIndex = 12;
        btnAddPrompt.Text = "+ Add Prompt";
        btnAddPrompt.UseVisualStyleBackColor = false;
        btnAddPrompt.Click += BtnAddPrompt_Click;
        // 
        // pnlPrompts
        // 
        pnlPrompts.AutoScroll = true;
        pnlPrompts.BackColor = Color.Transparent;
        pnlPrompts.FlowDirection = FlowDirection.TopDown;
        pnlPrompts.Location = new Point(10, 692);
        pnlPrompts.Name = "pnlPrompts";
        pnlPrompts.Size = new Size(520, 137);
        pnlPrompts.TabIndex = 13;
        // 
        // loadingOverlay
        // 
        loadingOverlay.Dock = DockStyle.Fill;
        loadingOverlay.Visible = false;
        // 
        // ProfileView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoScroll = true;
        BackColor = Color.FromArgb(30, 27, 75);
        Controls.Add(loadingOverlay);
        Controls.Add(lblInterestsHeader);
        Controls.Add(pnlHeader);
        Controls.Add(pnlCompletion);
        Controls.Add(pnlPhoto);
        Controls.Add(pnlFields);
        Controls.Add(lblPhotosHeader);
        Controls.Add(pnlPhotoGallery);
        Controls.Add(lblInterestCount);
        Controls.Add(pnlInterests);
        Controls.Add(lblPromptsHeader);
        Controls.Add(btnAddPrompt);
        Controls.Add(pnlPrompts);
        Controls.Add(btnSave);
        Controls.Add(lblSaveStatus);
        Name = "ProfileView";
        Size = new Size(547, 916);
        pnlHeader.ResumeLayout(false);
        pnlCompletion.ResumeLayout(false);
        pnlPhoto.ResumeLayout(false);
        pnlPhotoPlaceholder.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
        pnlFields.ResumeLayout(false);
        pnlFields.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Panel pnlHeader;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.Panel pnlCompletion;
    private System.Windows.Forms.Label lblCompletion;
    private System.Windows.Forms.Panel pnlProgressBar;
    private System.Windows.Forms.Label lblTips;
    private System.Windows.Forms.Panel pnlPhoto;
    private System.Windows.Forms.PictureBox picPreview;
    private System.Windows.Forms.Panel pnlPhotoPlaceholder;
    private System.Windows.Forms.Label lblPhotoInitials;
    private System.Windows.Forms.Label lblAddPhoto;
    private System.Windows.Forms.Panel pnlFields;
    private System.Windows.Forms.Label lblName;
    private System.Windows.Forms.TextBox txtName;
    private System.Windows.Forms.Label lblAge;
    private System.Windows.Forms.TextBox txtAge;
    private System.Windows.Forms.Label lblMajor;
    private System.Windows.Forms.TextBox txtMajor;
    private System.Windows.Forms.Label lblYear;
    private System.Windows.Forms.TextBox txtYear;
    private System.Windows.Forms.Label lblBioLabel;
    private System.Windows.Forms.TextBox txtBio;
    private System.Windows.Forms.Label lblPhotoUrl;
    private System.Windows.Forms.TextBox txtPhotoUrl;
    private System.Windows.Forms.Label lblGender;
    private System.Windows.Forms.ComboBox cboGender;
    private System.Windows.Forms.Label lblPref;
    private System.Windows.Forms.ComboBox cboPrefGender;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Label lblSaveStatus;
    private System.Windows.Forms.FlowLayoutPanel pnlPhotoGallery;
    private System.Windows.Forms.FlowLayoutPanel pnlInterests;
    private System.Windows.Forms.Label lblInterestCount;
    private System.Windows.Forms.Label lblInterestsHeader;
    private System.Windows.Forms.Label lblPhotosHeader;
    private System.Windows.Forms.Label lblPromptsHeader;
    private System.Windows.Forms.FlowLayoutPanel pnlPrompts;
    private System.Windows.Forms.Button btnAddPrompt;
    private LoadingOverlay loadingOverlay;

    private void PnlProgressBar_Paint(object? sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        
        // Background
        using var bgBrush = new SolidBrush(Color.FromArgb(60, 55, 105));
        e.Graphics.FillRectangle(bgBrush, 0, 0, pnlProgressBar.Width, pnlProgressBar.Height);
        
        // Progress fill with gradient
        int fillWidth = (int)(pnlProgressBar.Width * (completionPercentage / 100.0));
        if (fillWidth > 0)
        {
            using var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new Rectangle(0, 0, fillWidth, pnlProgressBar.Height),
                Color.FromArgb(124, 58, 237),
                Color.FromArgb(236, 72, 153),
                System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
            e.Graphics.FillRectangle(brush, 0, 0, fillWidth, pnlProgressBar.Height);
        }
    }

    private void PnlPhotoPlaceholder_Paint(object? sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var brush = new LinearGradientBrush(
            pnlPhotoPlaceholder.ClientRectangle,
            UIHelper.PrimaryPurple,
            UIHelper.PrimaryPink,
            LinearGradientMode.ForwardDiagonal);
        e.Graphics.FillRectangle(brush, pnlPhotoPlaceholder.ClientRectangle);
    }

    private void PnlHeader_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Panel panel) return;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        // Solid purple header
        using var brush = new SolidBrush(UIHelper.PrimaryPurple);
        e.Graphics.FillRectangle(brush, panel.ClientRectangle);
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
        
        // Subtle gradient overlay
        using var overlayBrush = new LinearGradientBrush(
            rect,
            Color.FromArgb(10, 255, 255, 255),
            Color.FromArgb(0, 255, 255, 255),
            LinearGradientMode.Vertical);
        e.Graphics.FillPath(overlayBrush, path);
    }

    private void BtnSave_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Button btn) return;
        var rect = new Rectangle(0, 0, btn.Width, btn.Height);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        using var path = RoundedRect(rect, 12);
        using var brush = new LinearGradientBrush(
            rect,
            UIHelper.PrimaryPurple,
            UIHelper.PrimaryPink,
            LinearGradientMode.Horizontal);
        
        e.Graphics.FillPath(brush, path);
        
        // Draw text
        using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        e.Graphics.DrawString(btn.Text, btn.Font, Brushes.White, rect, sf);
        
        btn.Region = new Region(path);
    }

    private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        int diameter = radius * 2;
        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }
}
