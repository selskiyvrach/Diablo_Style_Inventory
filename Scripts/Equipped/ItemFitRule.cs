using UnityEngine;

[CreateAssetMenu(menuName="Scriptable Objects/Inventory/New Equipment Fit Rule")]
public class ItemFitRule : ScriptableObject
{
    [SerializeField] bool all;
    [SerializeField] EquipmentFitType fitType;

    public EquipmentFitType FitType => fitType;
    
    public bool CanFit(ItemFitRule ruleOfCandidate)
        => all ? true : ruleOfCandidate.FitType == FitType; 
}
