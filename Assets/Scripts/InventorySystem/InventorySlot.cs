using ItemSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.InventorySystem
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class InventorySlot: MonoBehaviour
    {
        private ItemMono item = null;
        public ItemMono Item { get { return item; } }
        public List<SlotType> SlotType { get; set; }
        public string ID = string.Empty;
        public bool Locked = false;
        public bool IsOccupied = false;
        private Collider2D _collider;
        private void Start()
        {
            InitSlot();
            _collider = GetComponent<Collider2D>();
        }
        void Update()
        {
            if (item != null && transform.position != item.gameObject.transform.position && !Input.GetMouseButton(0))
            {
                Move();
            }
        }
        public void Move()
        {
            item.transform.position = _collider.bounds.center;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var item = collision.gameObject.GetComponent<ItemMono>();
            if (item != null && this.item == null)
            {
                Add(item);
            }
        }
        public void Add(ItemMono item)
        {
            var typeItem = item.GetSlotType();
            if (SlotType.Contains(typeItem))
            {
                this.item = item;
                IsOccupied = true;
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            var item = collision.gameObject.GetComponent<ItemMono>();
            if (item == this.item)
            {
                this.item = null;
                IsOccupied = false;
            }
        }
        private void InitSlot()
        {
            var collider = GetComponent<Collider2D>();
            if(collider != null)
            {
                collider.isTrigger = true;
            }
            var rb = GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
                rb.freezeRotation = true;
            }
        }
    }
}
