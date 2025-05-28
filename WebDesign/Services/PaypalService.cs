using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Final.Services
{
    public class PayPalService
    {
        private readonly string _clientId = "Afq1t7KkXuJkTWmDl-4Fz1EK_uPMh24hqj8jivA-w2QXG4jmBPlwT7xSnmwIeoCbePxjVwzgtoL8lW8E";
        private readonly string _secret = "EPn92-wFlmpCkxognpCF9-FyWqemSeFKvaOlQx2UHE6KWlxY7Dn7jdjWAcVci93cxVLW3WRiTj79tCMQ";
        private readonly string _baseUrl = "https://api-m.sandbox.paypal.com";

        private readonly HttpClient _httpClient;

        public PayPalService()
        {
            _httpClient = new HttpClient();
        }

        // Get access token
        public async Task<string> GetAccessTokenAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/v1/oauth2/token");
            var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_clientId}:{_secret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
            request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("access_token").GetString();
        }

        // Create order
        public async Task<string> CreateOrderAsync(decimal amount, string currency)
        {
            string accessToken = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var orderPayload = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        amount = new
                        {
                            currency_code = currency,
                            value = amount.ToString("F2")
                        }
                    }
                },
                application_context = new
                {
                    return_url = "https://example.com/success",
                    cancel_url = "https://example.com/cancel"
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(orderPayload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/v2/checkout/orders", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            // Get the approval URL
            foreach (var link in doc.RootElement.GetProperty("links").EnumerateArray())
            {
                if (link.GetProperty("rel").GetString() == "approve")
                {
                    return link.GetProperty("href").GetString();
                }
            }

            throw new Exception("Approval URL not found in PayPal response.");
        }

        // Capture order
        public async Task<string> CaptureOrderAsync(string orderId)
        {
            string accessToken = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.PostAsync($"{_baseUrl}/v2/checkout/orders/{orderId}/capture", null);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return json; // you can parse this if needed
        }
    }
}
