using System.Threading.Tasks;

namespace OrderFood.Infrastructure.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
