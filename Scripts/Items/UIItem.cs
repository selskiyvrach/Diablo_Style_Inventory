using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour, IVector2IntSizeAndPos, IUIItem
{
    // UIItem
    private Image _image;
    public Item TheItem { get; private set; }

    // IVector2IntSizeAndPos
    public Vector2Int SizeInt { get; private set; }
    public Vector2Int TopLeftCornerPosInt { get; set; }

    // IUIItem
    public Vector2 ScreenSize { get ; set; }
    public Vector2 ScreenPos { get ; set; }

    public void Init(Item item, float intUnitSize, Transform parent)
    {
        TheItem = item;
        SizeInt = item.ItemData.SizeInt;
        CreateVisuals(intUnitSize);
        transform.SetParent(parent.transform);
    }

    public void CreateVisuals(float squareSize)
    {
        _image = gameObject.AddComponent<Image>();
        _image.sprite = TheItem.ItemData.Sprite;
        _image.rectTransform.sizeDelta = new Vector2(squareSize * SizeInt.x, squareSize * SizeInt.y);
    }
}
