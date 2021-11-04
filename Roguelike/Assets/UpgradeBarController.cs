using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeBarController : MonoBehaviour
{
    [SerializeField] RectTransform myFrame;
    [SerializeField] float ratioPerFrame = 1f;
    public GameManager.Upgrade upgrade;

    int numLevels;
    float frameWidth;

    void Start() {
        frameWidth = myFrame.rect.width;
        numLevels = GameManager.instance.GetUpgradeNumLevels(upgrade);
    }

    private void Update() {
        var transform = GetComponent<RectTransform>();
        var rect = transform.rect;


        int currNumLevels = GameManager.instance.GetUpgradeLevel(upgrade);
        float goalWidth = frameWidth * currNumLevels / (numLevels - 1);
        float newWidth = Mathf.Lerp(rect.width, goalWidth, ratioPerFrame);

        transform.sizeDelta = new Vector2(newWidth, rect.height);
    }
}
