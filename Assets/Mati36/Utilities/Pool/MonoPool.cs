using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoPool<T> where T : MonoPoolObject<T>
{
    private Stack<T> _pool;
    private Func<T> _factoryMethod;

    public MonoPool(int defaultCapacity, Func<T> factoryMethod)
    {
        _factoryMethod = factoryMethod;
        _pool = new Stack<T>();
        for (int i = 0; i < defaultCapacity; i++)
        {
            T newObj = factoryMethod();
            newObj.DeActivate();
            _pool.Push(newObj);
        };
    }

    public T Get()
    {
        T objToGet;
        if (_pool.Count <= 0)
            objToGet = _factoryMethod();
        else
            objToGet = _pool.Pop();


        objToGet.Activate();
        objToGet.e_OnFinalization += Return;
        return objToGet;
    }

    private void Return(T obj)
    {
        obj.e_OnFinalization -= Return;
        obj.DeActivate();
        _pool.Push(obj);
    }
}
