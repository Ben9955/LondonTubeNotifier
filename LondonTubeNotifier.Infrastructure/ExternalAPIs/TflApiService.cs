using System.Net.Http.Json;
using System.Text.Json;
using LondonTubeNotifier.Core.Configuration;
using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Infrastructure.Dtos;
using LondonTubeNotifier.Core.Exceptions;
using LondonTubeNotifier.Core.ServiceContracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LondonTubeNotifier.Infrastructure.ExternalAPIs
{

    public class TflApiService : ITflApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TflApiService> _logger;
        private readonly string _mode;
        private readonly string _apiKey;
        public TflApiService(HttpClient client, IOptions<TflSettings> options, ILogger<TflApiService> logger)
        {
            _httpClient = client;
            _logger = logger;
            _mode = options.Value.Modes;
            _apiKey = options.Value.ApiKey;
        }

        public async Task<Dictionary<string, HashSet<LineStatus>>> GetLinesStatusAsync(CancellationToken cancellationToken)
        {
            try
            {
                var uri = $"Line/Mode/{_mode}/Status?app_key={_apiKey}";

                var response = await _httpClient.GetAsync(uri, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("TfL API request failed with status code {StatusCode}. Content: {Content}", response.StatusCode, content);
                    throw new TflApiException($"TfL API request failed with status code {response.StatusCode}.");
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var tflLineDtos = await response.Content.ReadFromJsonAsync<HashSet<TflLineDto>>(options, cancellationToken);

                if (tflLineDtos == null || !tflLineDtos.Any())
                {
                    _logger.LogWarning("TfL API response was empty or could not be deserialized.");
                    return new Dictionary<string, HashSet<LineStatus>>();
                }

                return tflLineDtos
                    .Where(line => line.LineId != null && line.LineStatuses != null)
                    .ToDictionary(
                    line => line.LineId,
                    line => line.LineStatuses.Select(ls => new LineStatus
                    {
                        LineId = line.LineId,
                        StatusSeverity = ls.StatusSeverity,
                        StatusDescription = ls.StatusSeverityDescription,
                        Reason = ls.Reason,
                    }).ToHashSet()
                    );
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize TfL API response");
                throw new TflApiException("Failed to deserialize TfL API response", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "TFL API request failed.");
                throw new TflApiException("TfL API request failed", ex);
            }
        }
    }
}