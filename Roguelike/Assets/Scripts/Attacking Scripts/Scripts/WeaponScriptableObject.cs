using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon_DESC", menuName = "ScriptableObjects/WeaponScriptableObject", order = 1)]
public class WeaponScriptableObject : ScriptableObject
{
    // This inherits from Item...
    // Her mind...
    
    public Sprite sprite;
    public float size = 1;
    public float comboResetTime;
    public bool autoFire;

    public AttackScriptableObject[] myAttacks;

    public enum weaponType {
        sword,
        bow,
        none
    }
    public weaponType type;

    
    public string weaponName;
    public string damage;
    [TextArea]
    public string weaponDescription;
    [TextArea]
    public string flavor;

    public Item associatedItem;
}
