namespace Matrix.Sdk.Core.Domain
{
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.Dto.Sync;
    using Room;
    using RoomEvent;

    public record SyncBatch
    {
        private SyncBatch(string nextBatch, List<MatrixRoom> matrixRooms,
            List<BaseRoomEvent> matrixRoomEvents)
        {
            NextBatch = nextBatch;
            MatrixRooms = matrixRooms;
            MatrixRoomEvents = matrixRoomEvents;
        }

        public string NextBatch { get; }
        public List<MatrixRoom> MatrixRooms { get; }
        public List<BaseRoomEvent> MatrixRoomEvents { get; }

        internal static class Factory
        {
            public static SyncBatch CreateFromSync(string nextBatch, Rooms rooms)
            {
                List<MatrixRoom> matrixRooms = GetMatrixRoomsFromSync(rooms);
                List<BaseRoomEvent> matrixRoomEvents = GetMatrixEventsFromSync(rooms);

                return new SyncBatch(nextBatch, matrixRooms, matrixRoomEvents);
            }

            private static List<MatrixRoom> GetMatrixRoomsFromSync(Rooms rooms)
            {
                if (rooms == null)
                    return new List<MatrixRoom>();
                
                var joinedMatrixRooms = rooms.Join == null ? new List<MatrixRoom>() : rooms.Join.Select(pair => MatrixRoom.Create(pair.Key, pair.Value, MatrixRoomStatus.Joined))
                    .ToList();
                var invitedMatrixRooms = rooms.Invite == null ? new List<MatrixRoom>() : rooms.Invite
                    .Select(pair => MatrixRoom.CreateInvite(pair.Key, pair.Value)).ToList();
                var leftMatrixRooms = rooms.Leave == null ? new List<MatrixRoom>() : rooms.Leave.Select(pair => MatrixRoom.Create(pair.Key, pair.Value, MatrixRoomStatus.Left))
                    .ToList();

                return joinedMatrixRooms.Concat(invitedMatrixRooms).Concat(leftMatrixRooms).ToList();
            }

            private static List<BaseRoomEvent> GetMatrixEventsFromSync(Rooms rooms)
            {
                if (rooms == null)
                    return new List<BaseRoomEvent>();
                    
                var joinedMatrixRoomEvents = rooms.Join == null ? new List<BaseRoomEvent>() : rooms.Join
                    .SelectMany(pair => BaseRoomEvent.Create(pair.Key, pair.Value)).ToList();
                var invitedMatrixRoomEvents = rooms.Invite == null ? new List<BaseRoomEvent>() : rooms.Invite
                    .SelectMany(pair => BaseRoomEvent.CreateFromInvited(pair.Key, pair.Value)).ToList();
                var leftMatrixRoomEvents = rooms.Leave == null ? new List<BaseRoomEvent>() : rooms.Leave 
                    .SelectMany(pair => BaseRoomEvent.Create(pair.Key, pair.Value)).ToList();

                return joinedMatrixRoomEvents.Concat(invitedMatrixRoomEvents).Concat(leftMatrixRoomEvents).ToList();
            }
        }
    }
}