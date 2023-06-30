using System.Threading.Tasks;

namespace A3_API_Project.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
