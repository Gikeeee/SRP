using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyEventType
{
    public static string GAMESTART { get { return "GAMESTART"; } }
    public static string LOADINGSCENE_UPDATE { get { return "LoadingSceneUpdate"; } }
    //public static string PLAYERLIFE_UPDATE { get { return "PlayerLifeUpdate"; } }
    //public static string PLAYER_DEAD { get { return "PlayerDead"; } }

    #region 语音触发事件
    //语音触发
    public static string CHANGEROLE { get { return "CHANGEROLE"; } }
    public static string HELLOW { get { return "HELLOW"; } }
    public static string SAD { get { return "SAD"; } }
    public static string SAY { get { return "SAY"; } }
    public static string KENAN { get { return "KENAN"; } }
    public static string MINGREN { get { return "MINGREN"; } }
    public static string HAPPY { get { return "HAPPY"; } }
    public static string DANCE { get { return "DANCE"; } }
    //语音结束或取消事件
    public static string UNSAY { get { return "UNSAY"; } }
    public static string UNSAD { get { return "UNSAD"; } }
    public static string UNHAPPY { get { return "UNHAPPY"; } }
    public static string UNDANCE { get { return "UNDANCE"; } }
    #endregion

    #region 手势触发事件
    //手势触发动画
    public static string YEAHGESTURE { get { return "YEAHGESTURE"; } }
    public static string WAVEGESTURE { get { return "WAVEGESTURE"; } }
    public static string ROTATEGESTURE { get { return "ROTATEGESTURE"; } }
    public static string BEATENGESTURE { get { return "BEATENGESTURE"; } }
    //手势出发环境改变
    public static string ZOOMIN { get { return "ZOOMIN"; } }
    public static string ZOOMOUT { get { return "ZOOMOUT"; } }
    //手势结束或取消
    public static string UNYEAHGESTURE { get { return "UNYEAHGESTURE"; } }
    public static string UNWAVEGESTURE { get { return "UNWAVEGESTURE"; } }
    public static string UNBEATENGESTURE { get { return "UNBEATENGESTURE"; } }
    #endregion
}
