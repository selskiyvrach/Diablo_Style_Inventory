using UnityEngine;


[CreateAssetMenu(menuName="Scriptable Objects/Inventory/New Item Pair Type")]
public class ItemPairType : ScriptableObject
{
    [SerializeField] string pairTypeName;
    public string PairTypeName => pairTypeName;
}
