using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class MonoPoolObject<T> : MonoBehaviour
{
    public event Action<T> e_OnFinalization = delegate { };

    abstract public void Activate();
    abstract public void DeActivate();

    protected void ReturnToPool(T obj)
    {
        e_OnFinalization(obj);
    }
}
