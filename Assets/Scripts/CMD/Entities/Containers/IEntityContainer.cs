namespace CMD.Entities
{
    public interface IEntityContainer
    {
        string OwnerId { get; } // "Player" / "Chest_42" / "NPC_17"
        string ContainerKey { get; } // "Backpack" / "Equipment" / "Chest"
        int Capacity { get; }

        bool Add(BaseEntityRuntime entity, int index = -1, string slotKey = null);
        bool Remove(BaseEntityRuntime entity);
        bool Move(BaseEntityRuntime entity, int newIndex, string newSlotKey = null);

        bool Contains(BaseEntityRuntime entity);
        int IndexOf(BaseEntityRuntime entity); // -1 если нет
        BaseEntityRuntime IndexOfSlot(int index);
    }
}
