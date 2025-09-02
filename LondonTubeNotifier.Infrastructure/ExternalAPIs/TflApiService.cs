using System.Net.Http.Json;
using System.Text.Json;
using LondonTubeNotifier.Core.Configuration;
using LondonTubeNotifier.Core.DTOs.TflDtos;
using LondonTubeNotifier.Core.Exceptions;
using LondonTubeNotifier.Core.ServiceContracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LondonTubeNotifier.Infrastructure.ExternalAPIs
{

    public class TflApiService : ITflApiService
    {
        private readonly HttpClient _httpClient;
        private readonly TflSettings _tflSettings;
        private readonly ILogger<TflApiService> _logger;
        public TflApiService(HttpClient client, IOptions<TflSettings> options, ILogger<TflApiService> logger)
        {
            _httpClient = client;
            _tflSettings = options.Value;
            _logger = logger;
        }

        public async Task<List<LineStatusDto>?> GetLinesStatusAsync()
        {
            try
            {
                var uri = $"Line/Mode/{_tflSettings.Modes}/Status?app_key={_tflSettings.ApiKey}";

                var response = await _httpClient.GetAsync(uri);

                response.EnsureSuccessStatusCode();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var lines = await response.Content.ReadFromJsonAsync<List<LineStatusDto>>(options);

                return lines;
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing TFL API response.");
                throw new TflApiException("Unexpected error while calling TfL API", ex);
            }

        }
    }
}