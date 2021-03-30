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
            if(!screenRect.ContainsPoint(screenPos))
                return lastProjection = Projection.EmptyProjection;

            var canPlace = item == null ? true : fitRule.CanFit(item.FitRule);

            if(lastProjection.FieldsEqual(screenRect.Rect, canPlace, content, null))
                return Projection.SameProjection;
            else 
                return lastProjection = new Projection(screenRect.Rect, canPlace, content, null);
        }


        public override InventoryItem PlaceItem(InventoryItem item)
        {
            var outt = content;
            content = item;
            content.ScreenPos = screenRect.Rect.center;
            return outt;
        }

        public override bool TryPlaceItemAuto(InventoryItem item)
        {
            if(content != null) return false;
            content = item;
            return true;
        }
    }
}