using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZMonoSingleton<T> : MonoBehaviour where T : Component
{
    private static T _instance = null;
    public static T Instance
    {
        get
        {
            return _instance;
        }
    }
    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this as T;
    }
}
