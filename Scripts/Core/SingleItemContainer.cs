
public class SingleItemContainer : ScreenSpaceItemContainer
{
    public override void Init()
        => _storeSpace = new SingleItemSlotSpace(_fitRule);

    public void DisableContentsVisuals()
        => ((SingleItemSlotSpace)_storeSpace)?.Content?.DisableInventoryViewOfItem();

    public void EnableContentsVisuals()
        => ((SingleItemSlotSpace)_storeSpace)?.Content?.EnableInventoryViewOfItem();
}
