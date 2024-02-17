using System.Net;
using System.Net.Mail;

public class EmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;

    // ToDo : Use the email service in the AuthController(password-reset) and ContactController to send an email with reset-link / response.
    public EmailService(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword)
    {
        _smtpServer = smtpServer;
        _smtpPort = smtpPort;
        _smtpUsername = smtpUsername;
        _smtpPassword = smtpPassword;
    }

    public void SendResetEmail(string userEmail, string resetToken)
    {
        try
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(_smtpUsername);
                mail.To.Add(userEmail);
                mail.Subject = "Password Reset";
                mail.Body = $"Click the following link to reset your password: {GenerateResetLink(resetToken)}";
                mail.IsBodyHtml = false;

                using (SmtpClient smtp = new SmtpClient(_smtpServer, _smtpPort))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }

                Console.WriteLine($"Reset email sent to {userEmail}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending reset email: {ex.Message}");
        }
    }

    private string GenerateResetLink(string resetToken)
    {
        // reset-password link
        return $"http://http://localhost:3000/reset-password?token={resetToken}";
    }
}
