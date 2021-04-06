using UnityEngine;
using UnityEngine.UI;

public static class Factory 
{

    public static T GetGameObjectOfType<T>(string Name = null) where T : Component
        => new GameObject(Name ??= typeof(T).ToString()).AddComponent(typeof(T)) as T;
    
}
