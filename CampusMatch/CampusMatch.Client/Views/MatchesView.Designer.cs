#nullable enable

using CampusMatch.Client.Helpers;
using CampusMatch.Client.Controls;
using System.Drawing.Drawing2D;

namespace CampusMatch.Client.Views;

partial class MatchesView
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
        typingTimer?.Dispose();
        base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        pnlMatchesHeader = new Panel();
        lblMatchesTitle = new Label();
        pnlMatchesList = new Panel();
        lblNoMatches = new Label();
        pnlChat = new Panel();
        pnlChatHeader = new Panel();
        lblChatHeader = new Label();
        lblOnlineStatus = new Label();
        pnlMessages = new Panel();
        lblTyping = new Label();
        pnlInput = new Panel();
        btnEmoji = new Button();
        txtMessage = new TextBox();
        btnSend = new Button();
        pnlEmoji = new FlowLayoutPanel();
        btnReportMatch = new Button();
        loadingOverlay = new LoadingOverlay();
        pnlMatchesHeader.SuspendLayout();
        pnlMatchesList.SuspendLayout();
        pnlChat.SuspendLayout();
        pnlChatHeader.SuspendLayout();
        pnlInput.SuspendLayout();
        SuspendLayout();
        // 
        // pnlMatchesHeader
        // 
        pnlMatchesHeader.BackColor = Color.FromArgb(35, 30, 70);
        pnlMatchesHeader.Controls.Add(lblMatchesTitle);
        pnlMatchesHeader.Location = new Point(0, 0);
        pnlMatchesHeader.Name = "pnlMatchesHeader";
        pnlMatchesHeader.Size = new Size(260, 50);
        pnlMatchesHeader.TabIndex = 0;
        pnlMatchesHeader.Paint += PnlMatchesHeader_Paint;
        // 
        // lblMatchesTitle
        // 
        lblMatchesTitle.BackColor = Color.Transparent;
        lblMatchesTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblMatchesTitle.ForeColor = Color.White;
        lblMatchesTitle.Location = new Point(10, 10);
        lblMatchesTitle.Name = "lblMatchesTitle";
        lblMatchesTitle.Size = new Size(240, 30);
        lblMatchesTitle.TabIndex = 0;
        lblMatchesTitle.Text = "💬 Matches";
        // 
        // pnlMatchesList
        // 
        pnlMatchesList.AutoScroll = true;
        pnlMatchesList.BackColor = Color.FromArgb(40, 35, 80);
        pnlMatchesList.Controls.Add(lblNoMatches);
        pnlMatchesList.Location = new Point(0, 50);
        pnlMatchesList.Name = "pnlMatchesList";
        pnlMatchesList.Size = new Size(260, 560);
        pnlMatchesList.TabIndex = 1;
        // 
        // lblNoMatches
        // 
        lblNoMatches.Font = new Font("Segoe UI", 11F);
        lblNoMatches.ForeColor = Color.FromArgb(150, 150, 170);
        lblNoMatches.Location = new Point(10, 200);
        lblNoMatches.Name = "lblNoMatches";
        lblNoMatches.Size = new Size(240, 80);
        lblNoMatches.TabIndex = 0;
        lblNoMatches.Text = "No matches yet 💔\r\n\r\nKeep swiping to find\r\nyour campus connection!";
        lblNoMatches.TextAlign = ContentAlignment.MiddleCenter;
        lblNoMatches.Visible = false;
        // 
        // pnlChat
        // 
        pnlChat.BackColor = Color.FromArgb(35, 30, 70);
        pnlChat.Controls.Add(pnlChatHeader);
        pnlChat.Controls.Add(pnlMessages);
        pnlChat.Controls.Add(lblTyping);
        pnlChat.Controls.Add(pnlInput);
        pnlChat.Controls.Add(pnlEmoji);
        pnlChat.Location = new Point(270, 0);
        pnlChat.Name = "pnlChat";
        pnlChat.Size = new Size(481, 610);
        pnlChat.TabIndex = 2;
        pnlChat.Visible = false;
        pnlChat.Paint += pnlChat_Paint;
        // 
        // pnlChatHeader
        // 
        pnlChatHeader.BackColor = Color.FromArgb(45, 40, 90);
        pnlChatHeader.Controls.Add(btnReportMatch);
        pnlChatHeader.Controls.Add(lblChatHeader);
        pnlChatHeader.Controls.Add(lblOnlineStatus);
        pnlChatHeader.Dock = DockStyle.Top;
        pnlChatHeader.Location = new Point(0, 0);
        pnlChatHeader.Name = "pnlChatHeader";
        pnlChatHeader.Size = new Size(481, 60);
        pnlChatHeader.TabIndex = 0;
        pnlChatHeader.Paint += PnlChatHeader_Paint;
        // 
        // lblChatHeader
        // 
        lblChatHeader.BackColor = Color.Transparent;
        lblChatHeader.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblChatHeader.ForeColor = Color.White;
        lblChatHeader.Location = new Point(15, 10);
        lblChatHeader.Name = "lblChatHeader";
        lblChatHeader.Size = new Size(300, 25);
        lblChatHeader.TabIndex = 0;
        lblChatHeader.Text = "Chat";
        // 
        // lblOnlineStatus
        // 
        lblOnlineStatus.BackColor = Color.Transparent;
        lblOnlineStatus.Font = new Font("Segoe UI", 9F);
        lblOnlineStatus.ForeColor = Color.FromArgb(34, 197, 94);
        lblOnlineStatus.Location = new Point(15, 35);
        lblOnlineStatus.Name = "lblOnlineStatus";
        lblOnlineStatus.Size = new Size(100, 20);
        lblOnlineStatus.TabIndex = 1;
        lblOnlineStatus.Text = "● Online";
        // 
        // btnReportMatch
        // 
        btnReportMatch.BackColor = Color.FromArgb(80, 239, 68, 68);
        btnReportMatch.Cursor = Cursors.Hand;
        btnReportMatch.FlatAppearance.BorderSize = 0;
        btnReportMatch.FlatAppearance.MouseOverBackColor = Color.FromArgb(239, 68, 68);
        btnReportMatch.FlatStyle = FlatStyle.Flat;
        btnReportMatch.Font = new Font("Segoe UI Emoji", 10F);
        btnReportMatch.ForeColor = Color.White;
        btnReportMatch.Location = new Point(420, 15);
        btnReportMatch.Name = "btnReportMatch";
        btnReportMatch.Size = new Size(50, 30);
        btnReportMatch.TabIndex = 2;
        btnReportMatch.Text = "⚠️";
        btnReportMatch.UseVisualStyleBackColor = false;
        btnReportMatch.Click += BtnReportMatch_Click;
        // 
        // pnlMessages
        // 
        pnlMessages.AutoScroll = true;
        pnlMessages.BackColor = Color.FromArgb(30, 27, 75);
        pnlMessages.Location = new Point(0, 60);
        pnlMessages.Name = "pnlMessages";
        pnlMessages.Size = new Size(520, 460);
        pnlMessages.TabIndex = 1;
        // 
        // lblTyping
        // 
        lblTyping.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
        lblTyping.ForeColor = Color.FromArgb(180, 180, 200);
        lblTyping.Location = new Point(15, 525);
        lblTyping.Name = "lblTyping";
        lblTyping.Size = new Size(200, 20);
        lblTyping.TabIndex = 2;
        lblTyping.Text = "typing...";
        lblTyping.Visible = false;
        // 
        // pnlInput
        // 
        pnlInput.BackColor = Color.FromArgb(45, 40, 90);
        pnlInput.Controls.Add(btnEmoji);
        pnlInput.Controls.Add(txtMessage);
        pnlInput.Controls.Add(btnSend);
        pnlInput.Location = new Point(0, 550);
        pnlInput.Name = "pnlInput";
        pnlInput.Size = new Size(478, 60);
        pnlInput.TabIndex = 3;
        pnlInput.Paint += PnlInput_Paint;
        // 
        // btnEmoji
        // 
        btnEmoji.BackColor = Color.Transparent;
        btnEmoji.Cursor = Cursors.Hand;
        btnEmoji.FlatAppearance.BorderSize = 0;
        btnEmoji.FlatStyle = FlatStyle.Flat;
        btnEmoji.Font = new Font("Segoe UI Emoji", 16F);
        btnEmoji.Location = new Point(10, 12);
        btnEmoji.Name = "btnEmoji";
        btnEmoji.Size = new Size(40, 36);
        btnEmoji.TabIndex = 0;
        btnEmoji.Text = "😊";
        btnEmoji.UseVisualStyleBackColor = false;
        btnEmoji.Click += BtnEmoji_Click;
        // 
        // txtMessage
        // 
        txtMessage.BackColor = Color.FromArgb(60, 55, 105);
        txtMessage.BorderStyle = BorderStyle.None;
        txtMessage.Font = new Font("Segoe UI", 11F);
        txtMessage.ForeColor = Color.White;
        txtMessage.Location = new Point(55, 18);
        txtMessage.Name = "txtMessage";
        txtMessage.PlaceholderText = "Type a message...";
        txtMessage.Size = new Size(348, 20);
        txtMessage.TabIndex = 1;
        txtMessage.TextChanged += TxtMessage_TextChanged;
        txtMessage.KeyDown += TxtMessage_KeyDown;
        // 
        // btnSend
        // 
        btnSend.BackColor = Color.FromArgb(124, 58, 237);
        btnSend.Cursor = Cursors.Hand;
        btnSend.FlatAppearance.BorderSize = 0;
        btnSend.FlatAppearance.MouseOverBackColor = Color.FromArgb(139, 92, 246);
        btnSend.FlatStyle = FlatStyle.Flat;
        btnSend.Font = new Font("Segoe UI", 12F);
        btnSend.ForeColor = Color.White;
        btnSend.Location = new Point(409, 12);
        btnSend.Name = "btnSend";
        btnSend.Size = new Size(60, 40);
        btnSend.TabIndex = 2;
        btnSend.Text = "➤";
        btnSend.UseVisualStyleBackColor = false;
        btnSend.Click += BtnSend_Click;
        btnSend.Paint += BtnSend_Paint;
        // 
        // pnlEmoji
        // 
        pnlEmoji.BackColor = Color.FromArgb(60, 55, 105);
        pnlEmoji.Location = new Point(10, 400);
        pnlEmoji.Name = "pnlEmoji";
        pnlEmoji.Padding = new Padding(5);
        pnlEmoji.Size = new Size(300, 120);
        pnlEmoji.TabIndex = 4;
        pnlEmoji.Visible = false;
        // 
        // loadingOverlay
        // 
        loadingOverlay.Dock = DockStyle.Fill;
        loadingOverlay.Visible = false;
        // 
        // MatchesView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(30, 27, 75);
        Controls.Add(loadingOverlay);
        Controls.Add(pnlMatchesHeader);
        Controls.Add(pnlMatchesList);
        Controls.Add(pnlChat);
        Name = "MatchesView";
        Size = new Size(777, 620);
        pnlMatchesHeader.ResumeLayout(false);
        pnlMatchesList.ResumeLayout(false);
        pnlChat.ResumeLayout(false);
        pnlChatHeader.ResumeLayout(false);
        pnlInput.ResumeLayout(false);
        pnlInput.PerformLayout();
        ResumeLayout(false);
    }

    private void PopulateEmojis()
    {
        string[] emojis = { "😊", "😂", "❤️", "😍", "🥰", "😘", "🔥", "✨", "💯", "👍", 
                          "🎉", "💜", "💕", "😎", "🤗", "😇", "🥳", "💪", "🙌", "👋" };
        
        foreach (var emoji in emojis)
        {
            var btn = new Button
            {
                Text = emoji,
                Size = new Size(40, 40),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Emoji", 16),
                Cursor = Cursors.Hand,
                Margin = new Padding(2)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) => EmojiSelected(emoji);
            pnlEmoji.Controls.Add(btn);
        }
    }

    #endregion

    private System.Windows.Forms.Panel pnlMatchesHeader;
    private System.Windows.Forms.Label lblMatchesTitle;
    private System.Windows.Forms.Panel pnlMatchesList;
    private System.Windows.Forms.Label lblNoMatches;
    private System.Windows.Forms.Panel pnlChat;
    private System.Windows.Forms.Panel pnlChatHeader;
    private System.Windows.Forms.Label lblChatHeader;
    private System.Windows.Forms.Label lblOnlineStatus;
    private System.Windows.Forms.Panel pnlMessages;
    private System.Windows.Forms.Label lblTyping;
    private System.Windows.Forms.Panel pnlInput;
    private System.Windows.Forms.Button btnEmoji;
    private System.Windows.Forms.TextBox txtMessage;
    private System.Windows.Forms.Button btnSend;
    private System.Windows.Forms.FlowLayoutPanel pnlEmoji;
    private System.Windows.Forms.Button btnReportMatch;
    private LoadingOverlay loadingOverlay;

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

    private void PnlMatchesHeader_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Panel panel) return;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        // Solid purple header
        using var brush = new SolidBrush(UIHelper.PrimaryPurple);
        e.Graphics.FillRectangle(brush, panel.ClientRectangle);
    }

    private void PnlChatHeader_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Panel panel) return;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        // Solid purple header
        using var brush = new SolidBrush(UIHelper.PrimaryPurple);
        e.Graphics.FillRectangle(brush, panel.ClientRectangle);
    }

    private void PnlInput_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Panel panel) return;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        // Draw rounded input background where the textbox is
        var inputRect = new Rectangle(50, 10, 390, 40);
        using var path = RoundedRect(inputRect, 20);
        using var brush = new SolidBrush(UIHelper.InputBackground);
        e.Graphics.FillPath(brush, path);
        
        // Subtle border
        using var pen = new Pen(Color.FromArgb(60, 124, 58, 237), 1);
        e.Graphics.DrawPath(pen, path);
    }

    private void BtnSend_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Button btn) return;
        var rect = new Rectangle(0, 0, btn.Width, btn.Height);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        using var path = RoundedRect(rect, 12);
        using var brush = new LinearGradientBrush(
            rect,
            UIHelper.PrimaryPurple,
            Color.FromArgb(168, 85, 247),
            LinearGradientMode.Horizontal);
        
        e.Graphics.FillPath(brush, path);
        
        // Draw text
        using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        e.Graphics.DrawString(btn.Text, btn.Font, Brushes.White, rect, sf);
        
        btn.Region = new Region(path);
    }
}
