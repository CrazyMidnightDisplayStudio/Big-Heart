namespace CMD.Entities
{
    public interface IEntityCatalog
    {
        EntityDefinitionSO GetByKey(string key);
        bool TryGetByKey(string key, out EntityDefinitionSO def);
    }
}
