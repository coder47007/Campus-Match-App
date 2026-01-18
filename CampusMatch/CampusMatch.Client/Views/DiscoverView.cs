using CampusMatch.Client.Services;
using CampusMatch.Shared.DTOs;
using System.Drawing.Drawing2D;

namespace CampusMatch.Client.Views;

public partial class DiscoverView : UserControl
{
    private readonly ApiService _api;
    private List<StudentDto> profiles = new();
    private int currentIndex = 0;
    private System.Windows.Forms.Timer? animationTimer;
    private int animationOffset = 0;
    private bool animatingLeft = false;
    private int currentPhotoIndex = 0;  // Track which photo in current profile is shown
    private List<PhotoDto> currentProfilePhotos = new();  // Current profile's photos

    public DiscoverView(ApiService api)
    {
        _api = api;
        InitializeComponent();
        SetupAnimations();
    }

    private void SetupAnimations()
    {
        animationTimer = new System.Windows.Forms.Timer { Interval = 16 }; // ~60fps
        animationTimer.Tick += AnimationTimer_Tick;
    }

    private void AnimationTimer_Tick(object? sender, EventArgs e)
    {
        animationOffset += animatingLeft ? -150 : 150;
        cardPanel.Left = 160 + animationOffset;
        cardPanel.Refresh();

        if (Math.Abs(animationOffset) > 400)
        {
            animationTimer?.Stop();
            animationOffset = 0;
            cardPanel.Left = 160;
            currentIndex++;
            ShowCurrentProfile();
        }
    }

    public async void LoadProfiles()
    {
        loadingOverlay.Show("Finding matches...");
        try
        {
            profiles = await _api.DiscoverProfilesAsync();
            currentIndex = 0;
            ShowCurrentProfile();
            
            // Also load likes count
            await LoadLikesCount();
        }
        finally
        {
            loadingOverlay.Hide();
        }
    }

    private void ShowCurrentProfile()
    {
        if (currentIndex >= profiles.Count)
        {
            cardPanel.Visible = false;
            btnPass.Visible = false;
            btnLike.Visible = false;
            btnSuperLike.Visible = false;
            lblEmpty.Visible = true;
            return;
        }

        cardPanel.Visible = true;
        btnPass.Visible = true;
        btnLike.Visible = true;
        btnSuperLike.Visible = true;
        lblEmpty.Visible = false;

        var profile = profiles[currentIndex];
        lblName.Text = $"{profile.Name}, {profile.Age ?? 0}";
        lblInfo.Text = $"📚 {profile.Major ?? "Undeclared"} • {profile.Year ?? "Student"}";
        lblBio.Text = profile.Bio ?? "";

        // Reset photo index for new profile
        currentPhotoIndex = 0;
        currentProfilePhotos = profile.Photos?.ToList() ?? new();
        
        // Show photo navigation if there are multiple photos
        UpdatePhotoNavigation();
        
        // Load first photo
        LoadCurrentPhoto(profile);
    }
    
    private void LoadCurrentPhoto(StudentDto profile)
    {
        // Try to load photo from Photos collection first
        if (currentProfilePhotos.Count > 0 && currentPhotoIndex < currentProfilePhotos.Count)
        {
            try
            {
                picProfile.Load(currentProfilePhotos[currentPhotoIndex].Url);
                pnlInitials.Visible = false;
                picProfile.Visible = true;
                return;
            }
            catch { }
        }
        
        // Fallback to PhotoUrl
        if (currentPhotoIndex == 0 && !string.IsNullOrEmpty(profile.PhotoUrl))
        {
            try
            {
                picProfile.Load(profile.PhotoUrl);
                pnlInitials.Visible = false;
                picProfile.Visible = true;
            }
            catch
            {
                ShowInitials(profile.Name);
            }
        }
        else
        {
            ShowInitials(profile.Name);
        }
    }
    
    private void UpdatePhotoNavigation()
    {
        bool hasMultiplePhotos = currentProfilePhotos.Count > 1;
        
        btnPrevPhoto.Visible = hasMultiplePhotos;
        btnNextPhoto.Visible = hasMultiplePhotos;
        pnlPhotoIndicators.Visible = hasMultiplePhotos;
        
        btnPrevPhoto.Enabled = currentPhotoIndex > 0;
        btnNextPhoto.Enabled = currentPhotoIndex < currentProfilePhotos.Count - 1;
        
        // Update indicator dots
        UpdatePhotoIndicators();
    }
    
