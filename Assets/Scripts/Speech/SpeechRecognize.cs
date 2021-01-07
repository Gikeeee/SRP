using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class SpeechRecognize : MonoBehaviour {

    public static SpeechRecognize Instance { get; private set; }

    //语音识别认证数据
    public string APP_ID = "18827858";
    public string API_KEY = "AVd4rXkILiQYoaZhHmIFTfoD";
    public string SECRET_KEY = "w6mWrgGu1MTyCGhatkMnSeXbVjfG3AAB";

    //动画控制相关
    //public GameObject character;
    //private Animator animator;
    //private AnimatorStateInfo curState;
    //private bool isFirstTime = true;
    private float timer = 0;

    /// <summary>
    /// 获取tocken的请求地址
    /// 返回json数据,每次获取token数据都不一样
    /// </summary>
    public string GetTokenUrl = "https://aip.baidubce.com/oauth/2.0/token";
    /// <summary>
    /// 获取到的Token
    /// </summary>
    public string Access_Token = "";

    /// <summary>
    /// 录音按钮
    /// </summary>
    [Header("录音按钮")]
    //public Button RecordButton;
    //public Text RecordInfo;
    //public GameObject myCube;
    public Image RecordImage;

    [Header("是否在录音的状态")]
    public bool isRecording = false;
    [Header("麦克风设备的名字")]
    public string MicrophoneName = "";
    /// <summary>
    /// 录制的音频片段
    /// </summary>
    public AudioClip RecordAudioClip;

    /// <summary>
    /// 播放音频的音源
    /// </summary>
    //public AudioSource audioSource;

    /// <summary>
    /// 语音识别地址
    /// </summary>
    public string SpeechRecognition_Address = "https://vop.baidu.com/server_api";
    /// <summary>
    /// 语音合成地址
    /// </summary>
    public string SpeechSynthesis_Address = "http://tsn.baidu.com/text2audio";
    [Header("识别到的文字")]
    public string RecognizeContent = "";
    [Header("录音最大时长")]
    public int recordTime = 60;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        //animator = character.GetComponent<Animator>();

        GetToken(GetTokenUrl);

        //获取电脑上第一个录音设备的名称
        if (Microphone.devices.Length > 0)
        {
            MicrophoneName = Microphone.devices[0];
        }
        else
        {
            Debug.LogError("当前设备缺少麦克风设备进行录制");
        }

        //audioSource = GetComponent<AudioSource>();

        EventCenter.GetInstance().AddEventListener(MyEventType.WAVEGESTURE, StartListen);
    }

    /// <summary>
    /// 开始语音监听
    /// </summary>
    private void StartListen()
    {
        BeginRecord();
    }

    private void FixedUpdate()
    {
        if (!isRecording) return;
        timer += Time.fixedDeltaTime;
        //最多录1分钟
        if (timer >= 59)
        {
            EndRecord(59);
        }
        else if (timer > 5)
        {
            int volume = GetVolume();
            if (volume < 200)
            {
                EndRecord((int)timer);
            }
        }
    }

    //private void FixedUpdate()
    //{
    //    if (!AnimateManager.Instance.HaveAnimator()) return;
    //    //如果不是Hello状态则重新视为第一次
    //    if (!curState.IsName("Base Layer.Hello"))
    //    {
    //        isFirstTime = true;
    //        return;
    //    } 

    //    //开启语音交互
    //    if (isFirstTime)
    //    {
    //        BeginRecord();
    //        isFirstTime = false;          
    //    }
    //    //录音时记录时长
    //    if(isRecording)
    //    {
    //        time += Time.fixedDeltaTime;
    //        //最多录1分钟
    //        if (time >= 59)
    //        {
    //            EndRecord(59);
    //        }
    //        else if(time > 5)
    //        {
    //            volume = GetVolume();
    //            if (volume < 200)
    //            {
    //                EndRecord((int)time);
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// 开始录音
    /// </summary>
    public void BeginRecord()
    {
        if (isRecording) return;
        //开始录制
        isRecording = true;
        //开始录制,频率16000
        RecordAudioClip = Microphone.Start(MicrophoneName, false, recordTime, 16000);
        //录制时为绿色
        RecordImage.GetComponent<Image>().color = Color.green;
    }

    /// <summary>
    /// 停止录音
    /// </summary>
    public void EndRecord(int recordTime)
    {
        //结束录制
        isRecording = false;
        timer = 0;

        Microphone.End(MicrophoneName);
        //停止录制红色
        RecordImage.GetComponent<Image>().color = Color.red;
        //播放录制的语音
        //audioSource.PlayOneShot(RecordAudioClip);
        //开始语音识别
        StartCoroutine(Recognition(RecordAudioClip, recordTime));
    }

    #region 网络链接请求
    /// <summary>
    /// 获取token
    /// </summary>
    /// <param name="url"></param>
    public void GetToken(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField("grant_type", "client_credentials");
        form.AddField("client_id", API_KEY);
        form.AddField("client_secret", SECRET_KEY);

        StartCoroutine(HttpPostRequest(url,form));
    }

    /// <summary>
    /// post http请求
    /// </summary>
    /// <param name="url">请求的地址</param>
    /// <param name="form">请求地址带的参数</param>
    /// <returns></returns>
    IEnumerator HttpPostRequest(string url, WWWForm form)
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Post(url, form);

        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.isNetworkError)
        {
            Debug.Log("网络错误:" + unityWebRequest.error);
        }
        else
        {
            if (unityWebRequest.responseCode == 200)
            {
                string result = unityWebRequest.downloadHandler.text;
                print("成功获取到数据:" + result);
                OnGetHttpResponse_Success(result);
            }
            else
            {
                print("状态码不为200:" + unityWebRequest.responseCode);
            } 
        }

    }
    
    /// <summary>
    /// 当成功获取到服务器返回的json数据时,进行解析
    /// </summary>
    /// <param name="result">json数据</param>
    private void OnGetHttpResponse_Success(string result)
    {
        AccessToken accessToken = JsonMapper.ToObject<AccessToken>(result);
        Access_Token = accessToken.access_token;
    }
    #endregion

    /// <summary>
    /// 返回前一段时间的分贝
    /// </summary>
    /// <returns>一段时间的分贝</returns>
    public int GetVolume()
    {

        if (Microphone.IsRecording(null))
        {
            int samplesize = 80000;
            float[] samples = new float[samplesize];
            int startpsition = Microphone.GetPosition(MicrophoneName) - (samplesize + 1);

            if (startpsition < 0) return 201;

            RecordAudioClip.GetData(samples, startpsition);

            float Maxlevel = 0;
            for (int i = 0; i < samplesize; ++i)
            {
                float wavepeak = samples[i];
                if (Maxlevel < wavepeak)
                {
                    Maxlevel = wavepeak;
                }
            }

            return (int)(Maxlevel * 10000);
        }
        return 0;
    }

    /// <summary>
    /// 语音识别
    /// </summary>
    /// <param name="audioClip"></param>
    /// <returns></returns>
    IEnumerator Recognition(AudioClip audioClip,int recordTime)
    {
        WWWForm form = new WWWForm();

        string url = string.Format("{0}?cuid={1}&token={2}", SpeechRecognition_Address, SystemInfo.deviceUniqueIdentifier, Access_Token);
        float[] samples = new float[16000 * recordTime * audioClip.channels];
        //将audioclip填充到数组中
        audioClip.GetData(samples, 0);
        short[] sampleShort = new short[samples.Length];
        for (int i = 0; i < samples.Length; i++)
        {
            sampleShort[i] = (short)(samples[i] * short.MaxValue);
        }

        byte[] data = new byte[sampleShort.Length * 2];
        Buffer.BlockCopy(sampleShort, 0, data, 0, data.Length);



        form.AddBinaryData("audio", data);
        UnityWebRequest request = UnityWebRequest.Post(url, form);
        request.SetRequestHeader("Content-Type", "audio/pcm;rate=16000");
        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log("网络错误:" + request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                string result = request.downloadHandler.text;
                print("获取的json数据:" + result);

                RecognizeResult resultContent = JsonMapper.ToObject<RecognizeResult>(result);
                if (resultContent.result != null)
                {
                    RecognizeContent = resultContent.result[0];
                    //对识别的文字做关键词检测
                    bool hasKeyWords = KeyWords.DetectALLWords(RecognizeContent);
                    if (hasKeyWords)
                    {
                        yield break;
                    }
                }

            }
            else
            {
                print("状态码不为200:" + request.responseCode);
            }
        }
        //未检测到关键词，回到idle
        EventCenter.GetInstance().EventTrigger(MyEventType.UNWAVEGESTURE);
        yield break;
    }
}

