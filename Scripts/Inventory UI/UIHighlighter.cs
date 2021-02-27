using UnityEngine;
using UnityEngine.UI;

public class UIHighlighter 
{
    private Image _image;
    private InventorySettings _settings;
    public bool Active { get; private set; }
    
// CONSTRUCTORS

    public UIHighlighter(Canvas parent, InventorySettings settings)
    {
        _settings = settings;
        _image = new GameObject("Highlighter").AddComponent<Image>();
        _image.transform.SetParent(parent.transform);
        _image.gameObject.SetActive(false);        
    }

    public UIHighlighter(Canvas parent, InventorySettings settings, Sprite sprite) : this(parent, settings)
        => _image.sprite = sprite;

// PUBLIC

    public void NewHighlight(Vector2 screenPos, Vector2 size, bool red = false)
    {
        if(!Active)
        {
            Active = true;
            _image.gameObject.SetActive(true);
        }
        _image.color = !red ? _settings.HighlightColor : _settings.CantPlaceHereColor;
        _image.transform.position = screenPos;
        _image.rectTransform.sizeDelta = size;
    }

    public void Hide()
    {
        _image.gameObject.SetActive(false);
        Active = false;
    }

}