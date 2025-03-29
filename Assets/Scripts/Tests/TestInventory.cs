using Assets.Scripts.InventorySystem;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    public class TestInventory: MonoBehaviour
    {
        public InventoryManager InventoryManager;
        private ItemService _itemService;
        private void Start()
        {
            _itemService = new ItemService();
            _itemService.Init();
        }
        public async void Update()
        {
            if (Input.GetKeyUp(KeyCode.V)) // добавляем в инвентарь эллемент
            {
                var item1 = await _itemService.CreateItem("FirstPerfume");
                InventoryManager.Items.AddItem(item1);
            }
            if (Input.GetKeyUp(KeyCode.C)) // добавляем в инвентарь эллемент
            {
                var item2 = await _itemService.CreateItem("RichPerfume");
                InventoryManager.Items.AddItem(item2);
            }
        }
    }
}
