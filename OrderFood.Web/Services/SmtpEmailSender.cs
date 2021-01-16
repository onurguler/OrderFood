using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace OrderFood.Web.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;
        private readonly bool _enableSSL;

        public SmtpEmailSender(string host, int port, string username, string password, bool enableSSL)
        {
            _enableSSL = enableSSL;
            _password = password;
            _username = username;
            _port = port;
            _host = host;
        }
        public Task SendEmailAsync(string to, string subject, string body)
        {
            var client = new SmtpClient(_host, _port)
            {
                Credentials = new NetworkCredential(_username, _password),
                EnableSsl = _enableSSL,
            };

            return client.SendMailAsync(new MailMessage(_username, to, subject, body)
            {
                IsBodyHtml = true
            });
        }
    }
}
