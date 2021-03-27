using UnityEngine;

public class SingleItemPairedContainerSpace : SingleItemSlotSpace
{
    private SingleItemPairedContainer _pair;
    private ScreenSpaceItemContainer _mainStorage;
    
    public SingleItemPairedContainerSpace(ItemFitRule fitRule, SingleItemPairedContainer pair, ScreenSpaceItemContainer mainStorage) : base(fitRule)
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
        return false;
    }

    public override bool CanPlaceItemAuto(InventoryItem item)
        => Empty() && _fitRule.CanFit(item.FitRule) && _pair.CanPair(item.FitRule);

    public override void PlaceItem(InventoryItem item, Vector2 leftCornerPosNormalized, out Vector2 itemCenterPosNormalized, out InventoryItem replaced)
    {
        itemCenterPosNormalized = new Vector2();
        replaced = null;
        if(!_fitRule.CanFit(item.FitRule)) return;

        if(((ItemFitAndPairRule)Content.FitRule).PairType.TwoHanded)
        {
            
        }

        // IF PAIRED SLOT EMPTY OR IT'S CONTENT CAN BE EQUIPPED WITH THE ITEM YOU ARE TRYING TO PUT - OK
        if(_pair.CanPair(item.FitRule))
        {
            base.PlaceItem(item, leftCornerPosNormalized, out Vector2 itemSenterPosNormalized2, out InventoryItem replaced2);
            itemCenterPosNormalized = itemSenterPosNormalized2;
            replaced = replaced2;
        }
        else
        {
            // replaced should be either this slot's content or pair two-handed content
            // TRY TO PUT PAIRED SLOTS'S CONTENT TO INVENTORY. IF THERE'S PLACE - OPERATION SUCCEDES
            // peek returns two-handed or pair's one-handed content
            if(_pair.PeekItem(Vector2.zero, out InventoryItem peeked))
            {
                // if peeked is single slot pair's content
                if(_mainStorage.TryPlaceItemAuto(peeked))
                {
                    base.PlaceItem(item, leftCornerPosNormalized, out Vector2 itemSenterPosNormalized2, out InventoryItem replaced2);
                    itemCenterPosNormalized = itemSenterPosNormalized2;
                    replaced = replaced2;
                    _pair.ExtractItem(peeked, out InventoryItem extracted);
                }

                // if peeked is two-handed
                // 
            }
        }
    }
}
