using Microsoft.AspNetCore.Authorization;
using Nest;
using System.Net;
using System.Net.Mail;
namespace Coursenix.Repository
{
    public class EmailService
    {
        private readonly IConfiguration config;

        public EmailService(IConfiguration configuration)
        {
            config = configuration;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com", 587)

            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    config["EmailSettings:Gmail:Email"],
                    config["EmailSettings:Gmail:Password"]
                ),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(config["EmailSettings:Gmail:Email"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }

}