using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeButtonController : ButtonController {
    public GameManager.Upgrade upgrade;
    public string mySuffix;
    [SerializeField] TMPro.TextMeshProUGUI myText;
    [SerializeField] TMPro.TextMeshProUGUI myLabel;
    [SerializeField] bool isPercentageLabel = false;

    private new void Update() {
        base.Update();

        int cost = GameManager.instance.GetUpgradeCost(upgrade);

        float currentMag = GameManager.instance.GetUpgradeMagnitude(upgrade);
        float nextMag = GameManager.instance.GetUpgradeNextMagnitude(upgrade);
        if (isPercentageLabel) {
            currentMag *= 100;
            nextMag *= 100;
        }

        if (cost == -1) {
            clickable = false;
            myText.text = "MAX LEVEL";
            myLabel.text = $"{currentMag}{mySuffix}";
        } else {
            myText.text = $"Upgrade ({cost} gold)";
            myLabel.text = hovered ? $"{currentMag}{mySuffix} -> {nextMag}{mySuffix}" : $"{currentMag}{mySuffix}";
        }
    }

    protected override void OnClick() {
        GameManager.instance.AttemptUpgrade(upgrade);
    }
}
