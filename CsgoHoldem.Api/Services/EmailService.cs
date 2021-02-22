using System.Net;
using System.Net.Mail;
using CsgoHoldem.Api.Config;
using Microsoft.AspNetCore.Mvc.Razor;

namespace CsgoHoldem.Api.Services
{
    
    public interface IEmailService
    {
        public void SendTextEmail(string email, string subject, string body);
        public void SendHtmlEmail(string email, string subject, string body, RazorView model);
    }
    
    public class EmailService: IEmailService
    {
        private readonly AppSettings _appSettings;
        
        public EmailService(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }
        
        private SmtpClient GetSmtpClient()
        {
            SmtpClient smtpClient = new SmtpClient(_appSettings.SmtpSettings.SmtpHost);
            smtpClient.UseDefaultCredentials = false;
            // We do not set the credentials because the smtp server is set to allow all anonymous in network mail.
            if (!_appSettings.SmtpSettings.IsAnonymous)
            {
                smtpClient.Credentials =
                    new NetworkCredential(_appSettings.SmtpSettings.SmtpUsername, _appSettings.SmtpSettings.SmtpPassword);
            }
            return smtpClient;
        }
        
        public void SendTextEmail(string email, string subject, string body)
        {
            
            
            var smtpClient = GetSmtpClient();
            var mailMessage = new MailMessage
            {
                From = new MailAddress("no-reply@CsgoHoldem.com")
            };
            // MailMessage mailMessage = new MailMessage();
            // mailMessage.From = new MailAddress("no-reply@CsgoHoldem.com");
            mailMessage.To.Add(email);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            smtpClient.Send(mailMessage);
            smtpClient.Dispose();
        }
        
        public void SendHtmlEmail(string email, string subject, string body, RazorView model)
        {
            var smtpClient = GetSmtpClient();
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("no-reply@CsgoHoldem.com");
            mailMessage.To.Add(email);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            smtpClient.Send(mailMessage);
            smtpClient.Dispose();
        }

    }
}