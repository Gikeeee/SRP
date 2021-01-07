using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResManager : BaseManager<ResManager>
{
    MonoManager monoManager = MonoManager.GetInstance();

    /// <summary>
    /// 同步加载资源
    /// </summary>
    /// <typeparam name="T">泛型类名</typeparam>
    /// <param name="name">资源路径</param>
    /// <returns></returns>
    public T Load<T>(string name) where T : Object
    {
        T res = Resources.Load<T>(name);
        if (res is GameObject)
        {
            return GameObject.Instantiate(res);
        }
        return res;
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    public void LoadAsyn<T>(string name, UnityAction<T> callback) where T : Object
    {
        monoManager.StartCoroutine(ReallyLoadAsyn<T>(name, callback));
    }

    private IEnumerator ReallyLoadAsyn<T>(string name, UnityAction<T> callback) where T : Object
    {
        ResourceRequest rR = Resources.LoadAsync<T>(name);
        yield return rR;

        if (rR.asset is GameObject)
        {
            callback.Invoke(GameObject.Instantiate(rR.asset) as T);
        }
        else
        {
            callback.Invoke(rR.asset as T);
        }
    }
}
