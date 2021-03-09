using UnityEngine;

public class SingleItemPairedSlotSpace : SingleItemSlotSpace
{
    private SingleItemPairedSlot _pair;
    private ScreenSpaceItemContainer _mainStorage;
    
    public SingleItemPairedSlotSpace(ItemFitRule fitRule, SingleItemPairedSlot pair, ScreenSpaceItemContainer mainStorage) : base(fitRule)
    {   
        _pair = pair;
        _mainStorage = mainStorage;
    }

    public bool CanPair(ItemFitRule fitRule)
        => Empty() ? true : Content.FitRule.CanPair(fitRule);

    public override bool CanPlaceItem(InventoryItem item, Vector2 normalizedRectPos)
    {
        if(_fitRule.CanFit(item.FitRule))
        {
            if(_pair.Empty())
                return true;
            if(_pair.CanPair(item.FitRule) || _mainStorage.CanPlaceItemAuto(_pair.SlotSpace.Content))
                return true;
        }
        Debug.Log("cant place");
        return false;
    }

    public override void PlaceItem(InventoryItem item, Vector2 leftCornerPosNormalized, out Vector2 itemSenterPosNormalized, out InventoryItem replaced)
    {
        itemSenterPosNormalized = new Vector2();
        replaced = null;
        if(!_fitRule.CanFit(item.FitRule)) return;

        if(_pair.CanPair(item.FitRule))
        {
            base.PlaceItem(item, leftCornerPosNormalized, out Vector2 itemSenterPosNormalized2, out InventoryItem replaced2);
            itemSenterPosNormalized = itemSenterPosNormalized2;
            replaced = replaced2;
        }
        else
        {
            if(_pair.PeekItem(Vector2.zero, out InventoryItem peeked))
            {
                if(_mainStorage.TryPlaceItemAuto(peeked))
                {
                    base.PlaceItem(item, leftCornerPosNormalized, out Vector2 itemSenterPosNormalized2, out InventoryItem replaced2);
                    itemSenterPosNormalized = itemSenterPosNormalized2;
                    replaced = replaced2;
                    _pair.RemoveItem(peeked);
                }
                else 
                    Debug.Log("Cannot place item. No space in main storage availible");
            }
        }
    }
}
