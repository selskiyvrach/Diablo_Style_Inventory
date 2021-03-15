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

    private SingleItemContainer[] _currSlots;
    private InventoryItemVisuals _lockedSlotPlaceholder;

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
        if(_currSlots == firstOption)
            SetSecondOption();
        else 
            SetFirstOption();

        var unequipped = firstOption == _currSlots ? secondOption : firstOption;
        var equipped = firstOption == _currSlots ? firstOption : secondOption;
        
        inventory.SetUpAvailibleSlotsChanges(
            unequipped.Where(n => n.Content != null).ToArray(), 
            equipped.Where(n => n.Content != null).ToArray());
    }

    public SingleItemContainer[] GetCurrentSlots()
    {
        if(_currSlots == null)
            SetFirstOption();
        return _currSlots;
    }

    public void SetFirstOption()
        => SetNewOptionActive(firstOption);
    
    public void SetSecondOption()
        => SetNewOptionActive(secondOption);

    private void SetNewOptionActive(SingleItemContainer[] newOption)
    {
        if(newOption == _currSlots) return;
        var unequipped = _currSlots;
        
        SetActiveCurrSlots(false);
        SetActiveCurrItems(false);
        _currSlots = newOption;
        SetActiveCurrItems(true);
        if(inventory.IsOn)
            SetActiveCurrSlots(true);
    }

    private void SetActiveCurrSlots(bool value)
        => Foreach(_currSlots, (SingleItemContainer s) => { s.SetActive(value);  } );

    private void SetActiveCurrItems(bool value)
        => Foreach(_currSlots, (SingleItemContainer s) => s.SetContentVisualsActive(value)); 

    private void Foreach(SingleItemContainer[] items, Action<SingleItemContainer> toDo)
    {
        if(items == null || items.Length == 0) return;
        foreach(var i in items)
            if(i != null)
                toDo(i);
    }

}