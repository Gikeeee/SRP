using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class GestureRecognizeManager : MonoBehaviour
{
    public static GestureRecognizeManager Instance { get; private set; }

    public HandModelBase leftHandModel;
    public HandModelBase rightHandModel;
    private GestureRecognize_OneHand leftHand = new GestureRecognize_OneHand();
    private GestureRecognize_OneHand rightHand = new GestureRecognize_OneHand();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Update()
    {
        //获取手数据
        leftHand.SetHand(leftHandModel.GetLeapHand());
        rightHand.SetHand(rightHandModel.GetLeapHand());
        //更新手状态信息
        leftHand.UpdateStatus();
        rightHand.UpdateStatus();

        if (!AnimateManager.Instance.HaveAnimator()) return;
        #region 第一层手势
        if (AnimateManager.Instance.GetStateInfo().IsName(AnimationStateName.IDLE) || 
            AnimateManager.Instance.GetStateInfo().fullPathHash == 1432961145)
        {
            if (leftHand.isYeah)
            {
                EventCenter.GetInstance().EventTrigger(MyEventType.YEAHGESTURE);
            }
            else if(rightHand.isWaving)
            {
                Debug.Log("挥手");
                EventCenter.GetInstance().EventTrigger(MyEventType.HELLOW);
                
            }
            else if(rightHand.indexMoveVel != 0)
            {
                EventCenter.GetInstance().EventTrigger<float>(MyEventType.ROTATEGESTURE, rightHand.indexMoveVel);
            }
            else if(rightHand.zoomState == (int)GestureRecognize_OneHand.ZoomStates.IN || leftHand.zoomState == (int)GestureRecognize_OneHand.ZoomStates.IN)
            {
                EventCenter.GetInstance().EventTrigger(MyEventType.ZOOMIN);
            }
            else if (rightHand.zoomState == (int)GestureRecognize_OneHand.ZoomStates.OUT || leftHand.zoomState == (int)GestureRecognize_OneHand.ZoomStates.OUT)
            {
                EventCenter.GetInstance().EventTrigger(MyEventType.ZOOMOUT);
            }
            else if (rightHand.isBeaten)
            {
                Debug.Log("da");
                EventCenter.GetInstance().EventTrigger(MyEventType.BEATENGESTURE);
            }
        }
        #endregion


    }
}
