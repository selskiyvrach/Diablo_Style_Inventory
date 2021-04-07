using System;

namespace MNS.Events
{

    public class EnhancedEventHandler<T> : IReadOnlyEnhancedHandler<T>
    {
        
        private EventHandler<T> _handler = delegate { };

        private object _lastSender;
        private T _lastArgs;
        
        public void AddListener(EventHandler<T> listener)
            => _handler += listener;

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
