using UnityEngine;

[CreateAssetMenu(menuName="Scriptable Objects/Inventory/New Equipment Fit Rule")]
public class ItemFitRule : ScriptableObject
{
    [SerializeField] bool all;
    [SerializeField] ItemFitType fitType;

    public ItemFitType FitType => fitType;
    
    public bool CanFit(ItemFitRule ruleOfCandidate)
        => all ? true : ruleOfCandidate.FitType == FitType; 

    public virtual bool CanPair(ItemFitRule pairCandidate)
        => true;

}   
