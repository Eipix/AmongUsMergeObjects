using System.Collections;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

public class SoundsPlayer : Singleton<SoundsPlayer>
{
    [SerializeField] private AudioClip _menuTheme;
    [SerializeField] private Volume _music;
    [SerializeField] private Volume _sound;

    public AudioClip MenuTheme => _menuTheme;

    private bool _savedMusicState => (bool)SaveSerial.Instance.Load(_music.Key, false);
    private bool _savedSoundState => (bool)SaveSerial.Instance.Load(_sound.Key, false);

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            Pause();
        else
            Resume();
    }

    private void OnApplicationFocus(bool focus)
    {
        if(focus)
            Resume();
        else
            Pause();
    }

    public void Init()
    {
        InitMusic();
        InitSound();

        PlayMusic(_menuTheme);
    }

    private void InitMusic()
    {
        _music.AudioSource.mute = _savedMusicState;
        _music.Checkmark.State = _savedMusicState;
        _music.Checkmark.OnChecked.AddListener(state =>
        {
            Mute(_music, state);
            SaveSerial.Instance.Save(_music.Key, state);
        });
    }
    private void InitSound()
    {
        _sound.AudioSource.mute = _savedSoundState;
        _sound.Checkmark.State = _savedSoundState;
        _sound.Checkmark.OnChecked.AddListener(state =>
        {
            Mute(_sound, state);
            SaveSerial.Instance.Save(_sound.Key, state);
        });
    }

    [Button]
    public void MuteSound() => _sound.AudioSource.mute = true;
    [Button]
    public void UnMuteSound()
    {
        if ((bool)SaveSerial.Instance.Load(_sound.Key, false) == true)
            return;

        _sound.AudioSource.mute = false;
    }

    [Button]
    public void MuteMusic() => _music.AudioSource.mute = true;
    [Button]
    public void UnMuteMusic()
    {
        if ((bool)SaveSerial.Instance.Load(_music.Key, false) == true)
            return;

        _music.AudioSource.mute = false;
    }

    private void Mute(Volume volume, bool state)
    {
        volume.AudioSource.mute = state;
        SaveSerial.Instance.Save(volume.Key, state);
    }

    public void PlayMusic(AudioClip clip)
    {
        PlayOneShotMusic(clip);
        _music.AudioSource.loop = true;
    }

    public void PlayOneShotMusic(AudioClip clip)
    {
        _music.AudioSource.clip = clip;
        _music.AudioSource.Play();
        _music.AudioSource.loop = false;
    }

    [Button]
    public void Pause()
    {
        Pause(_music);
        Pause(_sound);
    }

    [Button]
    public void Resume()
    {
        Resume(_music);
        Resume(_sound);
    }

    private void Pause(Volume audioSource) => audioSource.AudioSource.Pause();

    private void Resume(Volume audioSource) => audioSource.AudioSource.UnPause();

    public void StopMusic()
    {
        _music.AudioSource.Stop();
        _music.AudioSource.mute = _savedMusicState;
    }

    public void StopSound()
    {
        _sound.AudioSource.Stop();
        _sound.AudioSource.mute = _savedSoundState;
    }

    public void StopAndPlayOneShotSound(AudioClip clip)
    {
        _sound.AudioSource.Stop();
        _sound.AudioSource.PlayOneShot(clip);
    }

    public void PlayOneShotSound(AudioClip clip) => _sound.AudioSource.PlayOneShot(clip);

    public Coroutine PlayWhile(AudioClip clip, Func<bool> condition, AudioSource audioSource)
    {
        return StartCoroutine(WaitEnd(clip, condition, audioSource));
    }

    private IEnumerator WaitEnd(AudioClip clip, Func<bool> condition, AudioSource audioSource)
    {
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.volume = _sound.AudioSource.volume;
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
        yield return new WaitWhile(condition);
        audioSource.Stop();
        Destroy(audioSource);
    }

    [Serializable]
    public struct Volume
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private Checkmark _checkmark;
        [SerializeField] private string _key;

        public Checkmark Checkmark => _checkmark;
        public AudioSource AudioSource => _audioSource;
        public string Key => _key;
    }
}

