// ReSharper disable ArgumentsStyleNamedExpression

using System;
using System.Net;
using System.Runtime.CompilerServices.Dto.User;
using System.Text;
using System.Web;
using Matrix.Sdk.Core.Domain.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Matrix.Sdk.Core.Infrastructure.Services
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto.Login;
    using Extensions;

    public class UserService : BaseApiService
    {
        private ILogger logger;
        public UserService(IHttpClientFactory httpClientFactory, ILogger logger) : base(httpClientFactory)
        {
            this.logger = logger;
        }

        public async Task<LoginResponse> LoginAsync(string user, string password, string deviceId,
            CancellationToken cancellationToken)
        {
            logger.LogInformation($"LoginAsync({user}, {password}, {deviceId})");

            var model = new LoginRequest
            (
                new Identifier
                (
                    "m.id.user",
                    user
                ),
                password,
                deviceId,
                "m.login.password"
            );

            HttpClient httpClient = CreateHttpClient();

            var path = $"{ResourcePath}/login";

            int retries = 10;
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    var response = await httpClient.PostAsJsonAsync<LoginResponse>(path, model, cancellationToken);
                    return response;
                }
                catch (ApiException e)
                {
                    logger.LogWarning(e.ToString());
                    if (e.StatusCode == (HttpStatusCode)429)
                    {
                        var err = JsonConvert.DeserializeObject<ApiErrorResponse>(e.ResponseContent);
                        logger.LogWarning($"({i+1}/{retries}) Too many requests, waiting {err.retryAfterMs}ms before retrying");
                        await Task.Delay(err.retryAfterMs + 10000);
                    }
                }
            }
            throw new Exception($"Failed to login {retries} times");
        }

        public async Task<MatrixProfile> GetProfile(string accessToken, string userId, CancellationToken cancellationToken)
        {
            HttpClient httpClient = CreateHttpClient(accessToken);
            var path = $"{ResourcePath}/profile/{HttpUtility.HtmlEncode($"@{userId}:{httpClient.BaseAddress.Host}")}";
            return await httpClient.GetAsJsonAsync<MatrixProfile>(path, cancellationToken);
        }
    }
}