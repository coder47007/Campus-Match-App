using System.Drawing.Drawing2D;

namespace CampusMatch.Client.Helpers;

/// <summary>
/// Shared UI helper class containing reusable styling components
/// for consistent modern UI styling across all views.
/// </summary>
public static class UIHelper
{
    #region Color Palette

    /// <summary>Primary purple brand color</summary>
    public static readonly Color PrimaryPurple = Color.FromArgb(124, 58, 237);
    
    /// <summary>Primary pink accent color</summary>
    public static readonly Color PrimaryPink = Color.FromArgb(236, 72, 153);
    
    /// <summary>Lighter purple for hover states</summary>
    public static readonly Color PurpleHover = Color.FromArgb(139, 92, 246);
    
    /// <summary>Main dark background</summary>
    public static readonly Color DarkBackground = Color.FromArgb(30, 27, 75);
    
    /// <summary>Slightly lighter background for headers</summary>
    public static readonly Color HeaderBackground = Color.FromArgb(35, 30, 70);
    
    /// <summary>Card/Section panel background</summary>
    public static readonly Color CardBackground = Color.FromArgb(45, 40, 90);
    
    /// <summary>Input field background</summary>
    public static readonly Color InputBackground = Color.FromArgb(60, 55, 105);
    
    /// <summary>Success green color</summary>
    public static readonly Color SuccessGreen = Color.FromArgb(34, 197, 94);
    
    /// <summary>Danger red color</summary>
    public static readonly Color DangerRed = Color.FromArgb(239, 68, 68);
    
    /// <summary>Warning yellow color</summary>
    public static readonly Color WarningYellow = Color.FromArgb(234, 179, 8);
    
    /// <summary>Text color - primary white</summary>
    public static readonly Color TextPrimary = Color.White;
    
    /// <summary>Text color - secondary/muted</summary>
    public static readonly Color TextSecondary = Color.FromArgb(180, 180, 200);

    #endregion

    #region Graphics Helpers

    /// <summary>
    /// Creates a rounded rectangle graphics path.
    /// </summary>
    public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        int diameter = radius * 2;
        
