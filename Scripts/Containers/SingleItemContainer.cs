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
        
        public override InventoryItem[] GetContent()
            => new InventoryItem[]{ content };  

        public override InventoryItem ExtractItem(InventoryItem item)
        {
            if(item != content) return null;
            UnanchorCurrentContent();
            var outt = content;
            content = null;
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
            var outt = ExtractItem(content);
            content = item;
            AnchorNewContent();
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

        protected virtual void UnanchorCurrentContent()
        {
            if(content != null)
                content.Container = null;
        }

        protected virtual void AnchorNewContent()
        {
            if(content == null) return;

            content.Container = this;

            change.Add((content.IconIDs[0], IconInfo.GetMoveOnly(screenRect.Rect.center)));
            change.Add((content.IconIDs[0], IconInfo.GetChangeParent(screenRect.Transform)));
        }
    }
}