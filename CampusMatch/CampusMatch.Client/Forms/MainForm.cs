using CampusMatch.Client.Services;
using CampusMatch.Client.Views;

namespace CampusMatch.Client.Forms;

public partial class MainForm : Form
{
    private readonly ApiService _api;
    private readonly SignalRService _signalR;

    // Views
    private DiscoverView? discoverView;
    private MatchesView? matchesView;
    private ProfileView? profileView;
    private SettingsView? settingsView;

    private static readonly Color PrimaryColor = Color.FromArgb(124, 58, 237);

    public MainForm(ApiService api, SignalRService signalR)
    {
        _api = api;
        _signalR = signalR;
        InitializeComponent();
        InitializeViews();
        InitializeSignalR();
        
        // Update title with user name
        this.Text = "CampusMatch - " + (_api.CurrentStudent?.Name ?? "Dashboard");
        
        // Show discover view by default
        ShowView("discover");
        
        this.FormClosing += async (s, e) => await _signalR.DisposeAsync();
    }

    private void InitializeViews()
    {
        discoverView = new DiscoverView(_api);
        matchesView = new MatchesView(_api, _signalR);
        profileView = new ProfileView(_api);
        settingsView = new SettingsView(_api);
    }

    private async void InitializeSignalR()
    {
        try
        {
            var token = _api.GetToken();
            if (token != null) await _signalR.ConnectAsync(token);

            _signalR.OnNewMatch += match =>
            {
                this.BeginInvoke(() =>
                {
                    matchesView?.RefreshMatches();
                });
            };

            _signalR.OnMessageReceived += message =>
            {
                this.BeginInvoke(() =>
                {
                    matchesView?.HandleIncomingMessage(message);
                });
            };
        }
        catch { /* SignalR is optional */ }
    }

    private void BtnDiscover_Click(object? sender, EventArgs e)
    {
        ShowView("discover");
    }

    private void BtnMatches_Click(object? sender, EventArgs e)
    {
        ShowView("matches");
    }

    private void BtnProfile_Click(object? sender, EventArgs e)
    {
        ShowView("profile");
    }

    private void BtnSettings_Click(object? sender, EventArgs e)
    {
        ShowView("settings");
    }

    private void BtnLogout_Click(object? sender, EventArgs e)
    {
        _api.Logout();
        Application.Restart();
    }

    private void ShowView(string viewName)
    {
        pnlContent.Controls.Clear();

        btnDiscover.BackColor = Color.Transparent;
        btnMatches.BackColor = Color.Transparent;
        btnProfile.BackColor = Color.Transparent;
        btnSettings.BackColor = Color.Transparent;

        Control? view = null;
        switch (viewName)
        {
            case "discover":
                view = discoverView;
                btnDiscover.BackColor = PrimaryColor;
                discoverView?.LoadProfiles();
                break;
            case "matches":
                view = matchesView;
                btnMatches.BackColor = PrimaryColor;
                matchesView?.RefreshMatches();
                break;
            case "profile":
                view = profileView;
                btnProfile.BackColor = PrimaryColor;
                profileView?.LoadProfile();
                break;
            case "settings":
                view = settingsView;
                btnSettings.BackColor = PrimaryColor;
                settingsView?.LoadSettings();
                break;
        }

        if (view != null)
        {
            view.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(view);
        }
    }

    /// <summary>
    /// Public method to navigate to matches view (used from DiscoverView match popup)
    /// </summary>
    public void NavigateToMatches()
    {
        ShowView("matches");
    }
}
