using System;
using System.Linq;
using UnityEngine;

public class ContainersSwitcher : MonoBehaviour
{
    [SerializeField] SingleItemContainer[] firstOption;
    [SerializeField] SingleItemContainer[] secondOption;
    [SerializeField] Inventory inventory;

    public SingleItemContainer[] ActiveSlots { get; private set; } = null;

    private void Start() 
    {
        Foreach(firstOption.Concat(secondOption).ToArray(), (SingleItemContainer s) => { s.DisableContentsVisuals(); s.SetActive(false); });
        SetActiveFirstOption();
    }

    public void SetActiveFirstOption()
        => SetNewOptionActive(firstOption);
    
    public void SetActiveSecondOption()
        => SetNewOptionActive(secondOption);

    private void SetNewOptionActive(SingleItemContainer[] newOption)
    {
        if(!inventory.IsOn)
        {
            if(ActiveSlots != null)
            {
                Foreach(ActiveSlots, (SingleItemContainer s) => { s.DisableContentsVisuals(); s.SetActive(false); } );
                ActiveSlots = null;
                return;
            }
        }
        if(newOption == ActiveSlots) return;

        Foreach(ActiveSlots, (SingleItemContainer s) => { s.DisableContentsVisuals(); s.SetActive(false); } );
        ActiveSlots = newOption;
        Foreach(ActiveSlots, (SingleItemContainer s) => { s.EnableContentsVisuals();  s.SetActive(true);  } );
    }

    private void Foreach(SingleItemContainer[] items, Action<SingleItemContainer> toDo)
    {
        if(items == null || items.Length == 0) return;
        foreach(var i in items)
            toDo(i);
    }
}
