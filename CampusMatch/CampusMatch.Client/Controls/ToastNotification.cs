#nullable enable

using System.Drawing.Drawing2D;

namespace CampusMatch.Client.Controls;

/// <summary>
/// Toast notification that slides in from the top, displays briefly, then fades out.
/// </summary>
public class ToastNotification : Form
{
    private readonly System.Windows.Forms.Timer _displayTimer;
    private readonly System.Windows.Forms.Timer _fadeTimer;
    private readonly Label _lblMessage;
    private readonly Label _lblIcon;
    private double _opacity = 0;
    private bool _fadingIn = true;
    
    public enum ToastType { Success, Error, Warning, Info }
    
    private ToastNotification(string message, ToastType type)
    {
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        ShowInTaskbar = false;
        TopMost = true;
        Size = new Size(350, 60);
        Opacity = 0;
        
        // Colors based on type
        var (bgColor, icon) = type switch
        {
            ToastType.Success => (Color.FromArgb(34, 197, 94), "✓"),
            ToastType.Error => (Color.FromArgb(239, 68, 68), "✕"),
            ToastType.Warning => (Color.FromArgb(245, 158, 11), "⚠"),
            ToastType.Info => (Color.FromArgb(59, 130, 246), "ℹ"),
            _ => (Color.FromArgb(124, 58, 237), "•")
        };
        
        BackColor = bgColor;
        
        _lblIcon = new Label
        {
            Text = icon,
            Font = new Font("Segoe UI", 16f, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(15, 15),
            Size = new Size(30, 30),
            TextAlign = ContentAlignment.MiddleCenter
        };
        
        _lblMessage = new Label
        {
            Text = message,
            Font = new Font("Segoe UI", 10f),
            ForeColor = Color.White,
            Location = new Point(50, 10),
            Size = new Size(280, 40),
            TextAlign = ContentAlignment.MiddleLeft
        };
        
        Controls.Add(_lblIcon);
        Controls.Add(_lblMessage);
        
        // Position at top center of screen
        var screen = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 1920, 1080);
        Location = new Point(
            screen.Left + (screen.Width - Width) / 2,
            screen.Top + 20
        );
        
        _displayTimer = new System.Windows.Forms.Timer { Interval = 3000 };
        _displayTimer.Tick += (s, e) =>
        {
            _displayTimer.Stop();
            _fadingIn = false;
            _fadeTimer.Start();
        };
        
        _fadeTimer = new System.Windows.Forms.Timer { Interval = 30 };
        _fadeTimer.Tick += FadeTimer_Tick;
        
        // Round corners
        Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
    }
    
    [System.Runtime.InteropServices.DllImport("Gdi32.dll")]
    private static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int cx, int cy);
    
    private void FadeTimer_Tick(object? sender, EventArgs e)
    {
        if (_fadingIn)
        {
            _opacity += 0.1;
            if (_opacity >= 1)
            {
                _opacity = 1;
                _fadeTimer.Stop();
                _displayTimer.Start();
            }
        }
        else
        {
            _opacity -= 0.1;
            if (_opacity <= 0)
            {
                _fadeTimer.Stop();
                Close();
            }
        }
        Opacity = _opacity;
    }
    
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        _fadeTimer.Start();
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _displayTimer?.Dispose();
            _fadeTimer?.Dispose();
        }
        base.Dispose(disposing);
    }
    
    // Static show methods
    public static void ShowSuccess(string message) => Show(message, ToastType.Success);
    public static void ShowError(string message) => Show(message, ToastType.Error);
    public static void ShowWarning(string message) => Show(message, ToastType.Warning);
    public static void ShowInfo(string message) => Show(message, ToastType.Info);
    
    public static void Show(string message, ToastType type)
    {
        var toast = new ToastNotification(message, type);
        toast.Show();
    }
}
