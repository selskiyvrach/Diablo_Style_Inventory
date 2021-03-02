using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemVisuals : MonoBehaviour
{
    private static Stack<InventoryItemVisuals> _abandoned = new Stack<InventoryItemVisuals>();

    public static InventoryItemVisuals GetItemVisuals(InventoryItemData data, Transform parent, float unitSize)
    {
        InventoryItemVisuals visual = null;

        if(_abandoned.Count > 0 && _abandoned.Peek() != null)
        {
            visual = _abandoned.Pop();
            visual.gameObject.SetActive(true);
        }
        else 
        {
            visual = new GameObject().AddComponent<InventoryItemVisuals>();
            visual.RectTransform = visual.gameObject.AddComponent<RectTransform>();
            visual.transform.SetParent(parent);
        }
        visual.RectTransform.sizeDelta = new Vector2(data.SizeInt.x * unitSize, data.SizeInt.y * unitSize);
        visual.gameObject.name = data.Name;
        visual.SetUpSpriteObject(data, unitSize);
        return visual;
    }

    public static void AbandonItemVisuals(InventoryItemVisuals item)
    {
        item.gameObject.SetActive(false);
        _abandoned.Push(item);
    }

    private Image _image;

    public RectTransform RectTransform { get; private set; }

    private void SetUpSpriteObject(InventoryItemData data, float unitSize)
    {
        _image ??= new GameObject("Image").AddComponent<Image>();
        _image.transform.SetParent(transform);
        _image.sprite = data.Sprite;

        var parentSize = RectTransform.sizeDelta;
        var spriteRect = _image.sprite.rect;

        Vector2 size;
        float scale;

        if(spriteRect.width / spriteRect.height >= (float)data.SizeInt.x / (float)data.SizeInt.y)
        {
            size = new Vector2(spriteRect.width * (parentSize.x / spriteRect.width), spriteRect.height * (parentSize.x / spriteRect.width)); 
            scale = parentSize.x / size.x;
        }
        else
        {
            size = new Vector2(spriteRect.width * (parentSize.y / spriteRect.height), spriteRect.height * (parentSize.y / spriteRect.height));
            scale = parentSize.y / size.y;
        }

        _image.rectTransform.sizeDelta = size * scale;  
    }

}
