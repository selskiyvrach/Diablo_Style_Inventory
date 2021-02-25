using UnityEngine;
using UnityEngine.UI;

public class UIHighlighter 
{
    private Image _image;
    public bool Active { get; private set; }

    public UIHighlighter(Canvas parent, float alpha)
    {
        _image = new GameObject("Highlighter").AddComponent<Image>();
        _image.transform.SetParent(parent.transform);

        var c = _image.color;
        c.a = alpha;
        _image.color = c;

        _image.gameObject.SetActive(false);        
    }

    public UIHighlighter(Canvas parent, float alpha, Sprite sprite) : this(parent, alpha)
        => _image.sprite = sprite;

    public void NewHighlight(Vector2 screenPos, Vector2 size)
    {
        if(!Active)
        {
            Active = true;
            _image.gameObject.SetActive(true);
        }
        _image.transform.position = screenPos;
        _image.rectTransform.sizeDelta = size;
    }

    public void Hide()
    {
        _image.gameObject.SetActive(false);
        Active = false;
    }
}