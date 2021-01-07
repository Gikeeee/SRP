using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单例模块
/// </summary>
/// <typeparam name="T">泛型</typeparam>
public class BaseManager<T> where T:new()
{
    private static T _instance;

    public static T GetInstance()
    {
        if (_instance == null)
        {
            _instance = new T();
        }
        return _instance;
    }
}