using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace D2Inventory
{
    public class SingleItemContainer : ContainerBase
    {
        protected InventoryItem content;

        public override InventoryItem ExtractItem(InventoryItem item)
        {
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
            var outt = content;
            content = item;
            return outt;
        }

        public override bool TryPlaceItemAuto(InventoryItem item)
        {
            if(content != null || !fitRule.CanFit(item.ItemData.FitRule)) return false;
            content = item;
            return true;
        }
    }
}