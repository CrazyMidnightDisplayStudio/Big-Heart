using CMD.Base;

namespace BigHeart
{
    public interface IItemContainer : IEntityContainer
    {
        bool Add(ItemRuntime item, int index = -1, string slotKey = null);
        bool Remove(ItemRuntime item);
        bool Move(ItemRuntime item, int newIndex, string newSlotKey = null);
    }
}
