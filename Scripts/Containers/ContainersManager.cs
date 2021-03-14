using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContainersManager : MonoBehaviour
{
    [SerializeField] CelledSpaceItemContainer mainStorage;
    [SerializeField] SingleItemContainer[] equipmentSlots;
    [SerializeField] ContainersSwitcher[] switchableSlots;

    // all without nulls 
    // once checked and cashed

    public ScreenSpaceItemContainer GetMainStorage()
    {
        NotNull(mainStorage);
        return mainStorage;
    }

    public ScreenSpaceItemContainer[] GetAllActiveContainers()
        => (GetActiveEquipmentSlots().Concat(new ScreenSpaceItemContainer[] { GetMainStorage() })).ToArray();
    
    public ScreenSpaceItemContainer[] GetActiveEquipmentSlots()
    {
        var slots = equipmentSlots.Where(NotNull); 
        var activeSwitches = switchableSlots.Where(NotNull).SelectMany(n => n.CurrSlots).Where(NotNull);
        return slots.Concat(activeSwitches).ToArray();
    }

    private bool NotNull(Component c)
    {
        if(c == null)
            Debug.LogError($"{this.GetType()} on {gameObject.name} contains null SerializedFields or nested null SerializedFields");
        return c != null;
    }
}
