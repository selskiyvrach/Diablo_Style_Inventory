using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour, IVector2IntItem
{
    private Image _image;

    public Item TheItem { get; private set; }

    public Vector2Int SizeInt { get; private set; }

    public Vector2Int TopLeftCornerPos { get; private set; }

    public void Init(Item item, float squareSize, Transform parent)
    {
        TheItem = item;
        SizeInt = item.ItemData.SizeInt;
        CreateVisuals(squareSize);
        transform.SetParent(parent.transform);
    }

    public void CreateVisuals(float squareSize)
    {
        _image = gameObject.AddComponent<Image>();
        _image.sprite = TheItem.ItemData.Sprite;
        _image.rectTransform.sizeDelta = new Vector2(squareSize * SizeInt.x, squareSize * SizeInt.y);
    }
}
