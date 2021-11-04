using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_NAME", menuName = "ScriptableObjects/Item", order = 6)]
[System.Serializable]
public class Item : ScriptableObject {
    public string ID;
    public Sprite itemSprite;
    public int maxStack;
    public Category myCategory;
    [TextArea]
    public string description;

    public enum Category {
        Consumable,
        Loot,
        Weapon
    }

    public enum ConsumableType {
        none,
        spawn,
        heal
    }

    [HideInInspector]
    public ConsumableType myConsumableType = ConsumableType.none;

    [HideInInspector]
    public GameObject objectToSpawn;

    [HideInInspector]
    public float healthRestored;

    public bool Use() {
        if (myCategory != Category.Consumable) {
            return false;
        }

        switch (myConsumableType) {
            case ConsumableType.spawn: {
                    if (objectToSpawn == null) {
                        Debug.LogError("Attempted to use a spawn consumable with an unassigned object to spawn!");
                        return false;
                    }
                    PlayerController.instance.Deploy(objectToSpawn);
                }
                break;
            case ConsumableType.heal: {
                    if (healthRestored <= 0) {
                        Debug.LogError("Attempted to use a health consumable with an invalid heal amount!");
                        return false;
                    }
                    PlayerController.instance.Heal(healthRestored);
                } break;
        }

        // consumable successfully used
        return true;
    }
}
