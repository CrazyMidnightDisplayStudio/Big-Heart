namespace CMD.Events.ContainmentEvents
{
    public readonly struct AddedToContainer
    {
        public readonly string OwnerId;
        public readonly string ContainerKey;
        public readonly string EntityId;
        public readonly int Index;
        public readonly EChangeOrigin Origin;

        public AddedToContainer(string ownerId, string key, string entityId, int index, EChangeOrigin origin = EChangeOrigin.gameplay)
        { OwnerId = ownerId; ContainerKey = key; EntityId = entityId; Index = index; Origin = origin; }
    }
}
