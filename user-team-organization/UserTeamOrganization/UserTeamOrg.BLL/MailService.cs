using SendGrid;
using SendGrid.Helpers.Mail;
using UserTeamOrg.BLL.Interfaces;
using UserTeamOrg.Model.DTO;

namespace UserTeamOrg.BLL
{
    public class MailService : IMailService
    {
        public async Task<bool> SendEmail(EmailDTO emailDTO)
        {
            var apiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
            var client = new SendGridClient(apiKey);

            var msg = new SendGridMessage()
            {
                From = new EmailAddress("608452@student.inholland.nl", "Innovation Scan Game"),
                Subject = "Your teamId to login",
                HtmlContent = $"Here is the teamId necessary to create your user: <strong>{emailDTO.TeamId}</strong>"
            };
            msg.AddTo(new EmailAddress(emailDTO.Email, emailDTO.Email));

            var response = await client.SendEmailAsync(msg);

            return response.IsSuccessStatusCode;
        }
    }
}





