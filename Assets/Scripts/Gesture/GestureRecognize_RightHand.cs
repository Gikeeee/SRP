using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Leap;

/// <summary>
/// 右手手势识别交互的控制器
/// 识别2个手势：握拳、挥手
/// 其中挥手会触发语音识别
/// </summary>
public class GestureRecognize_RightHand : GestureRecognize
{
    //角色和动画
    public GameObject character;
    private Animator animator;
    private AnimatorStateInfo curState;
    //语音识别对象
    private SpeechRecognize speechRecognize;

    //单例协程
    private Coroutine fistCoroutine;
    private Coroutine waveCoroutine;

    //最小移动速度
    private float minMoveVelocity = 0.8f;

    //两次挥动的速度
    private float firstMoveVelocity = 0f;
    private float secondMoveVelocity = 0f;

    //是否执行动画转换（协程开启条件）
    private bool isFist = false;

    //是否检测挥手中
    //private bool detectWaving = false;
    //挥手次数
    private int moveCount = 0;
    //前一次挥手速度（主要是方向）
    private float preMoveVelocity = 0f;

    void Start()
    {
        animator = character.GetComponent<Animator>();
        speechRecognize = character.GetComponent<SpeechRecognize>();
    }

    void Update()
    {
        if (!handModel.IsTracked) return;

        curState = animator.GetCurrentAnimatorStateInfo(0);     //当前动画状态
        if (!curState.IsName("Action Layer.Idle")) return;
        isFist = animator.GetBool("Ges_Fist");
        if (isFist) return;

        hand = handModel.GetLeapHand();        //手数据

        //如果右手握拳
        if (IsCloseHand(hand))
        {
            fistCoroutine = StartCoroutine(FistGesture());
            return;
        }
        
        //挥手逻辑
        if (!IsMoving(hand)) return;
        //第一次挥动
        if (moveCount < 1)
        {
            //detectWaving = true;
            waveCoroutine = StartCoroutine(CheckWaveGesture());
            moveCount++;
            preMoveVelocity = hand.PalmVelocity.x;
            return;
        }
        //非第一次挥动则检测是否与前一次挥动的速度相反
        if (preMoveVelocity * hand.PalmVelocity.x < 0)
        {
            preMoveVelocity = hand.PalmVelocity.x;
            moveCount++;
        }

    }

    private bool IsMoving(Hand hand)
    {
        if (Mathf.Abs(hand.PalmVelocity.x) > minMoveVelocity) return true;
        return false;
    }

    /// <summary>
    /// 握拳手势逻辑
    /// </summary>
    /// <returns></returns>
    private IEnumerator FistGesture()
    {
        animator.SetBool("Ges_Fist", true);
        //yield return new WaitForSeconds(2.2f);
        //animator.SetBool("Ges_Fist", false);
        yield break;
    }

    /// <summary>
    /// 挥手手势逻辑
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckWaveGesture()
    {
        //两秒之后检测是否挥动了超过4次
        yield return new WaitForSeconds(2f);
        if(moveCount > 3)
        {
            animator.SetBool("Ges_Hi", true);
        }
        moveCount = 0;
        preMoveVelocity = 0f;
        //detectWaving = false;
        yield break;
    }
}
