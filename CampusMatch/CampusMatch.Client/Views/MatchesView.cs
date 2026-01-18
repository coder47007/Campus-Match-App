using CampusMatch.Client.Services;
using CampusMatch.Shared.DTOs;

namespace CampusMatch.Client.Views;

public partial class MatchesView : UserControl
{
    private readonly ApiService _api;
    private readonly SignalRService _signalR;
    private List<MatchDto> matches = new();
    private int? currentMatchId;
    private int? currentRecipientId;
    private System.Windows.Forms.Timer? typingTimer;
    private bool isTyping = false;

    public MatchesView(ApiService api, SignalRService signalR)
    {
        _api = api;
        _signalR = signalR;
        InitializeComponent();
        SetupTypingIndicator();
        SetupSignalRHandlers();
    }

    private void SetupTypingIndicator()
    {
        typingTimer = new System.Windows.Forms.Timer { Interval = 2000 };
        typingTimer.Tick += (s, e) =>
        {
            typingTimer?.Stop();
            isTyping = false;
            _ = _signalR.SendTypingIndicatorAsync(currentRecipientId ?? 0, false);
        };
    }

    private void SetupSignalRHandlers()
    {
        _signalR.OnTypingIndicator += (senderId, typing) =>
        {
            if (senderId == currentRecipientId)
            {
                this.BeginInvoke(() =>
                {
                    lblTyping.Visible = typing;
                });
            }
        };
    }

    public async void RefreshMatches()
    {
        loadingOverlay.Show("Loading matches...");
        try
        {
            matches = await _api.GetMatchesAsync();
            pnlMatchesList.Controls.Clear();

            int yPos = 5;
            foreach (var match in matches)
            {
                var matchPanel = CreateMatchItem(match, yPos);
                pnlMatchesList.Controls.Add(matchPanel);
                yPos += 75;
            }

            if (matches.Count == 0)
            {
                lblNoMatches.Visible = true;
            }
            else
            {
                lblNoMatches.Visible = false;
            }
        }
        finally
        {
            loadingOverlay.Hide();
        }
    }

    private Panel CreateMatchItem(MatchDto match, int yPos)
    {
        var panel = new Panel
        {
            Size = new Size(240, 70),
            Location = new Point(5, yPos),
            BackColor = Color.FromArgb(45, 40, 90),
            Cursor = Cursors.Hand,
            Tag = match
        };

        // Avatar with initials
        var avatar = new Panel
        {
            Size = new Size(50, 50),
            Location = new Point(10, 10),
            BackColor = Color.FromArgb(124, 58, 237)
        };
        avatar.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                avatar.ClientRectangle,
                Color.FromArgb(124, 58, 237),
                Color.FromArgb(236, 72, 153),
                System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal);
            e.Graphics.FillEllipse(brush, 0, 0, 49, 49);

