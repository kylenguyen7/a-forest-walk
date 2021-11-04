using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadIndicatorController : MonoBehaviour
{
    RectTransform myFrame;
    float frameWidth;

    void Start() {
        myFrame = GetComponent<RectTransform>();
        frameWidth = myFrame.rect.width;
    }

    private void Update() {
        var transform = GetComponent<RectTransform>();
        var rect = transform.rect;

        float goalWidth = frameWidth * WeaponControllerPlayer.instance.ReloadProgress;
        float t = goalWidth / frameWidth;
        float d = 1;
        float b = 0;
        float c = frameWidth;
        goalWidth = c * Mathf.Sin(t / d * (Mathf.PI / 2)) + b;



        transform.sizeDelta = new Vector2(goalWidth, rect.height);
    }
}
