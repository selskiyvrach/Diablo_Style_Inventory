using MNS.Utils.Values;
using UnityEngine;


public class InventoryItemFactory 
{
    private static float _unitSize;
    private static FloatHandlerSource _sizeHandler;

    public static void Init(FloatHandlerSource unitSizeSource)
    {
        if(unitSizeSource == null) 
            return;

        if(_sizeHandler != null)
            _sizeHandler.Value.Handler -= (o, f) => SetUnitSize(f);

        _sizeHandler = unitSizeSource;
        _sizeHandler.Value.AddWithInvoke((o, f) => SetUnitSize(f));
    }

    private static void SetUnitSize(float unitSize)
        => _unitSize = unitSize;

    public static InventoryItem GetInventoryItem(InventoryItemData data)
        => new InventoryItem(data);
}
