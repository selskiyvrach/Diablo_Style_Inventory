using UnityEngine;

[CreateAssetMenu(menuName="Scriptable Objects/Inventory/New Equipment Paired Fit Type")]
public class EquipmentPairedFitType : EquipmentFitType
{
    [SerializeField] EquipmentPairedFitType[] allowed;
    public EquipmentPairedFitType[] Allowed => allowed;
}
