using UnityEngine;


[CreateAssetMenu(menuName="Scriptable Objects/Inventory/Inventory Item Data")]
public class InventoryItemData : ScriptableObject
{
    [SerializeField] string itemName;
    [SerializeField] Sprite sprite;
    [SerializeField] ItemFitRule fitRule;
    [SerializeField] Vector2IntSpaceData size;
    [SerializeField] [Range(0.33f, 2f)] float imageScale = 1f;

    public string Name => itemName;
    public Sprite Sprite => sprite; 
    public ItemFitRule FitRule => fitRule;
    public Vector2Int SizeInt => size.SizeInt;
    public float ImageScale => imageScale;
}