using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum UI_Layer
{
    Bottom,
    Middle,
    Top,
    System,
}

public class UIManager : BaseManager<UIManager>
{
    ResManager resManager = ResManager.GetInstance();

    private Transform bottomTF;
    private Transform middleTF;
    private Transform topTF;
    private Transform systemTF;

    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

    public UIManager()
    {
        GameObject canvas =  resManager.Load<GameObject>("UI/Canvas");
        canvas.name = "Canvas";
        Transform canvasTF = canvas.transform;
        GameObject.DontDestroyOnLoad(canvas);
        GameObject es = resManager.Load<GameObject>("UI/EventSystem");
        es.name = "EventSystem";
        GameObject.DontDestroyOnLoad(es);

        bottomTF = canvasTF.Find("Bottom");
        middleTF = canvasTF.Find("Middle");
        topTF = canvasTF.Find("Top");
        systemTF = es.transform;
    }

    /// <summary>
    /// 加载并显示面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <param name="name">面板名</param>
    /// <param name="layer">层级</param>
    /// <param name="callback">回调函数</param>
    public void ShowPanel<T>(string name, UI_Layer layer = UI_Layer.Bottom, UnityAction<T> callback = null) where T : BasePanel
    {
        if (panelDic.ContainsKey(name))
        {
            callback?.Invoke(panelDic[name] as T);
            return;
        }
        resManager.LoadAsyn<GameObject>("UI/" + name, (gObj) =>
        {
            Transform parent = bottomTF;
            switch (layer)
            {
                case UI_Layer.Middle:
                    parent = middleTF;
                    break;
                case UI_Layer.Top:
                    parent = topTF;
                    break;
                case UI_Layer.System:
                    parent = systemTF;
                    break;
            }
            gObj.transform.SetParent(parent);
            gObj.transform.localPosition = Vector3.zero;
            gObj.transform.localScale = Vector3.one;
            (gObj.transform as RectTransform).offsetMax = Vector3.zero;
            (gObj.transform as RectTransform).offsetMin = Vector3.zero;

            T panel = gObj.GetComponent<T>();
            callback?.Invoke(panel);
            panelDic.Add(name, panel);
        });
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <param name="name">面板名</param>
    public void HidePanel(string name)
    {
        if (!panelDic.ContainsKey(name)) return;
        GameObject.Destroy(panelDic[name].gameObject);
        panelDic.Remove(name);
    }
}
