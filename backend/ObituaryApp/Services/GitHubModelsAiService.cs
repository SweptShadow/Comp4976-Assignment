using System.Text;
using System.Text.Json;

namespace ObituaryApp.Services
{
    public interface IAiService
    {
        Task<string> EnhanceObituaryDescriptionAsync(string fullName, string? currentDescription);
    }

    public class GitHubModelsAiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GitHubModelsAiService> _logger;

        public GitHubModelsAiService(HttpClient httpClient, IConfiguration configuration, ILogger<GitHubModelsAiService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> EnhanceObituaryDescriptionAsync(string fullName, string? currentDescription)
        {
            var token = _configuration["GITHUB_TOKEN"];
            var modelName = _configuration["GITHUB_MODEL_NAME"] ?? "gpt-4o";

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new InvalidOperationException("GitHub token not configured");
            }

            var prompt = BuildPrompt(fullName, currentDescription);

            var requestBody = new
            {
                messages = new[]
                {
                    new { role = "system", content = "You are a compassionate and professional obituary writer. Write respectful, dignified obituary descriptions." },
                    new { role = "user", content = prompt }
                },
                model = modelName,
                temperature = 0.7,
                max_tokens = 300
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            try
            {
                var response = await _httpClient.PostAsync("https://models.inference.ai.azure.com/chat/completions", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GitHub Models API error: {StatusCode} - {Content}", response.StatusCode, errorContent);
                    throw new HttpRequestException($"GitHub Models API returned {response.StatusCode}");
                }

                var responseText = await response.Content.ReadAsStringAsync();
                var responseObj = JsonSerializer.Deserialize<GitHubModelsResponse>(responseText);

                return responseObj?.choices?.FirstOrDefault()?.message?.content?.Trim()
                    ?? throw new InvalidOperationException("No response from AI model");
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("GitHub Models API request timed out");
                throw new TimeoutException("AI service request timed out. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling GitHub Models API");
                throw;
            }
        }

        private static string BuildPrompt(string fullName, string? currentDescription)
        {
            if (string.IsNullOrWhiteSpace(currentDescription))
            {
                return $"Write a respectful and dignified obituary description for {fullName}. " +
                       "Create a general but heartfelt description focusing on their character and legacy. " +
                       "Keep it between 100-200 words. Do not include specific dates, places, or family details as those will be added separately.";
            }
            else
            {
                return $"Enhance and refine the following obituary description for {fullName}. " +
                       "Make it more eloquent, respectful, and professionally written while preserving the original meaning and details. " +
                       "Keep it between 100-200 words:\n\n{currentDescription}";
            }
        }
    }

    // Response models for GitHub Models API
    public class GitHubModelsResponse
    {
        public GitHubModelsChoice[]? choices { get; set; }
    }

    public class GitHubModelsChoice
    {
        public GitHubModelsMessage? message { get; set; }
    }

    public class GitHubModelsMessage
    {
        public string? content { get; set; }
    }
}