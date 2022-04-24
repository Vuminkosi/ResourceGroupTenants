using System.Threading.Tasks;

using FluentEmail.Core.Models;

namespace ResourceGroupTenants.Relational.Emails.FluentServices
{
    public interface IFluentMailService
    {
        Task<SendResponse> SendEmailAsync(FluentEmailMessage message, string password = "Roots@97");
        Task<SendResponse> SendTemplatedEmailAsync(RazorFluentEmailMessage message, string password = "Roots@97");
    }
}