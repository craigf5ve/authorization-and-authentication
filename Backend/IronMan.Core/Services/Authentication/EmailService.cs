using IronMan.Core.Helpers;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using MailKit.Security;
using IronMan.Core.Dtos.Authentication;
using Microsoft.Extensions.Logging;

namespace IronMan.Core.Services.Authentication
{
    public class EmailService : IEmailService
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<AppSettings> appSettings, ILogger<EmailService> logger)
        {
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        public async Task Send(string to, string subject, EmailBodyDto body, string from = null)
        {
            try
            {
                string currentDirectory = Environment.CurrentDirectory;
                var PathToFile = currentDirectory + Path.DirectorySeparatorChar.ToString() + "EmailTemplates" + Path.DirectorySeparatorChar.ToString() + "authEmail.html";
                // create message
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(from ?? _appSettings.EmailFrom));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;

                //edit html and add template
                string HTMLBody = "";
                using (StreamReader streamReader = File.OpenText(PathToFile))
                {
                    HTMLBody = streamReader.ReadToEnd();
                }

                //{0}: Subject
                //{1}: Html
                //{2}: Button Link
                //{3}: Button Text

                string messageBody = string.Format(HTMLBody,
                       body.subject,
                       body.body,
                       body.link,
                       body.buttontext
                       );

                email.Body = new TextPart(TextFormat.Html) { Text = messageBody };

                // send email
                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                await smtp.ConnectAsync(_appSettings.SmtpHost, _appSettings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_appSettings.SmtpUser, _appSettings.SmtpPass);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email");
                throw;
            }
        }

        private class EmailTemplate
        {

        }
    }


   
}
