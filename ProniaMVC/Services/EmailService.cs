﻿using ProniaMVC.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace ProniaMVC.Services
{
    public class EmailService:IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

      
        public async Task SendMailAsync(string emailTo, string subject, string body, bool isHtml = false)
        {
            SmtpClient smpt = new SmtpClient(_configuration["Email:Host"], Convert.ToInt32(_configuration["Email:Port"]));
            smpt.EnableSsl = true;
            smpt.Credentials = new NetworkCredential(_configuration["Email:LoginEmail"], _configuration["Email:Password"]);

            MailAddress from = new MailAddress(_configuration["Email:LoginEmail"], "Pronia");
            MailAddress to = new MailAddress(emailTo);

            MailMessage message = new MailMessage(from, to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isHtml;


            await smpt.SendMailAsync(message);


        }
    }
}
