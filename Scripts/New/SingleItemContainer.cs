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
            => lastProjection = screenRect.ContainsPoint(screenPos) ? 
                new Projection(
                    screenRect.Rect, 
                    item == null ? 
                        true : 
                        fitRule.CanFit(item.FitRule),
                    content,
                    null) :
                Projection.EmptyProjection;

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