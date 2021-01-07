using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 缓存池中的一列数据
/// </summary>
public class PoolData
{
    public GameObject fatherGObj;       //父节点
    public List<GameObject> poolList;       //对象列表

    /// <summary>
    /// 创建一列缓存池数据
    /// </summary>
    /// <param name="gObj">此列中第一个游戏物体</param>
    /// <param name="poolGObj">缓存池对象</param>
    public PoolData(GameObject gObj, GameObject poolGObj)
    {
        fatherGObj = new GameObject(gObj.name);     //创建管理列表的父对象
        fatherGObj.transform.parent = poolGObj.transform;       //放入缓存池节点
        poolList = new List<GameObject>();
        PushGobj(gObj);
    }

    /// <summary>
    /// 压入此列
    /// </summary>
    /// <param name="gObj">被放入的对象</param>
    public void PushGobj(GameObject gObj)
    {
        poolList.Add(gObj);
        gObj.transform.parent = fatherGObj.transform;
        gObj.SetActive(false);
    }

    /// <summary>
    /// 从此列获取物体
    /// </summary>
    /// <returns></returns>
    public GameObject GetGObj()
    {
        GameObject gObj = null;
        gObj = poolList[0];
        poolList.RemoveAt(0);
        gObj.SetActive(true);
        gObj.transform.parent = null;
        return gObj;
    }
}


/// <summary>
/// 缓存池模块
/// </summary>
public class PoolManager : BaseManager<PoolManager>
{
    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    private GameObject poolObj;

    ResManager resManager = ResManager.GetInstance();

    /// <summary>
    /// 从缓存池取游戏物体
    /// </summary>
    /// <param name="name">物体名称</param>
    public void GetGObj(string name, UnityAction<GameObject> callback)
    {
        if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)
        {
            callback(poolDic[name].GetGObj());
        }
        else
        {
            resManager.LoadAsyn<GameObject>(name, (obj) => {
               obj.name = name;
               callback(obj);
           });
        }
    }

    /// <summary>
    /// 游戏物体放进缓存池
    /// </summary>
    /// <param name="name">物体名称</param>
    /// <param name="gObj">物体对象</param>
    public void PushGObj(string name, GameObject gObj)
    {
        if (poolObj == null)
        {
            poolObj = new GameObject("Pool");
        }
        gObj.transform.parent = poolObj.transform;      //放进缓存池节点
        gObj.SetActive(false);      //失活
        if (poolDic.ContainsKey(name))
        {
            poolDic[name].PushGobj(gObj);
        }
        else
        {
            PoolData data = new PoolData(gObj, poolObj);
            poolDic.Add(name, data);
        }
    }

    /// <summary>
    /// 清空缓存池，主要用在场景切换
    /// </summary>
    public void Clear()
    {
        poolDic.Clear();
        poolObj = null;
    }
}
