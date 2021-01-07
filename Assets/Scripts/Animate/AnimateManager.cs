using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationStateName
{
    public static string IDLE { get { return "Action Layer.Idle"; } }
    public static string CRY { get { return "Action Layer.Cry"; } }
    public static string YEAL { get { return "Action Layer.Yeah"; } }
    public static string HI { get { return "Action Layer.Hi"; } }
    public static string JUMP { get { return "Action Layer.Jump"; } }
    public static string HAPPY { get { return "Action Layer.Happy"; } }
    public static string SAY { get { return "Action Layer.Say"; } }
}

/// <summary>
/// 动画管理类
/// 其他类通过此单例控制角色动画
/// </summary>
public class AnimateManager : MonoBehaviour
{
    public static AnimateManager Instance;

    private Animator animator;

    private float rotateSpeed = 10f;        //角色旋转速度

    public string curRoleName = "Naoko";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        //播放背景音乐
        AudioManager.GetInstance().PlayBgMusic("BgMusic1", () =>
        {
            AudioManager.GetInstance().SetBgVolume(0.2f);
        });
        string path = "Role/" + curRoleName;
        PoolManager.GetInstance().GetGObj(path, (newRole) =>
        {
            animator = newRole.GetComponent<Animator>();
            EventCenter.GetInstance().EventTrigger(MyEventType.GAMESTART);
        });
        //注册启动动画事件
        //来自手势
        EventCenter.GetInstance().AddEventListener(MyEventType.YEAHGESTURE, DoYeah);
        EventCenter.GetInstance().AddEventListener<float>(MyEventType.ROTATEGESTURE, RotateRole);
        EventCenter.GetInstance().AddEventListener(MyEventType.WAVEGESTURE, DoHi);
        EventCenter.GetInstance().AddEventListener(MyEventType.BEATENGESTURE, DoBeaten);
        //来自语音
        EventCenter.GetInstance().AddEventListener(MyEventType.SAD, DoCry);
        EventCenter.GetInstance().AddEventListener(MyEventType.DANCE, DoJump);
        EventCenter.GetInstance().AddEventListener(MyEventType.HAPPY, DoHappy);
        EventCenter.GetInstance().AddEventListener(MyEventType.SAY, DoSay);
        EventCenter.GetInstance().AddEventListener(MyEventType.CHANGEROLE, ChangeRole);
       
