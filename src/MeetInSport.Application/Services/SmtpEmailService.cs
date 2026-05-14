using MeetInSport.Application.DTOs;
using MeetInSport.Application.Interface.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace MeetInSport.Application.Services;

public class SmtpEmailService : IEmailService {

    private readonly EmailSettings _emailSettings;
    public SmtpEmailService(IOptions<EmailSettings> emailSettings){
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body) {
        using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort){
            Credentials = new NetworkCredential(_emailSettings.SenderEmail,
            _emailSettings.Password), 
            EnableSsl = true  // for gmail.
        };
        var mailMessage = new MailMessage{
            From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false,
        };

        mailMessage.To.Add(to);
        await client.SendMailAsync(mailMessage);
    }
}