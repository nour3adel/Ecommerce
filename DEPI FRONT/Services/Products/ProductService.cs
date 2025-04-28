using Ecommerce.Frontend.Models.Common;
using Ecommerce.Frontend.Models.Product;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Ecommerce.Frontend.Services.Products
{
    public class ProductService : ResponseHandler, IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductService> _logger;
        private readonly ApiSettings _apiSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductService(HttpClient httpClient,
                              ILogger<ProductService> logger,
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

        public async Task<Response<List<ProductDTO>>> GetProductsAsync()
        {

            var response = await _httpClient.GetAsync(ApiConstants.BaseUrl + ApiConstants.ProductsEndpoint);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = JsonSerializer.Deserialize<Response<List<ProductDTO>>>(json, options);
                return apiResponse;
            }

            return BadRequest<List<ProductDTO>>("Failed"); // Return an empty list on failure
        }

        public async Task<Response<ProductDTO>> GetProductByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync(ApiConstants.BaseUrl + $"{ApiConstants.ProductsEndpoint}/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = JsonSerializer.Deserialize<Response<ProductDTO>>(json, options);
                return apiResponse;
            }

            return BadRequest<ProductDTO>("Failed");
        }


        public async Task<Response<string>> AddProductAsync(AddProductDto product)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                // Add simple fields
                content.Add(new StringContent(product.Name ?? ""), "Name");
                content.Add(new StringContent(product.Description ?? ""), "Description");
                content.Add(new StringContent(product.Price.ToString(CultureInfo.InvariantCulture)), "Price");
                content.Add(new StringContent(product.StockAmount.ToString()), "StockAmount");
                content.Add(new StringContent(product.CategoryId.ToString()), "CategoryId");
                content.Add(new StringContent(product.BrandId.ToString()), "BrandId");
                content.Add(new StringContent(product.DiscountPercentage.ToString(CultureInfo.InvariantCulture)), "DiscountPercentage");

                // Add images (array of files)
                if (product.ImageFiles != null && product.ImageFiles.Any())
                {
                    foreach (var file in product.ImageFiles)
                    {
                        if (file.Length > 0)
                        {
                            var streamContent = new StreamContent(file.OpenReadStream());
                            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
                            content.Add(streamContent, "ImageFiles", file.FileName);
                        }
                    }
                }

                // Add colors (array of strings)
                if (product.ColorNames != null && product.ColorNames.Any())
                {
                    foreach (var color in product.ColorNames)
                    {
                        content.Add(new StringContent(color), "ColorNames");
                    }
                }

                // Send POST request
                var response = await _httpClient.PostAsync($"{ApiConstants.BaseUrl}{ApiConstants.ProductsEndpoint}/AddProduct", content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<Response<string>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (apiResponse?.Data == null)
                    {
                        _logger.LogError("API returned a null or empty response.");
                        return BadRequest<string>("Product creation failed: No data returned.");
                    }

                    return apiResponse;
                }
                else
                {
                    _logger.LogError($"Failed API call. Status: {response.StatusCode}, Message: {responseContent}");
                    return BadRequest<string>($"API error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception during AddProductAsync");
                return BadRequest<string>("An unexpected error occurred.");
            }
        }

    }
}
