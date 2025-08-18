using System.Collections.Generic;

namespace CMD.Entities
{
    public interface ISaveRepository
    {
        void Upsert(EntitySaveData data); // запись в Runtime
        bool Flush(); // фиксирование изменений из Runtime в файл
        bool TryGet(string instanceId, out EntitySaveData data);
        IEnumerable<EntitySaveData> All(); // в простом виде без фильтров
        void Remove(string instanceId);
    }
}
