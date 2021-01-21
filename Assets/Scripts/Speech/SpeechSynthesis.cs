using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechSynthesis : MonoBehaviour
{
    public string STARTSTRING = "快来找我玩吧！";
    public string UNWAVESTRING = "随时找我玩呀！";
    public string LISTENSTRING = "我在听呀！";
    public string COMFORTSTRING = "哎呀你别难过了，一切都会好起来的！";
    public string HAPPYSTRING = "哇，我今天也很开心！";
    public string KENANSTRING = "高中生侦探工藤新一和青梅竹马的同学毛利兰一同去游乐园玩的时候，目击了黑衣男子的可疑交易现场。只顾偷看交易的工藤新一，却忽略了从背后接近的另一名同伙。他被那名男子灌下了毒药，当他醒来时，身体居然缩小了！";
    public string MINGRENSTRING = "火之国木叶隐村的忍者，四代目火影波风水门和漩涡玖辛奈之子，六道仙人次子阿修罗的查克拉转世者。刚出生时父母为保护村子而牺牲，并将尾兽“九尾”封印在鸣人体内。成为孤儿的鸣人从小被村民歧视，但在唯一认同他的老师海野伊鲁卡以及三代目火影猿飞日斩的鼓励下有了要成为火影的梦想，让所有人都认同他的存在。";
    public string YEAHSTRING = "嘿嘿 爱你哟！";

    public string APP_ID = "18827858";
    public string API_KEY = "AVd4rXkILiQYoaZhHmIFTfoD";
    public string SECRET_KEY = "w6mWrgGu1MTyCGhatkMnSeXbVjfG3AAB";
    private Tts _asr;
    private bool startPlaying;
    private bool isListening = false;
    private bool isOwnSpeak = false;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        _asr = new Tts(API_KEY, SECRET_KEY);
        StartCoroutine(_asr.GetAccessToken());
        audioSource = GetComponent<AudioSource>();
        EventCenter.GetInstance().AddEventListener(MyEventType.SAD, OnSad);
        EventCenter.GetInstance().AddEventListener(MyEventType.HAPPY, OnHappy);
        EventCenter.GetInstance().AddEventListener(MyEventType.KENAN, OnKenan);
        EventCenter.GetInstance().AddEventListener(MyEventType.MINGREN, OnMinren);
        EventCenter.GetInstance().AddEventListener(MyEventType.HELLOW, OnWave);
        EventCenter.GetInstance().AddEventListener(MyEventType.GAMESTART, OnGameStart);
        EventCenter.GetInstance().AddEventListener(MyEventType.UNWAVEGESTURE, OnUnWave);
        EventCenter.GetInstance().AddEventListener(MyEventType.YEAHGESTURE, OnYeah);
    }

    void Update()
    {
        if (startPlaying)
        {
            if (!audioSource.isPlaying)
            {
                startPlaying = false;
                if (isOwnSpeak)
                {
                    isOwnSpeak = false;
                    return;
                }
                if (isListening)
                {
                    isListening = false;
                }
                else
                {
                    EventCenter.GetInstance().EventTrigger(MyEventType.UNSAY);
                }
            }
        }
    }

    /// <summary>
    /// 游戏启动语音
    /// </summary>
    private void OnGameStart()
    {
        if (isOwnSpeak) return;
        isOwnSpeak = true;
        StartSpeechSyn(STARTSTRING);
    }

    /// <summary>
    /// 三指手势语音
    /// </summary>
    private void OnYeah()
    {
        if (isOwnSpeak) return;
        isOwnSpeak = true;
        StartSpeechSyn(YEAHSTRING);
    }

    /// <summary>
    /// 开启语音识别的语音反馈
    /// </summary>
    private void OnWave()
    {
        if (isListening) return;
        isListening = true;
        StartSpeechSyn(LISTENSTRING);
    }

    /// <summary>
    /// 退出语音识别
    /// </summary>
    private void OnUnWave()
    {
        if (isOwnSpeak) return;
        isOwnSpeak = true;
        StartSpeechSyn(UNWAVESTRING);
    }

    /// <summary>
    /// 伤心
    /// </summary>
    private void OnSad()
    {
        StartSpeechSyn(COMFORTSTRING);
    }

    /// <summary>
    /// 开心
    /// </summary>
    private void OnHappy()
    {
        StartSpeechSyn(HAPPYSTRING);
    }

    /// <summary>
    /// 介绍柯南
    /// </summary>
    private void OnKenan()
    {
        StartSpeechSyn(KENANSTRING);
    }

    /// <summary>
    /// 介绍鸣人
    /// </summary>
    private void OnMinren()
    {
        StartSpeechSyn(MINGRENSTRING);
    }

    private void StartSpeechSyn(string word)
    {
        StartCoroutine(_asr.Synthesis(word, s =>
        {
            if (s.Success)
            {
                audioSource.clip = s.clip;
                audioSource.Play();
                startPlaying = true;
                if (isOwnSpeak)
                {
                    return;
                }
                if (isListening)
                {
                    EventCenter.GetInstance().EventTrigger(MyEventType.GETWAVED);
                }
                else
                {
                    EventCenter.GetInstance().EventTrigger(MyEventType.SAY);
                }
            }
            else
            {
                Debug.LogError("合成失败！");
            }
        }));
    }
}
