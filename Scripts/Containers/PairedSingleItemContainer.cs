using System.Linq;
using UnityEngine;

namespace D2Inventory
{
    public class PairedSingleItemContainer : SingleItemContainer
    {
        [SerializeField] PairedSingleItemContainer pair;

        public override InventoryItem ExtractItem(InventoryItem item)
        {
            if(item != null)
            {
                if(item == content)
                    return ExtractContent();
                else if(item == pair.content)
                    return pair.ExtractContent();
            }
            return null;
        }

        public override Projection GetProjection(InventoryItem item, Vector2 screenPos)
        {
            // guard clause - whether the cursor pos overlaps the slot rect
            if(!ActiveOnScreen || !screenRect.ContainsPoint(screenPos))
                return lastProjection = Projection.EmptyProjection;

            InventoryItem replacement = null;
            InventoryItem refugee = null;

            // if pair has two-handed - it's the replacement
            if(pair.content != null && pair.content.ItemData.TwoHanded)
                replacement = pair.content;
            // else if trying to put item and pair content can't pair 
            else if(item != null && fitRule.CanFit(item.ItemData.FitRule) && pair.content != null && !pair.content.ItemData.FitRule.CanPair(item.ItemData.FitRule)) 
                // if this.content is present - pair content goes as refugee
                if(content != null)
                    refugee = pair.content;
                // if this.content isn't present - pair content goes as replacement
                else 
                    replacement = pair.content;
            // default replacement if still empty - this content
            replacement ??= content;  

            bool canPlace = item == null ? true : fitRule.CanFit(item.ItemData.FitRule); 
            var refs = refugee == null ? null : new InventoryItem[] { refugee }; 
            
            if(lastProjection.FieldsEqual(screenRect.NormalizedRectToScreenRect(new Rect(0,0,1,1)), canPlace, screenRect.Rect.center, replacement, refs))
                return Projection.SameProjection;
            else 
                return lastProjection = new Projection(this, screenRect.NormalizedRectToScreenRect(new Rect(0,0,1,1)), canPlace, screenRect.Rect.center, replacement, refs);
        }

        public override InventoryItem PlaceItem(InventoryItem item)
        {
            if(item == null) return null;
            lastProjection ??= GetProjection(item, item.DesiredScreenPos);

            var outt = lastProjection.Replacement;
            ExtractItem(outt);

            content = item;

            AnchorNewContent();
            return outt;
        }

        public override bool TryPlaceItemAuto(InventoryItem item)
        {
            if(item != null && fitRule.CanFit(item.ItemData.FitRule))
                if(content == null)
                    if((pair.content == null || pair.content.ItemData.FitRule.CanPair(item.ItemData.FitRule)))
                    {
                        content = item;
                        AnchorNewContent();
                        return true;
                    }
            return false;
        }
                
        public override bool CanPlaceItemsAuto(InventoryItem[] items)
            => 
            items.Length == 1 && 
            content == null && 
            fitRule.CanFit(items[0].ItemData.FitRule) && 
            (pair.content == null || pair.content.ItemData.FitRule.CanPair(items[0].ItemData.FitRule));

// PRIVATE

        private InventoryItem ExtractContent()
        {
            if(content == null) return null;

            if(content.ItemData.FitRule.TwoHanded)
            {
                change.Add((content.IconIDs[1], IconInfo.Hide));
                content.SecondTakenContainer = null;
            }
            return base.ExtractItem(base.content);
        }

        protected override void AnchorNewContent()
        {
            if(content == null) return;

            base.AnchorNewContent();

            if(content.ItemData.TwoHanded)
            {
                content.SecondTakenContainer = pair;
                change.Add((content.IconIDs[1], IconInfo.Reveal));
                change.Add((content.IconIDs[1], IconInfo.GetMoveOnly(pair.screenRect.Rect.center)));
                change.Add((content.IconIDs[1], IconInfo.SetHalfOpacity));
            }
        }

        public override (int, IconInfo)[] GetIconChange()
            => change.Concat(pair.change).ToArray();

        public override void RefreshIconCnange()
        {
            change.Clear();
            pair.change.Clear();
        }
    }
}