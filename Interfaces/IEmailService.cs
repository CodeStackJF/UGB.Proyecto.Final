using UGB.Proyecto.FinalHelper;

namespace UGB.Proyecto.FinalInterfaces
{
    public interface IEmailService
    {
        Task SendMail(Email emailData);
    }
}