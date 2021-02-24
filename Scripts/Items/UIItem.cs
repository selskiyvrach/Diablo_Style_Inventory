using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour, IScreenSizeAndPos
{
    // UIItem
    private Image _image;
    public Item TheItem { get; private set; }

    // IUIItem
    public Vector2 ScreenSize { get ; set; }
    public Vector2 ScreenPos { get ; set; }

    public void Init(Item item, float intUnitSize, Canvas parent)
    {
        TheItem = item;
        CreateVisuals(intUnitSize, item.SizeInt);
        transform.SetParent(parent.transform);
    }

    public void CreateVisuals(float unitSize, Vector2Int sizeInt)
    {
        _image = gameObject.AddComponent<Image>();
        _image.sprite = TheItem.ItemData.Sprite;
        _image.rectTransform.sizeDelta = new Vector2(unitSize * sizeInt.x, unitSize * sizeInt.y);
    }
}
