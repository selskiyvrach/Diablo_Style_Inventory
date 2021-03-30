using UnityEngine;

namespace D2Inventory
{
    public class PairedSingleItemContainer : ContainerBase
    {
        [SerializeField] PairedSingleItemContainer pair;

        private InventoryItem _content;
        private InventoryItemVisuals _cloneImage;

        public override InventoryItem ExtractItem(InventoryItem item)
        {
            if(item != null)
            {
                if(item == _content)
                    return ExtractContent();
                else if(
                    pair._content == item && 
                    pair._content.FitRule.TwoHanded)
                    return pair.ExtractContent();
            }
            return null;
        }

        private InventoryItem ExtractContent()
        {
            if(_content != null && _content.FitRule.TwoHanded)
                pair.RemoveCloneImage();

            var outt = _content;
            _content = null;
            return outt;
        }

        public override InventoryItem PlaceItem(InventoryItem item)
        {
            if(item == null) return null;
            lastProjection ??= GetProjection(item, item.ScreenPos);

            var outt = lastProjection.Replacement;
            if (outt == _content)
                ExtractContent();
            else if (outt == pair._content)
                pair.ExtractContent();

            SetAsContent(item);
            return outt;
        }

        private void SetAsContent(InventoryItem item)
        {
            // put item to this slot
            _content = item;
            _content.ScreenPos = screenRect.Rect.center;

            // if two-handed - set clone image to paired slot
            if (_content.FitRule.TwoHanded)
                pair.SetCloneImage(_content);
        }

        public override Projection GetProjection(InventoryItem item, Vector2 screenPos)
        {
            // guard clause - whether the cursor pos overlaps the slot rect
            if(!screenRect.ContainsPoint(screenPos))
                return lastProjection = Projection.EmptyProjection;

            InventoryItem replacement = null;
            InventoryItem refugee = null;

            // if pair has two-handed - it's the replacement
            if(pair._content != null && pair._content.FitRule.TwoHanded)
                replacement = pair._content;
            // else if trying to put item and pair content can't pair 
            else if(item != null && pair._content != null && !pair._content.FitRule.CanPair(item.FitRule)) 
                // if this.content is present - pair content goes as refugee
                if(_content != null)
                    refugee = pair._content;
                // if this.content isn't present - pair content goes as replacement
                else 
                    replacement = pair._content;
            // default replacement if still empty - this content
            replacement ??= _content;  

            bool canPlace = item == null ? true : fitRule.CanFit(item.FitRule); 
            var refs = refugee == null ? null : new InventoryItem[] { refugee }; 
            
            if(lastProjection.FieldsEqual(screenRect.Rect, canPlace, replacement, refs))
                return Projection.SameProjection;
            else 
                return lastProjection = new Projection(screenRect.Rect, canPlace, replacement, refs);
        }

        public override bool TryPlaceItemAuto(InventoryItem item)
        {
            if(fitRule.CanFit(item.FitRule))
                if(_content == null)
                    if((pair._content == null || pair._content.FitRule.CanPair(item.FitRule)))
                    {
                        SetAsContent(item); 
                        return true;
                    }
            return false;
        }

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