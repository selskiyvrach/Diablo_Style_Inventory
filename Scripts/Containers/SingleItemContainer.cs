using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace D2Inventory
{
    public class SingleItemContainer : ContainerBase
    {
        protected InventoryItem content;

        private void Update() {
            if(Input.GetKeyDown(KeyCode.T))
                Debug.Log(content != null ? content.ItemData.Name : "nothing");
        }

        public override InventoryItem ExtractItem(InventoryItem item)
        {
            var outt = content;
            content = null;
            if(outt != null)
                outt.Container = null;
            return outt;
        }

        public override Projection GetProjection(InventoryItem item, Vector2 screenPos)
        {
            if(!ActiveOnScreen || !screenRect.ContainsPoint(screenPos))
                return lastProjection = Projection.EmptyProjection;

            var canPlace = item == null ? true : fitRule.CanFit(item.ItemData.FitRule);

            if(lastProjection.FieldsEqual(screenRect.Rect, canPlace, screenRect.Rect.center, content, null))
                return Projection.SameProjection;
            else 
                return lastProjection = new Projection(this, screenRect.Rect, canPlace, screenRect.Rect.center, content, null);
        }

        public override InventoryItem PlaceItem(InventoryItem item)
        {
            item.DesiredScreenPos = screenRect.Rect.center;
            var outt = content;
            outt.Container = null;
            content = item;
            content.Container = this;
            return outt;
        }
        
        public override bool CanPlaceItemsAuto(InventoryItem[] items)
            => items.Length == 1 && content == null && fitRule.CanFit(items[0].ItemData.FitRule);

        public override bool TryPlaceItemAuto(InventoryItem item)
        {
            item.DesiredScreenPos = screenRect.Rect.center;
            if(content != null || !fitRule.CanFit(item.ItemData.FitRule)) return false;
            content = item;
            content.Container = this;
            return true;
        }

        public override bool TryPlaceItemsAuto(InventoryItem[] items)
            => items.Length > 1 ? false : TryPlaceItemAuto(items[0]);
    }
}