using System;
using System.Collections.Generic;
using Matrix.Sdk.Core.Infrastructure.Dto.Sync;
using Matrix.Sdk.Core.Infrastructure.Dto.Sync.Event.Room;

namespace Matrix.Sdk.Core.Domain.RoomEvent
{
    public abstract record BaseRoomEvent(string EventId, string RoomId, string SenderUserId, DateTimeOffset Timestamp)
    {
        public static List<BaseRoomEvent> Create(string roomId, RoomResponse joinedRoom)
        {
            var roomEvents = new List<BaseRoomEvent>();

            foreach (RoomEventResponse timelineEvent in joinedRoom.Timeline.Events)
            {
                var e = Create(roomId, timelineEvent);
                if (e != null) roomEvents.Add(e);

            }
            return roomEvents;
        }
        
        public static BaseRoomEvent Create(string roomId, RoomEventResponse timelineEvent)
        {
            if (MembershipEvent.Factory.TryCreateFrom(timelineEvent, roomId, out MembershipEvent joinRoomEvent)) return joinRoomEvent;
            if (CreateRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var createRoomEvent)) return createRoomEvent;
            if (TextMessageEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var textMessageEvent)) return textMessageEvent;
            if (ImageMessageEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var imageMessageEvent)) return imageMessageEvent;
            if (RedactionEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var redactionEvent)) return redactionEvent;
            if (ReactionEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var reactionEvent)) return reactionEvent;
            return null;
        }

        public static List<BaseRoomEvent> CreateFromInvited(string roomId, InvitedRoom invitedRoom)
        {
            var roomEvents = new List<BaseRoomEvent>();

            foreach (RoomStrippedState inviteStateEvent in invitedRoom.InviteState.Events)
            {
                var e = CreateFromInvited(roomId, inviteStateEvent);
                if (e != null)
                {
                    roomEvents.Add(e);
                }
            }
            return roomEvents;
        }
        
        public static BaseRoomEvent CreateFromInvited(string roomId, RoomStrippedState inviteStateEvent)
        {
            if (MembershipEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId, out var joinRoomEvent)) return joinRoomEvent;
            if (CreateRoomEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId, out var createRoomEvent)) return createRoomEvent;
            if (TextMessageEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId, out var textMessageEvent)) return textMessageEvent;
            if (ImageMessageEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId, out var imageMessageEvent)) return imageMessageEvent;
            if (RedactionEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId, out var redactionEvent)) return redactionEvent;
            if (ReactionEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId, out var reactionEvent)) return reactionEvent;
            return null;
        }
    }
}