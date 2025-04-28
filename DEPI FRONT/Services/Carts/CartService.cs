using Ecommerce.Frontend.Models.Carts;
using Ecommerce.Frontend.Models.Common;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Ecommerce.Frontend.Services.Carts
{
    public class CartService : ResponseHandler, ICartService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CartService> _logger;
        private readonly ApiSettings _apiSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(
            HttpClient httpClient,
            ILogger<CartService> logger,
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

        public async Task<Response<List<CartDto>>> GetCartsAsync()
        {

            var endpoint = $"{ApiConstants.BaseUrl}{ApiConstants.CartEndpoint}";
            var response = await _httpClient.GetAsync(endpoint);

            return await HandleApiResponseAsync<Response<List<CartDto>>>(response)
                   ?? BadRequest<List<CartDto>>("Failed to fetch carts.");
        }

        public async Task<Response<CartDto>> GetCartByIdAsync(int id)
        {
            var endpoint = $"{ApiConstants.BaseUrl}{ApiConstants.CartEndpoint}/{id}";
            var response = await _httpClient.GetAsync(endpoint);

            return await HandleApiResponseAsync<Response<CartDto>>(response)
                   ?? BadRequest<CartDto>($"Failed to fetch cart with ID: {id}.");
        }

        public async Task<Response<string>> AddCartAsync()
        {
            SetAuthorizationHeader();
            try
            {
                var endpoint = $"{ApiConstants.BaseUrl}{ApiConstants.CartEndpoint}/AddCart";
                var response = await _httpClient.PostAsync(endpoint, new StringContent(""));

                return await HandleApiResponseAsync<Response<string>>(response)
                       ?? BadRequest<string>("Failed to add Cart.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding a cart.");
                return new Response<string>
                {
                    Succeeded = false,
                    Message = "An unexpected error occurred while communicating with the server.",
                    Errors = new List<string> { "Failed to reach the server. Please try again later." }
                };
            }
        }



        public async Task<Response<List<CartItemsDto>>> GetAllItemsAsync()
        {
            var endpoint = $"{ApiConstants.BaseUrl}{ApiConstants.CartItemsEndpoint}";
            var response = await _httpClient.GetAsync(endpoint);

            return await HandleApiResponseAsync<Response<List<CartItemsDto>>>(response)
                   ?? BadRequest<List<CartItemsDto>>("Failed to fetch all cart Items.");
        }



        public async Task<Response<CartItemsDto>> GetCartItemByIdAsync(int id)
        {
            var endpoint = $"{ApiConstants.BaseUrl}{ApiConstants.CartItemsEndpoint}/{id}";
            var response = await _httpClient.GetAsync(endpoint);

            return await HandleApiResponseAsync<Response<CartItemsDto>>(response)
                   ?? BadRequest<CartItemsDto>($"Failed to fetch cartItem with ID: {id}.");
        }



        public async Task<Response<string>> AddCartItemAsync(AddCartItemDto addCartItemDto)
        {
            try
            {
                SetAuthorizationHeader();

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(addCartItemDto),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var endpoint = $"{ApiConstants.BaseUrl}{ApiConstants.CartItemsEndpoint}/AddItem";
                var response = await _httpClient.PostAsync(endpoint, jsonContent);

                return await HandleApiResponseAsync<Response<string>>(response)
                       ?? BadRequest<string>("Failed to add cartitems.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding a cartitems.");
                return new Response<string>
                {
                    Succeeded = false,
                    Message = "An unexpected error occurred while communicating with the server.",
                    Errors = new List<string> { "Failed to reach the server. Please try again later." }
                };
            }
        }

        public async Task<Response<string>> DeleteCartAsync()
        {
            SetAuthorizationHeader();
            var endpoint = $"{ApiConstants.BaseUrl}{ApiConstants.CartEndpoint}";
            var response = await _httpClient.DeleteAsync(endpoint);

            return await HandleApiResponseAsync<Response<string>>(response)
                   ?? BadRequest<string>($"Failed to delete cart.");
        }

        public async Task<Response<string>> DeleteCartItem(int id)
        {
            var endpoint = $"{ApiConstants.BaseUrl}{ApiConstants.CartItemsEndpoint}/DeleteItem/{id}";
            var response = await _httpClient.DeleteAsync(endpoint);

            return await HandleApiResponseAsync<Response<string>>(response)
                   ?? BadRequest<string>($"Failed to delete cart Item with ID: {id}.");
        }
        public async Task<Response<string>> UpdateCartItem(AddCartItemDto addCartItemDto)
        {
            try
            {
                SetAuthorizationHeader();

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(addCartItemDto),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var endpoint = $"{ApiConstants.BaseUrl}{ApiConstants.CartItemsEndpoint}/UpdateItem";
                var response = await _httpClient.PutAsync(endpoint, jsonContent);


                return await HandleApiResponseAsync<Response<string>>(response)
                       ?? BadRequest<string>($"Failed to update cart Item with ID:");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding a cartitems.");
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