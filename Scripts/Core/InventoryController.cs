using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    ///<summary>
    /// Default controls: <br/>
    /// CursorPos - Input.MousePos <br/>
    /// Open/Close - 'I' <br/>
    /// Switch Weapons - 'W' <br/>
    /// Interact with cursor - Input.GetMouseButtton(0)
    ///</summary>
    [Tooltip("Default controls: CursorPos - Input.MousePos; Open/Close - 'I'; Switch Weapons - 'W'; Interact with cursor - Input.GetMouseButtton(0)")]
    [SerializeField] Control control;
    [SerializeField] ContainersSwitcher[] switchers;

    private void Start() 
    {
        if(control == Control.Default)
            inventory.SetInventoryActive(false);
    }

    private void Update() 
    {
        if(control != Control.Default) return;

        if(inventory.IsOn)
            ExternalUpdate();

        SetCursorPos(Input.mousePosition);

        if(Input.GetKeyDown(KeyCode.I))
            SwitchState();

        else if(Input.GetKeyDown(KeyCode.W))
            SwitchWeapons();
        
        else if(Input.GetKeyDown(KeyCode.Mouse0))
            PerformPrimaryInteraction();
    }

    public void ExternalUpdate()
        =>inventory.ExternalUpdate();

    public void AddItem(InventoryItemData data)
        => inventory.AddItemAuto(data);

    public void SwitchWeapons()
    {
        foreach(var s in switchers)
            s.SwitchOptions();
    }

    public void SetCursorPos(Vector2 screenPos)
        => inventory.SetCursorPos(screenPos);

    public void PerformPrimaryInteraction()
        => inventory.PerformPrimaryInteraction();

    public void SwitchState()
    {
        if(inventory.IsOn)
            Close();
        else 
            Open();
    }

    public void Open()
        => inventory.SetInventoryActive(true);
    
    public void Close()
        => inventory.SetInventoryActive(false);
}

public enum Control
{
    Default,
    Custom
}
