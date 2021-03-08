using UnityEngine;

public class SingleItemPairedSlot : ScreenSpaceItemContainer
{
    [SerializeField] SingleItemPairedSlot pair;
    [SerializeField] ScreenSpaceItemContainer _mainStorage;
    
    public override void Init()
    {
        _storeSpace = new SingleItemSlotSpace(_fitRule);
        // new SingleItemPairedSlotSpace;
    }
}
