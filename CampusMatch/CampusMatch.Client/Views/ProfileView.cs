using CampusMatch.Client.Services;
using CampusMatch.Shared.DTOs;

namespace CampusMatch.Client.Views;

public partial class ProfileView : UserControl
{
    private readonly ApiService _api;
    private int completionPercentage = 0;
    private List<InterestDto> allInterests = new();
    private List<int> selectedInterestIds = new();
    private List<PhotoDto> photos = new();
    private List<PromptDto> availablePrompts = new();
    private List<StudentPromptDto> myPrompts = new();

    public ProfileView(ApiService api)
    {
        _api = api;
        InitializeComponent();
    }

    public async void LoadProfile()
    {
        loadingOverlay.Show("Loading profile...");
        try
        {
            var profile = await _api.GetMyProfileAsync();
            if (profile == null) return;

            txtName.Text = profile.Name;
            txtAge.Text = profile.Age?.ToString() ?? "";
            txtMajor.Text = profile.Major ?? "";
            txtYear.Text = profile.Year ?? "";
            txtBio.Text = profile.Bio ?? "";
            txtPhotoUrl.Text = profile.PhotoUrl ?? "";

            if (profile.Gender != null) cboGender.SelectedItem = profile.Gender;
            if (profile.PreferredGender != null) cboPrefGender.SelectedItem = profile.PreferredGender;

            // Load interests
            selectedInterestIds = profile.Interests?.Select(i => i.Id).ToList() ?? new();
            await LoadInterests();
            
            // Load photos
            photos = profile.Photos?.ToList() ?? new();
            UpdatePhotoGallery();
            
            // Load prompts
            await LoadPrompts();

            UpdateCompletionIndicator(profile);
            UpdatePhotoPreview(profile);
        }
        finally
        {
            loadingOverlay.Hide();
        }
    }

    private async Task LoadInterests()
    {
        allInterests = await _api.GetAllInterestsAsync();
        RenderInterestChips();
    }

    private void RenderInterestChips()
    {
        if (pnlInterests == null || allInterests == null) return;
        
        pnlInterests.Controls.Clear();
        
        foreach (var interest in allInterests)
        {
            var chip = new Button
            {
                Text = $"{interest.Emoji} {interest.Name}",
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                Cursor = Cursors.Hand,
                Padding = new Padding(8, 4, 8, 4),
                Margin = new Padding(4),
                Tag = interest.Id
            };
            
            var isSelected = selectedInterestIds.Contains(interest.Id);
            UpdateChipStyle(chip, isSelected);
            
            chip.Click += InterestChip_Click;
            pnlInterests.Controls.Add(chip);
        }
    }

    private void UpdateChipStyle(Button chip, bool selected)
    {
        if (selected)
        {
            chip.BackColor = Color.FromArgb(124, 58, 237);
            chip.ForeColor = Color.White;
            chip.FlatAppearance.BorderColor = Color.FromArgb(124, 58, 237);
        }
        else
        {
            chip.BackColor = Color.FromArgb(60, 55, 105);
            chip.ForeColor = Color.White;
            chip.FlatAppearance.BorderColor = Color.FromArgb(80, 75, 130);
        }
    }

