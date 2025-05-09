using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Online_Store.Interface;
using Online_Store.Models;

public class EmailSender : IEmailSender
{
    private readonly SmtpSettings _smtpSettings;

    public EmailSender(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var smtpClient = new SmtpClient(_smtpSettings.Host)
        {
            Port = _smtpSettings.Port,
            Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
            EnableSsl = true, // تأكد من تمكين SSL للتأكد من الأمان
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_smtpSettings.Username),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true, // تأكد من أن البريد يحتوي على تنسيق HTML
        };

        mailMessage.To.Add(email);

        try
        {
            await smtpClient.SendMailAsync(mailMessage); // إرسال البريد الإلكتروني
        }
        catch (Exception ex)
        {
            // في حالة حدوث خطأ أثناء إرسال البريد
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
