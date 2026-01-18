using CampusMatch.Client.Services;

namespace CampusMatch.Client.Forms;

public partial class LoginForm : Form
{
    private readonly ApiService _api;
    private bool isRegisterMode = false;

    public LoginForm(ApiService api)
    {
        _api = api;
        InitializeComponent();
    }

    private void BtnRegister_Click(object? sender, EventArgs e)
    {
        ToggleMode();
    }

    private void ToggleMode()
    {
        isRegisterMode = !isRegisterMode;
        
        // Toggle field visibility
        pnlNameField.Visible = isRegisterMode;
        pnlPhoneField.Visible = isRegisterMode;
        pnlInstagramField.Visible = isRegisterMode;
        lnkForgot.Visible = !isRegisterMode;
        lblOrDivider.Visible = !isRegisterMode;
        
        if (isRegisterMode)
        {
            // Register mode layout
            lblFormTitle.Text = "Create Account 🚀";
            lblFormSubtitle.Text = "Join the CampusMatch community";
            
            pnlNameField.Location = new Point(25, 95);
            pnlEmailField.Location = new Point(25, 155);
            pnlPasswordField.Location = new Point(25, 215);
            pnlPhoneField.Location = new Point(25, 275);
            pnlInstagramField.Location = new Point(25, 335);
            btnLogin.Location = new Point(25, 400);
            btnRegister.Location = new Point(25, 465);
            
            // Adjust card size for more fields
            pnlCard.Size = new Size(450, 540);
        }
        else
        {
            // Login mode layout
            lblFormTitle.Text = "Welcome Back! 👋";
            lblFormSubtitle.Text = "Sign in to continue to CampusMatch";
            
            pnlEmailField.Location = new Point(25, 95);
            pnlPasswordField.Location = new Point(25, 155);
            lnkForgot.Location = new Point(250, 215);
            btnLogin.Location = new Point(25, 250);
            lblOrDivider.Location = new Point(25, 320);
            btnRegister.Location = new Point(25, 355);
            
            // Reset card size
            pnlCard.Size = new Size(450, 440);
        }
        
        btnLogin.Text = isRegisterMode ? "Create Account  →" : "Sign In  →";
        btnRegister.Text = isRegisterMode ? "← Back to Sign In" : "Create New Account";
        
        // Clear fields when switching modes
        if (!isRegisterMode)
        {
            txtName.Clear();
            txtPhone.Clear();
            txtInstagram.Clear();
        }
    }

    private async void BtnLogin_Click(object? sender, EventArgs e)
    {
        await HandleLogin();
    }

    private async Task HandleLogin()
    {
        btnLogin.Enabled = false;
        var originalText = btnLogin.Text;
        btnLogin.Text = "Please wait...";

        try
        {
            if (isRegisterMode)
            {
                if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtEmail.Text) || 
                    string.IsNullOrWhiteSpace(txtPassword.Text) || string.IsNullOrWhiteSpace(txtPhone.Text))
                {
                    ShowError("Please fill in all required fields.", "Missing Information");
                    return;
                }

                if (!txtEmail.Text.Trim().EndsWith("@mybvc.ca", StringComparison.OrdinalIgnoreCase))
                {
                    ShowError("Please use a valid @mybvc.ca email address.", "Invalid Email");
                    return;
                }

                if (txtPassword.Text.Length < 8)
                {
                    ShowError("Password must be at least 8 characters long.", "Weak Password");
                    return;
                }

                var instagram = string.IsNullOrWhiteSpace(txtInstagram.Text) ? null : txtInstagram.Text.Trim();
                var result = await _api.RegisterAsync(txtEmail.Text.Trim(), txtPassword.Text, txtName.Text.Trim(), txtPhone.Text.Trim(), instagram);
                if (result == null)
                {
                    ShowError("Registration failed. Please check your information and try again.", "Registration Failed");
                    return;
                }
                
                ShowSuccess("Account created successfully! Welcome to CampusMatch! 🎉");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    ShowError("Please enter your email and password.", "Missing Information");
                    return;
                }

                var result = await _api.LoginAsync(txtEmail.Text.Trim(), txtPassword.Text);
                if (result == null)
                {
                    ShowError("Invalid email or password. Please try again.", "Login Failed");
                    return;
                }
                
                // Check if admin and offer admin dashboard
                if (result.IsAdmin)
                {
                    var choice = MessageBox.Show(
                        "Welcome, Admin! 🛡️\n\nWould you like to open the Admin Dashboard?\n\nYes = Admin Dashboard\nNo = Regular App",
                        "Admin Login",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                    
                    if (choice == DialogResult.Yes)
                    {
                        try
                        {
                            this.Hide();
                            using var adminDashboard = new AdminDashboard(_api);
                            adminDashboard.ShowDialog();
                            this.DialogResult = DialogResult.Cancel; // Don't open MainForm after
                            this.Close();
                            return;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error opening Admin Dashboard:\n\n{ex.Message}\n\n{ex.InnerException?.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Show();
                            return;
                        }
                    }
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        finally
        {
            btnLogin.Enabled = true;
            btnLogin.Text = originalText;
        }
    }

    private void ShowError(string message, string title)
    {
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private void ShowSuccess(string message)
    {
        MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void LnkForgot_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        var email = txtEmail.Text.Trim();
        if (string.IsNullOrWhiteSpace(email))
        {
            MessageBox.Show("Please enter your email address first.", "Email Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        
        _ = HandleForgotPassword(email);
    }

    private async Task HandleForgotPassword(string email)
    {
        lnkForgot.Enabled = false;
        lnkForgot.Text = "Sending...";
        
        try
        {
            await _api.ForgotPasswordAsync(email);
            MessageBox.Show("If the email exists, a password reset link has been sent.", "Password Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        finally
        {
            lnkForgot.Enabled = true;
            lnkForgot.Text = "Forgot password?";
        }
    }

    private static System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        var path = new System.Drawing.Drawing2D.GraphicsPath();
        path.AddArc(bounds.X, bounds.Y, radius * 2, radius * 2, 180, 90);
        path.AddArc(bounds.Right - radius * 2, bounds.Y, radius * 2, radius * 2, 270, 90);
        path.AddArc(bounds.Right - radius * 2, bounds.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
        path.CloseFigure();
        return path;
    }
}
