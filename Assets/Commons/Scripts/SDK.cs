using Playgama;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Playgama.Modules.Platform;
using Playgama.Modules.Advertisement;
using System;

public class SDK : Singleton<SDK>
{
    public bool IsLoad { get; private set; }

    private void Start() => Bridge.advertisement.interstitialStateChanged += OnInterstitialStateChanged;

    public void Init(Action onInit = null) => StartCoroutine(InitRoutine(onInit));

    private IEnumerator InitRoutine(Action onInit = null)
    {
        Language.Instance.ChangeLanguage(Bridge.platform.language);
        Load();
        GameReady();
        yield return new WaitUntil(() => IsLoad);
        ShowBanner();
        onInit?.Invoke();
    }

    public void Save() => Bridge.storage.Set(SaveSerial.SaveFile, SaveSerial.Instance.JsonData);

    public void Load()
    {
#if UNITY_EDITOR
        IsLoad = true;
#else
        Bridge.storage.Get(SaveSerial.SaveFile, OnLoadComplete);
#endif
    }

    public void OnLoadComplete(bool success, string data)
    {
        if (success && data != null && data != string.Empty)
            File.WriteAllText(SaveSerial.Instance.Path, data);

        IsLoad = true;
    }

    public void SetToLeaderBoard(int value)
    {
        if (Bridge.leaderboard.isSupported == false || Bridge.leaderboard.isSetScoreSupported == false)
            return;

        var options = new Dictionary<string, object>();

        switch (Bridge.platform.id)
        {
            case "yandex":
                options.Add("score", value);
                options.Add("leaderboardName", "Score");
                break;
        }

        Bridge.leaderboard.SetScore(options, success => { });
    }

    public void ShowFullScreenAd() => Bridge.advertisement.ShowInterstitial();

    public void ShowBanner() => Bridge.advertisement.ShowBanner();

    private void OnInterstitialStateChanged(InterstitialState state)
    {
        switch (state)
        {
            case InterstitialState.Loading:
                SoundsPlayer.Instance.MuteMusic();
                SoundsPlayer.Instance.MuteSound();
                break;
            case InterstitialState.Opened:
                SoundsPlayer.Instance.MuteMusic();
                SoundsPlayer.Instance.MuteSound();
                break;
            case InterstitialState.Closed:
                SoundsPlayer.Instance.UnMuteMusic();
                SoundsPlayer.Instance.UnMuteSound();
                break;
            case InterstitialState.Failed:
                SoundsPlayer.Instance.UnMuteMusic();
                SoundsPlayer.Instance.UnMuteSound();
                break;
            default:
                break;
        }
    }

    private void GameReady()
    {
        switch (Bridge.platform.id)
        {
            case "crazy_games":
                Bridge.platform.SendMessage(PlatformMessage.GameplayStarted);
                break;
            default:
                Bridge.platform.SendMessage(PlatformMessage.GameReady);
                break;
        }
    }
}
