using UnityEngine;



[CreateAssetMenu(menuName="Scriptable Objects/Inventory/New Inventory Settings")]
public class InventorySettings : ScriptableObject
{
    [SerializeField] Color highlightColor;
    [SerializeField] Color cantPlaceHereColor;
    public Color HighlightColor => highlightColor;
    public Color CantPlaceHereColor => cantPlaceHereColor;
}
