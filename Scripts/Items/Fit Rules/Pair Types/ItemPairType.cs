using UnityEngine;


[CreateAssetMenu(menuName="Scriptable Objects/Inventory/Pair Type")]
public class ItemPairType : ScriptableObject
{
    [SerializeField] string pairTypeName;
    [SerializeField] bool twoHanded;
    public string PairTypeName => pairTypeName;
    public bool TwoHanded => twoHanded; 
}
