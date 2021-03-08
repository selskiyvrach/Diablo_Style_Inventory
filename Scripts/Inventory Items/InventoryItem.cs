using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : IVector2IntItem, IEquipment
{
// STATIC:

    // PARENT GAMEOBJECT 
    private static Transform _parent;

    public static void Init(Canvas parent)
    {
        _parent ??= new GameObject("Inventory Items' Storage").transform;
        _parent.transform.SetParent(parent.transform);
    }

    public static void SetInventoryItemsActive(bool value)
        => _parent.gameObject.SetActive(value);

// INSTANCE: 

    // IVector2IntItem:
    public Vector2Int SizeInt { get; set; }
    public Vector2Int TopLeftCornerPosInt { get; set; }

    // IEquipment:
    public ItemFitRule FitRule { get; private set; }

    // InventoryItem:
    public InventoryItemData ItemData { get; private set; }
    public Vector2 ScreenPos { get => _image.transform.position; set => _image.DesiredScreenPos = value; }
    public Vector2 ScreenSize { get => _image.RectTransform.sizeDelta; set => _image.RectTransform.sizeDelta = value; }

    public bool OneCellItem;
    // VISUAL REPRESENTATION OF ITEM IN UI SPACE
    private InventoryItemVisuals _image;

    public InventoryItem(InventoryItemData data)
    {
        ItemData = data;
        FitRule = ItemData.FitRule;
        SizeInt = ItemData.SizeInt;
        OneCellItem = ItemData.SizeInt.magnitude < 2;
    }

    public void MoveOnTopOfViewSorting()
        => _image?.transform.SetSiblingIndex(_image.transform.parent.childCount);

    public void MoveInTheBackOfViewSorting()
        => _image?.transform.SetSiblingIndex(0);

    public void EnableInventoryViewOfItem(float unitSize)
        => _image ??= InventoryItemVisuals.GetItemVisuals(ItemData, _parent, unitSize);

    public void DisableInventoryViewOfItem()
    {
        if(_image != null)
        {
            InventoryItemVisuals.AbandonItemVisuals(_image);            
            _image = null;
        }
    }

    ///<param name="cornerNumber">if CornersNumber is 1: 0 = center of one-cell item, if 2: 0 = top-left corner, 1 = bottom-right</param>
    ///<summary>if CornersNumber is 1: 0 = center of one-cell item, if 2: 0 = top-left corner, 1 = bottom-right</summary>

    public Vector2 GetCornerCenterInScreen(int cornerNumber, float unitSize)
    {   
        Vector2 temp = new Vector2();
        cornerNumber = (int)Mathf.Clamp01(cornerNumber); 
        // CENTER OF ONE-CELL ITEM
        if(OneCellItem) temp = _image.transform.position; 
        // CENTER OF TOP-LEFT CORNER CELL
        if(cornerNumber == 0) temp.Set(NegativeX(), PositiveY()); 
        // CENTER OF BOTTOM-RIGHT CORNER CELL
        if(cornerNumber == 1) temp.Set(PositiveX(), NegativeY()); 
        return temp;

        float PositiveX() => ItemData.SizeInt.x == 1 ? _image.transform.position.x : ((float)ItemData.SizeInt.x / 2 - 0.5f) * unitSize + _image.transform.position.x;
        float NegativeX() => ItemData.SizeInt.x == 1 ? _image.transform.position.x : ( - (float)ItemData.SizeInt.x / 2 + 0.5f) * unitSize + _image.transform.position.x;
        float PositiveY() => ItemData.SizeInt.y == 1 ? _image.transform.position.y : ((float)ItemData.SizeInt.y / 2 - 0.5f) * unitSize + _image.transform.position.y;
        float NegativeY() => ItemData.SizeInt.y == 1 ? _image.transform.position.y : ( - (float)ItemData.SizeInt.y / 2 + 0.5f) * unitSize + _image.transform.position.y;
    }

}