using ItemSystem;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.InventorySystem
{
    public class DragAndDropItem: MonoBehaviour
    {
        private Camera cam;
        private GameObject _objSelected;
        private Vector3 _startPositionItem; //позиция откуда взяли предмет
        private void Start()
        {
            cam = Camera.main;
        }
        private void Update()
        {
            //Заменить на нвоый инпут
            if (Input.GetMouseButtonDown(0))
            {
                CheckHitObject();
            }
            if (Input.GetMouseButton(0) && _objSelected != null)
            {
                DragObject();
            }
            if (Input.GetMouseButtonUp(0) && _objSelected != null)
            {
                DropObject();
            }
        }


        private void CheckHitObject()
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition); // получаем координаты мыши в мировом пространстве
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero, 0); // получаем все объекты с колайдерами
            var raycastItemCplider = hits.FirstOrDefault(i => i.collider.GetComponent<ItemMono>() != null); // получаем первый item из нажатых колайдеров
            var raycastSlotColider = hits.FirstOrDefault(i => i.collider.GetComponent<InventorySlot>() != null); // проверяем под этим итемом есть слот или нет

            if (raycastItemCplider.collider != null && raycastSlotColider.collider != null)  //если итем есть, то проверяем, слот заблокирован или нет, если да, то такие итемы нельзя двигать
            {
                var slotLocked = raycastSlotColider.transform.gameObject.GetComponent<InventorySlot>().Locked;
                if (slotLocked == false)
                {
                    _startPositionItem = raycastItemCplider.collider.transform.position;
                    _objSelected = raycastItemCplider.transform.gameObject;
                }
            }
        }

        private void DragObject() // перемещаем выбранный item
        { 
            _objSelected.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + 10f));
        }
        private void DropObject()  
        {
            var raycast = Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(Input.mousePosition)); 
            var raycastCollider = raycast.FirstOrDefault(i => i.collider.GetComponent<InventorySlot>() != null);
            // если после отпускания мыши под itemom нет slot , то он  будет возвращен в исходную позицию
            if (raycastCollider.collider == null)
            {
                _objSelected.transform.position = _startPositionItem;
            }
            else
            {
                var slot = raycastCollider.transform.gameObject.GetComponent<InventorySlot>();
                var typeItem = _objSelected.gameObject.GetComponent<ItemMono>();
                //проверяем совпадает тип slot с типом предмета, если нет, то перемещаем обратно в исходную точку
                if (slot.Item != null || !slot.SlotType.Contains(typeItem.GetSlotType()))
                {
                    _objSelected.transform.position = _startPositionItem;
                    slot.Move();
                }
            }
            _objSelected = null;
        }
    }
}
