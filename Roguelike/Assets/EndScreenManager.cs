using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    void Start()
    {
        MusicPlayer.instance.Play(MusicPlayer.Track.title);
    }
}