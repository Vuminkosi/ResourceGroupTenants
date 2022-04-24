using System;
using System.Text;

namespace ResourceGroupTenants.Relational.Emails.FluentServices
{
    public class RazorFluentEmailMessage: FluentEmailMessage
    {

        public StringBuilder TemplateMessage { get; set; }
        public Object TemplateObjects { get; set; }

        public bool IsHtml { get; set; } = true;
    }
}
