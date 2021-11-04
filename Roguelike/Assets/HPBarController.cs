using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarController : MonoBehaviour
{
    [SerializeField] RectTransform myFrame;
    [SerializeField] float ratioPerFrame = 1f;
    float frameWidth;

    void Start() {
        frameWidth = myFrame.rect.width;
    }

    private void Update() {
        var transform = GetComponent<RectTransform>();
        var rect = transform.rect;
        
        float goalWidth = frameWidth * PlayerController.instance.HPPercentage;
        float newWidth = Mathf.Lerp(rect.width, goalWidth, ratioPerFrame);

        transform.sizeDelta = new Vector2(newWidth, rect.height);
    }
}
