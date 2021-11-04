using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnChance {
        public SpawnChance(GameObject enemy, float probability) {
            chance = probability;
            replacementEnemy = enemy;
        }

        public GameObject replacementEnemy;
        public float chance;
    }

    [SerializeField] bool isMelee;
    SpawnChance[] spawnChances;
    public void Awake() {
        spawnChances = GameManager.instance.GetSpawnChance(isMelee);

        /*
        foreach (SpawnChance spawn in spawnChances) {
            Debug.Log($"{spawn.replacementEnemy}, chance is {spawn.chance}");
        }
        */
    }

    void Start()
    {
        SpawnRandomEnemy();
    }

    void SpawnRandomEnemy() {
        float value = Random.value;
        float chance = 0;
        GameObject created = null;

        foreach (SpawnChance spawn in spawnChances) {
            chance += spawn.chance;
            if (value < chance) {
                if (spawn.replacementEnemy != null) {
                    created = Instantiate(spawn.replacementEnemy, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
            }
        }
    }
}
