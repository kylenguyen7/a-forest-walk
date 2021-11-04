using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponCardUI : MonoBehaviour
{
    public WeaponScriptableObject myWeapon;

    public TextMeshProUGUI title;
    public Image weaponImage;
    public TextMeshProUGUI damage;
    public TextMeshProUGUI desc;
    public TextMeshProUGUI flavor;

    public RectTransform firstWaypoint;
    public RectTransform secondWaypoint;
    RectTransform targetWaypoint;
    
    // Start is called before the first frame update
    void Start()
    {
        targetWaypoint = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        var pos = GetComponent<RectTransform>().anchoredPosition;
        pos = Vector2.Lerp(pos, targetWaypoint.anchoredPosition, 0.18f);
        GetComponent<RectTransform>().anchoredPosition = pos;
    }

    public void MoveToFirstWaypoint() {
        targetWaypoint = firstWaypoint;
    }

    public void MoveToSecondWaypoint() {
        targetWaypoint = secondWaypoint;
    }

    public void UpdateDisplay() {
        if (myWeapon == null) {
            weaponImage.enabled = false;
            desc.text = "No weapon equipped.";
        } else {
            title.text = myWeapon.weaponName;
            weaponImage.sprite = myWeapon.sprite;
            damage.text = "Damage: " + myWeapon.damage;
            desc.text = myWeapon.weaponDescription;
            flavor.text = myWeapon.flavor;
        }
    }
}
