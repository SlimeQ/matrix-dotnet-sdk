﻿using System.Diagnostics;
using System.Web;
using Matrix.Sdk.Core.Domain.RoomEvent;
using Matrix.Sdk.Core.Infrastructure.Dto.Sync;
using Matrix.Sdk.Core.Infrastructure.Dto.Sync.Event.Room;
using Newtonsoft.Json;
using Scooby;

namespace Matrix.Sdk.Core.Infrastructure.Services
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto.Room.Create;
    using Dto.Room.Join;
    using Dto.Room.Joined;
    using Extensions;

    public class RoomService : BaseApiService
    {
        public RoomService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task<CreateRoomResponse> CreateRoomAsync(string accessToken, string[]? members,
            CancellationToken cancellationToken)
        {
            var model = new CreateRoomRequest
            (
                Invite: members,
                Preset: Preset.TrustedPrivateChat,
                IsDirect: true
            );

            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/createRoom";

            return await httpClient.PostAsJsonAsync<CreateRoomResponse>(path, model, cancellationToken);
        }

        public async Task<JoinRoomResponse> JoinRoomAsync(string accessToken, string roomId,
            CancellationToken cancellationToken)
        {
            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/rooms/{roomId}/join";

            return await httpClient.PostAsJsonAsync<JoinRoomResponse>(path, null, cancellationToken);
        }


        public async Task<JoinedRoomsResponse> GetJoinedRoomsAsync(string accessToken,
            CancellationToken cancellationToken)
        {
            HttpClient httpClient = CreateHttpClient(accessToken);
            
            var path = $"{ResourcePath}/joined_rooms";
            UI.WriteLine(path);
            var json = await httpClient.GetStringAsync(path, cancellationToken);
            UI.WriteLine(json);
            var obj = JsonConvert.DeserializeObject<JoinedRoomsResponse>(json);
            return obj;
        }

        public async Task LeaveRoomAsync(string accessToken, string roomId,
            CancellationToken cancellationToken)
        {
            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/rooms/{roomId}/leave";

            await httpClient.PostAsync(path, cancellationToken);
        }

        public record RoomNameResponse
        {
            public string name;
        }
        public async Task<string> GetRoomNameAsync(string accessToken, string roomId, CancellationToken cancellationToken)
        {
            var path = $"{ResourcePath}/rooms/{roomId}/state/m.room.name/";
            HttpClient httpClient = CreateHttpClient(accessToken);
            var json = await httpClient.GetStringAsync(path, cancellationToken);
            var payload = JsonConvert.DeserializeObject<RoomNameResponse>(json);
            return payload.name;
        }
    }
}