using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class GestureRecognize : MonoBehaviour
{
    public HandModelBase handModel;
    protected Hand hand;

    protected float deltaVelocity = 0.7f;

    /// <summary>
    /// 判断哪几根手指打开
    /// </summary>
    /// <param name="hand"></param>
    /// <param name="arr"></param>
    /// <returns></returns>
    protected bool CheckFingerOpenToHand(Hand hand, Finger.FingerType[] fingerTypesArr, float deltaCloseFinger = 0.05f)
    {
        List<Finger> listOfFingers = hand.Fingers;
        float count = 0;
        // 遍历5个手指
        for (int f = 0; f < listOfFingers.Count; f++)
        {
            Finger finger = listOfFingers[f];
            // 判读每个手指的指尖位置和掌心位置的长度是不是小于某个值，以判断手指是否贴着掌心
            if ((finger.TipPosition - hand.PalmPosition).Magnitude < deltaCloseFinger)
            {
                // 如果传进来的数组长度是0，有一个手指那么count + 1，continue 跳出，不执行下面数组长度不是0 的逻辑
                if (fingerTypesArr.Length == 0)
                {
                    count++;
                    continue;
                }
                // 传进来的数组长度不是 0，
                for (int i = 0; i < fingerTypesArr.Length; i++)
                {
                    // 假如本例子传进来的是食指和中指，逻辑走到这里，如果你的食指是紧握的，下面会判断这个手指是不是食指，返回 false
                    if (finger.Type == fingerTypesArr[i])
                    {
                        return false;
                    }
                    else
                    {
                        count++;
                    }
                }

            }
        }
        if (fingerTypesArr.Length == 0)
        {
            return count == 5;
        }
        // 这里除以length 是因为上面数组在每次 for 循环 count ++ 会执行 length 次
        return (count / fingerTypesArr.Length == 5 - fingerTypesArr.Length);
    }

    /// <summary>
    /// 判断是否抓取
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    protected bool IsGrabHand(Hand hand)
    {
        return hand.GrabStrength > 0.8f;
    }

    /// <summary>
    /// 判断是不是握拳
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    protected bool IsCloseHand(Hand hand)
    {
        List<Finger> listOfFingers = hand.Fingers;
        int count = 0;
        for (int f = 0; f < listOfFingers.Count; f++)
        {
            Finger finger = listOfFingers[f];
            if ((finger.TipPosition - hand.PalmPosition).Magnitude < 0.05f)
            {
                count++;
            }
        }
        return (count == 4);
    }

    /// <summary>
    /// 判断手指是否全张开
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    protected bool IsOpenFullHand(Hand hand)
    {
        return hand.GrabStrength == 0;
    }

    /// <summary>
    /// 手滑向左边
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    protected bool IsMoveLeft(Hand hand)
    {
        //x轴移动的速度   deltaVelocity = 0.7f   
        return hand.PalmVelocity.x < -deltaVelocity;
    }

    /// <summary>
    /// 手滑向右边
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    protected bool IsMoveRight(Hand hand)
    {
        return hand.PalmVelocity.x > deltaVelocity;
    }

    /// <summary>
    /// 手滑向上边
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    protected bool IsMoveUp(Hand hand)
    {
        return hand.PalmVelocity.y > deltaVelocity;
    }

    /// <summary>
    /// 手滑向下边
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    protected bool IsMoveDown(Hand hand)
    {
        return hand.PalmVelocity.y < -deltaVelocity;
    }


    /// <summary>
    /// 判断四指是否靠向掌心
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    protected bool CheckFingerCloseToHand(Hand hand)
    {
        List<Finger> listOfFingers = hand.Fingers;
        int count = 0;
        for (int f = 0; f < listOfFingers.Count; f++)
        {
            Finger finger = listOfFingers[f];
            if ((finger.TipPosition - hand.PalmPosition).Magnitude < 0.05f)
            {
                if (finger.Type == Finger.FingerType.TYPE_THUMB)
                {
                    return false;
                }
                else
                {
                    count++;
                }
            }
        }
        return (count == 4);
    }
}
