using CMD.Base;

namespace CMD.Core
{
    public interface ICatalog<TDefinition> where TDefinition : EntityDefinitionSO
    {
        TDefinition GetByKey(string key);
        bool TryGetByKey(string key, out TDefinition def);
    }
}
