namespace Matrix.Sdk.Core.Infrastructure.Dto.Sync.Event.Room
{
    public record RoomStateEvent : RoomEventResponse
    {
        /// <summary>
        ///     <b>Required.</b>
        ///     A unique key which defines the overwriting semantics for this piece of room state.
        ///     This value is often a zero-length string.
        ///     The presence of this key makes this event a State Event.
        /// </summary>
        public string StateKey { get; init; }
    }
}