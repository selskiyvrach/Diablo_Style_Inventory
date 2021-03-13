using System;
using System.Collections.Generic;
using System.Linq;

public static class InventoryEventsManager 
{

    ///<summary>
    /// Cash of all enhancedEventHandler fields in the class</summary>
    private static IEnumerable<EnhancedEventHandler> handlersCash;

    ///<summary>
    /// Accessor/setter of handlers cash</summary>
    private static IEnumerable<EnhancedEventHandler> handlers 
        => handlersCash ??= 
        from prop in typeof(InventoryEventsManager).GetProperties()
        let casted = prop.GetValue(null) as EnhancedEventHandler
        where casted != null
        select casted;

    public static void ClearAllEvents()
    {
        foreach(var i in handlers)
            i.RemoveAllListeners();
    }

// INVENTORY 

    ///<summary>
    /// Sender: the inventory <br/> Args - empty</summary>
    public static EnhancedEventHandler OnInventoryOpened { get; private set; } = new EnhancedEventHandler();

    ///<summary>
    /// Sender: the inventory <br/> Args - empty</summary>
    public static EnhancedEventHandler OnInventoryClosed { get; } = new EnhancedEventHandler();
    
// EQUIPMENT

    ///<summary>
    /// Sender: the inventory <br/> Args - ItemEquippedEventArgs: slot, inventoryItem</summary>
    public static EnhancedEventHandler OnItemEquipped { get; } = new EnhancedEventHandler();

    ///<summary>
    /// Sender: the inventory <br/> Args - ItemEquippedEventArgs: slot, inventoryItem</summary>
    public static EnhancedEventHandler OnItemUnequipped { get; } = new EnhancedEventHandler();

// MANIPULATIONS

    ///<summary>
    /// Sender: the inventory <br/> Args - ItemManipulatedEventArgs: InventoryItem, Manipulation.Dropped, screen space coords</summary>
    public static EnhancedEventHandler OnItemDroppedIntoWorld { get; } = new EnhancedEventHandler();

    ///<summary>
    /// Sender: the inventory <br/> Args - ItemManipulatedEventArgs: InventoryItem, Manipulation.Taken, screen space coords</summary>
    public static EnhancedEventHandler OnItemTakenByCursor { get; } = new EnhancedEventHandler();

    ///<summary>
    /// Sender: the inventory <br/> Args - ItemManipulatedEventArgs: InventoryItem, Manipulation.Dragged, screen space coords</summary>
    public static EnhancedEventHandler OnItemDraggedByCursor { get; } = new EnhancedEventHandler();

    ///<summary>
    /// Sender: the inventory <br/> Args - ItemManipulatedEventArgs: InventoryItem, Manipulation.Impossible, screen space coords</summary>
    public static EnhancedEventHandler OnImpossibleToProceed { get; } = new EnhancedEventHandler();

// HIGHLIGHT

    ///<summary> 
    /// Sender: the inventory <br/> Args - InventoryHighlightsEventArgs: rect in screen coords, hovered over item if present</summary>
    public static EnhancedEventHandler OnNewHighlight { get; } = new EnhancedEventHandler();

    ///<summary> 
    /// Sender: the inventory <br/> Args - empty</summary>
    public static EnhancedEventHandler OnHighlightOff { get; } = new EnhancedEventHandler();

}

