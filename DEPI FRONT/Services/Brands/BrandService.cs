using Ecommerce.Frontend.Models.Brand;
using Ecommerce.Frontend.Models.Common;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Ecommerce.Frontend.Services.Brands
{
    public class BrandService : ResponseHandler, IBrandService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BrandService> _logger;
        private readonly ApiSettings _apiSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BrandService(
            HttpClient httpClient,
            ILogger<BrandService> logger,
            IOptions<ApiSettings> apiSettings,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiSettings = apiSettings.Value;
            _httpClient.BaseAddress = new Uri(_apiSettings.BaseUrl);
            _httpContextAccessor = httpContextAccessor;
            SetAuthorizationHeader();
        }

        private void SetAuthorizationHeader()
        {
            // Retrieve the access token from the session
            var accessToken = _httpContextAccessor.HttpContext?.Session.GetString("AccessToken");
            Console.WriteLine($"Access Token: {accessToken}");

            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            else
            {
                _logger.LogWarning("Access token not found in session. Authorization header not set.");
            }
        }

        private async Task<T> HandleApiResponseAsync<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<T>(json, options);
            }

            _logger.LogError($"API returned status code: {response.StatusCode}");
            return default(T);
        }

        public async Task<Response<List<BrandDto>>> GetBrandsAsync()
        {
            var endpoint = $"{ApiConstants.BaseUrl}{ApiConstants.BrandsEndpoint}/Brands";
            var response = await _httpClient.GetAsync(endpoint);

            return await HandleApiResponseAsync<Response<List<BrandDto>>>(response)
                   ?? BadRequest<List<BrandDto>>("Failed to fetch brands.");
        }

        public async Task<Response<BrandDto>> GetBrandByIdAsync(int id)
        {
            var endpoint = $"{ApiConstants.BaseUrl}{ApiConstants.BrandsEndpoint}/Brands/{id}";
            var response = await _httpClient.GetAsync(endpoint);

            return await HandleApiResponseAsync<Response<BrandDto>>(response)
                   ?? BadRequest<BrandDto>($"Failed to fetch brand with ID: {id}.");
        }

        public async Task<Response<string>> AddBrandAsync(AddBrandDto brand)
        {
            try
            {
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(brand),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var endpoint = $"{ApiConstants.BaseUrl}{ApiConstants.BrandsEndpoint}/AddBrand";
                var response = await _httpClient.PostAsync(endpoint, jsonContent);

                return await HandleApiResponseAsync<Response<string>>(response)
                       ?? BadRequest<string>("Failed to add brand.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding a brand.");
                return new Response<string>
                {
                    Succeeded = false,
                    Message = "An unexpected error occurred while communicating with the server.",
                    Errors = new List<string> { "Failed to reach the server. Please try again later." }
                };
            }
        }
    }
}