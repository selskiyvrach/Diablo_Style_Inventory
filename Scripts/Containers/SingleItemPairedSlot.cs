using UnityEngine;

public class SingleItemPairedSlot : SingleItemContainer
{
    [SerializeField] SingleItemPairedSlot pair;
    [SerializeField] ScreenSpaceItemContainer mainStorage;

    public SingleItemPairedSlotSpace SlotSpace => (SingleItemPairedSlotSpace)_storeSpace;
    
    public override void Init()
        => _storeSpace = new SingleItemPairedSlotSpace(_fitRule, pair, mainStorage);
    
    public bool CanPair(ItemFitRule fitRule)
        => ((SingleItemPairedSlotSpace)_storeSpace).CanPair(fitRule);

}
