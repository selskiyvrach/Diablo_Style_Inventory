using System;

namespace MNS.Events
{
    ///<summary>
    ///Exposes EnhancedEventHandler with subscribe/unsubcsribe functional only</summary>
    public interface IReadOnlyEnhancedHandler<T>
    {
        object LastSender { get; }

        T LastArgs { get; } 

        void AddListener(EventHandler<T> listener); 

        void AddWithInvoke(EventHandler<T> listener); 

        void RemoveListener(EventHandler<T> listener);

    }
    
}