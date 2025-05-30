using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace CompanyManagementSystem.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                _logger.LogInformation("Preparing to send email to {To}", to);

                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var server = smtpSettings["Server"] ?? throw new InvalidOperationException("SMTP Server not configured");
                var port = smtpSettings["Port"] ?? throw new InvalidOperationException("SMTP Port not configured");
                var username = smtpSettings["Username"] ?? throw new InvalidOperationException("SMTP Username not configured");
                var password = smtpSettings["Password"] ?? throw new InvalidOperationException("SMTP Password not configured");
                var fromEmail = smtpSettings["FromEmail"] ?? throw new InvalidOperationException("From Email not configured");
                var fromName = smtpSettings["FromName"] ?? "Purchase Order System";

                _logger.LogInformation("SMTP Configuration: Server={Server}, Port={Port}, FromEmail={FromEmail}, FromName={FromName}", 
                    server, port, fromEmail, fromName);

                using var client = new SmtpClient()
                {
                    Host = server,
                    Port = int.Parse(port),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(username, password)
                };

                // Format the body as HTML
                var htmlBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #2c3e50;'>{subject}</h2>
                            <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px;'>
                                {body.Replace("\n", "<br>")}
                            </div>
                            <p style='color: #666; font-size: 0.9em; margin-top: 20px;'>
                                This is an automated message from the Purchase Order System.
                            </p>
                        </div>
                    </body>
                    </html>";

                using var message = new MailMessage()
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };
                message.To.Add(to);

                _logger.LogInformation("Attempting to send email to {To} with subject: {Subject}", to, subject);
                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}. Error: {Message}", to, ex.Message);
                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner exception: {Message}", ex.InnerException.Message);
                }
                throw;
            }
        }
    }
}