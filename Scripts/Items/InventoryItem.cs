using UnityEngine;

public class InventoryItem : IVector2IntItem
{
// STATIC:

#region Parent GameObject

    private static Transform _parent;

    public static void Init(Canvas parent)
    {
        _parent ??= new GameObject("Inventory Items' Storage").transform;
        _parent.transform.SetParent(parent.transform);
    }

    public static void SetInventoryItemsActive(bool value)
        => _parent.gameObject.SetActive(value);

#endregion

// INSTANCE: 

    // IVector2IntItem:
    public Vector2Int SizeInt { get; set; }
    public Vector2Int TopLeftCornerPosInt { get; set; }
    public bool OneCellItem { get; private set; }

    // InventoryItem:
    public InventoryItemData ItemData { get; private set; }
    public ItemFitRule FitRule { get; private set; }
    public Vector2 ScreenPos { get 
        => _image.transform.position; set 
        => _image.DesiredScreenPos = value; }
    public Vector2 ScreenSize { get 
        => _image.RectTransform.sizeDelta; set 
        => _image.RectTransform.sizeDelta = value; }
    public Transform Parent { get 
        => _image.transform.parent; set 
        => _image.transform.SetParent(value); }

    // VISUAL REPRESENTATION OF ITEM IN UI SPACE
    private InventoryItemVisuals _image;
    private float _unitSize;

    public InventoryItem(InventoryItemData data, float unitSize)
    {
        ItemData = data;
        FitRule = ItemData.FitRule;
        SizeInt = ItemData.SizeInt;
        OneCellItem = ItemData.SizeInt.magnitude < 2;
        _unitSize = unitSize;
    }

    public void MoveOnTopOfViewSorting()
        => _image?.transform.SetSiblingIndex(_image.transform.parent.childCount);

    public void MoveInTheBackOfViewSorting()
        => _image?.transform.SetSiblingIndex(0);

    public void EnableInventoryViewOfItem()
        => _image ??= InventoryItemVisuals.GetItemVisuals(ItemData, _parent, _unitSize);

    public void SetVisualsActive(bool value)
        => _image?.gameObject.SetActive(value);

    public void DisableInventoryViewOfItem()
    {
        if(_image != null)
        {
            InventoryItemVisuals.AbandonItemVisuals(_image);            
            _image = null;
        }
    }

    ///<param name="cornerNumber"> 
    ///0 = center of one-cell item/top-left corner of multi-cell item, 1 = bottom-right corner</param>
    ///<summary>
    ///0 = center of one-cell item/top-left corner of multi-cell item, 1 = bottom-right corner</summary>
    public Vector2 GetCornerCenterInScreen(int cornerNumber, float unitSize)
    {   
        Vector2 temp = new Vector2();

        var sizeInt = ItemData.SizeInt;
        var pos = _image.transform.position;

        cornerNumber = (int)Mathf.Clamp01(cornerNumber); 
        // CENTER OF ONE-CELL ITEM
        if(OneCellItem) 
            temp = _image.transform.position; 
        // CENTER OF TOP-LEFT CORNER CELL
        if(cornerNumber == 0) 
            temp.Set(NegativeX(), PositiveY()); 
        // CENTER OF BOTTOM-RIGHT CORNER CELL
        if(cornerNumber == 1) 
            temp.Set(PositiveX(), NegativeY()); 

        return temp;

        float PositiveX() => sizeInt.x == 1 ? pos.x : (   (float)sizeInt.x / 2 - 0.5f) * unitSize + pos.x;
        float NegativeX() => sizeInt.x == 1 ? pos.x : ( - (float)sizeInt.x / 2 + 0.5f) * unitSize + pos.x;
        float PositiveY() => sizeInt.y == 1 ? pos.y : (   (float)sizeInt.y / 2 - 0.5f) * unitSize + pos.y;
        float NegativeY() => sizeInt.y == 1 ? pos.y : ( - (float)sizeInt.y / 2 + 0.5f) * unitSize + pos.y;
    }

}