using UnityEngine;

namespace Assets.Scripts.InventorySystem
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] public Inventory Parameters;
        [SerializeField] public Inventory Traits;
        [SerializeField] public Inventory Items;
        [SerializeField] public Inventory ClothTop;
        [SerializeField] public Inventory ClothBottom;
        [SerializeField] public Inventory Shoes;
    }
}
