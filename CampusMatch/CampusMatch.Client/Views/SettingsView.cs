using CampusMatch.Client.Services;
using CampusMatch.Shared.DTOs;

namespace CampusMatch.Client.Views;

public partial class SettingsView : UserControl
{
    private readonly ApiService _api;

    public SettingsView(ApiService api)
    {
        _api = api;
        InitializeComponent();
    }

    public async void LoadSettings()
    {
        var settings = await _api.GetSettingsAsync();
        if (settings == null)
        {
            // Use defaults if API fails
            numMinAge.Value = 18;
            numMaxAge.Value = 30;
            chkNotifyMatches.Checked = true;
            chkNotifyMessages.Checked = true;
            chkShowOnline.Checked = true;
            return;
        }

        numMinAge.Value = Math.Clamp(settings.MinAgePreference, 18, 100);
        numMaxAge.Value = Math.Clamp(settings.MaxAgePreference, 18, 100);
        chkNotifyMatches.Checked = settings.NotifyOnMatch;
        chkNotifyMessages.Checked = settings.NotifyOnMessage;
        chkShowOnline.Checked = settings.ShowOnlineStatus;
    }

    private async void BtnSaveSettings_Click(object? sender, EventArgs e)
    {
        btnSaveSettings.Enabled = false;
        lblStatus.Text = "Saving...";
        lblStatus.ForeColor = Color.Gray;
        lblStatus.Visible = true;

        var request = new UpdateSettingsRequest(
            MinAgePreference: (int)numMinAge.Value,
            MaxAgePreference: (int)numMaxAge.Value,
            ShowOnlineStatus: chkShowOnline.Checked,
            NotifyOnMatch: chkNotifyMatches.Checked,
            NotifyOnMessage: chkNotifyMessages.Checked
        );

        var result = await _api.UpdateSettingsAsync(request);
        
        if (result != null)
        {
            lblStatus.Text = "✓ Settings saved!";
            lblStatus.ForeColor = Color.FromArgb(34, 197, 94);
        }
        else
        {
            lblStatus.Text = "✗ Failed to save settings";
            lblStatus.ForeColor = Color.FromArgb(239, 68, 68);
        }
        
        btnSaveSettings.Enabled = true;
    }

    private void BtnLogout_Click(object? sender, EventArgs e)
    {
        var result = MessageBox.Show(
            "Are you sure you want to log out?",
            "Logout",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            _api.Logout();
            Application.Restart();
        }
    }

    private async void BtnDeleteAccount_Click(object? sender, EventArgs e)
    {
        var result = MessageBox.Show(
            "⚠️ This action cannot be undone!\n\nAre you sure you want to permanently delete your account and all your data?",
            "Delete Account",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (result == DialogResult.Yes)
        {
            // Prompt for password
            using var dialog = new Form
            {
                Text = "Confirm Account Deletion",
                Width = 350,
                Height = 180,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var label = new Label
            {
                Text = "Enter your password to confirm deletion:",
                Left = 20,
                Top = 20,
                Width = 300
            };

            var passwordBox = new TextBox
            {
                Left = 20,
                Top = 50,
                Width = 290,
                PasswordChar = '●'
            };

            var confirmBtn = new Button
            {
                Text = "Delete My Account",
                Left = 20,
                Top = 90,
                Width = 140,
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };

            var cancelBtn = new Button
            {
                Text = "Cancel",
                Left = 170,
                Top = 90,
                Width = 140,
                DialogResult = DialogResult.Cancel
            };

            dialog.Controls.AddRange(new Control[] { label, passwordBox, confirmBtn, cancelBtn });
            dialog.AcceptButton = confirmBtn;
            dialog.CancelButton = cancelBtn;

            if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(passwordBox.Text))
            {
                var success = await _api.DeleteAccountAsync(passwordBox.Text);
                
                if (success)
                {
                    MessageBox.Show("Your account has been deleted.", "Account Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _api.Logout();
                    Application.Restart();
                }
                else
                {
                    MessageBox.Show("Failed to delete account. Please check your password and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    private void LnkPrivacy_Click(object? sender, EventArgs e)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "https://campusmatch.example.com/privacy",
            UseShellExecute = true
        });
    }

    private void LnkTerms_Click(object? sender, EventArgs e)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "https://campusmatch.example.com/terms",
            UseShellExecute = true
        });
    }
}
