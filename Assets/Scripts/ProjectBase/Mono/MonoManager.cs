using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 用来管理MonoController，无需再从场景中获取MonoController
/// </summary>
public class MonoManager : BaseManager<MonoManager>
{
    private MonoController monoController;

    public MonoManager()
    {
        GameObject gObj = new GameObject("MonoController");
        monoController = gObj.AddComponent<MonoController>();
    }

    /// <summary>
    /// 添加监听帧更新事件函数
    /// </summary>
    /// <param name="fun">委托函数</param>
    public void AddUpdateListener(UnityAction fun)
    {
        monoController.AddUpdateListener(fun);
    }

    /// <summary>
    /// 移除监听函数
    /// </summary>
    /// <param name="fun">委托函数</param>
    public void RemoveUpdateListener(UnityAction fun)
    {
        monoController.RemoveUpdateListener(fun);
    }

    public Coroutine StartCoroutine(string methodName)
    {
        return monoController.StartCoroutine(methodName);
    }
    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return monoController.StartCoroutine(routine);
    }
    //public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
    //{
    //    return monoController.StartCoroutine(methodName, value);
    //}
    public Coroutine StartCoroutine_Auto(IEnumerator routine)
    {
        return monoController.StartCoroutine(routine);
    }
    public void StopAllCoroutines()
    {
        monoController.StopAllCoroutines();
    }
    public void StopCoroutine(IEnumerator routine)
    {
        monoController.StopCoroutine(routine);
    }
    public void StopCoroutine(Coroutine routine)
    {
        monoController.StopCoroutine(routine);
    }
    public void StopCoroutine(string methodName)
    {
        monoController.StopCoroutine(methodName);
    }
}
