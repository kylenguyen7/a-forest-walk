using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUIInfo : MonoBehaviour
{
    [SerializeField] string upgradeDesc;
    [SerializeField] GameManager.Upgrade upgrade;
    [SerializeField] string suffix;

    [SerializeField] TextMeshProUGUI myDesc;
    [SerializeField] UpgradeBarController myBar;
    [SerializeField] UpgradeButtonController myButton;
    

    private void Awake() {
        myBar.upgrade = upgrade;
        myDesc.text = upgradeDesc;
        myButton.upgrade = upgrade;
        myButton.mySuffix = suffix;
    }
}
