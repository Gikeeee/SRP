using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

/// <summary>
/// 左手手势识别交互的控制器
/// 识别3个手势：“6”、“Yeah”
/// </summary>
public class GestureRecognize_LeftHand : GestureRecognize
{
    //角色和动画
    public GameObject character;
    private Animator animator;
    private AnimatorStateInfo curState;

    //单例协程
    private Coroutine yeahCoroutine;
    private Coroutine sixCoroutine;

    //手势手指组
    private Finger.FingerType[] yeahFingerTypes = { Finger.FingerType.TYPE_INDEX, Finger.FingerType.TYPE_MIDDLE };
    private Finger.FingerType[] sixFingerTypes = { Finger.FingerType.TYPE_THUMB, Finger.FingerType.TYPE_PINKY };

    //是否执行动画转换（协程开启条件）
    private bool isYeah = false;
    private bool isSix = false;

    private void Start()
    {
        animator = character.GetComponent<Animator>();
    }
    void Update()
    {
        if (!handModel.IsTracked) return;

        curState = animator.GetCurrentAnimatorStateInfo(0);     //当前动画状态
        if (!curState.IsName("Action Layer.Idle")) return;
        isYeah = animator.GetBool("Ges_Yeah");
        isSix = animator.GetBool("Ges_Six");
        if (isYeah | isSix) return;

        hand = handModel.GetLeapHand();        //手数据

        if (CheckFingerOpenToHand(hand, yeahFingerTypes, 0.05F))
        { 
            yeahCoroutine = StartCoroutine(YeahGesture());
            return;

        }

        if (CheckFingerOpenToHand(hand, sixFingerTypes, 0.05F))
        {
            sixCoroutine = StartCoroutine(SixGesture());
        }
    }

    /// <summary>
    /// Yeah手势逻辑
    /// </summary>
    /// <returns></returns>
    private IEnumerator YeahGesture()
    {
        animator.SetBool("Ges_Yeah", true);
        //yield return new WaitForSeconds(2.2f);
        //animator.SetBool("Ges_Yeah", false);
        yield break;
    }

    /// <summary>
    /// 6手势逻辑
    /// </summary>
    /// <returns></returns>
    private IEnumerator SixGesture()
    {
        animator.SetBool("Ges_Six", true);
        //yield return new WaitForSeconds(2.2f);
        //animator.SetBool("Ges_Six", false);
        yield break;
    }
}
