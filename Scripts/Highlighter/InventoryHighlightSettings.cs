using UnityEngine;



[CreateAssetMenu(menuName="Scriptable Objects/Inventory/New Inventory Highlight Settings")]
public class InventoryHighlightSettings : ScriptableObject
{
    [SerializeField] Color highlightColor;
    [SerializeField] Color cantPlaceHereColor;
    public Color HighlightColor => highlightColor;
    public Color CantPlaceHereColor => cantPlaceHereColor;
}
