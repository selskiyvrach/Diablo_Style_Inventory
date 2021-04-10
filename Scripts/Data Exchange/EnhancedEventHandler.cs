using System;

namespace MNS.Events
{

    ///<summary>
    ///Stores last sender and args and allows to subcribe with instant call with previous call parameters</summary>
    public class EnhancedEventHandler<T> : IReadOnlyEnhancedHandler<T>
    {
        // inittialized as an empty delegate so no null checks will be needed
        private EventHandler<T> _handler = delegate { };

        // stored last call parameters
        private object _lastSender;
        private T _lastArgs;
        
        public void AddListener(EventHandler<T> listener)
            => _handler += listener;

        ///<summary>
        ///Adds listener and instantly invokes it with the parameters from the last call of the event</summary>
        public void AddWithInvoke(EventHandler<T> listener)
        {
            if(listener != null)
            {
                _handler += listener;
                if(_lastArgs != null && _lastSender != null)
                    listener.Invoke(_lastSender, _lastArgs);
            }
        }

        public void RemoveListener(EventHandler<T> listener)
            => _handler -= listener;

        public void RemoveAllListeners()
            => _handler = null;

        public void Invoke(object sender, T args)
            => _handler.Invoke(_lastSender = sender, _lastArgs = args);
            
    }
}
