using UnityEngine;

public class CelledSpaceItemContainer : ScreenSpaceItemContainer
{
    public override void Init()
        => _storeSpace = new ItemStorageSpace(SizeData);
}
