using System.Collections;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Razor;
using FluentEmail.Smtp;

namespace ResourceGroupTenants.Relational.Emails.FluentServices
{
    public class FluentMailService : IFluentMailService
    {

        public async Task<SendResponse> SendTemplatedEmailAsync(RazorFluentEmailMessage message, string password = "Roots@97")
        {
            Email.DefaultSender = new SmtpSender(() => new SmtpClient(message.HostServerName)
            {
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Port = 587,
                Credentials = new NetworkCredential(message.FromEmail, password),
            });


            Email.DefaultRenderer = new RazorRenderer();
            var email = await Email
                                .From(message.FromEmail, message.FromName)
                                .To(message.ToEmail, message.ToName)
                                .CC(message.CCs)
                                .Subject(message.Subject)
                                .Attach(message.Attachments)
                                .UsingTemplate(message.TemplateMessage.ToString(), message.TemplateObjects)
                                .HighPriority()
                                .SendAsync();


            return email;
        }

        public async Task<SendResponse> SendEmailAsync(FluentEmailMessage message, string password = "Roots@97")
        {
            Email.DefaultSender = new SmtpSender(() => new SmtpClient(message.HostServerName)
            {
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Port = 587,
                Credentials = new NetworkCredential(message.FromEmail, password)
            });

            var email = await Email
                                .From(message.FromEmail, message.FromName)
                                .To(message.ToEmail, message.ToName)
                                .CC(message.CCs)
                                .Subject(message.Subject)
                                .Attach(message.Attachments)
                                .Body(message.Message, false)
                                .HighPriority()
                                .SendAsync();

            return email;
        }

    }
}
