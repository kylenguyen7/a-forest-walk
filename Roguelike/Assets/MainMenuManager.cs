using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    void Start()
    {
        MusicPlayer.instance.Play(MusicPlayer.Track.title);
    }
}
