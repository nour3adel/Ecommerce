using Ecommerce.Frontend.Models.Category;
using Ecommerce.Frontend.Models.Common;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Ecommerce.Frontend.Services.Categories
{
    public class CategoryService : ResponseHandler, ICategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CategoryService> _logger;
        private readonly ApiSettings _apiSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CategoryService(HttpClient httpClient,
                              ILogger<CategoryService> logger,
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

        public async Task<Response<List<CategoryDto>>> GetCategoriessAsync()
        {
            var response = await _httpClient.GetAsync(ApiConstants.BaseUrl + ApiConstants.CategoriesEndpoint);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = JsonSerializer.Deserialize<Response<List<CategoryDto>>>(json, options);
                return apiResponse;
            }

            return BadRequest<List<CategoryDto>>("Failed"); // Return an empty list on failure
        }



        public async Task<Response<CategoryDto>> GetCategoryByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync(ApiConstants.BaseUrl + $"{ApiConstants.CategoriesEndpoint}/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = JsonSerializer.Deserialize<Response<CategoryDto>>(json, options);
                return apiResponse;
            }

            return BadRequest<CategoryDto>("Failed");
        }

        public async Task<Response<string>> AddCategoryAsync(AddCategoryDto category)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                if (category.ImageFile != null)
                {
                    content.Add(new StreamContent(category.ImageFile.OpenReadStream()), "ImageFile", category.ImageFile.FileName);
                }

                var encodedName = Uri.EscapeDataString(category.Name);

                var endpoint = $"{ApiConstants.BaseUrl}{ApiConstants.CategoriesEndpoint}/AddCategory?Name={encodedName}";


                var response = await _httpClient.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var apiResponse = JsonSerializer.Deserialize<Response<string>>(json, options);

                    if (apiResponse == null || string.IsNullOrEmpty(apiResponse.Data))
                    {
                        _logger.LogError("Invalid or empty response received from API.");
                        return BadRequest<string>("Failed to add product. Invalid response from API.");
                    }

                    return apiResponse;
                }
                else
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"API returned status code: {response.StatusCode}. Error: {errorJson}");
                    return BadRequest<string>($"Failed to add product. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the product.");
                return BadRequest<string>("An unexpected error occurred while adding the product.");
            }
        }
    }
}

