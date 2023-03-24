using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class SoundManeger
{
    public const string BGM_01 = "";
    public const string BGM_02 = "";
    public const string BGM_03 = "";
    public const string BGM_04 = "";

    public const string SE_01 = "";
    public const string SE_02 = "";
    public const string SE_03 = "";

    private const string EXT = ".mp3";

    private enum Type
    {
        BGM,
        SE
    }


    private AudioSource _souceBgm;
    private AudioSource _souceSe;
    private static SoundManeger _instance;
    private List<AudioClip> _soundList = new List<AudioClip>();

    public static SoundManeger Instance
    {
        get
        {
            if(_instance is not SoundManeger)
            {
                _instance = new SoundManeger();
            }
            return _instance;
        }
    }

    public async UniTask Init()
    {
        var obj = new GameObject("Audio");
        _souceBgm = obj.AddComponent<AudioSource>();
        _souceBgm.loop = true;
        _souceBgm.volume = 0.5f;
        _souceSe = obj.AddComponent<AudioSource>();
        _souceSe.volume = 0.5f;
    }

    public async UniTask BgmPlay(string bgm)
    {
        if (String.IsNullOrWhiteSpace(bgm)) return;

        var audio = await DownloadAudio(bgm, Type.BGM);
        _souceBgm.clip = audio;
        _souceBgm.Play();
    }
    public async UniTask SePlay(string se)
    {
        if (String.IsNullOrWhiteSpace(se)) return;

        var audio = await DownloadAudio(se, Type.SE);
        _souceSe.PlayOneShot(audio);
    }

    private async UniTask<AudioClip> DownloadAudio(string name, Type type)
    {
        foreach(var v in _soundList)
        {
            if (v.name == name)
            {
                Debug.Log(name + "������");
                return v;
            }
        }

        string path = "";
        switch (type)
        {
            case Type.BGM:
                path = "Sound/Bgm/";
                break;
            case Type.SE:
                path = "Sound/Se/";
                break;
        }

        Debug.Log(Application.dataPath + path + name);
        //var req = UnityWebRequestMultimedia.GetAudioClip(Application.dataPath + path + name + EXT, AudioType.MPEG);
        //var req = UnityWebRequestMultimedia.GetAudioClip(Application.streamingAssetsPath + path + name + EXT, AudioType.MPEG);
        //((DownloadHandlerAudioClip)req.downloadHandler).streamAudio = true;

        //await req.SendWebRequest();

        ResourceRequest req = Resources.LoadAsync<AudioClip>(path + name);
        //ResourceRequest req = Resources.LoadAsync<AudioClip>(Application.dataPath + path + name + EXT);

        await UniTask.WaitUntil(() => req.isDone);

        //var audio = DownloadHandlerAudioClip.GetContent(req);
        var audio = req.asset as AudioClip;
        audio.name = name;
        _soundList.Add(audio);

        return audio;
    }
}