using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.ServiceContracts;
using RazorLight;
using System.Text;

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

        public string GeneratePlainText(NotificationDto dto)
        {
            var content = new StringBuilder();

            content.AppendLine($"London Tube Status Update");
            content.AppendLine($"Hello {dto.RecipientName},");
            content.AppendLine();
            content.AppendLine($"The status of the {dto.LineUpdates.LineName} line has changed:");

            var reasons = dto.LineUpdates.StatusDescriptions
                .Where(s => !string.IsNullOrEmpty(s.Reason))
                .Select(s => s.Reason);

            if (reasons.Any())
            {
                content.AppendLine($"- {string.Join(Environment.NewLine + "- ", reasons)}");
            }
            else
            {
                content.AppendLine($"- New Status: {string.Join(", ", dto.LineUpdates.StatusDescriptions.Select(s => s.StatusDescription))}");
            }
            content.AppendLine();

            content.AppendLine("For more details, please visit our website.");

            return content.ToString();
        }
    }
}