using System;



public static class InventoryEventsManager 
{

// INVENTORY 

    public static EnhancedEventHandler OnInventoryOpened = new EnhancedEventHandler();

    public static EnhancedEventHandler OnInventoryClosed = new EnhancedEventHandler();
    
// ITEMS

    public static EnhancedEventHandler OnItemEquipped = new EnhancedEventHandler();

    public static EnhancedEventHandler OnItemUnequipped = new EnhancedEventHandler();

    public static EnhancedEventHandler OnItemDroppedIntoWorld = new EnhancedEventHandler();

    public static EnhancedEventHandler OnItemTakenByCursor = new EnhancedEventHandler();

    public static EnhancedEventHandler OnItemReleasedByCursor = new EnhancedEventHandler();

    public static EnhancedEventHandler OnItemPutInStorage = new EnhancedEventHandler();

    public static EnhancedEventHandler OnImpossibleToProceed = new EnhancedEventHandler();

// HIGHLIGHT

    public static EnhancedEventHandler OnNewHighlight = new EnhancedEventHandler();

    public static EnhancedEventHandler OnHighlightOff = new EnhancedEventHandler();

}

public class EnhancedEventHandler
{
    private EventHandler _handler;
    private object _lastSender;
    private EventArgs _lastArgs;

    public void AddListener(EventHandler listener, bool getLastUpdate)
    {
        _handler += listener;
        if(getLastUpdate)
            listener?.Invoke(_lastSender, _lastArgs);
    }

    public void RemoveListener(EventHandler listener)
        => _handler -= listener;

    public void RemoveAllListeners()
        => _handler = null;

    public void ClearLastSenderAndArgs()
        => _lastSender = _lastArgs = null;

    public void Invoke(object sender, EventArgs args)
    {
        _lastSender = sender;
        _lastArgs = args;
        _handler?.Invoke(_lastSender, _lastArgs);
    }
}

