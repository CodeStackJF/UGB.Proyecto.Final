using System.Text.Json.Serialization;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using UGB.Proyecto.FinalInterfaces;

namespace UGB.Proyecto.FinalHelper
{
    public class EmailService : IEmailService
    {
         private readonly SettingsBase mailSettings;
        public EmailService(IOptions<SettingsBase> _mailSettings)
        {
            mailSettings = _mailSettings.Value;
        }

        public async Task SendMail(Email emailData)
        {
            if(string.IsNullOrWhiteSpace(emailData.To))
            {
                throw new HttpRequestException("To property is required.");
            }
            if(string.IsNullOrWhiteSpace(emailData.Subject))
            {
                throw new HttpRequestException("Subject property is required.");
            }
            if(string.IsNullOrWhiteSpace(emailData.Body))
            {
                throw new HttpRequestException("Body property is required.");
            }
            var email = new MimeMessage
            {
                Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail)
            };

            email.To.Add(MailboxAddress.Parse(emailData.To));
            email.From.Add(MailboxAddress.Parse(mailSettings.Mail));

            email.Subject = emailData.Subject;

            BodyBuilder builder = new BodyBuilder();
            builder.HtmlBody = emailData.Body;
            
            foreach(MimePart attachment in emailData.Attachments)
            {
                builder.Attachments.Add(attachment.FileName!, attachment.Content!.Stream!, attachment.ContentType);
            }

            email.Body = builder.ToMessageBody();
            using(var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect(mailSettings.SMTP, mailSettings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(mailSettings.Mail, mailSettings.Password, new CancellationToken());
                if(client.IsAuthenticated)
                {
                    await client.SendAsync(email);
                }
                else
                {
                    throw new HttpRequestException("Couldn't be authenticated on email server.");
                }
            }            
        }
    }

    public class Email
    {
        public string? To { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        [JsonIgnore]
        public IEnumerable<MimePart> Attachments {get; set;} = new List<MimePart>();
    }
}