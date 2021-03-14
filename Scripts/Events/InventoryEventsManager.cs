using System;
using System.Collections.Generic;
using System.Linq;

public static class InventoryEventsManager 
{

// UTILITY STUFF (REFLECTION)

    ///<summary>
    /// Cash of all enhancedEventHandler properties in the class</summary>
    private static IEnumerable<EnhancedEventHandler> _handlersCash;

    private static void CashAllHandlers()
        => _handlersCash ??= 
        from prop in typeof(InventoryEventsManager).GetProperties()
        let casted = prop.GetValue(null) as EnhancedEventHandler
        where casted != null
        select casted;

    private static void ForeachHandler(Action<EnhancedEventHandler> toApply)
    {
        CashAllHandlers();
        foreach(var i in _handlersCash)
            toApply(i);
    }

    public static void ClearAllHandlers()
        => ForeachHandler((EnhancedEventHandler e) => e.RemoveAllListeners());

    public static void SubscribeToAll(EventHandler listener, bool getInstantlyLastUpdate)
        => ForeachHandler((EnhancedEventHandler e) => e.AddListener(listener, getInstantlyLastUpdate));

    public static void UnsubscribeFromAll(EventHandler listener)
        => ForeachHandler((EnhancedEventHandler e) => e.RemoveListener(listener));

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

