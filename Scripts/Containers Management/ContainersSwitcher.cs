using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainersSwitcher : MonoBehaviour
{
    [SerializeField] ScreenSpaceItemContainer[] firstOption;
    [SerializeField] ScreenSpaceItemContainer[] secondOption;

    public ScreenSpaceItemContainer[] ActiveSlots { get; private set; }

    private void Awake()
        => SetActiveFirstOption();

    public void SetActiveFirstOption()
        => ActiveSlots = firstOption;
    
    public void SetActiveSecondOption()
        => ActiveSlots = secondOption;
}
