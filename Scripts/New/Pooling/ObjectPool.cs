using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T: Component
{
    private Stack<T> _pool = new Stack<T>();
    private T _sample;
    private Transform _poolParent;
    
    // TRACKERS
    private static int _poolsCreated = -1;

    public ObjectPool(T sample, int quantity, string specName = null)
    {
        // VALIDATION
        if(sample == null) { Debug.LogError("Cannot create pool of a null object"); return; }
        try 
        { 
            // this call will throw an exception itself when trying to access gameObject field
            // of a MonoBehaviour that was created via constructor. if gO is null - throw too for double-check
            if(((Component)sample).gameObject == null)
                throw new Exception();
        }
        catch (Exception) 
        { 
            Debug.LogError("You shouldn't create MonobeHaviour subtypes via contructor! Use gameObject.AddComponent instead");
            return;
        }
        quantity = Mathf.Max(quantity, 0); 
        // ENDVAL

        string number = _poolsCreated++ == 0 ? "" : $"({_poolsCreated++})";
        string specialName = specName == null ? $"of {sample.GetType().ToString()}'s" : specName;
        _poolParent = new GameObject($"Pool {number} {specialName}").transform;
        _poolParent.SetParent(PoolingMasterObject.PoolingParent.transform);
        _sample = GetNewItem(sample);
        Prewarm(quantity);
    }

    public T Pop()
    {
        if(_pool.Count == 0)
            Prewarm(1);
        var i = _pool.Pop();
        i.transform.SetParent(null);
        i.gameObject.SetActive(true);
        return i;
    }

    public void Prewarm(int quantity)
    {
        for(int i = 0; i < quantity; i++)
            GetNewItem(_sample);
    }

    public void ReturnItem(T item)
        => PutIntoPool(item);

    public void ClearPool()
    {
        foreach(var i in _pool)
            GameObject.Destroy(i.gameObject);
        _pool.Clear();
    }

    public void DestroyPool()
    {
        ClearPool();
        GameObject.Destroy(_poolParent);
    }

    private T GetNewItem(T sample)
    {
        var i = GameObject.Instantiate(sample);
        PutIntoPool(i);
        return i;
    }

    private void PutIntoPool(T item)
    {
        item.transform.SetParent(_poolParent.transform);
        item.transform.position = _poolParent.transform.position;
        SetUpPoolItem(item);
        item.gameObject.SetActive(false);
        _pool.Push(item);
    }

    private void SetUpPoolItem(T item)
    {
        var pI = item.GetComponent<PoolItem>();
        pI ??= item.gameObject.AddComponent<PoolItem>();
        pI.ResetEvent();
        pI.OnReturnRequested += () => ReturnItem(item);
    }
}