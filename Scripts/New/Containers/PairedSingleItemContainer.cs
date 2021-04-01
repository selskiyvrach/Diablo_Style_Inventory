using UnityEngine;

namespace D2Inventory
{
    public class PairedSingleItemContainer : SingleItemContainer
    {
        [SerializeField] PairedSingleItemContainer pair;

        private InventoryItemVisuals _cloneImage;

// PUBLIC

        public override InventoryItem ExtractItem(InventoryItem item)
        {
            if(item != null)
            {
                if(item == content)
                    return ExtractContent();
                else if(
                    pair.content == item && 
                    pair.content.FitRule.TwoHanded)
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
            if(pair.content != null && pair.content.FitRule.TwoHanded)
                replacement = pair.content;
            // else if trying to put item and pair content can't pair 
            else if(item != null && pair.content != null && !pair.content.FitRule.CanPair(item.FitRule)) 
                // if this.content is present - pair content goes as refugee
                if(content != null)
                    refugee = pair.content;
                // if this.content isn't present - pair content goes as replacement
                else 
                    replacement = pair.content;
            // default replacement if still empty - this content
            replacement ??= content;  

            bool canPlace = item == null ? true : fitRule.CanFit(item.FitRule); 
            var refs = refugee == null ? null : new InventoryItem[] { refugee }; 
            
            if(lastProjection.FieldsEqual(screenRect.Rect, canPlace, replacement, refs))
                return Projection.SameProjection;
            else 
                return lastProjection = new Projection(screenRect.Rect, canPlace, replacement, refs);
        }

        public override InventoryItem PlaceItem(InventoryItem item)
        {
            if(item == null) return null;
            lastProjection ??= GetProjection(item, item.ScreenPos);

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
            if(fitRule.CanFit(item.FitRule))
                if(content == null)
                    if((pair.content == null || pair.content.FitRule.CanPair(item.FitRule)))
                    {
                        SetAsContent(item); 
                        return true;
                    }
            return false;
        }

// PRIVATE

        private InventoryItem ExtractContent()
        {
            if(content != null && content.FitRule.TwoHanded)
                pair.RemoveCloneImage();

            return base.ExtractItem(base.content);
        }

        private void SetAsContent(InventoryItem item)
        {
            // put item to this slot
            content = item;
            content.ScreenPos = screenRect.Rect.center;

            // if two-handed - set clone image to paired slot
            if(content.FitRule.TwoHanded)
                pair.SetCloneImage(content);
        }

// CLONE IMAGE FOR TWO-HANDED

        private void SetCloneImage(InventoryItem item)
        {
            _cloneImage = item.GetClone();
            _cloneImage.DesiredScreenPos = screenRect.Rect.center;
        }

        private void RemoveCloneImage()
        {
            InventoryItemVisuals.AbandonItemVisuals(_cloneImage);
            _cloneImage = null;
        }
    
    }
}