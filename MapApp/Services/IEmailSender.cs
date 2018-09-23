using System.Threading.Tasks;

namespace MapApp.Services
{
	public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
