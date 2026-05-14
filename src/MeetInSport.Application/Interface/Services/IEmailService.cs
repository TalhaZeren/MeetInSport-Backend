namespace MeetInSport.Application.Interface.Services;

public interface IEmailService {

    Task SendEmailAsync (string to, string subject, string body);
}