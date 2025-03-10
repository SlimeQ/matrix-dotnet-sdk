using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Matrix.Sdk.Core.Infrastructure.Extensions
{
    internal static class HttpClientExtensions
    {
        private static JsonSerializerSettings GetJsonSettings()
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            var settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,

                //MatrixClientService.CreateRoomAsync not working with null in Json
                NullValueHandling = NullValueHandling.Ignore
            };
            settings.Converters.Add(new StringEnumConverter());

            return settings;
        }

        private static async Task HandleApiException(HttpResponseMessage response)
        {
            string result = await response.Content.ReadAsStringAsync();
            
            // Unhandled exception. Matrix API error. Status: TooManyRequests, json: {"errcode":"M_LIMIT_EXCEEDED","error":"Too Many Requests","retry_after_ms":133243}
            
            throw new ApiException(response.RequestMessage.RequestUri,
                    null, null, response.StatusCode);
        }
        
        public static async Task PostAsync(this HttpClient httpClient,
            string requestUri, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await httpClient.PostAsync(requestUri, null, cancellationToken);
            
            Console.WriteLine($"[POST] {requestUri} => {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                // HandleApiException(response);
                throw new ApiException(response.RequestMessage.RequestUri,
                    null, null, response.StatusCode);
            }
        }

        // Todo: Refactor
        // See: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-5-0#httpclient-and-httpcontent-extension-methods
        public static async Task<TResponse> PostAsJsonAsync<TResponse>(this HttpClient httpClient,
            string requestUri, object? model, CancellationToken cancellationToken)
        {
            
            JsonSerializerSettings settings = GetJsonSettings();

            string json = JsonConvert.SerializeObject(model, settings);
            var content = new StringContent(json, Encoding.Default, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync(requestUri, content, cancellationToken);//.ConfigureAwait(false);
            Console.WriteLine($"[POST] {requestUri} => {response.StatusCode}");
            string result = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(response.RequestMessage.RequestUri,
                    json, result, response.StatusCode);
            }

            return JsonConvert.DeserializeObject<TResponse>(result, settings)!;
        }

        // Todo: Refactor
        // See: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-5-0#httpclient-and-httpcontent-extension-methods
        public static async Task<TResponse> PutAsJsonAsync<TResponse>(this HttpClient httpClient,
            string requestUri, object model, CancellationToken cancellationToken)
        {
            JsonSerializerSettings settings = GetJsonSettings();

            string json = JsonConvert.SerializeObject(model, settings);
            var content = new StringContent(json, Encoding.Default, "application/json");

            HttpResponseMessage response = await httpClient.PutAsync(requestUri, content, cancellationToken);
            Console.WriteLine($"[PUT] {requestUri} => {response.StatusCode}");
            
            string result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(response.RequestMessage.RequestUri,
                    json, result, response.StatusCode);
            }
            
            return JsonConvert.DeserializeObject<TResponse>(result, settings)!;
        }

        public static async Task<TResponse> GetAsJsonAsync<TResponse>(this HttpClient httpClient,
            string requestUri, CancellationToken cancellationToken)
        {
            var json = await httpClient.GetAsStringAsync(requestUri, cancellationToken);
            return JsonConvert.DeserializeObject<TResponse>(json, GetJsonSettings())!;
        }
        
        public static async Task<string> GetAsStringAsync(this HttpClient httpClient,
            string requestUri, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await httpClient.GetAsync(requestUri, cancellationToken);//.ConfigureAwait(false);
            Console.WriteLine($"[GET] {requestUri} => {response.StatusCode}");
            
            string result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new ApiException(response.RequestMessage.RequestUri,
                    null, result, response.StatusCode);

            return result;
        }
        
        public static async Task<byte[]> GetAsBytesAsync(this HttpClient httpClient,
            string requestUri, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await httpClient.GetAsync(requestUri, cancellationToken);
            Console.WriteLine($"[GET] {requestUri} => {response.StatusCode}");
            
            byte[] result = await response.Content.ReadAsByteArrayAsync();

            if (!response.IsSuccessStatusCode)
                throw new ApiException(response.RequestMessage.RequestUri,
            null, $"{result.Length} bytes", response.StatusCode);

            return result;
        }

        public static void AddBearerToken(this HttpClient httpClient, string bearer) =>
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", bearer);
    }
}