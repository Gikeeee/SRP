using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : BaseManager<InputManager>
{
    EventCenter eventCenter = EventCenter.GetInstance();
    MonoManager monoManager = MonoManager.GetInstance();

    private int listenerCount = 0;
    public InputManager()
    {
        monoManager.AddUpdateListener(Update);
    }

    private void Update()
    {
        if (listenerCount <= 0) return;
        //CheckKeyCodeDownAndUp(KeyCode.W);
    }

    /// <summary>
    /// 检测按键是否按下和抬起
    /// </summary>
    /// <param name="keyCode"></param>
    private void CheckKeyCodeDownAndUp(KeyCode keyCode)
    {
        if (Input.GetKeyDown(keyCode))
        {
            //eventCenter.EventTrigger("", keyCode);
        }
        if (Input.GetKeyUp(keyCode))
        {
            //eventCenter.EventTrigger("", keyCode);
        }
    }

    /// <summary>
    /// 增加对Input的监听
    /// </summary>
    /// <param name="isOpen"></param>
    public void AddInputListener()
    {
        listenerCount++;
    }

    /// <summary>
    /// 减少对Input的监听
    /// </summary>
    public void RemoveInputListener()
    {
        listenerCount--;
        if (listenerCount < 0) listenerCount = 0;
    }
}
