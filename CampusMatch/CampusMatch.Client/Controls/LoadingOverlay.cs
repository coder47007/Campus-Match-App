#nullable enable

using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace CampusMatch.Client.Controls;

/// <summary>
/// A semi-transparent loading overlay with animated spinner.
/// Can be shown over any control during async operations.
/// </summary>
public class LoadingOverlay : UserControl
{
    private readonly System.Windows.Forms.Timer _animationTimer;
    private int _rotationAngle = 0;
    private string _message = "Loading...";
    
    public LoadingOverlay()
    {
        SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        
        BackColor = Color.FromArgb(180, 20, 18, 50);
        Dock = DockStyle.Fill;
        Visible = false;
        
        _animationTimer = new System.Windows.Forms.Timer { Interval = 50 };
        _animationTimer.Tick += (s, e) =>
        {
            _rotationAngle = (_rotationAngle + 30) % 360;
            Invalidate();
        };
    }
    
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Message
    {
        get => _message;
        set { _message = value; Invalidate(); }
    }
    
    public void Show(string? message = null)
    {
        if (message != null) _message = message;
        Visible = true;
        BringToFront();
        _animationTimer.Start();
    }
    
    public new void Hide()
    {
        _animationTimer.Stop();
        Visible = false;
    }
    
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        
        // Draw spinner
        int size = 40;
        int centerX = Width / 2;
        int centerY = Height / 2 - 20;
        
        using var pen = new Pen(Color.FromArgb(124, 58, 237), 4);
        pen.StartCap = LineCap.Round;
        pen.EndCap = LineCap.Round;
        
        for (int i = 0; i < 8; i++)
        {
            float angle = (i * 45 + _rotationAngle) * (float)Math.PI / 180;
            float alpha = (i + 1) / 8f;
            
            using var segmentPen = new Pen(Color.FromArgb((int)(alpha * 255), 124, 58, 237), 4);
            segmentPen.StartCap = LineCap.Round;
            segmentPen.EndCap = LineCap.Round;
            
            float innerRadius = size / 3f;
            float outerRadius = size / 2f;
            
            float x1 = centerX + (float)Math.Cos(angle) * innerRadius;
            float y1 = centerY + (float)Math.Sin(angle) * innerRadius;
            float x2 = centerX + (float)Math.Cos(angle) * outerRadius;
            float y2 = centerY + (float)Math.Sin(angle) * outerRadius;
            
            g.DrawLine(segmentPen, x1, y1, x2, y2);
        }
        
        // Draw message
        using var font = new Font("Segoe UI", 11f, FontStyle.Regular);
        using var brush = new SolidBrush(Color.White);
        var textSize = g.MeasureString(_message, font);
        g.DrawString(_message, font, brush, centerX - textSize.Width / 2, centerY + size / 2 + 10);
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _animationTimer?.Dispose();
        }
        base.Dispose(disposing);
    }
}
