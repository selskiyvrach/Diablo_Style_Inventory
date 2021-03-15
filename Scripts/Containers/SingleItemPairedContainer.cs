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

    public override void PlaceItem(InventoryItem item, out InventoryItem replaced)
    {
        base.PlaceItem(item, out replaced);
        if(((ItemFitAndPairRule)item.FitRule).PairType.TwoHanded)
        {
            pair._twoHandedPlaceholder = item.GetClone();
            pair._twoHandedPlaceholder.DesiredScreenPos = pair.ScreenRect.Rect.center;
        }
    }

    public override void RemoveItem(InventoryItem toRemove)
    {
        if(toRemove == Content)
        {
            base.RemoveItem(toRemove);
            if(((ItemFitAndPairRule)toRemove.FitRule).PairType.TwoHanded)
            {
                InventoryItemVisuals.AbandonItemVisuals(pair._twoHandedPlaceholder);
                pair._twoHandedPlaceholder = null;
            }
        }
        else if(toRemove == pair.Content)
            pair.RemoveItem(toRemove);
    }

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
