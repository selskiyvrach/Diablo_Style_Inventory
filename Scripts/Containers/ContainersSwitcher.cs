using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ContainersSwitcher : MonoBehaviour
{
    [SerializeField] Button switchButton;
    [SerializeField] SingleItemContainer[] firstOption;
    [SerializeField] SingleItemContainer[] secondOption;
    [SerializeField] Inventory inventory;

    public SingleItemContainer[] CurrSlots { get; private set; } = null;
    public bool Active { get; private set; } = false;

    private void Awake() 
    {
        Foreach(firstOption.Concat(secondOption).ToArray(), (SingleItemContainer s) => { s.SetContentVisualsActive(inventory.IsOn); s.SetActive(inventory.IsOn); });
        SetFirstOption();
        switchButton.onClick.AddListener(SwitchOptions);
    }

    private void OnDestroy() 
        => switchButton.onClick.RemoveListener(SwitchOptions);

    public void SwitchOptions()
    {
        if(CurrSlots == firstOption)
            SetSecondOption();
        else 
            SetFirstOption();
    }

    public void SetFirstOption()
        => SetNewOptionActive(firstOption);
    
    public void SetSecondOption()
        => SetNewOptionActive(secondOption);

    private void SetNewOptionActive(SingleItemContainer[] newOption)
    {
        if(newOption == CurrSlots) return;
        SetActiveCurrSlots(false);
        SetActiveCurrItems(false);
        CurrSlots = newOption;
        SetActiveCurrItems(true);
        if(inventory.IsOn)
            SetActiveCurrSlots(true);
        // inventory newWeaponSlots(GetActiveSlots, item[equipped], item[unequipped]);
    }

    private void SetActiveCurrSlots(bool value)
        => Foreach(CurrSlots, (SingleItemContainer s) => { s.SetActive(value);  } );

    private void SetActiveCurrItems(bool value)
        => Foreach(CurrSlots, (SingleItemContainer s) => s.SetContentVisualsActive(value)); 

    private void Foreach(SingleItemContainer[] items, Action<SingleItemContainer> toDo)
    {
        if(items == null || items.Length == 0) return;
        foreach(var i in items)
            if(i != null)
                toDo(i);
    }
}
