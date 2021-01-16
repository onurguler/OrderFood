using System.Threading.Tasks;

namespace OrderFood.Web.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
