namespace CMD.Events.ContainmentEvents
{
    public readonly struct RemovedFromContainer
    {
        public readonly string OwnerId;
        public readonly string ContainerKey;
        public readonly string EntityId;
        public readonly EChangeOrigin Origin;
        public RemovedFromContainer(string ownerId, string key, string entityId, EChangeOrigin origin = EChangeOrigin.gameplay)
        { OwnerId = ownerId; ContainerKey = key; EntityId = entityId; Origin = origin; }
    }
}
