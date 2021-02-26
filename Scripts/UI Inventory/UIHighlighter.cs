using UnityEngine;
using UnityEngine.UI;

public class UIHighlighter 
{
    private Image _image;
    private float _alpha;
    public bool Active { get; private set; }
    

    public UIHighlighter(Canvas parent, float alpha)
    {
        _image = new GameObject("Highlighter").AddComponent<Image>();
        _image.transform.SetParent(parent.transform);
        _alpha = alpha;
        SetAlpha();
        _image.gameObject.SetActive(false);        
    }

    public UIHighlighter(Canvas parent, float alpha, Sprite sprite) : this(parent, alpha)
        => _image.sprite = sprite;

    public void NewHighlight(Vector2 screenPos, Vector2 size, bool red = false)
    {
        if(!Active)
        {
            Active = true;
            _image.gameObject.SetActive(true);
        }
        _image.color = !red ? Color.white : Color.red;
        SetAlpha();
        _image.transform.position = screenPos;
        _image.rectTransform.sizeDelta = size;
    }

    public void Hide()
    {
        _image.gameObject.SetActive(false);
        Active = false;
    }

    private void SetAlpha()
    {
        var c = _image.color;
        c.a = _alpha;
        _image.color = c;
    }
}