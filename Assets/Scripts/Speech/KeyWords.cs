using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///// <summary>
///// 关键词检索名
///// </summary>
//public static class KeyWord
//{
//    public static string CHANGEROLE { get { return "CHANGEROLE"; } }
//    public static string SAD { get { return "SAD"; } }
//    public static string DANCE { get { return "DANCE"; } }
//}

/// <summary>
/// 关键词字典类
/// 提供关键词字典和搜寻关键词的方法，搜索到关键词触发事件
/// </summary>
public static class KeyWords
{
    //关键词字典
    public static Dictionary<string, List<string>> keyWordsDic = new Dictionary<string, List<string>>
    {
        { MyEventType.CHANGEROLE, new List<string>{ "换角色", "换个角色", "换个人", "换人", "换虚拟角色", "换装", "换装扮"} },
        { MyEventType.SAD, new List<string>{ "难过", "委屈" , "沮丧", "痛苦", "不开心", "不高兴"} },
        { MyEventType.DANCE, new List<string>{ "跳舞", "跳个舞" , "表演", "跳支舞"} },
        { MyEventType.HAPPY, new List<string>{ "开心", "高兴" , "心情好", "快乐"} },
        { MyEventType.KENAN, new List<string>{ "柯南剧情", "柯南的剧情", "柯南为什么变小"} },
        { MyEventType.MINGREN, new List<string>{ "鸣人", "漩涡鸣人"} },
    };

    public static bool DetectALLWords(string paragraph)
    {
        foreach (KeyValuePair<string, List<string>> words in keyWordsDic)
        {
            for (int i = 0; i < words.Value.Count; i++)
            {
                if(paragraph.Contains(words.Value[i]))
                {
                    EventCenter.GetInstance().EventTrigger(words.Key);
                    return true;
                }
            }
        }
        return false;
    }
}