/// <summary>
/// AccessToken序列化json的对象
/// </summary>
public class AccessToken
{
    public string access_token;

    public int expires_in;

    public string session_key;

    public string scope;

    public string refresh_token;

    public string session_secret;

    /*
    
    {
    "access_token": "24.b243f17d64fa69b413d827f6a0965846.2592000.1542375343.282335-14131279",
    "session_key": "9mzdWWhYL0oUaqTY7WohNY0Fhd8Wxm4M7t4bTtlaq9/fyw7RXgztqR8+tmnAFpgywswOL3CQsU/v6PZ3ijK91/RmmiLb9Q==",
    "scope": "audio_voice_assistant_get audio_tts_post public brain_all_scope wise_adapt lebo_resource_base lightservice_public hetu_basic lightcms_map_poi kaidian_kaidian ApsMisTest_Test权限 vis-classify_flower lpq_开放 cop_helloScope ApsMis_fangdi_permission smartapp_snsapi_base iop_autocar oauth_tp_app smartapp_smart_game_openapi oauth_sessionkey",
    "refresh_token": "25.c2cf87484f244b6ef3d1d6a330727700.315360000.1855143343.282335-14131279",
    "session_secret": "7b9a68a03cbad17db3d13985dc7690d2",
    "expires_in": 2592000
}
    

    */
}

/// <summary>
/// 语音识别成功后,返回的json数据格式
/// </summary>
public class RecognizeResult
{
    public string corpus_no;

    public string err_msg;

    public int err_no;
    /// <summary>
    /// 语音识别到的结果
    /// </summary>
    public List<string> result;

    public string sn;

    //{"corpus_no":"6612962645817945596","err_msg":"success.","err_no":0,"result":["你今年多大，"],"sn":"845877030391539700349"}
}
