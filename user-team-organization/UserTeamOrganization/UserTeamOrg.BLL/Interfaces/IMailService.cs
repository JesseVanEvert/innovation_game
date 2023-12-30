using UserTeamOrg.Model.DTO;

namespace UserTeamOrg.BLL.Interfaces
{
    public interface IMailService
    {
        Task<bool> SendEmail(EmailDTO emailDTO);
    }
}
