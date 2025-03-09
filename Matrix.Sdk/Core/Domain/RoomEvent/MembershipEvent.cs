using System;

namespace Matrix.Sdk.Core.Domain.RoomEvent
{
    using Infrastructure.Dto.Sync.Event;
    using Infrastructure.Dto.Sync.Event.Room;
    using Infrastructure.Dto.Sync.Event.Room.State;

    public record MembershipEvent(string EventId, string RoomId, string SenderUserId, UserMembershipState MembershipState, string MembershipChangedUserId, string Reason, DateTimeOffset Timestamp) : BaseRoomEvent(EventId, RoomId, SenderUserId, Timestamp)
    {
        public static class Factory
        {
            public static bool TryCreateFrom(RoomEventResponse roomEvent, string roomId, out MembershipEvent membershipEvent)
            {
                RoomMemberContent content = roomEvent.Content.ToObject<RoomMemberContent>();
                if (content != null && roomEvent.EventType == EventType.Member)
                {
                    membershipEvent = new MembershipEvent(roomEvent.EventId, roomId, roomEvent.SenderUserId, content.UserMembershipState, roomEvent.StateKey, content.Reason, roomEvent.Timestamp);
                    return true;
                }

                membershipEvent = null;
                return false;
            }

            public static bool TryCreateFromStrippedState(RoomStrippedState roomStrippedState, string roomId, out MembershipEvent membershipEvent)
            {
                RoomMemberContent content = roomStrippedState.Content.ToObject<RoomMemberContent>();
                if (content != null && roomStrippedState.EventType == EventType.Member)
                {
                    membershipEvent = new MembershipEvent(string.Empty, roomId, roomStrippedState.SenderUserId, content.UserMembershipState, roomStrippedState.StateKey, content.Reason, DateTimeOffset.MinValue);
                    return true;
                }

                membershipEvent = null;
                return false;
            }
        }
    }
}