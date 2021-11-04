using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickupManager : MonoBehaviour
{
    #region singleton
    public static PickupManager instance;
    public void Awake() {
        if(instance != null) {
            Debug.LogError("More than one PickupManager detected!");
            return;
        }
        instance = this;
    }
    #endregion

    Dictionary<Item, int> pickupHistory = new Dictionary<Item, int>();
    Dictionary<Item, Coroutine> pickupCoroutines = new Dictionary<Item, Coroutine>();

    public void CreatePickupText(Item item, Vector2 position) {

        if(item == null) {
            Debug.LogError("Attempted to create a pickup text with a null Item.");
            return;
        }

        // Increment item and create text object
        if (!pickupHistory.ContainsKey(item)) {
            pickupHistory[item] = 0;
        }
        pickupHistory[item] += 1;

        GameObject prefab = Resources.Load("Prefabs/Rising Text", typeof(GameObject)) as GameObject;
        var text = Instantiate(prefab, position, Quaternion.identity);

        foreach(TextMeshPro tmp in text.GetComponentsInChildren<TextMeshPro>()) {
            tmp.text = $"x{pickupHistory[item]} {item.ID}";
        }

        // Start new reset Coroutine, or reset an existing one
        if (!pickupCoroutines.ContainsKey(item)) {
            pickupCoroutines[item] = StartCoroutine(RemoveItem(item));
        } else {
            StopCoroutine(pickupCoroutines[item]);
            pickupCoroutines[item] = StartCoroutine(RemoveItem(item));
        }
    }

    IEnumerator RemoveItem(Item item) {
        yield return new WaitForSeconds(2f);
        pickupHistory.Remove(item);
        pickupCoroutines.Remove(item);
    }
}
