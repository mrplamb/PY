using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Final.Services
{
    public class NordigenService
    {
        private readonly string _clientId;
        private readonly string _secret;
        private readonly string _accessToken;
        private readonly string _baseUrl = "https://bankaccountdata.gocardless.com/api/v2";
        private readonly HttpClient _httpClient;

        // Constructor for getting an access token
        public NordigenService(string clientId, string secret)
        {
            _clientId = "6b00fa9b-24fe-4904-b953-3516e6de8a0a";
            _secret = "3500bd016ac5e0e776fb272cd4ae395943f1a343979ac8a20665a4183083800aaa7d636281855d6a800e8dba1eb5c06b41fbfab6b8a3276831e2a986f23edf14";
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl)
            };
        }

        // Constructor for making authenticated calls (with an access token)
        public NordigenService(string accessToken)
        {
            _accessToken = "sandbox_b98yS_QfoQtdN0R0Uye3edkv7pscidSKTxHIl0fh";
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl)
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }

        // Fetch the access token
        public async Task<string> GetAccessTokenAsync()
        {
            var requestBody = new
            {
                secret_id = _clientId,
                secret_key = _secret
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/token/new/", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var accessToken = doc.RootElement.GetProperty("access").GetString();
            return accessToken!;
        }

        // Fetch linked accounts for a given requisition
        public async Task<List<string>> GetLinkedAccountsAsync(string requisitionId)
        {
            var response = await _httpClient.GetAsync($"/requisitions/{requisitionId}/accounts/");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var accounts = new List<string>();

            if (doc.RootElement.TryGetProperty("accounts", out var accountsElement))
            {
                foreach (var accountId in accountsElement.EnumerateArray())
                {
                    accounts.Add(accountId.GetString());
                }
            }

            return accounts;
        }

        // Create a requisition and get the requisition ID + link
        public async Task<(string RequisitionId, string Link)> CreateRequisitionAsync(string redirectUrl, string institutionId, string reference)
        {
            string accessToken = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var requestBody = new
            {
                redirect = redirectUrl,
                institution_id = institutionId,
                reference = reference
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/requisitions/", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var requisitionId = doc.RootElement.GetProperty("id").GetString()!;
            var link = doc.RootElement.GetProperty("link").GetString()!;

            return (requisitionId, link);
        }


        public async Task<string> CreateRequisitionAsync(string redirectUri, string institutionId)
        {
            var requestBody = new
            {
                redirect = redirectUri,
                institution_id = institutionId
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/requisitions/", content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonResponse);
            var link = doc.RootElement.GetProperty("link").GetString();

            return link;
        }



        // Fetch transactions for a given accountId
        public async Task<string> FetchTransactionsAsync(string accountId)
        {
            var response = await _httpClient.GetAsync($"/accounts/{accountId}/transactions/");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        // Optional: Fetch account details
        public async Task<string> GetAccountDetailsAsync(string accountId)
        {
            var response = await _httpClient.GetAsync($"/accounts/{accountId}/details/");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
