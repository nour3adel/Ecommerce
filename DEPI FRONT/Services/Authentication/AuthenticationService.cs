using Ecommerce.Frontend.Models.Authentication;
using Ecommerce.Frontend.Models.Common;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Ecommerce.Frontend.Services.Authentication
{
    public class AuthenticationService : ResponseHandler, IAuthenticationService
    {

        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthenticationService> _logger;

        private readonly ApiSettings _apiSettings;

        public AuthenticationService(HttpClient httpClient, ILogger<AuthenticationService> logger, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _logger = logger;

            if (string.IsNullOrWhiteSpace(apiSettings?.Value?.BaseUrl))
            {
                throw new ArgumentException("BaseUrl cannot be null or empty.", nameof(apiSettings));
            }
            _apiSettings = apiSettings.Value;

            _httpClient.BaseAddress = new Uri(_apiSettings.BaseUrl);

        }



        public async Task<Response<LoginResponse>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(loginDto),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(ApiConstants.BaseUrl + ApiConstants.AuthEndpoint + "/login", jsonContent);
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = JsonSerializer.Deserialize<Response<LoginResponse>>(json, options);

                // Ensure apiResponse is not null before returning
                if (apiResponse == null)
                {
                    return new Response<LoginResponse>
                    {
                        Succeeded = false,
                        Message = "Failed to deserialize the server response.",
                        Errors = new List<string> { "The server response could not be processed." }
                    };
                }

                return apiResponse;
            }
            catch (Exception ex)
            {
                // If there is an exception (e.g., network failure), log the error and return a custom error response
                _logger.LogError(ex, "An unexpected error occurred while attempting to log in.");
                return new Response<LoginResponse>
                {
                    Succeeded = false,
                    Message = "An unexpected error occurred while communicating with the server.",
                    Errors = new List<string> { "Failed to reach the authentication server. Please try again later." }
                };
            }
        }
        public async Task<Response<string>> SendResetPasswordAsync(string email)
        {
            try
            {
                var encodedEmail = Uri.EscapeDataString(email);
                var endpoint = $"{ApiConstants.BaseUrl}{ApiConstants.AuthEndpoint}/SendResetPasswordEmail?email={encodedEmail}";

                // Use empty content instead of null
                var response = await _httpClient.PostAsync(endpoint, new StringContent(""));

                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = JsonSerializer.Deserialize<Response<string>>(json, options);

                if (apiResponse == null)
                {
                    _logger.LogWarning("Deserialization failed for reset password response. Raw JSON: {Json}", json);
                    return new Response<string>
                    {
                        Succeeded = false,
                        Message = "Failed to process server response.",
                        Errors = new List<string> { "The server response could not be processed." }
                    };
                }

                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while attempting to reset password.");
                return new Response<string>
                {
                    Succeeded = false,
                    Message = "An unexpected error occurred while communicating with the server.",
                    Errors = new List<string> { "Failed to reach the authentication server. Please try again later." }
                };
            }
        }
        public async Task<Response<string>> ConfirmResetPassword(string code, string email)
        {
            try
            {
                var encodedCode = Uri.EscapeDataString(code);
                var encodedEmail = Uri.EscapeDataString(email);
                var endpoint = $"{ApiConstants.BaseUrl}{ApiConstants.AuthEndpoint}/ConfirmResetPassword?code={encodedCode}&email={encodedEmail}";

                // Use empty content instead of null
                var response = await _httpClient.PostAsync(endpoint, new StringContent(""));

                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = JsonSerializer.Deserialize<Response<string>>(json, options);

                if (apiResponse == null)
                {
                    _logger.LogWarning("Deserialization failed for reset password response. Raw JSON: {Json}", json);
                    return new Response<string>
                    {
                        Succeeded = false,
                        Message = "Failed to process server response.",
                        Errors = new List<string> { "The server response could not be processed." }
                    };
                }

                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while attempting to reset password.");
                return new Response<string>
                {
                    Succeeded = false,
                    Message = "An unexpected error occurred while communicating with the server.",
                    Errors = new List<string> { "Failed to reach the authentication server. Please try again later." }
                };
            }
        }


        public async Task<Response<string>> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var jsonContent = new StringContent(
                JsonSerializer.Serialize(registerDto),
                System.Text.Encoding.UTF8,
                "application/json"
            );
                var response = await _httpClient.PostAsync(ApiConstants.BaseUrl + ApiConstants.AuthEndpoint + "/register", jsonContent);



                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = JsonSerializer.Deserialize<Response<string>>(json, options);
                // Ensure apiResponse is not null before returning
                if (apiResponse == null)
                {
                    return new Response<string>
                    {
                        Succeeded = false,
                        Message = "Failed to deserialize the server response.",
                        Errors = new List<string> { "The server response could not be processed." }
                    };
                }
                return apiResponse;

            }
            catch (Exception ex)
            {
                // If there is an exception (e.g., network failure), log the error and return a custom error response
                _logger.LogError(ex, "An unexpected error occurred while attempting to log in.");
                return new Response<string>
                {
                    Succeeded = false,
                    Message = "An unexpected error occurred while communicating with the server.",
                    Errors = new List<string> { "Failed to reach the authentication server. Please try again later." }
                };
            }


        }

        public async Task<Response<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var jsonContent = new StringContent(
                JsonSerializer.Serialize(resetPasswordDto),
                Encoding.UTF8,
                "application/json"
            );

                var response = await _httpClient.PutAsync(
                    $"{ApiConstants.BaseUrl}{ApiConstants.AuthEndpoint}/ResetPassword",
                    jsonContent
                );


                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = JsonSerializer.Deserialize<Response<string>>(json, options);
                // Ensure apiResponse is not null before returning
                if (apiResponse == null)
                {
                    return new Response<string>
                    {
                        Succeeded = false,
                        Message = "Failed to deserialize the server response.",
                        Errors = new List<string> { "The server response could not be processed." }
                    };
                }
                return apiResponse;

            }
            catch (Exception ex)
            {
                // If there is an exception (e.g., network failure), log the error and return a custom error response
                _logger.LogError(ex, "An unexpected error occurred while attempting to log in.");
                return new Response<string>
                {
                    Succeeded = false,
                    Message = "An unexpected error occurred while communicating with the server.",
                    Errors = new List<string> { "Failed to reach the authentication server. Please try again later." }
                };
            }
        }
    }
}