#nullable enable

namespace CampusMatch.Client.Forms;

partial class AdminDashboard
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        tabControl = new TabControl();
        tabDashboard = new TabPage();
        tabUsers = new TabPage();
        tabReports = new TabPage();
        tabInterests = new TabPage();
        tabPrompts = new TabPage();
        tabLogs = new TabPage();
        
        // Dashboard controls
        lblTotalUsers = new Label();
        lblActiveUsers = new Label();
        lblBannedUsers = new Label();
        lblTotalMatches = new Label();
        lblTotalMessages = new Label();
        lblPendingReports = new Label();
        lblTotalInterests = new Label();
        lblTotalPrompts = new Label();
        btnRefreshAll = new Button();
        
        // Users tab controls
        txtUserSearch = new TextBox();
        btnSearchUsers = new Button();
        chkShowBannedOnly = new CheckBox();
        btnBanUser = new Button();
        btnUnbanUser = new Button();
        btnDeleteUser = new Button();
        dgvUsers = new DataGridView();
        
        // Reports tab controls
        chkShowResolvedReports = new CheckBox();
        btnResolveReport = new Button();
        btnDeleteReport = new Button();
        dgvReports = new DataGridView();
        
        // Interests tab controls
        txtInterestName = new TextBox();
        txtInterestEmoji = new TextBox();
        txtInterestCategory = new TextBox();
        btnAddInterest = new Button();
        btnDeleteInterest = new Button();
        dgvInterests = new DataGridView();
        
        // Prompts tab controls
        txtPromptQuestion = new TextBox();
        txtPromptCategory = new TextBox();
        btnAddPrompt = new Button();
        btnDeletePrompt = new Button();
        dgvPrompts = new DataGridView();
        
        // Logs tab controls
        btnRefreshLogs = new Button();
        dgvLogs = new DataGridView();
        
        SuspendLayout();
        
        // === ENHANCED THEME COLORS ===
        var darkBg = Color.FromArgb(15, 12, 41);
        var cardBg = Color.FromArgb(25, 22, 55);
        var accentPurple = Color.FromArgb(139, 92, 246);
        var accentPink = Color.FromArgb(236, 72, 153);
        var accentCyan = Color.FromArgb(34, 211, 238);
        var successGreen = Color.FromArgb(34, 197, 94);
        var dangerRed = Color.FromArgb(239, 68, 68);
        var warningOrange = Color.FromArgb(251, 146, 60);
        var textPrimary = Color.White;
        var textSecondary = Color.FromArgb(148, 163, 184);
        var inputBg = Color.FromArgb(40, 35, 80);
        var gridBg = Color.FromArgb(30, 27, 65);
        
        // tabControl - Custom styled tabs
        tabControl.Controls.Add(tabDashboard);
        tabControl.Controls.Add(tabUsers);
        tabControl.Controls.Add(tabReports);
        tabControl.Controls.Add(tabInterests);
        tabControl.Controls.Add(tabPrompts);
        tabControl.Controls.Add(tabLogs);
        tabControl.Dock = DockStyle.Fill;
        tabControl.Font = new Font("Segoe UI Semibold", 11F);
        tabControl.Name = "tabControl";
        tabControl.Padding = new Point(20, 8);
        tabControl.ItemSize = new Size(120, 40);
        tabControl.SizeMode = TabSizeMode.Fixed;
        
        // tabDashboard
        tabDashboard.BackColor = darkBg;
        tabDashboard.Text = "📊 Dashboard";
        tabDashboard.Padding = new Padding(25);
        SetupDashboardTab(darkBg, cardBg, accentPurple, accentPink, accentCyan, textPrimary, textSecondary);
        
        // tabUsers
        tabUsers.BackColor = darkBg;
        tabUsers.Text = "👥 Users";
        tabUsers.Padding = new Padding(15);
        SetupUsersTab(darkBg, cardBg, accentPurple, successGreen, dangerRed, textPrimary, inputBg, gridBg);
        
        // tabReports
        tabReports.BackColor = darkBg;
        tabReports.Text = "🚨 Reports";
        tabReports.Padding = new Padding(15);
        SetupReportsTab(darkBg, cardBg, successGreen, dangerRed, textPrimary, gridBg);
        
        // tabInterests
        tabInterests.BackColor = darkBg;
        tabInterests.Text = "💡 Interests";
        tabInterests.Padding = new Padding(15);
        SetupInterestsTab(darkBg, cardBg, accentPurple, successGreen, dangerRed, textPrimary, inputBg, gridBg);
        
        // tabPrompts
        tabPrompts.BackColor = darkBg;
        tabPrompts.Text = "💬 Prompts";
        tabPrompts.Padding = new Padding(15);
        SetupPromptsTab(darkBg, cardBg, accentPurple, successGreen, dangerRed, textPrimary, inputBg, gridBg);
        
        // tabLogs
        tabLogs.BackColor = darkBg;
        tabLogs.Text = "📜 Activity";
        tabLogs.Padding = new Padding(15);
        SetupLogsTab(darkBg, cardBg, accentPurple, textPrimary, gridBg);
        
        // AdminDashboard Form
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = darkBg;
        ClientSize = new Size(1200, 750);
        Controls.Add(tabControl);
        MinimumSize = new Size(1000, 650);
        Name = "AdminDashboard";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "🔐 CampusMatch Admin Dashboard";
        
        ResumeLayout(false);
    }
    
    private void SetupDashboardTab(Color darkBg, Color cardBg, Color accentPurple, Color accentPink, Color accentCyan, Color textPrimary, Color textSecondary)
    {
        // Header Panel with gradient effect
        var headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 100,
            BackColor = cardBg
        };
        headerPanel.Paint += (s, e) => {
            using var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                headerPanel.ClientRectangle, 
                Color.FromArgb(40, accentPurple), 
                Color.FromArgb(20, accentPink), 
                System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
            e.Graphics.FillRectangle(brush, headerPanel.ClientRectangle);
        };
        
        var title = new Label
        {
            Text = "🚀 Admin Command Center",
            Font = new Font("Segoe UI", 26F, FontStyle.Bold),
            ForeColor = textPrimary,
            Location = new Point(30, 25),
            AutoSize = true
        };
        headerPanel.Controls.Add(title);
        
        var subtitle = new Label
        {
            Text = "Monitor and manage your CampusMatch platform",
            Font = new Font("Segoe UI", 11F),
            ForeColor = textSecondary,
            Location = new Point(32, 65),
            AutoSize = true
        };
        headerPanel.Controls.Add(subtitle);
        
        btnRefreshAll.Text = "⟳ Refresh All";
        btnRefreshAll.Location = new Point(880, 35);
        btnRefreshAll.Size = new Size(130, 40);
        btnRefreshAll.Font = new Font("Segoe UI Semibold", 10F);
        btnRefreshAll.BackColor = accentPurple;
        btnRefreshAll.ForeColor = Color.White;
        btnRefreshAll.FlatStyle = FlatStyle.Flat;
        btnRefreshAll.FlatAppearance.BorderSize = 0;
        btnRefreshAll.Cursor = Cursors.Hand;
        btnRefreshAll.Click += BtnRefreshAll_Click;
        headerPanel.Controls.Add(btnRefreshAll);
        
        // Logout button
        btnLogout = new Button();
        btnLogout.Text = "🚪 Logout";
        btnLogout.Location = new Point(1020, 35);
        btnLogout.Size = new Size(110, 40);
        btnLogout.Font = new Font("Segoe UI Semibold", 10F);
        btnLogout.BackColor = Color.FromArgb(239, 68, 68);
        btnLogout.ForeColor = Color.White;
        btnLogout.FlatStyle = FlatStyle.Flat;
        btnLogout.FlatAppearance.BorderSize = 0;
        btnLogout.Cursor = Cursors.Hand;
        btnLogout.Click += BtnLogout_Click;
        headerPanel.Controls.Add(btnLogout);
        
        tabDashboard.Controls.Add(headerPanel);
        
        // Stats Cards Container
        var statsContainer = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20, 20, 20, 20),
            AutoScroll = true,
            WrapContents = true
        };
        
        // Create stat cards with different accent colors
        AddStatCard(statsContainer, "👤 Total Users", lblTotalUsers, accentPurple, cardBg, textPrimary, textSecondary);
        AddStatCard(statsContainer, "✅ Active Users", lblActiveUsers, Color.FromArgb(34, 197, 94), cardBg, textPrimary, textSecondary);
        AddStatCard(statsContainer, "🚫 Banned Users", lblBannedUsers, Color.FromArgb(239, 68, 68), cardBg, textPrimary, textSecondary);
        AddStatCard(statsContainer, "💕 Total Matches", lblTotalMatches, accentPink, cardBg, textPrimary, textSecondary);
        AddStatCard(statsContainer, "💬 Messages", lblTotalMessages, accentCyan, cardBg, textPrimary, textSecondary);
        AddStatCard(statsContainer, "⚠️ Pending Reports", lblPendingReports, Color.FromArgb(251, 146, 60), cardBg, textPrimary, textSecondary);
        AddStatCard(statsContainer, "🏷️ Interests", lblTotalInterests, Color.FromArgb(168, 85, 247), cardBg, textPrimary, textSecondary);
        AddStatCard(statsContainer, "❓ Prompts", lblTotalPrompts, Color.FromArgb(59, 130, 246), cardBg, textPrimary, textSecondary);
        
        tabDashboard.Controls.Add(statsContainer);
        statsContainer.BringToFront();
    }
    
    private void AddStatCard(FlowLayoutPanel container, string labelText, Label valueLabel, Color accentColor, Color cardBg, Color textPrimary, Color textSecondary)
    {
        var card = new Panel
        {
            Size = new Size(260, 140),
            Margin = new Padding(10),
            BackColor = cardBg
        };
        
        // Add gradient accent bar at top
        card.Paint += (s, e) => {
            using var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new Rectangle(0, 0, card.Width, 4), 
                accentColor, 
                Color.FromArgb(100, accentColor), 
                System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
            e.Graphics.FillRectangle(brush, 0, 0, card.Width, 4);
            
            // Add subtle rounded corner effect
            using var pen = new Pen(Color.FromArgb(30, 255, 255, 255), 1);
            e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
        };
        
        var lbl = new Label
        {
            Text = labelText,
            Location = new Point(20, 25),
            ForeColor = textSecondary,
            Font = new Font("Segoe UI", 10F),
            AutoSize = true
        };
        card.Controls.Add(lbl);
        
        valueLabel.Text = "0";
        valueLabel.Location = new Point(20, 55);
        valueLabel.Font = new Font("Segoe UI", 36F, FontStyle.Bold);
        valueLabel.ForeColor = accentColor;
        valueLabel.AutoSize = true;
        card.Controls.Add(valueLabel);
        
        container.Controls.Add(card);
    }
    
    private void SetupUsersTab(Color darkBg, Color cardBg, Color accentPurple, Color successGreen, Color dangerRed, Color textPrimary, Color inputBg, Color gridBg)
    {
        var toolPanel = new Panel { 
            Dock = DockStyle.Top, 
            Height = 70,
            BackColor = cardBg,
            Padding = new Padding(15, 15, 15, 15)
        };
        
        var searchLabel = new Label
        {
            Text = "🔍",
            Location = new Point(20, 22),
            Font = new Font("Segoe UI", 12F),
            AutoSize = true
        };
        toolPanel.Controls.Add(searchLabel);
        
        txtUserSearch.Location = new Point(50, 18);
        txtUserSearch.Size = new Size(250, 35);
        txtUserSearch.Font = new Font("Segoe UI", 11F);
        txtUserSearch.BackColor = inputBg;
        txtUserSearch.ForeColor = textPrimary;
        txtUserSearch.BorderStyle = BorderStyle.FixedSingle;
        var placeholderText = "Search users by name or email...";
        txtUserSearch.Text = placeholderText;
        txtUserSearch.ForeColor = Color.Gray;
        txtUserSearch.GotFocus += (s, e) => { if (txtUserSearch.Text == placeholderText) { txtUserSearch.Text = ""; txtUserSearch.ForeColor = textPrimary; } };
        txtUserSearch.LostFocus += (s, e) => { if (string.IsNullOrEmpty(txtUserSearch.Text)) { txtUserSearch.Text = placeholderText; txtUserSearch.ForeColor = Color.Gray; } };
        toolPanel.Controls.Add(txtUserSearch);
        
        btnSearchUsers.Text = "Search";
        btnSearchUsers.Location = new Point(310, 16);
        btnSearchUsers.Size = new Size(100, 38);
        btnSearchUsers.Font = new Font("Segoe UI Semibold", 10F);
        btnSearchUsers.BackColor = accentPurple;
        btnSearchUsers.ForeColor = Color.White;
        btnSearchUsers.FlatStyle = FlatStyle.Flat;
        btnSearchUsers.FlatAppearance.BorderSize = 0;
        btnSearchUsers.Cursor = Cursors.Hand;
        btnSearchUsers.Click += BtnSearchUsers_Click;
        toolPanel.Controls.Add(btnSearchUsers);
        
        chkShowBannedOnly.Text = "   Show Banned Only";
        chkShowBannedOnly.Location = new Point(430, 22);
        chkShowBannedOnly.ForeColor = textPrimary;
        chkShowBannedOnly.Font = new Font("Segoe UI", 10F);
        chkShowBannedOnly.AutoSize = true;
        toolPanel.Controls.Add(chkShowBannedOnly);
        
        // Action buttons with better styling
        btnBanUser.Text = "⛔ Ban";
        btnBanUser.Location = new Point(850, 16);
        btnBanUser.Size = new Size(90, 38);
        btnBanUser.Font = new Font("Segoe UI Semibold", 10F);
        btnBanUser.BackColor = Color.FromArgb(220, 38, 38);
        btnBanUser.ForeColor = Color.White;
        btnBanUser.FlatStyle = FlatStyle.Flat;
        btnBanUser.FlatAppearance.BorderSize = 0;
        btnBanUser.Cursor = Cursors.Hand;
        btnBanUser.Click += BtnBanUser_Click;
        toolPanel.Controls.Add(btnBanUser);
        
        btnUnbanUser.Text = "✅ Unban";
        btnUnbanUser.Location = new Point(950, 16);
        btnUnbanUser.Size = new Size(100, 38);
        btnUnbanUser.Font = new Font("Segoe UI Semibold", 10F);
        btnUnbanUser.BackColor = successGreen;
        btnUnbanUser.ForeColor = Color.White;
        btnUnbanUser.FlatStyle = FlatStyle.Flat;
        btnUnbanUser.FlatAppearance.BorderSize = 0;
        btnUnbanUser.Cursor = Cursors.Hand;
        btnUnbanUser.Click += BtnUnbanUser_Click;
        toolPanel.Controls.Add(btnUnbanUser);
        
        btnDeleteUser.Text = "🗑️ Delete";
        btnDeleteUser.Location = new Point(1060, 16);
        btnDeleteUser.Size = new Size(100, 38);
        btnDeleteUser.Font = new Font("Segoe UI Semibold", 10F);
        btnDeleteUser.BackColor = Color.FromArgb(127, 29, 29);
        btnDeleteUser.ForeColor = Color.White;
        btnDeleteUser.FlatStyle = FlatStyle.Flat;
        btnDeleteUser.FlatAppearance.BorderSize = 0;
        btnDeleteUser.Cursor = Cursors.Hand;
        btnDeleteUser.Click += BtnDeleteUser_Click;
        toolPanel.Controls.Add(btnDeleteUser);
        
        tabUsers.Controls.Add(toolPanel);
        
        dgvUsers.Dock = DockStyle.Fill;
        StyleGrid(dgvUsers, gridBg, inputBg, textPrimary, accentPurple);
        tabUsers.Controls.Add(dgvUsers);
        dgvUsers.BringToFront();
    }
    
    private void SetupReportsTab(Color darkBg, Color cardBg, Color successGreen, Color dangerRed, Color textPrimary, Color gridBg)
    {
        var toolPanel = new Panel { 
            Dock = DockStyle.Top, 
            Height = 70,
            BackColor = cardBg,
            Padding = new Padding(15)
        };
        
        chkShowResolvedReports.Text = "   Show Resolved Reports";
        chkShowResolvedReports.Location = new Point(20, 22);
        chkShowResolvedReports.ForeColor = textPrimary;
        chkShowResolvedReports.Font = new Font("Segoe UI", 10F);
        chkShowResolvedReports.AutoSize = true;
        chkShowResolvedReports.CheckedChanged += ChkShowResolvedReports_CheckedChanged;
        toolPanel.Controls.Add(chkShowResolvedReports);
        
        btnResolveReport.Text = "✅ Mark Resolved";
        btnResolveReport.Location = new Point(920, 16);
        btnResolveReport.Size = new Size(130, 38);
        btnResolveReport.Font = new Font("Segoe UI Semibold", 10F);
        btnResolveReport.BackColor = successGreen;
        btnResolveReport.ForeColor = Color.White;
        btnResolveReport.FlatStyle = FlatStyle.Flat;
        btnResolveReport.FlatAppearance.BorderSize = 0;
        btnResolveReport.Cursor = Cursors.Hand;
        btnResolveReport.Click += BtnResolveReport_Click;
        toolPanel.Controls.Add(btnResolveReport);
        
        btnDeleteReport.Text = "🗑️ Delete";
        btnDeleteReport.Location = new Point(1060, 16);
        btnDeleteReport.Size = new Size(100, 38);
        btnDeleteReport.Font = new Font("Segoe UI Semibold", 10F);
        btnDeleteReport.BackColor = Color.FromArgb(127, 29, 29);
        btnDeleteReport.ForeColor = Color.White;
        btnDeleteReport.FlatStyle = FlatStyle.Flat;
        btnDeleteReport.FlatAppearance.BorderSize = 0;
        btnDeleteReport.Cursor = Cursors.Hand;
        btnDeleteReport.Click += BtnDeleteReport_Click;
        toolPanel.Controls.Add(btnDeleteReport);
        
        tabReports.Controls.Add(toolPanel);
        
        dgvReports.Dock = DockStyle.Fill;
        StyleGrid(dgvReports, gridBg, Color.FromArgb(40, 35, 80), textPrimary, Color.FromArgb(139, 92, 246));
        tabReports.Controls.Add(dgvReports);
        dgvReports.BringToFront();
    }
    
    private void SetupInterestsTab(Color darkBg, Color cardBg, Color accentPurple, Color successGreen, Color dangerRed, Color textPrimary, Color inputBg, Color gridBg)
    {
        var toolPanel = new Panel { 
            Dock = DockStyle.Top, 
            Height = 70,
            BackColor = cardBg,
            Padding = new Padding(15)
        };
        
        var lblEmoji = new Label { Text = "Emoji:", Location = new Point(20, 24), ForeColor = textPrimary, Font = new Font("Segoe UI", 10F), AutoSize = true };
        toolPanel.Controls.Add(lblEmoji);
        
        txtInterestEmoji.Location = new Point(75, 18);
        txtInterestEmoji.Size = new Size(60, 35);
        txtInterestEmoji.Font = new Font("Segoe UI Emoji", 14F);
        txtInterestEmoji.BackColor = inputBg;
        txtInterestEmoji.ForeColor = textPrimary;
        txtInterestEmoji.BorderStyle = BorderStyle.FixedSingle;
        txtInterestEmoji.TextAlign = HorizontalAlignment.Center;
        toolPanel.Controls.Add(txtInterestEmoji);
        
        var lblName = new Label { Text = "Name:", Location = new Point(150, 24), ForeColor = textPrimary, Font = new Font("Segoe UI", 10F), AutoSize = true };
        toolPanel.Controls.Add(lblName);
        
        txtInterestName.Location = new Point(205, 18);
        txtInterestName.Size = new Size(180, 35);
        txtInterestName.Font = new Font("Segoe UI", 11F);
        txtInterestName.BackColor = inputBg;
        txtInterestName.ForeColor = textPrimary;
        txtInterestName.BorderStyle = BorderStyle.FixedSingle;
        toolPanel.Controls.Add(txtInterestName);
        
        var lblCat = new Label { Text = "Category:", Location = new Point(400, 24), ForeColor = textPrimary, Font = new Font("Segoe UI", 10F), AutoSize = true };
        toolPanel.Controls.Add(lblCat);
        
        txtInterestCategory.Location = new Point(475, 18);
        txtInterestCategory.Size = new Size(150, 35);
        txtInterestCategory.Font = new Font("Segoe UI", 11F);
        txtInterestCategory.BackColor = inputBg;
        txtInterestCategory.ForeColor = textPrimary;
        txtInterestCategory.BorderStyle = BorderStyle.FixedSingle;
        toolPanel.Controls.Add(txtInterestCategory);
        
        btnAddInterest.Text = "➕ Add Interest";
        btnAddInterest.Location = new Point(650, 16);
        btnAddInterest.Size = new Size(130, 38);
        btnAddInterest.Font = new Font("Segoe UI Semibold", 10F);
        btnAddInterest.BackColor = successGreen;
        btnAddInterest.ForeColor = Color.White;
        btnAddInterest.FlatStyle = FlatStyle.Flat;
        btnAddInterest.FlatAppearance.BorderSize = 0;
        btnAddInterest.Cursor = Cursors.Hand;
        btnAddInterest.Click += BtnAddInterest_Click;
        toolPanel.Controls.Add(btnAddInterest);
        
        btnDeleteInterest.Text = "🗑️ Delete";
        btnDeleteInterest.Location = new Point(1060, 16);
        btnDeleteInterest.Size = new Size(100, 38);
        btnDeleteInterest.Font = new Font("Segoe UI Semibold", 10F);
        btnDeleteInterest.BackColor = Color.FromArgb(127, 29, 29);
        btnDeleteInterest.ForeColor = Color.White;
        btnDeleteInterest.FlatStyle = FlatStyle.Flat;
        btnDeleteInterest.FlatAppearance.BorderSize = 0;
        btnDeleteInterest.Cursor = Cursors.Hand;
        btnDeleteInterest.Click += BtnDeleteInterest_Click;
        toolPanel.Controls.Add(btnDeleteInterest);
        
        tabInterests.Controls.Add(toolPanel);
        
        dgvInterests.Dock = DockStyle.Fill;
        StyleGrid(dgvInterests, gridBg, inputBg, textPrimary, accentPurple);
        tabInterests.Controls.Add(dgvInterests);
        dgvInterests.BringToFront();
    }
    
    private void SetupPromptsTab(Color darkBg, Color cardBg, Color accentPurple, Color successGreen, Color dangerRed, Color textPrimary, Color inputBg, Color gridBg)
    {
        var toolPanel = new Panel { 
            Dock = DockStyle.Top, 
            Height = 70,
            BackColor = cardBg,
            Padding = new Padding(15)
        };
        
        var lblQ = new Label { Text = "Question:", Location = new Point(20, 24), ForeColor = textPrimary, Font = new Font("Segoe UI", 10F), AutoSize = true };
        toolPanel.Controls.Add(lblQ);
        
        txtPromptQuestion.Location = new Point(100, 18);
        txtPromptQuestion.Size = new Size(400, 35);
        txtPromptQuestion.Font = new Font("Segoe UI", 11F);
        txtPromptQuestion.BackColor = inputBg;
        txtPromptQuestion.ForeColor = textPrimary;
        txtPromptQuestion.BorderStyle = BorderStyle.FixedSingle;
        toolPanel.Controls.Add(txtPromptQuestion);
        
        var lblCat = new Label { Text = "Category:", Location = new Point(520, 24), ForeColor = textPrimary, Font = new Font("Segoe UI", 10F), AutoSize = true };
        toolPanel.Controls.Add(lblCat);
        
        txtPromptCategory.Location = new Point(595, 18);
        txtPromptCategory.Size = new Size(150, 35);
        txtPromptCategory.Font = new Font("Segoe UI", 11F);
        txtPromptCategory.BackColor = inputBg;
        txtPromptCategory.ForeColor = textPrimary;
        txtPromptCategory.BorderStyle = BorderStyle.FixedSingle;
        toolPanel.Controls.Add(txtPromptCategory);
        
        btnAddPrompt.Text = "➕ Add Prompt";
        btnAddPrompt.Location = new Point(770, 16);
        btnAddPrompt.Size = new Size(130, 38);
        btnAddPrompt.Font = new Font("Segoe UI Semibold", 10F);
        btnAddPrompt.BackColor = successGreen;
        btnAddPrompt.ForeColor = Color.White;
        btnAddPrompt.FlatStyle = FlatStyle.Flat;
        btnAddPrompt.FlatAppearance.BorderSize = 0;
        btnAddPrompt.Cursor = Cursors.Hand;
        btnAddPrompt.Click += BtnAddPrompt_Click;
        toolPanel.Controls.Add(btnAddPrompt);
        
        btnDeletePrompt.Text = "🗑️ Delete";
        btnDeletePrompt.Location = new Point(1060, 16);
        btnDeletePrompt.Size = new Size(100, 38);
        btnDeletePrompt.Font = new Font("Segoe UI Semibold", 10F);
        btnDeletePrompt.BackColor = Color.FromArgb(127, 29, 29);
        btnDeletePrompt.ForeColor = Color.White;
        btnDeletePrompt.FlatStyle = FlatStyle.Flat;
        btnDeletePrompt.FlatAppearance.BorderSize = 0;
        btnDeletePrompt.Cursor = Cursors.Hand;
        btnDeletePrompt.Click += BtnDeletePrompt_Click;
        toolPanel.Controls.Add(btnDeletePrompt);
        
        tabPrompts.Controls.Add(toolPanel);
        
        dgvPrompts.Dock = DockStyle.Fill;
        StyleGrid(dgvPrompts, gridBg, inputBg, textPrimary, accentPurple);
        tabPrompts.Controls.Add(dgvPrompts);
        dgvPrompts.BringToFront();
    }
    
    private void SetupLogsTab(Color darkBg, Color cardBg, Color accentPurple, Color textPrimary, Color gridBg)
    {
        var toolPanel = new Panel { 
            Dock = DockStyle.Top, 
            Height = 70,
            BackColor = cardBg,
            Padding = new Padding(15)
        };
        
        var title = new Label
        {
            Text = "📋 Activity Log - Recent Admin Actions",
            Font = new Font("Segoe UI Semibold", 12F),
            ForeColor = textPrimary,
            Location = new Point(20, 22),
            AutoSize = true
        };
        toolPanel.Controls.Add(title);
        
        btnRefreshLogs.Text = "⟳ Refresh";
        btnRefreshLogs.Location = new Point(1040, 16);
        btnRefreshLogs.Size = new Size(120, 38);
        btnRefreshLogs.Font = new Font("Segoe UI Semibold", 10F);
        btnRefreshLogs.BackColor = accentPurple;
        btnRefreshLogs.ForeColor = Color.White;
        btnRefreshLogs.FlatStyle = FlatStyle.Flat;
        btnRefreshLogs.FlatAppearance.BorderSize = 0;
        btnRefreshLogs.Cursor = Cursors.Hand;
        btnRefreshLogs.Click += BtnRefreshLogs_Click;
        toolPanel.Controls.Add(btnRefreshLogs);
        
        tabLogs.Controls.Add(toolPanel);
        
        dgvLogs.Dock = DockStyle.Fill;
        StyleGrid(dgvLogs, gridBg, Color.FromArgb(40, 35, 80), textPrimary, accentPurple);
        tabLogs.Controls.Add(dgvLogs);
        dgvLogs.BringToFront();
    }
    
    private void StyleGrid(DataGridView dgv, Color gridBg, Color headerBg, Color textColor, Color selectionColor)
    {
        dgv.AllowUserToAddRows = false;
        dgv.AllowUserToDeleteRows = false;
        dgv.AllowUserToResizeRows = false;
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgv.BackgroundColor = gridBg;
        dgv.BorderStyle = BorderStyle.None;
        dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgv.ReadOnly = true;
        dgv.RowHeadersVisible = false;
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgv.EnableHeadersVisualStyles = false;
        dgv.RowTemplate.Height = 45;
        dgv.Font = new Font("Segoe UI", 10F);
        
        // Header styling
        dgv.ColumnHeadersHeight = 50;
        dgv.ColumnHeadersDefaultCellStyle.BackColor = headerBg;
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = textColor;
        dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F);
        dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
        dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        
        // Cell styling
        dgv.DefaultCellStyle.BackColor = gridBg;
        dgv.DefaultCellStyle.ForeColor = textColor;
        dgv.DefaultCellStyle.SelectionBackColor = selectionColor;
        dgv.DefaultCellStyle.SelectionForeColor = Color.White;
        dgv.DefaultCellStyle.Padding = new Padding(10, 5, 5, 5);
        
        // Alternating row colors for better readability
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(35, 32, 70);
        dgv.AlternatingRowsDefaultCellStyle.ForeColor = textColor;
        
        dgv.GridColor = Color.FromArgb(50, 45, 90);
    }

    private TabControl tabControl;
    private TabPage tabDashboard;
    private TabPage tabUsers;
    private TabPage tabReports;
    private TabPage tabInterests;
    private TabPage tabPrompts;
    private TabPage tabLogs;
    
    private Label lblTotalUsers;
    private Label lblActiveUsers;
    private Label lblBannedUsers;
    private Label lblTotalMatches;
    private Label lblTotalMessages;
    private Label lblPendingReports;
    private Label lblTotalInterests;
    private Label lblTotalPrompts;
    private Button btnRefreshAll;
    private Button btnLogout;
    
    private TextBox txtUserSearch;
    private Button btnSearchUsers;
    private CheckBox chkShowBannedOnly;
    private Button btnBanUser;
    private Button btnUnbanUser;
    private Button btnDeleteUser;
    private DataGridView dgvUsers;
    
    private CheckBox chkShowResolvedReports;
    private Button btnResolveReport;
    private Button btnDeleteReport;
    private DataGridView dgvReports;
    
    private TextBox txtInterestName;
    private TextBox txtInterestEmoji;
    private TextBox txtInterestCategory;
    private Button btnAddInterest;
    private Button btnDeleteInterest;
    private DataGridView dgvInterests;
    
    private TextBox txtPromptQuestion;
    private TextBox txtPromptCategory;
    private Button btnAddPrompt;
    private Button btnDeletePrompt;
    private DataGridView dgvPrompts;
    
    private Button btnRefreshLogs;
    private DataGridView dgvLogs;
}