    private async void InterestChip_Click(object? sender, EventArgs e)
    {
        if (sender is not Button chip || chip.Tag is not int interestId) return;
        
        if (selectedInterestIds.Contains(interestId))
        {
            selectedInterestIds.Remove(interestId);
            UpdateChipStyle(chip, false);
        }
        else if (selectedInterestIds.Count < 10)
        {
            selectedInterestIds.Add(interestId);
            UpdateChipStyle(chip, true);
        }
        else
        {
            MessageBox.Show("Maximum 10 interests allowed.", "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        
        lblInterestCount.Text = $"{selectedInterestIds.Count}/10 selected";
        await _api.UpdateInterestsAsync(selectedInterestIds);
    }

    private void UpdatePhotoGallery()
    {
        pnlPhotoGallery.Controls.Clear();
        
        for (int i = 0; i < 6; i++)
        {
            var slot = new Panel
            {
                Size = new Size(70, 70),
                BackColor = Color.FromArgb(60, 55, 105),
                Margin = new Padding(4),
                Cursor = Cursors.Hand,
                Tag = i
            };
            
            if (i < photos.Count)
            {
                var photo = photos[i];
                var pic = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Tag = photo.Id
                };
                try { pic.Load(photo.Url); } catch { }
                
                // Delete button
                var btnDelete = new Button
                {
                    Text = "✕",
                    Size = new Size(20, 20),
                    Location = new Point(48, 2),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(239, 68, 68),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 8F),
                    Cursor = Cursors.Hand,
                    Tag = photo.Id
                };
                btnDelete.FlatAppearance.BorderSize = 0;
                btnDelete.Click += BtnDeletePhoto_Click;
                
                slot.Controls.Add(pic);
                slot.Controls.Add(btnDelete);
                btnDelete.BringToFront();
                
                if (photo.IsPrimary)
                {
                    slot.BackColor = Color.FromArgb(124, 58, 237);
                }
            }
            else
            {
                var lbl = new Label
                {
                    Text = "+",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 20F),
                    ForeColor = Color.FromArgb(100, 100, 130)
                };
                slot.Controls.Add(lbl);
                slot.Click += (s, e) => BtnAddPhoto_Click(s, e);
                lbl.Click += (s, e) => BtnAddPhoto_Click(s, e);
            }
            
            pnlPhotoGallery.Controls.Add(slot);
        }
    }

    private async void BtnAddPhoto_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Image files|*.jpg;*.jpeg;*.png;*.gif;*.webp",
            Title = "Select Photo"
        };
        
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                using var stream = File.OpenRead(dialog.FileName);
                var photo = await _api.UploadPhotoAsync(stream, Path.GetFileName(dialog.FileName));
                if (photo != null)
                {
                    photos.Add(photo);
                    UpdatePhotoGallery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to upload photo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void BtnDeletePhoto_Click(object? sender, EventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not int photoId) return;
        
        var result = MessageBox.Show("Delete this photo?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result == DialogResult.Yes)
        {
            if (await _api.DeletePhotoAsync(photoId))
            {
                photos.RemoveAll(p => p.Id == photoId);
                UpdatePhotoGallery();
            }
        }
    }

    private void UpdateCompletionIndicator(StudentDto profile)
    {
        int filled = 0;
        int total = 9;

        if (!string.IsNullOrEmpty(profile.Name)) filled++;
        if (profile.Age.HasValue) filled++;
        if (!string.IsNullOrEmpty(profile.Major)) filled++;
        if (!string.IsNullOrEmpty(profile.Year)) filled++;
        if (!string.IsNullOrEmpty(profile.Bio)) filled++;
        if (!string.IsNullOrEmpty(profile.PhotoUrl) || photos.Count > 0) filled++;
        if (!string.IsNullOrEmpty(profile.Gender)) filled++;
        if (!string.IsNullOrEmpty(profile.PreferredGender)) filled++;
        if ((profile.Interests?.Count ?? 0) > 0) filled++;

        completionPercentage = (int)((filled / (double)total) * 100);
        lblCompletion.Text = $"{completionPercentage}% Complete";
        pnlProgressBar.Invalidate();

        if (completionPercentage < 100)
        {
            var tips = new List<string>();
            if (string.IsNullOrEmpty(profile.PhotoUrl) && photos.Count == 0) tips.Add("Add a photo");
            if (string.IsNullOrEmpty(profile.Bio)) tips.Add("Write a bio");
            if ((profile.Interests?.Count ?? 0) == 0) tips.Add("Add interests");
            
            lblTips.Text = tips.Count > 0 ? "💡 " + string.Join(", ", tips.Take(2)) : "";
        }
        else
        {
            lblTips.Text = "✨ Profile complete!";
        }
    }

    private void UpdatePhotoPreview(StudentDto profile)
    {
        if (photos.Count > 0)
        {
            try
            {
                picPreview.Load(photos[0].Url);
                lblPhotoInitials.Visible = false;
                picPreview.Visible = true;
                picPreview.BringToFront();
                return;
            }
            catch { }
        }
        
        if (!string.IsNullOrEmpty(profile.PhotoUrl))
        {
            try
            {
                picPreview.Load(profile.PhotoUrl);
                lblPhotoInitials.Visible = false;
                picPreview.Visible = true;
                picPreview.BringToFront();
            }
            catch
            {
                ShowPhotoPlaceholder(profile.Name);
            }
        }
        else
        {
            ShowPhotoPlaceholder(profile.Name);
        }
    }

    private void ShowPhotoPlaceholder(string name)
    {
        picPreview.Visible = false;
        lblPhotoInitials.Visible = true;

        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var initials = parts.Length >= 2 
            ? $"{parts[0][0]}{parts[^1][0]}" 
            : name.Length > 0 ? name[0].ToString() : "?";
        lblPhotoInitials.Text = initials.ToUpper();
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        _ = SaveProfile();
    }

    private async Task SaveProfile()
    {
        btnSave.Enabled = false;
        btnSave.Text = "Saving...";
        lblSaveStatus.Visible = false;

        try
        {
            int? age = int.TryParse(txtAge.Text, out var a) ? a : null;
            var request = new ProfileUpdateRequest(
                txtName.Text, age, txtMajor.Text, txtYear.Text, txtBio.Text, txtPhotoUrl.Text,
                null, cboGender.SelectedItem?.ToString(), cboPrefGender.SelectedItem?.ToString(),
                null, null // PhoneNumber and InstagramHandle preserved from existing values
            );

            var result = await _api.UpdateProfileAsync(request);
            if (result != null)
            {
                lblSaveStatus.Text = "✓ Profile saved!";
                lblSaveStatus.ForeColor = Color.FromArgb(34, 197, 94);
                lblSaveStatus.Visible = true;
                UpdateCompletionIndicator(result);
                UpdatePhotoPreview(result);
            }
        }
        catch
        {
            lblSaveStatus.Text = "✗ Failed to save";
            lblSaveStatus.ForeColor = Color.FromArgb(239, 68, 68);
            lblSaveStatus.Visible = true;
        }
        finally
        {
            btnSave.Enabled = true;
            btnSave.Text = "💾 Save Profile";
        }
    }

    private void TxtPhotoUrl_TextChanged(object? sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtPhotoUrl.Text) && Uri.IsWellFormedUriString(txtPhotoUrl.Text, UriKind.Absolute))
        {
            try
            {
                picPreview.Load(txtPhotoUrl.Text);
                lblPhotoInitials.Visible = false;
                picPreview.Visible = true;
                picPreview.BringToFront();
            }
            catch { }
        }
    }

    private async void LblAddPhoto_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Image files|*.jpg;*.jpeg;*.png;*.gif;*.webp",
            Title = "Select Profile Photo"
        };
        
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                using var stream = File.OpenRead(dialog.FileName);
                var photo = await _api.UploadPhotoAsync(stream, Path.GetFileName(dialog.FileName));
                if (photo != null)
                {
                    // Set as primary if it's the first photo or update existing
                    photos.Insert(0, photo);
                    UpdatePhotoGallery();
                    
                    // Update the main preview
                    picPreview.Load(photo.Url);
                    lblPhotoInitials.Visible = false;
                    picPreview.Visible = true;
                    picPreview.BringToFront();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to upload photo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    
    private async Task LoadPrompts()
    {
        availablePrompts = await _api.GetAllPromptsAsync();
        myPrompts = await _api.GetMyPromptsAsync();
        RenderPrompts();
    }
    
    private void RenderPrompts()
    {
        if (pnlPrompts == null || lblPromptsHeader == null || btnAddPrompt == null) return;
        
        pnlPrompts.Controls.Clear();
        lblPromptsHeader.Text = $"💬 Profile Prompts ({myPrompts.Count}/3)";
        btnAddPrompt.Visible = myPrompts.Count < 3;
        
        foreach (var prompt in myPrompts)
        {
            var card = new Panel
            {
                Width = 500,
                Height = 50,
                BackColor = Color.FromArgb(60, 55, 105),
                Margin = new Padding(0, 0, 0, 5),
                Padding = new Padding(10)
            };
            
            var lblQuestion = new Label
            {
                Text = prompt.Question,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(124, 58, 237),
                Location = new Point(10, 5),
                AutoSize = true
            };
            
            var lblAnswer = new Label
            {
                Text = prompt.Answer,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.White,
                Location = new Point(10, 25),
                AutoSize = true,
                MaximumSize = new Size(430, 0)
            };
            
            var btnDelete = new Button
            {
                Text = "✕",
                Size = new Size(25, 25),
                Location = new Point(465, 12),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8F),
                Cursor = Cursors.Hand,
                Tag = prompt.PromptId
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDeletePrompt_Click;
            
            card.Controls.AddRange(new Control[] { lblQuestion, lblAnswer, btnDelete });
            pnlPrompts.Controls.Add(card);
        }
    }
    
    private async void BtnAddPrompt_Click(object? sender, EventArgs e)
    {
        if (myPrompts.Count >= 3)
        {
            MessageBox.Show("Maximum 3 prompts allowed.", "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        
        // Filter out already-used prompts
        var usedIds = myPrompts.Select(p => p.PromptId).ToHashSet();
        var available = availablePrompts.Where(p => !usedIds.Contains(p.Id)).ToList();
        
        if (available.Count == 0)
        {
            MessageBox.Show("No more prompts available.", "No Prompts", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        
        // Create picker dialog
        using var dialog = new Form
        {
            Text = "Add Profile Prompt",
            Width = 450,
            Height = 350,
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            BackColor = Color.FromArgb(30, 27, 75)
        };
        
        var lblSelect = new Label
        {
            Text = "Choose a prompt:",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Location = new Point(15, 15),
            AutoSize = true
        };
        
        var cboPrompts = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(60, 55, 105),
            ForeColor = Color.White,
            Location = new Point(15, 45),
            Width = 400
        };
        foreach (var p in available) cboPrompts.Items.Add(p);
        cboPrompts.DisplayMember = "Question";
        if (cboPrompts.Items.Count > 0) cboPrompts.SelectedIndex = 0;
        
        var lblAnswer = new Label
        {
            Text = "Your answer:",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F),
            Location = new Point(15, 85),
            AutoSize = true
        };
        
        var txtAnswer = new TextBox
        {
            BackColor = Color.FromArgb(60, 55, 105),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F),
            Location = new Point(15, 110),
            Width = 400,
            Height = 100,
            Multiline = true,
            MaxLength = 200,
            PlaceholderText = "Enter your answer (max 200 chars)..."
        };
        
        var btnAdd = new Button
        {
            Text = "Add Prompt",
            BackColor = Color.FromArgb(124, 58, 237),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Location = new Point(15, 230),
            Size = new Size(120, 40),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            DialogResult = DialogResult.OK
        };
        btnAdd.FlatAppearance.BorderSize = 0;
        
        var btnCancel = new Button
        {
            Text = "Cancel",
            BackColor = Color.FromArgb(60, 55, 105),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Location = new Point(145, 230),
            Size = new Size(100, 40),
            DialogResult = DialogResult.Cancel
        };
        
        dialog.Controls.AddRange(new Control[] { lblSelect, cboPrompts, lblAnswer, txtAnswer, btnAdd, btnCancel });
        dialog.AcceptButton = btnAdd;
        dialog.CancelButton = btnCancel;
        
        if (dialog.ShowDialog() == DialogResult.OK && cboPrompts.SelectedItem is PromptDto selectedPrompt)
        {
            var answer = txtAnswer.Text.Trim();
            if (string.IsNullOrEmpty(answer))
            {
                MessageBox.Show("Please enter an answer.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            var result = await _api.UpdatePromptAsync(selectedPrompt.Id, answer);
            if (result != null)
            {
                await LoadPrompts();
            }
        }
    }
    
    private async void BtnDeletePrompt_Click(object? sender, EventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not int promptId) return;
        
        var result = MessageBox.Show("Remove this prompt?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result == DialogResult.Yes)
        {
            if (await _api.DeletePromptAsync(promptId))
            {
                await LoadPrompts();
            }
        }
    }
}