    private void UpdatePhotoIndicators()
    {
        pnlPhotoIndicators.Controls.Clear();
        
        if (currentProfilePhotos.Count <= 1) return;
        
        int dotSize = 10;
        int spacing = 6;
        int totalWidth = (dotSize + spacing) * currentProfilePhotos.Count - spacing;
        int startX = (pnlPhotoIndicators.Width - totalWidth) / 2;
        
        for (int i = 0; i < currentProfilePhotos.Count; i++)
        {
            var dot = new Panel
            {
                Size = new Size(dotSize, dotSize),
                Location = new Point(startX + (dotSize + spacing) * i, 5),
                BackColor = i == currentPhotoIndex 
                    ? Color.FromArgb(124, 58, 237)  // Active - purple
                    : Color.FromArgb(180, 180, 180) // Inactive - gray
            };
            pnlPhotoIndicators.Controls.Add(dot);
        }
    }
    
    private void BtnPrevPhoto_Click(object? sender, EventArgs e)
    {
        if (currentPhotoIndex > 0)
        {
            currentPhotoIndex--;
            LoadCurrentPhoto(profiles[currentIndex]);
            UpdatePhotoNavigation();
        }
    }
    
    private void BtnNextPhoto_Click(object? sender, EventArgs e)
    {
        if (currentPhotoIndex < currentProfilePhotos.Count - 1)
        {
            currentPhotoIndex++;
            LoadCurrentPhoto(profiles[currentIndex]);
            UpdatePhotoNavigation();
        }
    }

    private void ShowInitials(string name)
    {
        picProfile.Visible = false;
        pnlInitials.Visible = true;

        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var initials = parts.Length >= 2
            ? $"{parts[0][0]}{parts[^1][0]}"
            : name.Length > 0 ? name[0].ToString() : "?";
        lblInitials.Text = initials.ToUpper();
    }

    private async void BtnPass_Click(object? sender, EventArgs e)
    {
        animatingLeft = true;
        animationTimer?.Start();
        await SwipeAsync(false);
    }

    private async void BtnLike_Click(object? sender, EventArgs e)
    {
        animatingLeft = false;
        animationTimer?.Start();
        await SwipeAsync(true);
    }

    private async void BtnSuperLike_Click(object? sender, EventArgs e)
    {
        // Flash effect for super like
        btnSuperLike.BackColor = Color.FromArgb(234, 179, 8);
        await Task.Delay(200);
        btnSuperLike.BackColor = Color.FromArgb(254, 249, 195);

        animatingLeft = false;
        animationTimer?.Start();
        await SwipeAsync(true, isSuperLike: true);
    }

    private async Task SwipeAsync(bool isLike, bool isSuperLike = false)
    {
        if (currentIndex >= profiles.Count) return;

        var profile = profiles[currentIndex];
        var result = await _api.SwipeAsync(profile.Id, isLike);

        if (result?.IsMatch == true)
        {
            ShowMatchAnimation(profile.Name);
        }
    }

