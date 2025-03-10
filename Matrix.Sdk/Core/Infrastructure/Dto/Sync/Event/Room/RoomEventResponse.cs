using System;
using Newtonsoft.Json;

namespace Matrix.Sdk.Core.Infrastructure.Dto.Sync.Event.Room
{
    public record RoomEventResponse : BaseEvent
    {
        /// <summary>
        ///     <b>Required.</b> The globally unique event identifier.
        /// </summary>
        public string EventId { get; init; }
        
        public string RoomId { get; init; }

        /// <summary>
        ///     <b>Required.</b> Contains the fully-qualified ID of the user who sent this event.
        /// </summary>
        [JsonProperty("sender")]
        public string SenderUserId { get; init; }

        /// <summary>
        ///     <b>Required.</b> Timestamp in milliseconds on originating homeserver when this event was sent.
        /// </summary>
        [JsonProperty("origin_server_ts")]
        public long OriginServerTimestamp { get; init; }

        [JsonIgnore]
        public DateTimeOffset Timestamp => DateTimeOffset.FromUnixTimeMilliseconds(OriginServerTimestamp).UtcDateTime;

        // ReSharper disable once InvalidXmlDocComment
        /// <summary>
        ///     <b>Required.</b> The ID of the room associated with this event.
        ///     Will not be present on events that arrive through /sync, despite being required everywhere else.
        /// </summary>
        // public string room_id { get; set; }
    }
}