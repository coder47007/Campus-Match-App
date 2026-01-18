#nullable enable

using CampusMatch.Client.Helpers;
using CampusMatch.Client.Controls;
using System.Drawing.Drawing2D;

namespace CampusMatch.Client.Views;

partial class DiscoverView
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
        animationTimer?.Dispose();
        base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        cardPanel = new Panel();
        pnlInitials = new Panel();
        lblInitials = new Label();
        picProfile = new PictureBox();
        lblName = new Label();
        lblInfo = new Label();
        lblBio = new Label();
        lblUniversity = new Label();
        btnPass = new Button();
        btnSuperLike = new Button();
        btnLike = new Button();
        btnUndo = new Button();
        lblEmpty = new Label();
        lblHint = new Label();
        btnPrevPhoto = new Button();
        btnNextPhoto = new Button();
        pnlPhotoIndicators = new Panel();
        btnReport = new Button();
        pnlWhoLikedYou = new Panel();
        lblLikesTitle = new Label();
        lblLikesCount = new Label();
        pnlLikesPreview = new Panel();
        loadingOverlay = new LoadingOverlay();
        cardPanel.SuspendLayout();
        pnlInitials.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)picProfile).BeginInit();
        SuspendLayout();
        // 
        // cardPanel
        // 
        cardPanel.BackColor = Color.White;
        cardPanel.Controls.Add(btnReport);
        cardPanel.Controls.Add(pnlInitials);
        cardPanel.Controls.Add(btnPrevPhoto);
        cardPanel.Controls.Add(btnNextPhoto);
        cardPanel.Controls.Add(pnlPhotoIndicators);
        cardPanel.Controls.Add(picProfile);
        cardPanel.Controls.Add(lblName);
        cardPanel.Controls.Add(lblInfo);
        cardPanel.Controls.Add(lblBio);
        cardPanel.Controls.Add(lblUniversity);
        cardPanel.Location = new Point(160, 20);
        cardPanel.Name = "cardPanel";
        cardPanel.Size = new Size(400, 520);
        cardPanel.TabIndex = 0;
        cardPanel.Paint += CardPanel_Paint;
        // 
        // pnlInitials
        // 
        pnlInitials.Controls.Add(lblInitials);
        pnlInitials.Location = new Point(20, 20);
        pnlInitials.Name = "pnlInitials";
        pnlInitials.Size = new Size(360, 280);
        pnlInitials.TabIndex = 0;
        pnlInitials.Visible = false;
        pnlInitials.Paint += PnlInitials_Paint;
        // 
        // lblInitials
        // 
        lblInitials.Dock = DockStyle.Fill;
        lblInitials.Font = new Font("Segoe UI", 72F, FontStyle.Bold);
        lblInitials.ForeColor = Color.White;
        lblInitials.Location = new Point(0, 0);
        lblInitials.Name = "lblInitials";
        lblInitials.Size = new Size(360, 280);
        lblInitials.TabIndex = 0;
        lblInitials.Text = "JD";
        lblInitials.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // picProfile
        // 
        picProfile.BackColor = Color.FromArgb(245, 245, 250);
        picProfile.Location = new Point(20, 20);
        picProfile.Name = "picProfile";
        picProfile.Size = new Size(360, 280);
        picProfile.SizeMode = PictureBoxSizeMode.Zoom;
        picProfile.TabIndex = 1;
        picProfile.TabStop = false;
        // 
        // btnPrevPhoto
        // 
        btnPrevPhoto.BackColor = Color.FromArgb(150, 0, 0, 0);
        btnPrevPhoto.Cursor = Cursors.Hand;
        btnPrevPhoto.FlatAppearance.BorderSize = 0;
        btnPrevPhoto.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 0, 0, 0);
        btnPrevPhoto.FlatStyle = FlatStyle.Flat;
        btnPrevPhoto.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        btnPrevPhoto.ForeColor = Color.White;
        btnPrevPhoto.Location = new Point(25, 130);
        btnPrevPhoto.Name = "btnPrevPhoto";
        btnPrevPhoto.Size = new Size(40, 40);
        btnPrevPhoto.TabIndex = 10;
        btnPrevPhoto.Text = "‹";
        btnPrevPhoto.UseVisualStyleBackColor = false;
        btnPrevPhoto.Visible = false;
        btnPrevPhoto.Click += BtnPrevPhoto_Click;
        // 
        // btnNextPhoto
        // 
        btnNextPhoto.BackColor = Color.FromArgb(150, 0, 0, 0);
        btnNextPhoto.Cursor = Cursors.Hand;
        btnNextPhoto.FlatAppearance.BorderSize = 0;
        btnNextPhoto.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 0, 0, 0);
        btnNextPhoto.FlatStyle = FlatStyle.Flat;
        btnNextPhoto.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        btnNextPhoto.ForeColor = Color.White;
        btnNextPhoto.Location = new Point(335, 130);
        btnNextPhoto.Name = "btnNextPhoto";
        btnNextPhoto.Size = new Size(40, 40);
        btnNextPhoto.TabIndex = 11;
        btnNextPhoto.Text = "›";
        btnNextPhoto.UseVisualStyleBackColor = false;
        btnNextPhoto.Visible = false;
        btnNextPhoto.Click += BtnNextPhoto_Click;
        // 
        // pnlPhotoIndicators
        // 
        pnlPhotoIndicators.BackColor = Color.Transparent;
        pnlPhotoIndicators.Location = new Point(130, 270);
        pnlPhotoIndicators.Name = "pnlPhotoIndicators";
        pnlPhotoIndicators.Size = new Size(140, 20);
        pnlPhotoIndicators.TabIndex = 12;
        pnlPhotoIndicators.Visible = false;
        // 
        // btnReport
        // 
        btnReport.BackColor = Color.FromArgb(60, 255, 255, 255);
        btnReport.Cursor = Cursors.Hand;
        btnReport.FlatAppearance.BorderSize = 0;
        btnReport.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 239, 68, 68);
        btnReport.FlatStyle = FlatStyle.Flat;
        btnReport.Font = new Font("Segoe UI Emoji", 14F);
        btnReport.ForeColor = Color.FromArgb(239, 68, 68);
        btnReport.Location = new Point(350, 25);
        btnReport.Name = "btnReport";
        btnReport.Size = new Size(35, 35);
        btnReport.TabIndex = 13;
        btnReport.Text = "⚠️";
        btnReport.UseVisualStyleBackColor = false;
        btnReport.Click += BtnReport_Click;
        // 
        // lblName
        // 
        lblName.AutoSize = true;
        lblName.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
        lblName.ForeColor = Color.FromArgb(30, 27, 75);
        lblName.Location = new Point(20, 310);
        lblName.Name = "lblName";
        lblName.Size = new Size(0, 41);
        lblName.TabIndex = 2;
        // 
        // lblInfo
        // 
        lblInfo.AutoSize = true;
        lblInfo.Font = new Font("Segoe UI", 11F);
        lblInfo.ForeColor = Color.FromArgb(124, 58, 237);
        lblInfo.Location = new Point(20, 355);
        lblInfo.Name = "lblInfo";
        lblInfo.Size = new Size(0, 20);
        lblInfo.TabIndex = 3;
        // 
        // lblBio
        // 
        lblBio.Font = new Font("Segoe UI", 10F);
        lblBio.ForeColor = Color.FromArgb(80, 80, 80);
        lblBio.Location = new Point(20, 410);
        lblBio.Name = "lblBio";
        lblBio.Size = new Size(360, 95);
        lblBio.TabIndex = 5;
        // 
        // lblUniversity
        // 
        lblUniversity.AutoSize = true;
        lblUniversity.Font = new Font("Segoe UI", 10F);
        lblUniversity.ForeColor = Color.FromArgb(100, 100, 100);
        lblUniversity.Location = new Point(20, 380);
        lblUniversity.Name = "lblUniversity";
        lblUniversity.Size = new Size(0, 19);
        lblUniversity.TabIndex = 4;
        // 
        // btnPass
        // 
        btnPass.BackColor = Color.FromArgb(254, 226, 226);
        btnPass.Cursor = Cursors.Hand;
        btnPass.FlatAppearance.BorderSize = 0;
        btnPass.FlatAppearance.MouseOverBackColor = Color.FromArgb(254, 202, 202);
        btnPass.FlatStyle = FlatStyle.Flat;
        btnPass.Font = new Font("Segoe UI", 24F);
        btnPass.ForeColor = Color.FromArgb(239, 68, 68);
        btnPass.Location = new Point(215, 557);
        btnPass.Name = "btnPass";
        btnPass.Size = new Size(70, 70);
        btnPass.TabIndex = 1;
        btnPass.Text = "✖";
        btnPass.UseVisualStyleBackColor = false;
        btnPass.Click += BtnPass_Click;
        btnPass.Paint += BtnPass_Paint;
        // 
        // btnSuperLike
        // 
        btnSuperLike.BackColor = Color.FromArgb(254, 249, 195);
        btnSuperLike.Cursor = Cursors.Hand;
        btnSuperLike.FlatAppearance.BorderSize = 0;
        btnSuperLike.FlatAppearance.MouseOverBackColor = Color.FromArgb(253, 224, 71);
        btnSuperLike.FlatStyle = FlatStyle.Flat;
        btnSuperLike.Font = new Font("Segoe UI Emoji", 20F);
        btnSuperLike.ForeColor = Color.FromArgb(234, 179, 8);
        btnSuperLike.Location = new Point(334, 546);
        btnSuperLike.Name = "btnSuperLike";
        btnSuperLike.Size = new Size(60, 60);
        btnSuperLike.TabIndex = 2;
        btnSuperLike.Text = "⭐";
        btnSuperLike.UseVisualStyleBackColor = false;
        btnSuperLike.Click += BtnSuperLike_Click;
        btnSuperLike.Paint += BtnSuperLike_Paint;
        // 
        // btnLike
        // 
        btnLike.BackColor = Color.FromArgb(220, 252, 231);
        btnLike.Cursor = Cursors.Hand;
        btnLike.FlatAppearance.BorderSize = 0;
        btnLike.FlatAppearance.MouseOverBackColor = Color.FromArgb(187, 247, 208);
        btnLike.FlatStyle = FlatStyle.Flat;
        btnLike.Font = new Font("Segoe UI Emoji", 24F);
        btnLike.ForeColor = Color.FromArgb(34, 197, 94);
        btnLike.Location = new Point(445, 557);
        btnLike.Name = "btnLike";
        btnLike.Size = new Size(70, 70);
        btnLike.TabIndex = 3;
        btnLike.Text = "💚";
        btnLike.UseVisualStyleBackColor = false;
        btnLike.Click += BtnLike_Click;
        btnLike.Paint += BtnLike_Paint;
        // 
        // btnUndo
        // 
        btnUndo.BackColor = Color.FromArgb(238, 242, 255);
        btnUndo.Cursor = Cursors.Hand;
        btnUndo.FlatAppearance.BorderSize = 0;
        btnUndo.FlatAppearance.MouseOverBackColor = Color.FromArgb(224, 231, 255);
        btnUndo.FlatStyle = FlatStyle.Flat;
        btnUndo.Font = new Font("Segoe UI Emoji", 16F);
        btnUndo.ForeColor = Color.FromArgb(99, 102, 241);
        btnUndo.Location = new Point(108, 557);
        btnUndo.Name = "btnUndo";
        btnUndo.Size = new Size(50, 50);
        btnUndo.TabIndex = 6;
        btnUndo.Text = "↩";
        btnUndo.UseVisualStyleBackColor = false;
        btnUndo.Click += BtnUndo_Click;
        btnUndo.Paint += BtnUndo_Paint;
        // 
        // lblEmpty
        // 
        lblEmpty.Font = new Font("Segoe UI", 16F);
        lblEmpty.ForeColor = Color.White;
        lblEmpty.Location = new Point(160, 200);
        lblEmpty.Name = "lblEmpty";
        lblEmpty.Size = new Size(400, 150);
        lblEmpty.TabIndex = 5;
        lblEmpty.Text = "😢\r\n\r\nNo more profiles to discover\r\nCheck back later!";
        lblEmpty.TextAlign = ContentAlignment.MiddleCenter;
        lblEmpty.Visible = false;
        // 
        // lblHint
        // 
        lblHint.Font = new Font("Segoe UI", 9F);
        lblHint.ForeColor = Color.FromArgb(150, 150, 170);
        lblHint.Location = new Point(78, 640);
        lblHint.Name = "lblHint";
        lblHint.Size = new Size(560, 20);
        lblHint.TabIndex = 4;
        lblHint.Text = "↩ Undo          ✖ Pass          ⭐ Super Like          💚 Like";
        lblHint.TextAlign = ContentAlignment.MiddleCenter;
        lblHint.Click += lblHint_Click;
        // 
        // pnlWhoLikedYou
        // 
        pnlWhoLikedYou.BackColor = Color.FromArgb(45, 40, 90);
        pnlWhoLikedYou.Controls.Add(lblLikesTitle);
        pnlWhoLikedYou.Controls.Add(lblLikesCount);
        pnlWhoLikedYou.Controls.Add(pnlLikesPreview);
        pnlWhoLikedYou.Location = new Point(580, 20);
        pnlWhoLikedYou.Name = "pnlWhoLikedYou";
        pnlWhoLikedYou.Size = new Size(130, 180);
        pnlWhoLikedYou.TabIndex = 10;
        pnlWhoLikedYou.Cursor = Cursors.Hand;
        pnlWhoLikedYou.Click += PnlWhoLikedYou_Click;
        pnlWhoLikedYou.Paint += PnlWhoLikedYou_Paint;
        // 
        // lblLikesTitle
        // 
        lblLikesTitle.BackColor = Color.Transparent;
        lblLikesTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblLikesTitle.ForeColor = Color.White;
        lblLikesTitle.Location = new Point(5, 10);
        lblLikesTitle.Name = "lblLikesTitle";
        lblLikesTitle.Size = new Size(120, 20);
        lblLikesTitle.Text = "💕 Likes You";
        lblLikesTitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblLikesCount
        // 
        lblLikesCount.BackColor = Color.Transparent;
        lblLikesCount.Font = new Font("Segoe UI", 28F, FontStyle.Bold);
        lblLikesCount.ForeColor = Color.FromArgb(236, 72, 153);
        lblLikesCount.Location = new Point(5, 35);
        lblLikesCount.Name = "lblLikesCount";
        lblLikesCount.Size = new Size(120, 50);
        lblLikesCount.Text = "0";
        lblLikesCount.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // pnlLikesPreview
        // 
        pnlLikesPreview.BackColor = Color.Transparent;
        pnlLikesPreview.Location = new Point(15, 90);
        pnlLikesPreview.Name = "pnlLikesPreview";
        pnlLikesPreview.Size = new Size(100, 80);
        pnlLikesPreview.Paint += PnlLikesPreview_Paint;
        // 
        // loadingOverlay
        // 
        loadingOverlay.Dock = DockStyle.Fill;
        loadingOverlay.Visible = false;
        // 
        // DiscoverView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(30, 27, 75);
        Controls.Add(loadingOverlay);
        Controls.Add(pnlWhoLikedYou);
        Controls.Add(cardPanel);
        Controls.Add(btnUndo);
        Controls.Add(btnPass);
        Controls.Add(btnSuperLike);
        Controls.Add(btnLike);
        Controls.Add(lblHint);
        Controls.Add(lblEmpty);
        DoubleBuffered = true;
        Name = "DiscoverView";
        Size = new Size(720, 660);
        cardPanel.ResumeLayout(false);
        cardPanel.PerformLayout();
        pnlInitials.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)picProfile).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Panel cardPanel;
    private System.Windows.Forms.Panel pnlInitials;
    private System.Windows.Forms.Label lblInitials;
    private System.Windows.Forms.PictureBox picProfile;
    private System.Windows.Forms.Label lblName;
    private System.Windows.Forms.Label lblInfo;
    private System.Windows.Forms.Label lblUniversity;
    private System.Windows.Forms.Label lblBio;
    private System.Windows.Forms.Button btnPass;
    private System.Windows.Forms.Button btnSuperLike;
    private System.Windows.Forms.Button btnLike;
    private System.Windows.Forms.Button btnUndo;
    private System.Windows.Forms.Label lblHint;
    private System.Windows.Forms.Label lblEmpty;
    private System.Windows.Forms.Button btnPrevPhoto;
    private System.Windows.Forms.Button btnNextPhoto;
    private System.Windows.Forms.Panel pnlPhotoIndicators;
    private System.Windows.Forms.Button btnReport;
    
    // Who Liked You panel
    private System.Windows.Forms.Panel pnlWhoLikedYou;
    private System.Windows.Forms.Label lblLikesTitle;
    private System.Windows.Forms.Label lblLikesCount;
    private System.Windows.Forms.Panel pnlLikesPreview;
    
    // Loading overlay
    private LoadingOverlay loadingOverlay;

    private void CardPanel_Paint(object? sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        
        // Draw shadow
        var shadowRect = new Rectangle(4, 4, cardPanel.Width - 5, cardPanel.Height - 5);
        using var shadowPath = RoundedRect(shadowRect, 25);
        using var shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0));
        e.Graphics.FillPath(shadowBrush, shadowPath);
        
        // Draw card
        var cardRect = new Rectangle(0, 0, cardPanel.Width - 1, cardPanel.Height - 1);
        using var path = RoundedRect(cardRect, 25);
        using var brush = new SolidBrush(Color.White);
        e.Graphics.FillPath(brush, path);
    }

    private void PnlInitials_Paint(object? sender, PaintEventArgs e)
    {
        // Draw gradient background for initials
        using var brush = new LinearGradientBrush(
            pnlInitials.ClientRectangle,
            UIHelper.PrimaryPurple,
            UIHelper.PrimaryPink,
            LinearGradientMode.ForwardDiagonal);
        
        using var path = RoundedRect(new Rectangle(0, 0, pnlInitials.Width - 1, pnlInitials.Height - 1), 20);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.FillPath(brush, path);
    }

    private void BtnPass_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Button btn) return;
        int size = Math.Min(btn.Width, btn.Height);
        var rect = new Rectangle(0, 0, size, size);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        using var path = new GraphicsPath();
        path.AddEllipse(rect);
        
        // Gradient from light red to red
        using var brush = new LinearGradientBrush(
            rect,
            Color.FromArgb(254, 202, 202),
            Color.FromArgb(252, 165, 165),
            LinearGradientMode.ForwardDiagonal);
        
        e.Graphics.FillPath(brush, path);
        
        // Draw text
        using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        using var textBrush = new SolidBrush(Color.FromArgb(220, 38, 38));
        e.Graphics.DrawString(btn.Text, btn.Font, textBrush, rect, sf);
        
        btn.Region = new Region(path);
    }

    private void BtnLike_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Button btn) return;
        int size = Math.Min(btn.Width, btn.Height);
        var rect = new Rectangle(0, 0, size, size);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        using var path = new GraphicsPath();
        path.AddEllipse(rect);
        
        // Gradient from light green to green
        using var brush = new LinearGradientBrush(
            rect,
            Color.FromArgb(187, 247, 208),
            Color.FromArgb(134, 239, 172),
            LinearGradientMode.ForwardDiagonal);
        
        e.Graphics.FillPath(brush, path);
        
        // Draw text
        using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        using var textBrush = new SolidBrush(Color.FromArgb(22, 163, 74));
        e.Graphics.DrawString(btn.Text, btn.Font, textBrush, rect, sf);
        
        btn.Region = new Region(path);
    }

    private void BtnSuperLike_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Button btn) return;
        int size = Math.Min(btn.Width, btn.Height);
        var rect = new Rectangle(0, 0, size, size);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        using var path = new GraphicsPath();
        path.AddEllipse(rect);
        
        // Gradient from light yellow to gold
        using var brush = new LinearGradientBrush(
            rect,
            Color.FromArgb(254, 240, 138),
            Color.FromArgb(250, 204, 21),
            LinearGradientMode.ForwardDiagonal);
        
        e.Graphics.FillPath(brush, path);
        
        // Draw text
        using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        using var textBrush = new SolidBrush(Color.FromArgb(161, 98, 7));
        e.Graphics.DrawString(btn.Text, btn.Font, textBrush, rect, sf);
        
        btn.Region = new Region(path);
    }

    private void BtnUndo_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Button btn) return;
        int size = Math.Min(btn.Width, btn.Height);
        var rect = new Rectangle(0, 0, size, size);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        using var path = new GraphicsPath();
        path.AddEllipse(rect);
        
        // Gradient from light indigo to indigo
        using var brush = new LinearGradientBrush(
            rect,
            Color.FromArgb(224, 231, 255),
            Color.FromArgb(199, 210, 254),
            LinearGradientMode.ForwardDiagonal);
        
        e.Graphics.FillPath(brush, path);
        
        // Draw text
        using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        using var textBrush = new SolidBrush(Color.FromArgb(79, 70, 229));
        e.Graphics.DrawString(btn.Text, btn.Font, textBrush, rect, sf);
        
        btn.Region = new Region(path);
    }
}
