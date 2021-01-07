using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// 面板基类
/// </summary>
public class BasePanel : MonoBehaviour
{
    private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();

    private void Awake()
    {
        FindChildrenControls<Button>();
        FindChildrenControls<Image>();
        FindChildrenControls<Text>();
        FindChildrenControls<Toggle>();
        FindChildrenControls<Slider>();
        FindChildrenControls<ScrollRect>();
        Init();
    }
    
    /// <summary>
    /// 寻找子物体控件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void FindChildrenControls<T>() where T : UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>();
        string name;
        for (int i = 0; i < controls.Length; i++)
        {
            name = controls[i].gameObject.name;
            if (name.Contains("(Clone)"))
            {
                name = System.Text.RegularExpressions.Regex.Replace(name, @"\(Clone\)", "");
            }
            if (controlDic.ContainsKey(name))
            {
                controlDic[name].Add(controls[i]);
            }
            else
            {
                List<UIBehaviour> list = new List<UIBehaviour>() { controls[i] };
                controlDic.Add(name, list);
            }
        }
    }

    /// <summary>
    /// 获取UI控件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T GetControl<T>(string name) where T :UIBehaviour
    {
        if (controlDic.ContainsKey(name))
        {
            for (int i = 0; i < controlDic[name].Count; i++)
            {
                if (controlDic[name][i] is T)
                    return controlDic[name][i] as T;
            }
        }
        return null;
    }

    public void RemoveControl<T>(string name) where T : UIBehaviour
    {
        if (controlDic.ContainsKey(name))
        {
            for (int i = 0; i < controlDic[name].Count; i++)
            {
                if (controlDic[name][i] is T)
                {
                    Destroy(controlDic[name][i]);
                    controlDic[name].RemoveAt(i);
                    return;
                }
            }
        }
    }

    public virtual void Show()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public virtual void Init()
    {
        
    }
}
