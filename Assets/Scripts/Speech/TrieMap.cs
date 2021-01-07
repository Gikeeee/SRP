using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TireMap
{
    public class TireNode
    {
        private bool m_isEnd = false;

        public bool IsEnd
        {
            get { return m_isEnd; }
            set { m_isEnd = value; }
        }
        public Dictionary<char, TireNode> NextNodes = new Dictionary<char, TireNode>();     //子节点们
    }

    private TireNode m_HeadNode = null;     //根节点

    public TireMap()
    {
        //初始化根节点
        m_HeadNode = new TireNode();
        m_HeadNode.IsEnd = false;
    }

    /// <summary>
    /// 添加关键词
    /// </summary>
    /// <param name="word"></param>
    public void AddWord(string word)
    {
        int idx = 0;
        //从根节点开始
        TireNode curNode = m_HeadNode;
        while (idx < word.Length)
        {
            char key = word.ElementAt(idx);
            //如果当前节点不包含这个字，则把这个字作为当前节点的子节点
            if (!curNode.NextNodes.ContainsKey(key))
            {
                curNode.NextNodes.Add(key, new TireNode());
            }
            //把这个字的节点作为当前节点
            curNode.NextNodes.TryGetValue(key, out curNode);
            idx++;
        }
        //最后一个节点设为末尾节点
        if (curNode != m_HeadNode)
        {
            curNode.IsEnd = true;
        }
    }
    
    /// <summary>
    /// 检测关键词
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public bool HasWord(string word)
    {
        int idx = 0;
        TireNode curNode = m_HeadNode;
        while (idx < word.Length)
        {
            char key = word.ElementAt(idx);
            //如果当前节点的所有子节点中没有此字直接跳出
            if (!curNode.NextNodes.ContainsKey(key))
            {
                curNode = null;
                break;
            }
            curNode.NextNodes.TryGetValue(key, out curNode);
            idx++;
        }
        return curNode != m_HeadNode && curNode != null && curNode.IsEnd;
    }


    /// <summary>
    ///   没有释放空间版的RemoveWord
    /// </summary>
    public void RemoveWord(string word)
    {

        int idx = 0;
        TireNode curNode = m_HeadNode;
        while (idx < word.Length)
        {
            char key = word.ElementAt(idx);
            if (!curNode.NextNodes.ContainsKey(key))
            {
                curNode = null;
                break;
            }
            curNode.NextNodes.TryGetValue(key, out curNode);
            idx++;
        }

        if (curNode != m_HeadNode && curNode != null && curNode.IsEnd)
        {
            curNode.IsEnd = false;
        }
    }
}
