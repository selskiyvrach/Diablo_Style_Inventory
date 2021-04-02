using UnityEngine;
using UnityEngine.UI;

public class ScreenRect : MonoBehaviour
{
    [SerializeField] Image _panel;

    public Rect Rect => _panel.rectTransform.RectTransformToScreenSpace();

    public bool Active => _panel.isActiveAndEnabled;

    public void SetSizeDelta(Vector2 newSize)
        => _panel.rectTransform.sizeDelta = newSize;

    public bool ContainsPoint(Vector3 screenPos)
        => Rect.Contains(screenPos);

    public Vector2 ScreenToNormalized(Vector2 screenPos)
        => (screenPos - Rect.position) / Rect.size;

    public Vector2 NormalizedRectPointToScreen(Vector2 normalizedRectPoint)
    {
        float xPos = Rect.position.x + Rect.size.x * normalizedRectPoint.x;
        float yPos = Rect.position.y + Rect.size.y - Rect.size.y * normalizedRectPoint.y;
        Vector2 pos = new Vector2(xPos, yPos);
        return pos;
    }

    public Rect NormalizedRectToScreenRect(Rect normalized)
    {
        Vector2 size = Rect.size * normalized.size;
        float xPos = Rect.position.x + Rect.size.x * normalized.position.x;
        float yPos = Rect.position.y + Rect.size.y - size.y - Rect.size.y * normalized.position.y;
        Vector2 pos = new Vector2(xPos, yPos);
        return new Rect(pos, size);
    }

    public void SetActive(bool value)
        => _panel.gameObject.SetActive(value);
}