    private void ShowMatchAnimation(string name)
    {
        // Custom match dialog with animation
        using var matchForm = new Form
        {
            Text = "It's a Match!",
            Size = new Size(350, 280),
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.None,
            BackColor = Color.FromArgb(124, 58, 237)
        };

        var btnClose = new Button
        {
            Text = "✕",
            Size = new Size(30, 30),
            Location = new Point(310, 10),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.Transparent,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnClose.FlatAppearance.BorderSize = 0;
        btnClose.Click += (s, e) => matchForm.DialogResult = DialogResult.Cancel;

        var lblMatch = new Label
        {
            Text = "🎉",
            Font = new Font("Segoe UI Emoji", 48),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(130, 30)
        };

        var lblText = new Label
        {
            Text = $"You matched with\n{name}!",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(350, 80),
            Location = new Point(0, 110)
        };

        var btnOk = new Button
        {
            Text = "💬 Start Chatting",
            Size = new Size(200, 45),
            Location = new Point(75, 210),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(124, 58, 237),
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnOk.FlatAppearance.BorderSize = 0;
        btnOk.Click += (s, e) => matchForm.DialogResult = DialogResult.OK;

        matchForm.Controls.AddRange(new Control[] { btnClose, lblMatch, lblText, btnOk });
        
        var result = matchForm.ShowDialog(this.ParentForm);
        
        // Navigate to matches view if user clicked Start Chatting
        if (result == DialogResult.OK && this.ParentForm is Forms.MainForm mainForm)
        {
            // Trigger navigation to matches view
            mainForm.NavigateToMatches();
        }
    }

    private async void BtnUndo_Click(object? sender, EventArgs e)
    {
        btnUndo.Enabled = false;

        var result = await _api.UndoLastSwipeAsync();

        if (result != null && result.Success)
        {
            // Reload profiles to get the undone profile back
            LoadProfiles();
            MessageBox.Show(result.Message, "Undo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            MessageBox.Show(result?.Message ?? "Cannot undo swipe right now.", "Undo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        btnUndo.Enabled = true;
    }

    private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        path.AddArc(bounds.X, bounds.Y, radius * 2, radius * 2, 180, 90);
        path.AddArc(bounds.Right - radius * 2, bounds.Y, radius * 2, radius * 2, 270, 90);
        path.AddArc(bounds.Right - radius * 2, bounds.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
        path.CloseFigure();
        return path;
    }

    private void lblHint_Click(object sender, EventArgs e)
    {

    }
    
    private async void BtnReport_Click(object? sender, EventArgs e)
    {
        if (currentIndex >= profiles.Count) return;
        
        var profile = profiles[currentIndex];
        
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
            Text = $"⚠️ Report {profile.Name}",
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
        cboReason.Items.AddRange(new[] { "Inappropriate Content", "Spam", "Harassment", "Fake Profile", "Other" });
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
            
            var success = await _api.ReportUserAsync(profile.Id, reason, details, "discover");
            if (success)
            {
                CampusMatch.Client.Controls.ToastNotification.ShowSuccess("Report submitted. Thank you!");
                dialog.Close();
            }
            else
            {
                CampusMatch.Client.Controls.ToastNotification.ShowError("Failed to submit report.");
            }
        };
        
        btnCancel.Click += (s, args) => dialog.Close();
        
        dialog.Controls.AddRange(new Control[] { lblTitle, lblReason, cboReason, lblDetails, txtDetails, btnSubmit, btnCancel });
        dialog.ShowDialog();
    }
    
    // Who Liked You functionality
    private List<LikePreviewDto> receivedLikes = new();
    
    public async Task LoadLikesCount()
    {
        try
        {
            var count = await _api.GetLikesCountAsync();
            receivedLikes = await _api.GetReceivedLikesAsync();
            
            lblLikesCount.Text = count.ToString();
            pnlWhoLikedYou.Visible = count > 0;
            pnlLikesPreview.Invalidate();
        }
        catch
        {
            pnlWhoLikedYou.Visible = false;
        }
    }
    
    private void PnlWhoLikedYou_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        
        // Draw rounded rectangle background
        using var path = new System.Drawing.Drawing2D.GraphicsPath();
        var rect = new Rectangle(0, 0, pnlWhoLikedYou.Width - 1, pnlWhoLikedYou.Height - 1);
        int radius = 15;
        path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
        path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
        path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
        path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
        path.CloseFigure();
        
        using var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
            rect, 
            Color.FromArgb(55, 50, 100), 
            Color.FromArgb(45, 40, 90), 
            System.Drawing.Drawing2D.LinearGradientMode.Vertical);
        g.FillPath(brush, path);
        
        using var borderPen = new Pen(Color.FromArgb(80, 124, 58, 237), 2);
        g.DrawPath(borderPen, path);
    }
    
    private void PnlLikesPreview_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        
        // Draw blurred circle placeholders for likes
        int circleSize = 35;
        int spacing = 10;
        int x = 0;
        
        for (int i = 0; i < Math.Min(3, receivedLikes.Count); i++)
        {
            var like = receivedLikes[i];
            
            // Draw blurred circle
            using var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new Rectangle(x, 10, circleSize, circleSize),
                Color.FromArgb(180, 124, 58, 237),
                Color.FromArgb(180, 236, 72, 153),
                System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal);
            g.FillEllipse(brush, x, 10, circleSize, circleSize);
            
            // Draw first letter
            using var font = new Font("Segoe UI", 14f, FontStyle.Bold);
            using var textBrush = new SolidBrush(Color.White);
            var textSize = g.MeasureString(like.FirstLetter, font);
            g.DrawString(like.FirstLetter, font, textBrush, 
                x + (circleSize - textSize.Width) / 2, 
                10 + (circleSize - textSize.Height) / 2);
            
            // Super like indicator
            if (like.IsSuperLike)
            {
                using var starFont = new Font("Segoe UI", 10f);
                g.DrawString("⭐", starFont, textBrush, x + circleSize - 12, 8);
            }
            
            x += circleSize + spacing;
        }
        
        // Show "+N" if more likes
        if (receivedLikes.Count > 3)
        {
            using var font = new Font("Segoe UI", 10f, FontStyle.Bold);
            using var brush = new SolidBrush(Color.FromArgb(200, 200, 220));
            g.DrawString($"+{receivedLikes.Count - 3}", font, brush, x, 55);
        }
    }
    
    private async void PnlWhoLikedYou_Click(object? sender, EventArgs e)
    {
        if (receivedLikes.Count == 0) return;
        
        var result = MessageBox.Show(
            $"You have {receivedLikes.Count} people who like you! 💕\n\nWould you like to see who they are?",
            "Who Liked You",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        
        if (result == DialogResult.Yes)
        {
            // Show the first like as a potential match
            CampusMatch.Client.Controls.ToastNotification.ShowInfo("Check your Discover feed to find your admirers!");
            await LoadLikesCount();
        }
    }
}

