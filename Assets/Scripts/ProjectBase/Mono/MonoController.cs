using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Mono管理者
/// </summary>
public class MonoController : MonoBehaviour
{
    private event UnityAction updateEvent;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if(updateEvent!=null)
        {
            updateEvent.Invoke();
        }
    }

    /// <summary>
    /// 添加监听帧更新事件函数
    /// </summary>
    /// <param name="fun">委托函数</param>
    public void AddUpdateListener(UnityAction fun)
    {
        updateEvent += fun;
    }

    /// <summary>
    /// 移除监听函数
    /// </summary>
    /// <param name="fun">委托函数</param>
    public void RemoveUpdateListener(UnityAction fun)
    {
        updateEvent -= fun;
    }
}
