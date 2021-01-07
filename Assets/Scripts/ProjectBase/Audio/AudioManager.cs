using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioManager : BaseManager<AudioManager>
{
    ResManager resManager = ResManager.GetInstance();
    MonoManager monoManager = MonoManager.GetInstance();

    private AudioSource bgMusic;    //背景音乐
    private float bgVolume = 1;  //背景音乐音量

    private float soundVolume = 1;  //音效音量
    private GameObject audio2D;
    private List<AudioSource> audioAutoList = new List<AudioSource>();
    private List<AudioSource> audioList = new List<AudioSource>();

    public AudioManager()
    {
        monoManager.AddUpdateListener(Update);
    }

    private void Update()
    {
        for (int i = audioAutoList.Count - 1; i >= 0; i--)
        {
            if (!audioAutoList[i].isPlaying)
            {
                GameObject.Destroy(audioAutoList[i]);
                audioAutoList.RemoveAt(i);
            }
        }
    }


    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name">背景音乐名</param>
    public void PlayBgMusic(string name, UnityAction callback = null)
    {
        if (bgMusic == null)
        {
            GameObject gObj = new GameObject("BackgroundMusic");
            GameObject.DontDestroyOnLoad(gObj);
            bgMusic = gObj.AddComponent<AudioSource>();
        }
        LoadAudioAsyn("Audio/BgMusics/" + name, bgMusic, bgVolume, true, 
            (audioSource) => {
                audioSource.loop = true;
                callback?.Invoke();
        }, true);
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBgMusic()
    {
        if (bgMusic == null) return;
        bgMusic.Stop();
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    /// <param name="audio"></param>
    public void PauseBgMusic(AudioSource audio)
    {
        if (bgMusic == null) return;
        bgMusic.Pause();
    }

    /// <summary>
    /// 设置背景音乐音量
    /// </summary>
    /// <param name="value"></param>
    public void SetBgVolume(float value)
    {
        bgVolume = value;
        if (bgMusic != null)
            bgMusic.volume = value;
    }

    /// <summary>
    /// 播放2D音效
    /// </summary>
    /// <param name="name">音效名</param>
    /// <param name="autoManage">是否自动管理</param>
    /// <param name="callback">回调函数</param>
    public void PlaySound(string name, bool autoManage = true,
        UnityAction<AudioSource> callback = null)
    {
        if (audio2D == null)
        {
            audio2D = new GameObject("Audio2D");
        }
        AudioSource audioSource = audio2D.AddComponent<AudioSource>();
        LoadAudioAsyn("Audio/Sounds/" + name, audioSource, soundVolume, autoManage,
            (source) => {
            callback?.Invoke(source);
        });
    }

    /// <summary>
    /// 播放3D音效
    /// </summary>
    /// <param name="name">音效名</param>
    /// <param name="gObj">挂在的游戏物体</param>
    /// <param name="autoManage">是否自动管理</param>
    /// <param name="spatialBlend">3D指数</param>
    /// <param name="callback">回调函数</param>
    public void PlaySound(string name, GameObject gObj, bool autoManage = true,
        float spatialBlend = 1f, UnityAction<AudioSource> callback = null)
    {
        AudioSource audioSource = gObj.AddComponent<AudioSource>();
        LoadAudioAsyn("Audio/Sounds/" + name, audioSource, soundVolume, autoManage,
            (source) => {
            source.spatialBlend = spatialBlend;
            callback?.Invoke(source);
        });
    }

    /// <summary>
    /// 加载音效clip
    /// </summary>
    /// <param name="path">路径名</param>
    /// <param name="audioSource">音效播放组件</param>
    /// <param name="auto">是否自动管理</param>
    /// <param name="callback">回调函数</param>
    /// <param name="isBg">是否背景音乐</param>
    private void LoadAudioAsyn(string path, AudioSource audioSource, float volume,bool auto = true,
        UnityAction<AudioSource> callback = null, bool isBg = false)
    {
        resManager.LoadAsyn<AudioClip>(path, (audioClip) =>
        {
            audioSource.clip = audioClip;
            audioSource.Play();
            audioSource.volume = volume;
            if (auto && !isBg)
                audioAutoList.Add(audioSource);
            else if (!isBg)
                audioList.Add(audioSource);
            callback?.Invoke(audioSource);
        });
    }

    /// <summary>
    /// 停止播放音效
    /// </summary>
    /// <param name="audioSource"></param>
    public void StopSound(AudioSource audioSource)
    {
        bool isContains = false;
        if (audioAutoList.Contains(audioSource))
        {
            audioAutoList.Remove(audioSource);
            isContains = true;
        }
        else if(audioList.Contains(audioSource))
        {
            audioList.Remove(audioSource);
            isContains = true;
        }
        if (!isContains) return;
        audioSource.Stop();
        GameObject.Destroy(audioSource);

    }

    /// <summary>
    /// 设置音效大小
    /// </summary>
    /// <param name="value"></param>
    public void SetSoundVolume(float value)
    {
        soundVolume = value;
        for (int i = 0; i < audioAutoList.Count; i++)
        {
            audioAutoList[i].volume = value;
        }
        for (int i = 0; i < audioList.Count; i++)
        {
            audioList[i].volume = value;
        }
    }
}
