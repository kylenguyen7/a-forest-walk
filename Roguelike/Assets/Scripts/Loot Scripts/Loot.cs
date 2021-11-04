using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Loot_DESC", menuName = "ScriptableObjects/Loot", order = 3)]
public class Loot : ScriptableObject
{
    public string ID;
    public Sprite sprite;

    public AudioClip pickupSfx;
    public float gravity;

    public enum lootType {
        coin,
        heart,
        item
    }
    public lootType myLootType;
    public Item myItem;
}
