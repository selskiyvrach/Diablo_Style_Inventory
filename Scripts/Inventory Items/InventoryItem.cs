using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : IVector2IntItem
{
// STATIC:

    // PARENT GAMEOBJECT 
    private static Transform _parentHolder;
    private static Transform GetParent(Canvas parent) 
    {
        _parentHolder ??= new GameObject("Inventory Items' Storage").transform;
        _parentHolder.SetParent(parent.transform);
        return _parentHolder;
    } 

    // POOL OF UNUSED ITEMS
    private static Stack<Image> _abandoned = new Stack<Image>();
    private static Image GetImage(InventoryItemData data, Canvas parent, float unitSize)
    {
        Image image = null;
        // TAKE FROM STACK
        if(_abandoned.Count > 0 && _abandoned.Peek() != null)
        {
            image = _abandoned.Pop();
            image.gameObject.name = data.Name;
            image.gameObject.SetActive(true);
        }
        // OR CREATE
        else 
        {
            image = new GameObject(data.Name).AddComponent<Image>();
            image.transform.SetParent(GetParent(parent));
        }
        image.sprite = data.Sprite;
        image.rectTransform.sizeDelta = new Vector2(data.SizeInt.x * unitSize, data.SizeInt.y * unitSize); 
        return image;
    }

// INSTANCE: 

    // IVector2IntItem:
    public Vector2Int SizeInt { get; set; }
    public Vector2Int TopLeftCornerPosInt { get; set; }

    // InventoryItem:
    public InventoryItemData ItemData { get; private set; }
    public Vector2 ScreenPos { get => _image.transform.position; set => _image.transform.position = value; }
    public Vector2 ScreenSize => _image.rectTransform.sizeDelta;
    public bool _oneCellItem;
    // VISUAL REPRESENTATION OF ITEM IN UI SPACE
    private Image _image;

    public InventoryItem(InventoryItemData data)
    {
        ItemData = data;
        SizeInt = ItemData.SizeInt;
        _oneCellItem = ItemData.SizeInt.magnitude < 2;
    }

    public void MoveOnTopOfViewSorting()
        => _image?.transform.SetSiblingIndex(_image.transform.parent.childCount);

    public void MoveInTheBackOfViewSorting()
        => _image?.transform.SetSiblingIndex(0);

    public void EnableInventoryViewOfItem(Canvas parent, float unitSize)
        => _image ??= GetImage(ItemData, parent, unitSize);

    public void DisableInventoryViewOfItem()
    {
        if(_image != null)
        {
            _image.gameObject.SetActive(false);
            _abandoned.Push(_image);
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
        if(_oneCellItem) temp = _image.transform.position; 
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