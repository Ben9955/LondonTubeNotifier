using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.ServiceContracts;
using RazorLight;

namespace LondonTubeNotifier.Infrastructure.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IRazorLightEngine _engine;

        public EmailTemplateService(IRazorLightEngine engine)
        {
            _engine = engine;
        }

        public async Task<string> RenderAsync<T>(string templateName, T model)
        {
            return await _engine.CompileRenderAsync(templateName, model);
        }

        public string GeneratePlainText(EmailContentDto content)
        {
            return $@"London Tube Status Update
Hello {content.RecipientName},

The status of the {content.LineName} line has changed to:
{content.NewStatus}

For more details, please visit our website.";
        }
    }
}