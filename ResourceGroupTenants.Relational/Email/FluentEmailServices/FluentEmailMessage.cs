using System.Collections.Generic;

using FluentEmail.Core.Models;

namespace ResourceGroupTenants.Relational.Emails.FluentServices
{
    public class FluentEmailMessage
    {
        public string HostServerName { get; set; } = "mail.rootinsure.online";
        public string FromEmail { get; set; } = "notifications@rootinsure.online";
        public string FromName { get; set; } = "RootInsure Cloud Services";
        public string ToName { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public IEnumerable<Address> CCs { get; set; }
        public IEnumerable<Attachment> Attachments { get; set; }
    }
}
