using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace D2Inventory
{   
    [RequireComponent(typeof(ContainerManager))]
    public class Inventory : MonoBehaviour
    {
        [SerializeField] Canvas inventoryCanvas;
        [SerializeField] InventoryHighlighter highlighter;


        [SerializeField] ContainerManager manager;
        [SerializeField] ContainerBase mainStorage;
        [SerializeField] ContainerBase[] containers;



        [SerializeField] InventoryItemData[] testItemData;

        private InventoryItemDragger _dragger;
        private ContainerBase _currContainer;
        private Projection _proj;
        
        private void Awake()
        {
            highlighter.Initialize(inventoryCanvas);
            InventoryItem.Init(inventoryCanvas);
            _dragger = new InventoryItemDragger(inventoryCanvas);
            PickUpItem();
        }

        private void PickUpItem()
        {
            var item = new InventoryItem(testItemData[Random.Range(0, testItemData.Length)], containers[0].ScreenRect.Rect.size.x / containers[0].SizeData.SizeInt.x);
            item.EnableInventoryViewOfItem();
            _dragger.PickUp(item);
        }

        private void Update() {
            bool overlapsContainer = false;
            foreach(var c in containers)
            {
                var proj = c.GetProjection(_dragger.DraggedItem, Input.mousePosition);
                if(proj != Projection.EmptyProjection)
                {
                    if(proj != Projection.SameProjection)
                    {
                        _currContainer = c;
                        _proj = proj;
                        highlighter.NewHighlight(_proj.ScreenRect.center, _proj.ScreenRect.size, !_proj.CanPlace);
                        Debug.Log("new highlight");
                    }
                    overlapsContainer = true;
                    break;
                }
            }
            if(!overlapsContainer)
            {
                highlighter.HideHighlight();
                _currContainer = null;
            }


            _dragger.UpdateDraggersCursor(Input.mousePosition);
            if(Input.GetKeyDown(KeyCode.Mouse0) && _currContainer != null)
                if(_dragger.Empty)
                    _dragger.PickUp(_currContainer.ExtractItem(_proj.Replacement));
                else if(_proj.CanPlace)
                    if(TryHandleRefugees())
                        _dragger.PickUp(_currContainer.PlaceItem(_dragger.ExtractItem()));

            if(Input.GetKeyDown(KeyCode.Mouse1))
                PickUpItem();
        }

        private bool TryHandleRefugees()
        {
            foreach(var i in _proj.Refugees)
                if(!mainStorage.TryPlaceItemAuto(i))
                {
                    foreach(var j in _proj.Refugees)
                        mainStorage.ExtractItem(j);
                    return false;                    
                }
            return true;
        }
    }
}

