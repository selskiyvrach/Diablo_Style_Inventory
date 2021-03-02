using UnityEngine;
using UnityEngine.UI;

public class InventoryHighlighter : MonoBehaviour
{
    [SerializeField] InventoryHighlightSettings settings;    
    private Image _image;

    public bool Active { get; private set; }
    
// PUBLIC

    public void Initialize(Canvas parent)
    {
        _image = new GameObject("Highlighter").AddComponent<Image>();
        _image.transform.SetParent(transform);
        _image.gameObject.SetActive(Active = false);
    }

    public void NewHighlight(Vector2 screenPos, Vector2 size, bool red = false)
    {
        _image.color = !red ? settings.HighlightColor : settings.CantPlaceHereColor;
        _image.transform.position = screenPos;
        _image.rectTransform.sizeDelta = size;
        _image.gameObject.SetActive(Active = true);
    }

    public void HideHighlight()
        => _image.gameObject.SetActive(Active = false);

}