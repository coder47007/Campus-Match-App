#nullable enable

using CampusMatch.Client.Services;
using CampusMatch.Client.Helpers;
using CampusMatch.Shared.DTOs;

namespace CampusMatch.Client.Forms;

public partial class AdminDashboard : Form
{
    private readonly ApiService _api;
    private List<AdminUserDto> users = new();
    private List<AdminReportDto> reports = new();
    private List<InterestDto> interests = new();
    private List<PromptDto> prompts = new();
    private List<ActivityLogDto> logs = new();
    
    public AdminDashboard(ApiService api)
    {
        _api = api;
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"InitializeComponent Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw;
        }
        this.Load += AdminDashboard_Load;
    }
    
    private async void AdminDashboard_Load(object? sender, EventArgs e)
    {
        try
        {
            await LoadDashboard();
            await LoadUsers();
            await LoadReports();
            await LoadInterests();
            await LoadPrompts();
            await LoadActivityLogs();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Load Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    // Dashboard Tab
    private async Task LoadDashboard()
    {
        var stats = await _api.GetAdminStatsAsync();
        if (stats != null)
        {
            lblTotalUsers.Text = stats.TotalUsers.ToString();
            lblActiveUsers.Text = stats.ActiveUsers.ToString();
            lblBannedUsers.Text = stats.BannedUsers.ToString();
            lblTotalMatches.Text = stats.TotalMatches.ToString();
            lblTotalMessages.Text = stats.TotalMessages.ToString();
            lblPendingReports.Text = stats.PendingReports.ToString();
            lblTotalInterests.Text = stats.TotalInterests.ToString();
            lblTotalPrompts.Text = stats.TotalPrompts.ToString();
        }
    }
    
    // Users Tab
    private const string SearchPlaceholder = "Search users by name or email...";
    
    private async Task LoadUsers()
    {
        var searchText = txtUserSearch.Text.Trim();
        // Ignore placeholder text - treat as empty search
        var search = (searchText == SearchPlaceholder || string.IsNullOrEmpty(searchText)) ? "" : searchText;
        bool? banned = chkShowBannedOnly.Checked ? true : null;
        users = await _api.GetAdminUsersAsync(search, banned);
        dgvUsers.DataSource = null;
        
        if (users.Count > 0)
        {
            dgvUsers.DataSource = users.Select(u => new
            {
                u.Id,
                u.Name,
                u.Email,
                u.Age,
                u.Major,
                Status = u.IsBanned ? "Banned" : "Active",
                Created = u.CreatedAt.ToString("MM/dd/yyyy"),
                LastActive = u.LastActiveAt.ToString("MM/dd/yyyy"),
                Matches = u.MatchCount,
                Reports = u.ReportCount
            }).ToList();
        }
    }
    
    private async void BtnSearchUsers_Click(object? sender, EventArgs e)
    {
        await LoadUsers();
    }
    
    private async void BtnBanUser_Click(object? sender, EventArgs e)
    {
        if (dgvUsers.CurrentRow == null) return;
        var userId = (int)dgvUsers.CurrentRow.Cells["Id"].Value;
        var userName = dgvUsers.CurrentRow.Cells["Name"].Value?.ToString();
        
        // Create simple input dialog
        var reason = ShowInputDialog($"Enter reason for banning {userName}:", "Ban User", "Violation of community guidelines");
            
        if (string.IsNullOrEmpty(reason)) return;
        
        if (await _api.AdminBanUserAsync(userId, reason))
        {
            MessageBox.Show("User banned successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadUsers();
            await LoadDashboard();
        }
    }
    
    private string? ShowInputDialog(string prompt, string title, string defaultValue)
    {
        var dialog = new Form
        {
            Width = 400,
            Height = 180,
            Text = title,
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            BackColor = Color.FromArgb(30, 27, 75)
        };
        
        var lblPrompt = new Label { Left = 20, Top = 20, Width = 350, Text = prompt, ForeColor = Color.White };
        var txtInput = new TextBox { Left = 20, Top = 50, Width = 340, Text = defaultValue, BackColor = Color.FromArgb(60, 55, 105), ForeColor = Color.White };
        var btnOk = new Button { Text = "OK", Left = 180, Top = 90, Width = 80, DialogResult = DialogResult.OK, BackColor = Color.FromArgb(124, 58, 237), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        var btnCancel = new Button { Text = "Cancel", Left = 280, Top = 90, Width = 80, DialogResult = DialogResult.Cancel, BackColor = Color.FromArgb(60, 55, 105), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        
        dialog.Controls.AddRange(new Control[] { lblPrompt, txtInput, btnOk, btnCancel });
        dialog.AcceptButton = btnOk;
        dialog.CancelButton = btnCancel;
        
        return dialog.ShowDialog() == DialogResult.OK ? txtInput.Text : null;
    }
    
    private async void BtnUnbanUser_Click(object? sender, EventArgs e)
    {
        if (dgvUsers.CurrentRow == null) return;
        var userId = (int)dgvUsers.CurrentRow.Cells["Id"].Value;
        
        if (await _api.AdminUnbanUserAsync(userId))
        {
            MessageBox.Show("User unbanned successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadUsers();
            await LoadDashboard();
        }
    }
    
    private async void BtnDeleteUser_Click(object? sender, EventArgs e)
    {
        if (dgvUsers.CurrentRow == null) return;
        var userId = (int)dgvUsers.CurrentRow.Cells["Id"].Value;
        var userName = dgvUsers.CurrentRow.Cells["Name"].Value?.ToString();
        
        var result = MessageBox.Show(
            $"Are you sure you want to permanently delete {userName}?\n\nThis action cannot be undone.", 
            "Delete User", 
            MessageBoxButtons.YesNo, 
            MessageBoxIcon.Warning);
            
        if (result != DialogResult.Yes) return;
        
        if (await _api.AdminDeleteUserAsync(userId))
        {
            MessageBox.Show("User deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadUsers();
            await LoadDashboard();
        }
    }
    
    // Reports Tab
    private async Task LoadReports()
    {
        reports = await _api.GetAdminReportsAsync(!chkShowResolvedReports.Checked);
        dgvReports.DataSource = null;
        dgvReports.DataSource = reports.Select(r => new
        {
            r.Id,
            Reporter = r.ReporterName,
            Reported = r.ReportedName,
            r.Reason,
            r.Details,
            Date = r.CreatedAt.ToString("MM/dd/yyyy HH:mm"),
            Status = r.IsReviewed ? "✅ Resolved" : "⏳ Pending"
        }).ToList();
    }
    
    private async void BtnResolveReport_Click(object? sender, EventArgs e)
    {
        if (dgvReports.CurrentRow == null) return;
        var reportId = (int)dgvReports.CurrentRow.Cells["Id"].Value;
        
        if (await _api.ResolveReportAsync(reportId))
        {
            MessageBox.Show("Report resolved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadReports();
            await LoadDashboard();
        }
    }
    
    private async void BtnDeleteReport_Click(object? sender, EventArgs e)
    {
        if (dgvReports.CurrentRow == null) return;
        var reportId = (int)dgvReports.CurrentRow.Cells["Id"].Value;
        
        if (await _api.DeleteReportAsync(reportId))
        {
            MessageBox.Show("Report deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadReports();
        }
    }
    
    private async void ChkShowResolvedReports_CheckedChanged(object? sender, EventArgs e)
    {
        await LoadReports();
    }
    
    // Interests Tab
    private async Task LoadInterests()
    {
        interests = await _api.GetAdminInterestsAsync();
        dgvInterests.DataSource = null;
        dgvInterests.DataSource = interests.Select(i => new
        {
            i.Id,
            i.Emoji,
            i.Name,
            i.Category
        }).ToList();
    }
    
    private async void BtnAddInterest_Click(object? sender, EventArgs e)
    {
        var name = txtInterestName.Text.Trim();
        var emoji = txtInterestEmoji.Text.Trim();
        var category = txtInterestCategory.Text.Trim();
        
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(emoji) || string.IsNullOrEmpty(category))
        {
            MessageBox.Show("Please fill in all fields.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        if (await _api.CreateInterestAsync(name, emoji, category))
        {
            txtInterestName.Clear();
            txtInterestEmoji.Clear();
            txtInterestCategory.Clear();
            await LoadInterests();
            await LoadDashboard();
        }
    }
    
    private async void BtnDeleteInterest_Click(object? sender, EventArgs e)
    {
        if (dgvInterests.CurrentRow == null) return;
        var id = (int)dgvInterests.CurrentRow.Cells["Id"].Value;
        
        if (await _api.DeleteInterestAsync(id))
        {
            await LoadInterests();
            await LoadDashboard();
        }
    }
    
    // Prompts Tab
    private async Task LoadPrompts()
    {
        prompts = await _api.GetAdminPromptsAsync();
        dgvPrompts.DataSource = null;
        dgvPrompts.DataSource = prompts.Select(p => new
        {
            p.Id,
            p.Question,
            p.Category
        }).ToList();
    }
    
    private async void BtnAddPrompt_Click(object? sender, EventArgs e)
    {
        var question = txtPromptQuestion.Text.Trim();
        var category = txtPromptCategory.Text.Trim();
        
        if (string.IsNullOrEmpty(question) || string.IsNullOrEmpty(category))
        {
            MessageBox.Show("Please fill in all fields.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        if (await _api.CreatePromptAsync(question, category))
        {
            txtPromptQuestion.Clear();
            txtPromptCategory.Clear();
            await LoadPrompts();
            await LoadDashboard();
        }
    }
    
    private async void BtnDeletePrompt_Click(object? sender, EventArgs e)
    {
        if (dgvPrompts.CurrentRow == null) return;
        var id = (int)dgvPrompts.CurrentRow.Cells["Id"].Value;
        
        if (await _api.DeletePromptAdminAsync(id))
        {
            await LoadPrompts();
            await LoadDashboard();
        }
    }
    
    // Activity Logs Tab
    private async Task LoadActivityLogs()
    {
        logs = await _api.GetActivityLogsAsync();
        dgvLogs.DataSource = null;
        dgvLogs.DataSource = logs.Select(l => new
        {
            l.Id,
            Admin = l.AdminName ?? "System",
            Target = l.TargetUserName ?? "-",
            l.Action,
            l.Details,
            Time = l.CreatedAt.ToString("MM/dd/yyyy HH:mm:ss")
        }).ToList();
    }
    
    private async void BtnRefreshLogs_Click(object? sender, EventArgs e)
    {
        await LoadActivityLogs();
    }
    
    private async void BtnRefreshAll_Click(object? sender, EventArgs e)
    {
        await LoadDashboard();
        await LoadUsers();
        await LoadReports();
        await LoadInterests();
        await LoadPrompts();
        await LoadActivityLogs();
    }
    
    private void BtnLogout_Click(object? sender, EventArgs e)
    {
        var result = MessageBox.Show(
            "Are you sure you want to logout?", 
            "Confirm Logout", 
            MessageBoxButtons.YesNo, 
            MessageBoxIcon.Question);
        
        if (result == DialogResult.Yes)
        {
            _api.Logout();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