        // Top left arc
        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
        // Top right arc
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
        // Bottom right arc
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        // Bottom left arc
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        
        path.CloseFigure();
        return path;
    }

    /// <summary>
    /// Creates a rounded region for a control.
    /// </summary>
    public static void ApplyRoundedCorners(Control control, int radius)
    {
        var rect = new Rectangle(0, 0, control.Width, control.Height);
        using var path = RoundedRect(rect, radius);
        control.Region = new Region(path);
    }

    #endregion

    #region Button Paint Handlers

    /// <summary>
    /// Paints a button with gradient background and rounded corners (primary style).
    /// Use: button.Paint += (s, e) => UIHelper.PaintGradientButton((Button)s!, e);
    /// </summary>
    public static void PaintGradientButton(Button btn, PaintEventArgs e, int radius = 12)
    {
        var rect = new Rectangle(0, 0, btn.Width, btn.Height);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        using var path = RoundedRect(rect, radius);
        using var brush = new LinearGradientBrush(
            rect,
            PrimaryPurple,
            Color.FromArgb(168, 85, 247), // Lighter purple
            LinearGradientMode.Horizontal);
        
        e.Graphics.FillPath(brush, path);
        
        // Draw text
        using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        e.Graphics.DrawString(btn.Text, btn.Font, Brushes.White, rect, sf);
        
        // Apply rounded corners
        btn.Region = new Region(path);
    }

    /// <summary>
    /// Paints a button with solid rounded corners.
    /// </summary>
    public static void PaintRoundedButton(Button btn, PaintEventArgs e, Color backgroundColor, int radius = 12)
    {
        var rect = new Rectangle(0, 0, btn.Width, btn.Height);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        using var path = RoundedRect(rect, radius);
        using var brush = new SolidBrush(backgroundColor);
        
        e.Graphics.FillPath(brush, path);
        
        // Draw text
        using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        using var textBrush = new SolidBrush(btn.ForeColor);
        e.Graphics.DrawString(btn.Text, btn.Font, textBrush, rect, sf);
        
        // Apply rounded corners
        btn.Region = new Region(path);
    }

    /// <summary>
    /// Paints a circular action button with gradient (for Like, Pass, etc).
    /// </summary>
    public static void PaintCircularButton(Button btn, PaintEventArgs e, Color startColor, Color endColor)
    {
        int size = Math.Min(btn.Width, btn.Height);
        var rect = new Rectangle(0, 0, size, size);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        using var path = new GraphicsPath();
        path.AddEllipse(rect);
        
        using var brush = new LinearGradientBrush(
            rect,
            startColor,
            endColor,
            LinearGradientMode.ForwardDiagonal);
        
        e.Graphics.FillPath(brush, path);
        
        // Draw text/emoji
        using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        e.Graphics.DrawString(btn.Text, btn.Font, new SolidBrush(btn.ForeColor), rect, sf);
        
        // Apply circular region
        btn.Region = new Region(path);
    }

    #endregion

    #region Panel Paint Handlers

    /// <summary>
    /// Paints a panel with rounded corners and subtle shadow.
    /// </summary>
    public static void PaintCardWithShadow(Panel panel, PaintEventArgs e, int radius = 20)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        // Draw shadow
        var shadowRect = new Rectangle(4, 4, panel.Width - 5, panel.Height - 5);
        using var shadowPath = RoundedRect(shadowRect, radius);
        using var shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0));
        e.Graphics.FillPath(shadowBrush, shadowPath);
        
        // Draw card
        var cardRect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
        using var path = RoundedRect(cardRect, radius);
        using var brush = new SolidBrush(panel.BackColor);
        e.Graphics.FillPath(brush, path);
    }

    /// <summary>
    /// Paints a panel with gradient background (for headers).
    /// </summary>
    public static void PaintGradientPanel(Panel panel, PaintEventArgs e, bool horizontal = true)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        using var brush = new LinearGradientBrush(
            panel.ClientRectangle,
            PrimaryPurple,
            PrimaryPink,
            horizontal ? LinearGradientMode.Horizontal : LinearGradientMode.ForwardDiagonal);
        
        e.Graphics.FillRectangle(brush, panel.ClientRectangle);
    }

    /// <summary>
    /// Paints a section panel with rounded corners and solid background.
    /// </summary>
    public static void PaintSectionPanel(Panel panel, PaintEventArgs e, int radius = 15)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
        using var path = RoundedRect(rect, radius);
        using var brush = new SolidBrush(CardBackground);
        e.Graphics.FillPath(brush, path);
        
        // Subtle border
        using var pen = new Pen(Color.FromArgb(60, 255, 255, 255), 1);
        e.Graphics.DrawPath(pen, path);
    }

    /// <summary>
    /// Paints an input field container with rounded corners.
    /// </summary>
    public static void PaintInputContainer(Panel panel, PaintEventArgs e, int radius = 12)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
        using var path = RoundedRect(rect, radius);
        using var brush = new SolidBrush(InputBackground);
        e.Graphics.FillPath(brush, path);
        
        // Subtle border
        using var pen = new Pen(Color.FromArgb(80, 124, 58, 237), 1);
        e.Graphics.DrawPath(pen, path);
    }

    #endregion

    #region Factory Methods

    /// <summary>
    /// Creates a styled primary button with gradient.
    /// </summary>
    public static Button CreatePrimaryButton(string text, int width = 150, int height = 45)
    {
        var btn = new Button
        {
            Text = text,
            Size = new Size(width, height),
            BackColor = PrimaryPurple,
            ForeColor = TextPrimary,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.FlatAppearance.MouseOverBackColor = PurpleHover;
        btn.Paint += (s, e) => PaintGradientButton((Button)s!, e);
        return btn;
    }

    /// <summary>
    /// Creates a styled secondary (outline) button.
    /// </summary>
    public static Button CreateSecondaryButton(string text, int width = 150, int height = 45)
    {
        var btn = new Button
        {
            Text = text,
            Size = new Size(width, height),
            BackColor = Color.Transparent,
            ForeColor = PrimaryPurple,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btn.FlatAppearance.BorderColor = PrimaryPurple;
        btn.FlatAppearance.BorderSize = 2;
        btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(250, 245, 255);
        return btn;
    }

    #endregion
}
