using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IEventInfo { }

/// <summary>
/// 用于存委托方法（无指定类型）
/// </summary>
public class EventInfo : IEventInfo
{
    public UnityAction actions { get; set; }
    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}

/// <summary>
/// 用于存委托方法（指定类型）
/// </summary>
/// <typeparam name="T"></typeparam>
public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions { get; set; }
    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}

/// <summary>
/// 用于存委托方法（2指定类型）
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
public class EventInfo<T1, T2> : IEventInfo
{
    public UnityAction<T1, T2> actions { get; set; }
    public EventInfo(UnityAction<T1, T2> action)
    {
        actions += action;
    }
}

/// <summary>
/// 事件中心
/// </summary>
public class EventCenter : BaseManager<EventCenter>
{
    /// <summary>
    /// key：事件名，value：监听这个事件的委托函数们
    /// </summary>
    private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();

    /// <summary>
    /// 添加监听者（无指定类型参数）
    /// </summary>
    /// <param name="name">事件名</param>
    /// <param name="callback">回调函数</param>
    public void AddEventListener(string name, UnityAction callback)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions += callback;
        }
        else
        {
            EventInfo e = new EventInfo(callback);
            eventDic.Add(name, e);
        }
    }

    /// <summary>
    /// 添加监听者（指定类型参数）
    /// </summary>
    /// <param name="name">监听的事件名</param>
    /// <param name="callback">委托函数</param>
    public void AddEventListener<T>(string name, UnityAction<T> callback)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions += callback;
        }
        else
        {
            EventInfo<T> e = new EventInfo<T>(callback);
            eventDic.Add(name, e);
        }
    }

    /// <summary>
    /// 添加监听者（2指定类型参数）
    /// </summary>
    /// <param name="name">监听的事件名</param>
    /// <param name="callback">委托函数</param>
    public void AddEventListener<T1, T2>(string name, UnityAction<T1, T2> callback)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T1, T2>).actions += callback;
        }
        else
        {
            EventInfo<T1, T2> e = new EventInfo<T1, T2>(callback);
            eventDic.Add(name, e);
        }
    }

    /// <summary>
    /// 移除事件监听（无指定类型参数）
    /// </summary>
    /// <param name="name">事件名</param>
    /// <param name="anction">委托函数</param>
    public void RemoveEventListener(string name, UnityAction callback)
    {
        if (!eventDic.ContainsKey(name)) return;
        (eventDic[name] as EventInfo).actions -= callback;

        if ((eventDic[name] as EventInfo).actions == null)
            eventDic.Remove(name);
    }

    /// <summary>
    /// 移除事件监听（指定类型参数）
    /// </summary>
    /// <param name="name">事件名</param>
    /// <param name="anction">委托函数</param>
    public void RemoveEventListener<T>(string name, UnityAction<T> callback)
    {
        if (!eventDic.ContainsKey(name)) return;
        (eventDic[name] as EventInfo<T>).actions -= callback;

        if ((eventDic[name] as EventInfo<T>).actions == null)
            eventDic.Remove(name);
    }

    /// <summary>
    /// 移除事件监听（2指定类型参数）
    /// </summary>
    /// <param name="name">事件名</param>
    /// <param name="anction">委托函数</param>
    public void RemoveEventListener<T1, T2>(string name, UnityAction<T1, T2> callback)
    {
        if (!eventDic.ContainsKey(name)) return;
        (eventDic[name] as EventInfo<T1, T2>).actions -= callback;

        if ((eventDic[name] as EventInfo<T1, T2>).actions == null)
            eventDic.Remove(name);
    }

    /// <summary>
    /// 触发事件（无指定类型参数）
    /// </summary>
    /// <param name="name">触发的事件名</param>
    public void EventTrigger(string name)
    {
        if (!eventDic.ContainsKey(name)) return;
        (eventDic[name] as EventInfo).actions.Invoke();
    }

    /// <summary>
    /// 触发事件（指定类型参数）
    /// </summary>
    /// <param name="name">触发的事件名</param>
    public void EventTrigger<T>(string name, T info)
    {
        if (!eventDic.ContainsKey(name)) return;
        (eventDic[name] as EventInfo<T>).actions.Invoke(info);
    }

    /// <summary>
    /// 触发事件（2指定类型参数）
    /// </summary>
    /// <param name="name">触发的事件名</param>
    public void EventTrigger<T1, T2>(string name, T1 info, T2 info2)
    {
        if (!eventDic.ContainsKey(name)) return;
        (eventDic[name] as EventInfo<T1, T2>).actions.Invoke(info,info2);
    }

    /// <summary>
    /// 清空事件中心
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
}
