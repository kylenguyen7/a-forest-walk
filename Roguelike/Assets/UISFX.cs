using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UISFX : MonoBehaviour
{
    public static UISFX instance;
    public AudioClip scrollSFX;
    AudioSource mySource;

    public enum sounds {
        scroll
    }

    private void Awake() {
        if(instance != null) {
            Debug.LogError("Found more than one UISFX!");
            return;
        }

        instance = this;
    }

    private void Start() {
        mySource = GetComponent<AudioSource>();
    }

    public void PlaySound(sounds sound) {
        AudioClip sfx = null;

        switch(sound) {
            case sounds.scroll: sfx = scrollSFX; break;
        }

        if(sfx == null) {
            Debug.LogError("UISFX failed to find the specificed sound effect for: " + sound);
            return;
        }

        mySource.PlayOneShot(sfx);
    }
}
