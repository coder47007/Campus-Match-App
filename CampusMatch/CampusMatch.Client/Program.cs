using CampusMatch.Client.Forms;
using CampusMatch.Client.Services;

namespace CampusMatch.Client;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        
        var api = new ApiService();
        var signalR = new SignalRService();
        
        // Show login
        using var loginForm = new LoginForm(api);
        if (loginForm.ShowDialog() != DialogResult.OK)
        {
            return;
        }
        
        // Show main dashboard
        Application.Run(new MainForm(api, signalR));
    }
}
