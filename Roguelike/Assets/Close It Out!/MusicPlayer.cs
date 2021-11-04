using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    private const float MUSIC_FADE_TIME = 0.5f;
    
    public enum Track {
        title,
        mainTheme,
        bossTheme,
        shopTheme,
        lastLevelTheme,
        credits
    }

    public AudioClip mainTheme;
    public AudioClip bossTheme;
    public AudioClip shopTheme;
    public AudioClip lastLevelTheme;
    public AudioClip credits;

    private AudioSource mySource;
    private AudioClip currentlyPlaying;
    private IEnumerator coroutine;

    private float volume;

    public static MusicPlayer instance;

    private void Update() {
    }

    private void Awake() {
        if (instance != null) {
            return;
        }

        instance = this;
        mySource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        SetVolume(0);
    }

    public void SwitchTrack(Track track) {
        switch(track) {
            case Track.title: currentlyPlaying = mainTheme; break;
            case Track.mainTheme: currentlyPlaying = mainTheme; break;
            case Track.bossTheme: currentlyPlaying = bossTheme; break;
            case Track.shopTheme: currentlyPlaying = shopTheme; break;
            case Track.lastLevelTheme: currentlyPlaying = lastLevelTheme; break;
            case Track.credits: currentlyPlaying = credits; break;
        }

        mySource.clip = currentlyPlaying;
    }

    private float GetVolume() {
        return volume;
    }

    private void SetVolume(float newVolume) {
        volume = newVolume;
        mySource.volume = volume;
    }

    public void Play() {
        mySource.UnPause();
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = FadeIn();
        StartCoroutine(coroutine);
    }

    public void Play(Track track) {
        SwitchTrack(track);

        mySource.Play();
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = FadeIn();
        StartCoroutine(coroutine);
    }

    public void Pause() {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = FadeOut();
        StartCoroutine(coroutine);
    }

    public void PauseThenPlay(Track track) {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = FadeOutAndIn(track);
        StartCoroutine(coroutine);
    }

    IEnumerator FadeOutAndIn(Track track) {
        while (GetVolume() > 0) {
            SetVolume(Mathf.Max(0, volume - Time.unscaledDeltaTime / MUSIC_FADE_TIME));
            yield return null;
        }

        SwitchTrack(track);

        mySource.Play();
        while (GetVolume() < 1) {
            SetVolume(Mathf.Min(1, volume + Time.unscaledDeltaTime / MUSIC_FADE_TIME));
            yield return null;
        }
    }

    IEnumerator FadeIn() {
        while (GetVolume() < 1) {
            SetVolume(Mathf.Min(1, volume + Time.unscaledDeltaTime / MUSIC_FADE_TIME));
            yield return null;
        }        
    }

    IEnumerator FadeOut() {
        while (GetVolume() > 0) {
            SetVolume(Mathf.Max(0, volume - Time.unscaledDeltaTime / MUSIC_FADE_TIME));
            yield return null;
        }
        mySource.Pause();
    }
}
