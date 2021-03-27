using UnityEngine;

[CreateAssetMenu(menuName="Scriptable Objects/Inventory/Fit Rule")]
public class ItemFitRule : ScriptableObject
{
    [SerializeField] bool all;
    [SerializeField] bool twoHanded;
    [SerializeField] ItemFitType fitType;

    public ItemFitType FitType => fitType;

    public bool TwoHanded => twoHanded;
    
    public bool CanFit(ItemFitRule ruleOfCandidate)
        => all ? true : ruleOfCandidate.FitType == FitType; 

    public virtual bool CanPair(ItemFitRule pairCandidate)
        => true;

}   
