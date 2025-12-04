using Domain.Errors;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace SafeVaultApi.Tests.Infrastructure
{
    internal class SafeVaultApiClient
    {
        private readonly HttpClient _client;

        public SafeVaultApiClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<T> GetAsync<T>(string url)
        {
            var response = await _client.GetAsync(url);
            return await ResolveResponse<T>(response);
        }

        public async Task<T> PostAsync<T>(string url, object body)
        {
            var content = Serialize(body);
            var response = await _client.PostAsync(url, content);
            return await ResolveResponse<T>(response);
        }

        public async Task<HttpResponseMessage> PostRawAsync(string url, object? body = null)
        {
            string json = body != null
                ? System.Text.Json.JsonSerializer.Serialize(body)
                : string.Empty;

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _client.PostAsync(url, content);
        }

        // 🔹 PUT with response model (success only)
        public async Task<T> PutAsync<T>(string url, object body)
        {
            var content = Serialize(body);
            var response = await _client.PutAsync(url, content);
            return await ResolveResponse<T>(response);
        }

        // 🔹 PUT raw (used to assert HttpStatusCode in tests)
        public async Task<HttpResponseMessage> PutRawAsync(string url, object? body = null)
        {
            string json = body != null
                ? JsonConvert.SerializeObject(body)
                : string.Empty;

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _client.PutAsync(url, content);
        }

        // 🔹 DELETE with no response model
        public async Task<HttpResponseMessage> DeleteRawAsync(string url)
        {
            return await _client.DeleteAsync(url);
        }

        private async Task<T> ResolveResponse<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<T>(json);

                if (data == null)
                    throw new Exception("API returned success but response body was null.");

                return data;
            }

            var apiError = TryParseError(json)
                           ?? new ApiError
                           {
                               StatusCode = (int)response.StatusCode,
                               ErrorCode = "UNKNOWN",
                               Message = "An unknown error occurred."
                           };

            throw new Exception($"API Error ({apiError.ErrorCode}): {apiError.Message}");
        }

        private static StringContent Serialize(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private static ApiError? TryParseError(string json)
        {
            try { return JsonConvert.DeserializeObject<ApiError>(json); }
            catch { return null; }
        }
    }
}
