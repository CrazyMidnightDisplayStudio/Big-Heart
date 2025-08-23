namespace CMD.Events.ContainmentEvents
{
    public readonly struct MovedIntoContainer
    {
        public readonly string OwnerId;
        public readonly string ContainerKey;
        public readonly string EntityId;
        public readonly int FromIndex;
        public readonly int ToIndex;
        public readonly EChangeOrigin Origin;

        public MovedIntoContainer(string ownerId, string key, string entityId, int from, int to, EChangeOrigin origin = EChangeOrigin.gameplay)
        { OwnerId = ownerId; ContainerKey = key; EntityId = entityId; FromIndex = from; ToIndex = to; Origin = origin; }
    }
}
