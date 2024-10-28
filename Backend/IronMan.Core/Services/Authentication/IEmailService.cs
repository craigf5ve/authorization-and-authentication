using IronMan.Core.Dtos.Authentication;

namespace IronMan.Core.Services.Authentication
{
    public interface IEmailService
    {
        Task Send(string to, string subject, EmailBodyDto body,string from = null);
    }
}
