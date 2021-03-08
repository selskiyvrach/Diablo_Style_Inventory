
public class SingleItemContainer : ScreenSpaceItemContainer
{
    public override void Init()
        => _storeSpace = new SingleItemSlotSpace(_fitRule);
}
