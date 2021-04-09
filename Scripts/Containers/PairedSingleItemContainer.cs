using UnityEngine;

namespace D2Inventory
{
    public class PairedSingleItemContainer : SingleItemContainer
    {
        [SerializeField] PairedSingleItemContainer pair;

// PUBLIC

        public override InventoryItem ExtractItem(InventoryItem item)
        {
            if(item != null)
            {
                if(item == content)
                    return ExtractContent();
                else if(
                    pair.content == item && 
                    pair.content.ItemData.FitRule.TwoHanded)
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
            if(pair.content != null && pair.content.ItemData.FitRule.TwoHanded)
                replacement = pair.content;
            // else if trying to put item and pair content can't pair 
            else if(item != null && pair.content != null && !pair.content.ItemData.FitRule.CanPair(item.ItemData.FitRule)) 
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
            
            if(lastProjection.FieldsEqual(screenRect.Rect, canPlace, screenRect.Rect.center, replacement, refs))
                return Projection.SameProjection;
            else 
                return lastProjection = new Projection(this, screenRect.Rect, canPlace, screenRect.Rect.center, replacement, refs);
        }

        public override InventoryItem PlaceItem(InventoryItem item)
        {
            if(item == null) return null;
            lastProjection ??= GetProjection(item, item.DesiredScreenPos);

            var outt = lastProjection.Replacement;
            if (outt == content)
                ExtractContent();
            else if (outt == pair.content)
                pair.ExtractContent();

            SetAsContent(item);
            return outt;
        }

        public override bool TryPlaceItemAuto(InventoryItem item)
        {
            if(fitRule.CanFit(item.ItemData.FitRule))
                if(content == null)
                    if((pair.content == null || pair.content.ItemData.FitRule.CanPair(item.ItemData.FitRule)))
                    {
                        SetAsContent(item); 
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
            if(content != null && content.ItemData.FitRule.TwoHanded)
                pair.RemoveCloneImage();

            return base.ExtractItem(base.content);
        }

        private void SetAsContent(InventoryItem item)
        {
            content = item;
            item.DesiredScreenPos = screenRect.Rect.center;
            content.Container = this;

            if(content.ItemData.FitRule.TwoHanded)
                pair.SetCloneImage(content);
        }

// CLONE IMAGE FOR TWO-HANDED

        private void SetCloneImage(InventoryItem item)
        {
            Debug.LogError("Cloning hasn't been reimplemented");
        }

        private void RemoveCloneImage()
        {
            Debug.LogError("Cloning hasn't been reimplemented");
        }
    
    }
}