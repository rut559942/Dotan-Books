using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MimeKit;

namespace Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var smtpHost = _config["Email:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.TryParse(_config["Email:SmtpPort"], out var parsedPort) ? parsedPort : 587;
            var senderName = _config["Email:SenderName"] ?? "Dotan Books";
            var senderAddress = _config["Email:SenderAddress"]
                ?? throw new InvalidOperationException("Email:SenderAddress is missing from configuration.");
            var senderPassword = _config["Email:SenderPassword"]
                ?? throw new InvalidOperationException("Email:SenderPassword is missing from configuration.");

            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(senderName, senderAddress));

            emailMessage.To.Add(new MailboxAddress("", toEmail));

            emailMessage.Subject = subject;

            emailMessage.Body = new TextPart("html") { Text = message };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                
                await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);

                
                await client.AuthenticateAsync(senderAddress, senderPassword);

                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
