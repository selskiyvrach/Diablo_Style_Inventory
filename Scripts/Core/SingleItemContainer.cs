
public class SingleItemContainer : ScreenSpaceItemContainer
{
    public override void Init()
        => _storeSpace = new SingleItemSlotSpace(_fitRule);

    public void SetContentVisualsActive(bool value)
        => ((SingleItemSlotSpace)_storeSpace)?.Content?.SetVisualsActive(value);
}
