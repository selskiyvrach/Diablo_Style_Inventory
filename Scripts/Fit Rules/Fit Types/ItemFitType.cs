using UnityEngine;


[CreateAssetMenu(menuName="Scriptable Objects/Inventory/New Equipment Fit Type")]
public class ItemFitType : ScriptableObject
{
    [SerializeField] string fitTypeName;
    public string FitTypeName => fitTypeName;
}
