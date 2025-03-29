using Configs;
using Rounds;
using UnityEngine;
using UnityEngine.Serialization;

namespace Services
{
    public class ConfigService : BaseServiceSingleton<ConfigService> 
    {
        public RoundConfig roundConfig;
        public InventoryConfig inventoryConfig;

        public override void Init()
        {
            base.Init();
            Debug.Log("ConfigService initialized");
        }
    }
}