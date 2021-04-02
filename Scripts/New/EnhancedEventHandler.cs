using System;
using System.Collections.Generic;
using UnityEngine;

namespace D2Inventory.Utils
{

    public class EnhancedEventHandler<T> 
    {
        public event EventHandler<T> Handler = delegate { };

        private object _lastSender;
        private T _lastArgs;
        
        public void AddWithInvoke(EventHandler<T> listener)
        {
            if(listener != null)
            {
                Handler += listener;
                if(_lastSender != null && _lastArgs != null)
                    listener.Invoke(_lastSender, _lastArgs);
            }
        }

        public void Invoke(object sender, T args)
            => Handler.Invoke(_lastSender = sender, _lastArgs = args);
    }
}