            var initials = GetInitials(match.OtherStudentName);
            using var font = new Font("Segoe UI", 14, FontStyle.Bold);
            var size = e.Graphics.MeasureString(initials, font);
            e.Graphics.DrawString(initials, font, Brushes.White,
                (50 - size.Width) / 2, (50 - size.Height) / 2);
        };

        var lblName = new Label
        {
            Text = match.OtherStudentName,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(70, 12),
            AutoSize = true
        };

        var lblMajor = new Label
        {
            Text = match.OtherStudentMajor ?? "Student",
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(180, 180, 200),
            Location = new Point(70, 35),
            AutoSize = true
        };

        // Online indicator
        var onlineDot = new Panel
        {
            Size = new Size(10, 10),
            Location = new Point(220, 30),
            BackColor = Color.FromArgb(34, 197, 94)
        };
        onlineDot.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.FillEllipse(Brushes.LimeGreen, 0, 0, 9, 9);
        };

        panel.Controls.AddRange(new Control[] { avatar, lblName, lblMajor, onlineDot });

        panel.Click += async (s, e) => await SelectMatch(match);
        foreach (Control c in panel.Controls)
            c.Click += async (s, e) => await SelectMatch(match);

        panel.MouseEnter += (s, e) => panel.BackColor = Color.FromArgb(55, 50, 100);
        panel.MouseLeave += (s, e) => panel.BackColor = Color.FromArgb(45, 40, 90);

        return panel;
    }

    private string GetInitials(string name)
    {
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2
            ? $"{parts[0][0]}{parts[^1][0]}"
            : name.Length > 0 ? name[0].ToString() : "?";
    }

    private async Task SelectMatch(MatchDto match)
    {
        currentMatchId = match.Id;
        currentRecipientId = match.OtherStudentId;
        lblChatHeader.Text = match.OtherStudentName;
        pnlChat.Visible = true;

        await LoadChat();
    }

    private async Task LoadChat()
    {
        if (!currentMatchId.HasValue) return;

        var messages = await _api.GetMessagesAsync(currentMatchId.Value);
        pnlMessages.Controls.Clear();

        int yPos = 10;
        foreach (var msg in messages)
        {
            var bubble = CreateMessageBubble(msg, ref yPos);
            pnlMessages.Controls.Add(bubble);
        }

        // Scroll to bottom
        pnlMessages.AutoScrollPosition = new Point(0, pnlMessages.VerticalScroll.Maximum);
    }

    private Panel CreateMessageBubble(MessageDto msg, ref int yPos)
    {
        bool isMine = msg.SenderId == _api.CurrentStudent?.Id;

        var bubble = new Panel
        {
            AutoSize = true,
            MaximumSize = new Size(320, 0),
            MinimumSize = new Size(80, 40),
            BackColor = isMine ? Color.FromArgb(124, 58, 237) : Color.FromArgb(60, 55, 105),
            Padding = new Padding(12, 8, 12, 8)
        };

        var lblText = new Label
        {
            Text = msg.Content,
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.White,
            AutoSize = true,
            MaximumSize = new Size(296, 0),
            Location = new Point(12, 8)
        };

        // Build time string with read receipts for sent messages
        string timeText = msg.SentAt.ToString("h:mm tt");
        string statusIcon = "";
        Color statusColor = Color.FromArgb(150, 150, 170);

        if (isMine)
        {
            if (msg.ReadAt.HasValue)
            {
                // Read - double blue checkmarks
                statusIcon = " ✓✓";
                statusColor = Color.FromArgb(59, 130, 246); // Blue
            }
            else if (msg.DeliveredAt.HasValue)
            {
                // Delivered - double gray checkmarks
                statusIcon = " ✓✓";
                statusColor = Color.FromArgb(150, 150, 170); // Gray
            }
            else
            {
                // Sent (not delivered yet) - single gray checkmark
                statusIcon = " ✓";
                statusColor = Color.FromArgb(150, 150, 170); // Gray
            }
        }

        var lblTime = new Label
        {
            Text = timeText,
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(180, 180, 200),
            AutoSize = true,
            Location = new Point(12, lblText.Bottom + 4)
        };

        // Add status indicator for sent messages
        Label? lblStatus = null;
        if (isMine && !string.IsNullOrEmpty(statusIcon))
        {
            lblStatus = new Label
            {
                Text = statusIcon,
                Font = new Font("Segoe UI", 8),
                ForeColor = statusColor,
                AutoSize = true,
                Location = new Point(lblTime.Right, lblText.Bottom + 4)
            };
        }

        bubble.Controls.Add(lblText);
        bubble.Controls.Add(lblTime);
        if (lblStatus != null) bubble.Controls.Add(lblStatus);
        bubble.Height = lblTime.Bottom + 8;

        bubble.Location = new Point(isMine ? 450 - bubble.Width : 10, yPos);
        bubble.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var path = CreateRoundedRectPath(new Rectangle(0, 0, bubble.Width - 1, bubble.Height - 1), 12);
            using var brush = new SolidBrush(bubble.BackColor);
            e.Graphics.FillPath(brush, path);
        };

        yPos += bubble.Height + 8;
        return bubble;
    }

    private System.Drawing.Drawing2D.GraphicsPath CreateRoundedRectPath(Rectangle bounds, int radius)
    {
        var path = new System.Drawing.Drawing2D.GraphicsPath();
        path.AddArc(bounds.X, bounds.Y, radius * 2, radius * 2, 180, 90);
        path.AddArc(bounds.Right - radius * 2, bounds.Y, radius * 2, radius * 2, 270, 90);
        path.AddArc(bounds.Right - radius * 2, bounds.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
        path.CloseFigure();
        return path;
    }

    private void TxtMessage_TextChanged(object? sender, EventArgs e)
    {
        if (!isTyping && currentRecipientId.HasValue)
        {
            isTyping = true;
            _ = _signalR.SendTypingIndicatorAsync(currentRecipientId.Value, true);
        }
        typingTimer?.Stop();
        typingTimer?.Start();
    }

    private async void TxtMessage_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter && !e.Shift)
        {
            e.SuppressKeyPress = true;
            await SendMessage();
        }
    }

    private async void BtnSend_Click(object? sender, EventArgs e)
    {
        await SendMessage();
    }

    private async void BtnEmoji_Click(object? sender, EventArgs e)
    {
        // Toggle emoji picker
        pnlEmoji.Visible = !pnlEmoji.Visible;
    }

    private void EmojiSelected(string emoji)
    {
        txtMessage.Text += emoji;
        txtMessage.Focus();
        txtMessage.SelectionStart = txtMessage.Text.Length;
        pnlEmoji.Visible = false;
    }

    private async Task SendMessage()
    {
        if (!currentMatchId.HasValue || string.IsNullOrWhiteSpace(txtMessage.Text)) return;

        var content = txtMessage.Text.Trim();
        txtMessage.Clear();
        typingTimer?.Stop();
        isTyping = false;

        var result = await _api.SendMessageAsync(currentMatchId.Value, content);
        if (result != null)
        {
            await LoadChat();
        }
    }

    public void HandleIncomingMessage(MessageDto message)
    {
        if (message.SenderId == currentRecipientId)
        {
            this.BeginInvoke(async () => await LoadChat());
        }
    }

    private void pnlChat_Paint(object sender, PaintEventArgs e)
    {

    }
    
    private async void BtnReportMatch_Click(object? sender, EventArgs e)
    {
        if (!currentRecipientId.HasValue || !currentMatchId.HasValue) return;
        
        var match = matches.FirstOrDefault(m => m.Id == currentMatchId.Value);
        if (match == null) return;
        
        // Create report dialog
        var dialog = new Form
        {
            Text = "Report User",
            Size = new Size(350, 320),
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            BackColor = Color.FromArgb(30, 27, 75)
        };
        
        var lblTitle = new Label
        {
            Text = $"⚠️ Report {match.OtherStudentName}",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(20, 20),
            AutoSize = true
        };
        
        var lblReason = new Label
        {
            Text = "Select reason:",
            Font = new Font("Segoe UI", 10F),
            ForeColor = Color.White,
            Location = new Point(20, 60),
            AutoSize = true
        };
        
        var cboReason = new ComboBox
        {
            Location = new Point(20, 85),
            Size = new Size(290, 30),
            Font = new Font("Segoe UI", 10F),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cboReason.Items.AddRange(new[] { "Inappropriate Messages", "Harassment", "Spam", "Fake Profile", "Other" });
        cboReason.SelectedIndex = 0;
        
        var lblDetails = new Label
        {
            Text = "Additional details (optional):",
            Font = new Font("Segoe UI", 10F),
            ForeColor = Color.White,
            Location = new Point(20, 125),
            AutoSize = true
        };
        
        var txtDetails = new TextBox
        {
            Location = new Point(20, 150),
            Size = new Size(290, 60),
            Multiline = true,
            BackColor = Color.FromArgb(60, 55, 105),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };
        
        var btnSubmit = new Button
        {
            Text = "Submit Report",
            Location = new Point(20, 225),
            Size = new Size(140, 35),
            BackColor = Color.FromArgb(239, 68, 68),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnSubmit.FlatAppearance.BorderSize = 0;
        
        var btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(170, 225),
            Size = new Size(140, 35),
            BackColor = Color.FromArgb(60, 55, 105),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnCancel.FlatAppearance.BorderSize = 0;
        
        btnSubmit.Click += async (s, args) =>
        {
            var reason = cboReason.SelectedItem?.ToString() ?? "Other";
            var details = string.IsNullOrWhiteSpace(txtDetails.Text) ? null : txtDetails.Text;
            
            var success = await _api.ReportUserAsync(currentRecipientId.Value, reason, details, "chat");
            if (success)
            {
                MessageBox.Show("Report submitted and user unmatched. Thank you for helping keep our community safe.", "Report Submitted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dialog.Close();
                
                // Hide chat and refresh matches since user is now unmatched
                pnlChat.Visible = false;
                currentMatchId = null;
                currentRecipientId = null;
                RefreshMatches();
            }
            else
            {
                MessageBox.Show("Failed to submit report. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };
        
        btnCancel.Click += (s, args) => dialog.Close();
        
        dialog.Controls.AddRange(new Control[] { lblTitle, lblReason, cboReason, lblDetails, txtDetails, btnSubmit, btnCancel });
        dialog.ShowDialog();
    }
}
