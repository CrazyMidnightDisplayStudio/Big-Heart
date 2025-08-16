using System.Collections.Generic;

namespace CMD.Core
{
    public interface ISaveRepository
    {
        void Upsert(EntitySaveData data);
        bool TryGet(string instanceId, out EntitySaveData data);
        IEnumerable<EntitySaveData> All(); // в простом виде без фильтров
        void Remove(string instanceId);
    }
}
