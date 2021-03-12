using System;


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

    public void Invoke(object sender, InventoryItemEventArgs args)
    {
        _lastSender = sender;
        _lastArgs = args;
        _handler?.Invoke(_lastSender, _lastArgs);
    }
}
