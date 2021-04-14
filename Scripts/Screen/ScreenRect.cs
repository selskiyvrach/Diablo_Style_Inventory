using MNS.Utils;
using UnityEngine;
using UnityEngine.UI;

public class ScreenRect : MonoBehaviour
{
    [SerializeField] Image _panel;

    public bool Active { get; private set; }
    
    public Rect Rect { get; private set; }

    public Transform Transform { get; private set; }

    private void Awake() 
    {
        Rect = _panel.rectTransform.ScreenRectFromRectTransform();
        Active = _panel.gameObject.activeInHierarchy;
        Transform = _panel.transform;
    }

    public void SetSizeDelta(Vector2 newSize)
        => _panel.rectTransform.sizeDelta = newSize;

    public void SetActive(bool value)
        => _panel.gameObject.SetActive(Active = value);

    public bool ContainsPoint(Vector2 screenPoint)
        => Rect.Contains(screenPoint);

    public Vector2 ScreenPointToNormalized(Vector2 screenPos)
        => Rect.ScreenPointToNormalized(screenPos);

    public Rect NormalizedRectToScreenRect(Rect normalized)
        => Rect.NormalizedRectToScreenRect(normalized);

    public Vector2 NormalizedRectPointToScreen(Vector2 normalizedRectPoint)
        => Rect.NormalizedRectPointToScreen(normalizedRectPoint);

}
