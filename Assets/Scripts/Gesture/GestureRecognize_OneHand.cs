using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class GestureRecognize_OneHand
{
    private Hand hand;
    //常量
    private float deltaVelocity = 0.5f;
    private float grabStandard = 0.8f;
    private float deltaCloseFinger = 0.05f;
    private float deltaClosePalm = 0.03f;
    private float deltaQigongPalm = 0.3f;
    private float angleQigongPalmMax = 90f;        //气功手势手掌方向夹角最大值
    private float angleQigongPalmMin = 50f;        //气功手势手掌方向夹角最小值
    private float angleQigongHandMin = 140f;        //气功手势手方向夹角


    private int waveConut;      //挥手计数
    //private float indexWaveConut;      //食指挥动计数
    private float preWaveVelocity;
    private bool isStationary;

    //public bool isGarbing { get; private set; }
    //public bool isClosing { get; private set; }
    public bool isFullOpening { get; private set; }
    public bool isWaving { get; private set; }      //挥手
    public bool isYeah { get; private set; }        //三指手势
    public float indexMoveVel { get; private set; }       //食指滑动速度
    public int zoomState { get; private set; }         //缩放状态
    public bool isBeaten { get; private set; }      //敲打

    //放大缩小相关的状态参数
    public enum ZoomStates
    {
        NONE = 0,
        OUT,
        IN
    }

    public List<Finger.FingerType> openingFingers { get; private set; }


    public GestureRecognize_OneHand()
    {
        //isGarbing = false;
        //isClosing = false;
        isFullOpening = false;
        isWaving = false;
        openingFingers = new List<Finger.FingerType>();
        waveConut = 0;
        indexMoveVel = 0f;
        preWaveVelocity = 0f;
        zoomState = (int)ZoomStates.NONE;

    }

    public void SetHand(Hand hand)
    {
        this.hand = hand;
    }

    /// <summary>
    /// 更新手的状态
    /// </summary>
    public void UpdateStatus()
    {
        if (hand == null) return;
        //isGarbing = IsGrabHand();
        //isClosing = IsCloseHand();
        isFullOpening = IsOpenFullHand();
        openingFingers = GetOpeningFingers();

        #region 检测三指手势
        isYeah = IsYeahGesture();
        #endregion

        #region 检测食指移动
        indexMoveVel = IndexMoveVelocity();
        #endregion

        #region 检测挥手逻辑
        DetectWavingGesture();
        #endregion

        #region 检测放大缩小状态
        zoomState = GetZoomGesture();
        #endregion

        #region 检测击打
        isBeaten = IsBeatGesture();
        #endregion

    }

    /// <summary>
    /// 判断是否比出三指手势
    /// </summary>
    /// <returns></returns>
    private bool IsYeahGesture()
    {
        if (openingFingers.Count != 3) return false;

        for (int i = 0; i < openingFingers.Count; i++)
        {
            if ((openingFingers[i] != Finger.FingerType.TYPE_THUMB)
                && (openingFingers[i] != Finger.FingerType.TYPE_INDEX)
                && (openingFingers[i] != Finger.FingerType.TYPE_MIDDLE))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 抓取状态
    /// </summary>
    /// <returns></returns>
    private bool IsGrabHand()
    {
        return hand.GrabStrength > grabStandard;
    }

    /// <summary>
    /// 是否握拳
    /// </summary>
    /// <returns></returns>
    private bool IsCloseHand()
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
    /// <returns></returns>
    private bool IsOpenFullHand()
    {
        return openingFingers.Count >= 5;
    }

    /// <summary>
    /// 手正在向左划
    /// </summary>
    /// <returns></returns>
    private bool IsMoveLeft()
    {
        return hand.PalmVelocity.x < -deltaVelocity;
    }

    /// <summary>
    /// 手正在向右划
    /// </summary>
    /// <returns></returns>
    private bool IsMoveRight()
    {
        return hand.PalmVelocity.x > deltaVelocity;
    }

    /// <summary>
    /// 手正在向上划
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    private bool IsMoveUp()
    {
        return hand.PalmVelocity.y > deltaVelocity;
    }

    /// <summary>
    /// 手正在向下划
    /// </summary>
    /// <returns></returns>
    private bool IsMoveDown()
    {
        return hand.PalmVelocity.y < -deltaVelocity;
    }

    /// <summary>
    /// 手正在向下快速划
    /// </summary>
    private bool IsMoveDownFast()
    {
        return hand.PalmVelocity.y < -(deltaVelocity*2f);
    }

    /// <summary>
    /// 获取打开的手指
    /// </summary>
    /// <returns></returns>
    private List<Finger.FingerType> GetOpeningFingers()
    {
        List<Finger.FingerType> openingFingers = new List<Finger.FingerType>();

        List<Finger> listOfFingers = hand.Fingers;
        for (int f = 0; f < listOfFingers.Count; f++)
        {
            Finger finger = listOfFingers[f];
            //远离掌心
            if ((finger.TipPosition - hand.PalmPosition).Magnitude > deltaCloseFinger)
            {
                openingFingers.Add(finger.Type);
            }
        }
        return openingFingers;
    }

    /// <summary>
    /// 获取某根手指是否打开
    /// </summary>
    /// <param name="fingerType"></param>
    /// <returns></returns>
    public bool IsFingerOpening(Finger.FingerType fingerType)
    {
        if (hand == null) return false;

        for (int i = 0; i < openingFingers.Count; i++)
        {
            if (openingFingers[i] == fingerType)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 手掌位置
    /// </summary>
    /// <returns></returns>
    public Leap.Vector GetPalmPosition()
    {
        return hand.PalmPosition;
    }

    /// <summary>
    /// 手腕位置
    /// </summary>
    /// <returns></returns>
    public Leap.Vector GetWristPosition()
    {
        return hand.WristPosition;
    }
    
    /// <summary>
    /// 手掌法线
    /// </summary>
    /// <returns></returns>
    public Leap.Vector GetPalmDir()
    {
        return hand.PalmNormal;
    }

    /// <summary>
    /// 手掌方向
    /// </summary>
    /// <returns></returns>
    public Leap.Vector GetHandDir()
    {
        return hand.Direction;
    }

    /// <summary>
    /// 食指移动
    /// </summary>
    /// <returns></returns>
    private float IndexMoveVelocity()
    {
        if (openingFingers.Count != 2) return 0f;
        if (openingFingers[0] != Finger.FingerType.TYPE_INDEX) return 0f;
        if (!IsMoveLeft() && !IsMoveRight()) return 0f;
        return hand.PalmVelocity.x;
    }

    /// <summary>
    /// OK手势
    /// </summary>
    /// <returns></returns>
    private bool OkGesture()
    {
        if (openingFingers.Count != 3) return false;

        for (int i = 0; i < openingFingers.Count; i++)
        {
            if ((openingFingers[i] != Finger.FingerType.TYPE_MIDDLE)
                && (openingFingers[i] != Finger.FingerType.TYPE_RING)
                && (openingFingers[i] != Finger.FingerType.TYPE_PINKY))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 检测挥动手势
    /// </summary>
    private void DetectWavingGesture()
    {
        if ((!IsMoveLeft() && !IsMoveRight()) || !isFullOpening)
        {
            if (waveConut < 1)
                isWaving = false;
            return;
        }
        if (waveConut < 1)
        {
            //detectWaving = true;
            MonoManager.GetInstance().StartCoroutine(CheckWaveGesture());
            waveConut++;
            preWaveVelocity = hand.PalmVelocity.x;
            return;
        }
        if (preWaveVelocity * hand.PalmVelocity.x < 0)
        {
            preWaveVelocity = hand.PalmVelocity.x;
            waveConut++;
        }
    }

    /// <summary>
    /// 检测挥手的协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckWaveGesture()
    {
        //两秒之后检测是否挥动了超过4次
        yield return new WaitForSeconds(2f);
        if (waveConut > 3)
        {
            isWaving = true;
        }
        else
        {
            isWaving = false;
        }
        waveConut = 0;
        preWaveVelocity = 0f;
        yield break;
    }

    /// <summary>
    /// 是否双手合十
    /// </summary>
    /// <param name="oherHand"></param>
    /// <returns></returns>
    public bool IsHandsTogether(GestureRecognize_OneHand otherHand)
    {
        if ((otherHand.GetPalmPosition() - hand.PalmPosition).Magnitude < deltaClosePalm)
        {
            if (this.isFullOpening && otherHand.isFullOpening)
                return true;
        }
        return false;
    }

    private bool IsQigongGesture(GestureRecognize_OneHand otherHand)
    {
        //手腕接近
        if (!((otherHand.GetWristPosition() - this.GetWristPosition()).Magnitude < deltaQigongPalm))
        {
            return false;
        }
        //手成抓取状态
        if (!this.IsGrabHand() || !otherHand.IsGrabHand())
        {
            return false;
        }

        Vector3 vec1 = new Vector3(
            otherHand.GetPalmDir().x,
            otherHand.GetPalmDir().y,
            otherHand.GetPalmDir().z);
        Vector3 vec2 = new Vector3(
            this.GetPalmDir().x,
            this.GetPalmDir().y,
            this.GetPalmDir().z);
        float anglePalm =  Vector3.Angle(vec1,  vec2);     //手掌方向夹角
        //手掌方向相近
        if (!((anglePalm < angleQigongPalmMax) && (anglePalm > angleQigongPalmMin)))
        {
            return false;
        }
        //手方向相反
        vec1 = new Vector3(
            otherHand.GetHandDir().x,
            otherHand.GetHandDir().y,
            otherHand.GetHandDir().z);
        vec2 = new Vector3(
            this.GetHandDir().x,
            this.GetHandDir().y,
            this.GetHandDir().z);
        float angleHand = Vector3.Angle(vec1, vec2);     //手掌方向夹角
        if (!((angleHand < -angleQigongHandMin) || (angleHand > angleQigongHandMin)))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 是否放大或缩小手势
    /// </summary>
    private int GetZoomGesture()
    {
        //首先确保五指张开
        if (openingFingers.Count != 5) return (int)ZoomStates.NONE;
        //手掌向上且向上移动，为放大
        if ((GetPalmDir().y > 0) && IsMoveUp()) return (int)ZoomStates.IN;
        //手掌向下且向下移动，为缩小
        else if ((GetPalmDir().y < 0) && IsMoveDown()) return (int)ZoomStates.OUT;
        
        return (int)ZoomStates.NONE;
    }

    /// <summary>
    /// 是否敲打手势
    /// </summary>
    private bool IsBeatGesture()
    {
        if (IsCloseHand() && IsMoveDownFast())
            return true;
        return false;
    }

}