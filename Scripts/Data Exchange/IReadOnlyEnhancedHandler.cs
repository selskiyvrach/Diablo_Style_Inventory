using System;

namespace MNS.Events
{

    public interface IReadOnlyEnhancedHandler<T>
    {

        void AddListener(EventHandler<T> listener); 

        void AddWithInvoke(EventHandler<T> listener); 

        void RemoveListener(EventHandler<T> listener);

    }
    
}