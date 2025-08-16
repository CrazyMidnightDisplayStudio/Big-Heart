using UnityEngine;

namespace CMD.Core
{
    public sealed class EntityFactory : IEntityFactory
    {
        readonly IGameContext _ctx;
        public EntityFactory(IGameContext ctx) { _ctx = ctx; }

        public EntityRuntime Create(EntityDefinitionSO def, Vector3 pos, Quaternion rot)
        {
            var prefab = def.viewPrefab ?? new GameObject($"Item_{def.Key}");
            var go = Object.Instantiate(prefab, pos, rot);
            var runtime = go.GetComponent<EntityRuntime>() ?? go.AddComponent<EntityRuntime>();
            runtime.Init(def, _ctx);
            return runtime;
        }

        public EntityRuntime CreateFromSave(EntitySaveData s, IEntityCatalog catalog)
        {
            var def = catalog.GetByKey(s.definitionKey);
            var inst = Create(def, s.position, s.rotation);
            var sid = inst.GetComponent<StableId>();
            if (System.Guid.TryParse(s.instanceId, out var g))
            {
                sid.SetFromSave(g);
            }
            inst.Restore(s);
            return inst;
        }
    }
}
