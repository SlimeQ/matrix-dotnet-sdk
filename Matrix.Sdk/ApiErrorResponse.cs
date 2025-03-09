using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Matrix.Sdk
{
    public class ApiErrorResponse
    {
        [JsonProperty("retry_after_ms")]
        public int retryAfterMs;
    }
}