        //注册结束动画，回归状态事件
        EventCenter.GetInstance().AddEventListener(MyEventType.UNYEAHGESTURE, UnDoYeah);
        EventCenter.GetInstance().AddEventListener(MyEventType.UNWAVEGESTURE, UnDoHi);
        EventCenter.GetInstance().AddEventListener(MyEventType.UNBEATENGESTURE, UndoBeaten);
        EventCenter.GetInstance().AddEventListener(MyEventType.UNSAD, UnDoCry);
        EventCenter.GetInstance().AddEventListener(MyEventType.UNDANCE, UnDoJump);
        EventCenter.GetInstance().AddEventListener(MyEventType.UNHAPPY, UnDoHappy);
        EventCenter.GetInstance().AddEventListener(MyEventType.UNSAY, UnDoSay);
        
    }

    /// <summary>
    /// 设置动画控制器
    /// </summary>
    /// <param name="ani"></param>
    public void SetAnimator(Animator ani)
    {
        animator = ani;
    }

    /// <summary>
    /// 检测是否有动画控制器
    /// </summary>
    /// <returns></returns>
    public bool HaveAnimator()
    {
        bool b = animator != null;
        if (!b)
            Debug.Log("没有找到控制器！");
        return b;
    }

    #region 动画信息设置获取等
    /// <summary>
    /// 获取当前动画状态
    /// </summary>
    /// <returns></returns>
    public AnimatorStateInfo GetStateInfo()
    {
        return animator.GetCurrentAnimatorStateInfo(0);
    }

    public bool SetAnimatorParam(string name)
    {
        if (!HaveAnimator())
            return false;
        animator.SetTrigger(name);
        return true;
    }

    public bool SetAnimatorParam(string name, int value)
    {
        if (!HaveAnimator())
            return false;
        animator.SetInteger(name, value);
        return true;
    }

    public bool SetAnimatorParam(string name, float value)
    {
        if (!HaveAnimator())
            return false;
        animator.SetFloat(name, value);
        return true;
    }

    public bool SetAnimatorParam(string name, bool value)
    {
        if (!HaveAnimator())
            return false;
        animator.SetBool(name, value);
        return true;
    }

    public void ResetTrigger(string name)
    {
        if (!HaveAnimator())
            return;
        animator.ResetTrigger(name);
    }

    public bool GetBool(string name)
    {
        if (!HaveAnimator())
            return false;
        return animator.GetBool(name);
    }

    public int GetInt(string name)
    {
        if (!HaveAnimator())
            return -404;
        return animator.GetInteger(name);
    }

    public float GetFloat(string name)
    {
        if (!HaveAnimator())
            return -404;
        return animator.GetFloat(name);
    }
    #endregion

    #region Yeah动画
    /// <summary>
    /// Yeah动画
    /// </summary>
    private void DoYeah()
    {
        if (!HaveAnimator()) return;
        SetAnimatorParam("Yeah", true);
    }

    /// <summary>
    /// 结束Yeah动画
    /// </summary>
    private void UnDoYeah()
    {
        if (!HaveAnimator()) return;
        SetAnimatorParam("Yeah", false);
    }
    #endregion

    #region 挥手动画
    /// <summary>
    /// 挥手动作
    /// </summary>
    private void DoHi()
    {
        if (!HaveAnimator()) return;
        SetAnimatorParam("Hi", true);
    }

    /// <summary>
    /// 结束挥手
    /// </summary>
    private void UnDoHi()
    {
        if (!HaveAnimator()) return;
        SetAnimatorParam("Hi", false);
    }
    #endregion

    #region 哭泣动画
    private void DoCry()
    {
        if (!HaveAnimator()) return;
        animator.transform.rotation = Quaternion.Euler(new Vector3(-60f, -90f, 270f));
        SetAnimatorParam("Cry", true);
    }

    /// <summary>
    /// 结束哭泣
    /// </summary>
    private void UnDoCry()
    {
        if (!HaveAnimator()) return;
        SetAnimatorParam("Cry", false);
        animator.transform.rotation = Quaternion.Euler(new Vector3(-90f, -90f, 270f));
        EventCenter.GetInstance().EventTrigger(MyEventType.WAVEGESTURE);
    }
    #endregion

    #region 角色旋转
    /// <summary>
    /// 旋转角色
    /// </summary>
    /// <param name="vel"></param>
    private void RotateRole(float vel)
    {
        if (!HaveAnimator()) return;
        animator.transform.Rotate(Vector3.up * vel * rotateSpeed);
    }
    #endregion

    #region 跳动画
    private void DoJump()
    {
        if (!HaveAnimator()) return;
        SetAnimatorParam("Jump", true);
    }

    private void UnDoJump()
    {
        if (!HaveAnimator()) return;
        SetAnimatorParam("Jump", false);
        EventCenter.GetInstance().EventTrigger(MyEventType.WAVEGESTURE);
    }
    #endregion

    #region 开心动画
    private void DoHappy()
    {
        if (!HaveAnimator()) return;
        SetAnimatorParam("Happy", true);
    }

    private void UnDoHappy()
    {
        if (!HaveAnimator()) return;
        SetAnimatorParam("Happy", false);
        EventCenter.GetInstance().EventTrigger(MyEventType.WAVEGESTURE);
    }
    #endregion

    #region 说话动画
    private void DoSay()
    {
        if (!HaveAnimator()) return;
        SetAnimatorParam("Say", true);
    }

    private void UnDoSay()
    {
        if (!HaveAnimator()) return;
        SetAnimatorParam("Say", false);
        EventCenter.GetInstance().EventTrigger(MyEventType.UNSAD);
        EventCenter.GetInstance().EventTrigger(MyEventType.UNHAPPY);
        EventCenter.GetInstance().EventTrigger(MyEventType.WAVEGESTURE);
    }
    #endregion

    #region 换角色
    private void ChangeRole()
    {
        SetAnimatorParam("Hi", false);
        string path = string.Format("Role/{0}", (curRoleName == "Naoko") ? "Riko" : "Naoko");

        PoolManager.GetInstance().GetGObj(path, (newRole) =>
        {
            newRole.transform.parent = animator.transform.parent;
            newRole.transform.position = animator.transform.position;
            newRole.transform.localScale = animator.transform.localScale;
            PoolManager.GetInstance().PushGObj(animator.gameObject.name, animator.gameObject);
            SetAnimator(newRole.GetComponent<Animator>());
        });
    }
    #endregion

    #region 被打动画
    private void DoBeaten()
    {
        if (!HaveAnimator()) return;
        SetAnimatorParam("Beaten", true);
    }

    private void UndoBeaten()
    {
        if (!HaveAnimator()) return;
        SetAnimatorParam("Beaten", false);
    }
    #endregion

    private void OnDestroy()
    {
        EventCenter.GetInstance().Clear();
    }
}