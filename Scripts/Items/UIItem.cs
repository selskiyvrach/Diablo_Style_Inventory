using System;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
    // UIItem
    private Vector2[] _corners;
    public int CornersNumber { get; private set; }
    public Vector2 ScreenSize { get; private set; }
    private Image _image;

    public Item TheItem { get; private set; }

    public void Init(Item item, float intUnitSize, Canvas parent)
    {
        TheItem = item;
        CreateVisuals(intUnitSize, item.SizeInt);
        ScreenSize = new Vector2(item.SizeInt.x * intUnitSize, item.SizeInt.y * intUnitSize);
        transform.SetParent(parent.transform);
        _corners = new Vector2[TheItem.SizeInt.x * TheItem.SizeInt.y];
        CornersNumber = _corners.Length;
    }

    ///<param name="cornerNumber">if CornersNumbers is 1: 0 = center of one-cell item, if 2: 0 = top-left corner, 1 = bottom-right</param>
    ///<summary>if CornersNumbers is 1: 0 = center of one-cell item, if 2: 0 = top-left corner, 1 = bottom-right</summary>
    public Vector2 GetCornerCenterInScreen(int cornerNumber, float unitSize)
    {   
        Mathf.Clamp(cornerNumber, 0, CornersNumber); 
        if(CornersNumber == 1) return transform.position; // center of one-cell item
        if(cornerNumber == 0) _corners[cornerNumber].Set(NegativeX(), PositiveY()); // center of top-left corner cell
        if(cornerNumber == 1) _corners[cornerNumber].Set(PositiveX(), NegativeY()); // center of bottom-right corner cell
        return _corners[cornerNumber];

        float PositiveX() => TheItem.SizeInt.x == 1 ? transform.position.x : ((float)TheItem.SizeInt.x / 2 - 0.5f) * unitSize + transform.position.x;
        float NegativeX() => TheItem.SizeInt.x == 1 ? transform.position.x : ( - (float)TheItem.SizeInt.x / 2 + 0.5f) * unitSize + transform.position.x;
        float PositiveY() => TheItem.SizeInt.y == 1 ? transform.position.y : ((float)TheItem.SizeInt.y / 2 - 0.5f) * unitSize + transform.position.y;
        float NegativeY() => TheItem.SizeInt.y == 1 ? transform.position.y : ( - (float)TheItem.SizeInt.y / 2 + 0.5f) * unitSize + transform.position.y;
    }

    public void CreateVisuals(float unitSize, Vector2Int sizeInt)
    {
        if(_image == null)
            _image = gameObject.AddComponent<Image>();
        _image.sprite = TheItem.ItemData.Sprite;
        _image.rectTransform.sizeDelta = new Vector2(unitSize * sizeInt.x, unitSize * sizeInt.y);
    }
}
