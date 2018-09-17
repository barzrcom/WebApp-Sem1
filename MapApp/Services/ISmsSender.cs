using System.Threading.Tasks;

namespace MapApp.Services
{
	public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
