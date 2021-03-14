using System;
using System.Collections.Generic;
using System.Linq;

public static class InventoryEventsManager 
{

// UTILITY STUFF (REFLECTION)

    ///<summary>
    /// Cash of all enhancedEventHandler properties in the class</summary>
    private static IEnumerable<EnhancedEventHandler> _handlersCash;

    public static void ClearAllEvents()
    {
        _handlersCash ??= 
        from prop in typeof(InventoryEventsManager).GetProperties()
        let casted = prop.GetValue(null) as EnhancedEventHandler
        where casted != null
        select casted;

        foreach(var i in _handlersCash)
            i.RemoveAllListeners();
    }

// INVENTORY 

    ///<summary>
    /// Sender: the inventory <br/> Args: empty</summary>
    public static EnhancedEventHandler OnInventoryOpened { get; private set; } = new EnhancedEventHandler();

    ///<summary>
    /// Sender: the inventory <br/> Args: empty</summary>
    public static EnhancedEventHandler OnInventoryClosed { get; } = new EnhancedEventHandler();
    
// EQUIPMENT

    ///<summary>
    /// Sender: the inventory <br/> Args: InventoryItemEventArgs (item, container, screenPos, message)</summary>
    public static EnhancedEventHandler OnItemEquipped { get; } = new EnhancedEventHandler();

    ///<summary>
    /// Sender: the inventory <br/> Args: InventoryItemEventArgs (item, container, screenPos, message)</summary>
    public static EnhancedEventHandler OnItemUnequipped { get; } = new EnhancedEventHandler();

// MANIPULATIONS

    ///<summary>
    /// Sender: the inventory <br/> Args: InventoryItemEventArgs (item, container, screenPos, message)</summary>
    public static EnhancedEventHandler OnItemDroppedIntoWorld { get; } = new EnhancedEventHandler();

    ///<summary>
    /// Sender: the inventory <br/> Args: InventoryItemEventArgs (item, container, screenPos, message)</summary>
    public static EnhancedEventHandler OnItemTakenByCursor { get; } = new EnhancedEventHandler();

    ///<summary>
    /// Sender: the inventory <br/> Args: InventoryItemEventArgs (item, container, screenPos, message)</summary>
    public static EnhancedEventHandler OnImpossibleToProceed { get; } = new EnhancedEventHandler();

// HIGHLIGHT

    ///<summary> 
    /// Sender: the inventory <br/> Args: InventoryItemEventArgs (item, container, screenPos, message)</summary>
    public static EnhancedEventHandler OnNewHighlight { get; } = new EnhancedEventHandler();

    ///<summary> 
    /// Sender: the inventory <br/> Args: empty</summary>
    public static EnhancedEventHandler OnHighlightOff { get; } = new EnhancedEventHandler();

}

