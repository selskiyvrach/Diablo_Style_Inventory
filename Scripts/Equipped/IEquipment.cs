
public interface IEquipment 
{
    EquipmentFitType FitType { get; }
    void OnEquip();
    void OnUnequip();
}
