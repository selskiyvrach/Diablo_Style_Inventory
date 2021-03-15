using UnityEngine;

[CreateAssetMenu(menuName="Scriptable Objects/Inventory/Fit And Pair Rule")]
public class ItemFitAndPairRule : ItemFitRule
{
    [SerializeField] ItemPairType pairType;
    [SerializeField] ItemPairType[] allowed;

    public ItemPairType PairType => pairType;

    public override bool CanPair(ItemFitRule potentialPair)
    {
        var p = potentialPair as ItemFitAndPairRule;
        if(p == null)
        {
            Debug.LogError("Cannot place item to paired slot if item has no ItemFitAndPairRule attached");
            return false;
        }

        foreach(var i in allowed)
            if(i == p.PairType)
                return true;
        return false;
    }
}
