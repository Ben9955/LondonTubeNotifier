using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Core.ServiceContracts
{
    public interface IEmailTemplateService
    {
        Task<string> RenderAsync<T>(string templateName, T model);
        string GeneratePlainText(EmailContentDto content);

    }
}