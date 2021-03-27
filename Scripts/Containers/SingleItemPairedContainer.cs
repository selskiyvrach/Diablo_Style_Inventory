using UnityEngine;

public class SingleItemPairedContainer : SingleItemContainer
{
    [SerializeField] SingleItemPairedContainer pair;
    [SerializeField] ScreenSpaceItemContainer mainStorage;
    
    private InventoryItemVisuals _twoHandedPlaceholder;

    public SingleItemPairedContainerSpace SlotSpace => (SingleItemPairedContainerSpace)_storeSpace;
    
    public override void Init()
        => _storeSpace = new SingleItemPairedContainerSpace(_fitRule, pair, mainStorage);
    
    public bool CanPair(ItemFitRule fitRule)
        => ((SingleItemPairedContainerSpace)_storeSpace).CanPair(fitRule);

    public bool ContainsTwoHanded()
        => !Empty() && ((ItemFitAndPairRule)Content.FitRule).PairType.TwoHanded;

    public override void PlaceItem(InventoryItem item, out InventoryItem replaced)
    {
        replaced = null;

        // remove clone pictures if any of the paired contains two-handed
        if(this.ContainsTwoHanded())
            pair.RemoveCloneImage();
        else if(pair.ContainsTwoHanded())
        {
            RemoveCloneImage();
            replaced = pair.Content;
        }

        // place item here
        base.PlaceItem(item, out InventoryItem replaced2);
        replaced ??= replaced2;

        // create clone image
        if(((ItemFitAndPairRule)item.FitRule).PairType.TwoHanded)
            pair.SetCloneImageForItem(item);
    }

    private void SetCloneImageForItem(InventoryItem toClone)
    {
        RemoveCloneImage();
        _twoHandedPlaceholder = toClone.GetClone();
        _twoHandedPlaceholder.DesiredScreenPos = ScreenRect.Rect.center;
    }

    private void RemoveCloneImage()
    {
        if(_twoHandedPlaceholder != null)
        {
            InventoryItemVisuals.AbandonItemVisuals(_twoHandedPlaceholder);
            _twoHandedPlaceholder = null;
        }
    }

    public override void ExtractItem(InventoryItem toExtract, out InventoryItem extracted)
    {
        extracted = null;
        if(toExtract == Content)
        {
            if(ContainsTwoHanded())
                pair.RemoveCloneImage();
            base.ExtractItem(toExtract, out InventoryItem extracted2);
            extracted = extracted2;
        }
        else if(toExtract == pair.Content)
        {
            if(pair.ContainsTwoHanded())
                RemoveCloneImage(); 
            pair.ExtractItem(toExtract, out InventoryItem extracted2);
            extracted = extracted2;
        }
    }

    ///<summary>returns contnent or pair's content if equipped item is two-handed</summary>
    public override bool PeekItem(Vector3 screenPos, out InventoryItem peeked)
    {
        if(base.PeekItem(screenPos, out peeked))
            return true;
        else if(!pair.Empty())
        {
            if(((ItemFitAndPairRule)pair.Content.FitRule).PairType.TwoHanded)
            {
                peeked = pair.Content;
                return true;
            }
        }
        peeked = null;
        return false;
    }

    public override void SetActive(bool value)
    {
        base.SetActive(value);
        _twoHandedPlaceholder?.gameObject.SetActive(value);
    }
}
