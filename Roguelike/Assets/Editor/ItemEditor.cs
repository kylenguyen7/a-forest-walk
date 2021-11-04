using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        Item item = target as Item;
        if(item.myCategory == Item.Category.Consumable) {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Consumable Type");
            item.myConsumableType = (Item.ConsumableType)EditorGUILayout.EnumPopup(item.myConsumableType);
        }

        if(item.myConsumableType == Item.ConsumableType.spawn) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Object to Spawn");
            item.objectToSpawn = EditorGUILayout.ObjectField(item.objectToSpawn, typeof(GameObject), false) as GameObject;
            EditorGUILayout.EndHorizontal();
        }

        if (item.myConsumableType == Item.ConsumableType.heal) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Health Restored");
            item.healthRestored = EditorGUILayout.FloatField(item.healthRestored);
            EditorGUILayout.EndHorizontal();
        }
    }
}
