using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    public Dictionary<Upgrade, UpgradeInfo> upgradeData;

    public class UpgradeInfo {
        int maxLevel;
        public int currentLevel;
        float[] magnitudes;
        int[] costs;

        public UpgradeInfo(float[] levelMagnitudes, int[] levelCosts) {
            magnitudes = levelMagnitudes;
            maxLevel = levelMagnitudes.Length - 1;
            currentLevel = 0;
            costs = levelCosts;

            if(levelMagnitudes.Length - 1 != levelCosts.Length) {
                Debug.LogWarning($"There's probably a mismatched number of levels/costs. mag: {levelMagnitudes[0]}, {levelMagnitudes[1]}, {levelMagnitudes[2]}");
            }
        }

        public int MaxLevel {
            get { return maxLevel; }
        }

        public int CurrentLevel {
            get { return currentLevel; }
        }

        public int Cost {
            get {
                if (currentLevel == maxLevel) return -1;
                else return costs[currentLevel];
            }
        }



        public void LevelUp() {
            if(currentLevel == maxLevel) {
                Debug.LogError("Upgrade is already at max level!");
                return;
            }
            currentLevel += 1;
        }
        

        public float CurrentMagnitude {
            get { return GetMagnitude(currentLevel); }
        }

        public float NextMagnitude {
            get { return GetMagnitude(currentLevel + 1); }
        }

        public float GetMagnitude(int level) {
            if (level < 0 || level >= magnitudes.Length) {
                return -1f;
            } else {
                return magnitudes[level];
            }
        }
    }

    private void InitializeUpgradeData() {
        SetUpgradeData();
        UpdateAllUpgrades();
    }

    private void SetUpgradeData() {
        upgradeData = new Dictionary<Upgrade, UpgradeInfo>();

        float[] meleeSizes = { 1f, 1.25f, 1.5f, 1.75f, 2f };
        int[] costs1 = { 25, 50, 100, 200 };
        // int[] costs1 = { 0, 0, 0, 0 };
        upgradeData[Upgrade.meleeSize] = new UpgradeInfo(meleeSizes, costs1);

        float[] meleeDamages = { 1f, 1.25f, 1.5f, 1.75f, 2f, 3f };
        int[] costs2 = { 50, 100, 200, 300, 500 };
        upgradeData[Upgrade.meleeDamage] = new UpgradeInfo(meleeDamages, costs2);

        float[] meleeSlows = { 0f, 0.25f, 0.5f, 1f };
        int[] costs3 = { 100, 250, 500 };
        upgradeData[Upgrade.meleeSlow] = new UpgradeInfo(meleeSlows, costs3);

        float[] rangedAttackSpeeds = { 1f, 1.15f, 1.3f, 1.45f, 1.6f, 1.8f, 2f, 2.75f, 3f };
        int[] costs4 = { 50, 100, 150, 200, 250, 300, 350, 400 };
        upgradeData[Upgrade.rangedAttackSpeed] = new UpgradeInfo(rangedAttackSpeeds, costs4);

        float[] rangedAmmos = { 1f, 2f, 3f, 5f, 7f, 9f };
        int[] costs5 = { 100, 150, 200, 300, 400 };
        upgradeData[Upgrade.rangedAmmo] = new UpgradeInfo(rangedAmmos, costs5);

        float[] rangedPosions = { 0f, 0.15f, 0.30f, 0.45f, 0.6f, 0.75f, 1f };
        int[] costs6 = { 100, 200, 300, 350, 400, 450 };
        upgradeData[Upgrade.rangedPoison] = new UpgradeInfo(rangedPosions, costs6);

        float[] critChances = { 0f, 0.05f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f };
        int[] costs7 = { 25, 50, 75, 100, 200, 300 };
        upgradeData[Upgrade.critChance] = new UpgradeInfo(critChances, costs7);

        float[] lifeTotals = { 10f, 20f, 30f, 40f, 60f, 80f };
        int[] costs8 = { 75, 150, 300, 500, 750 };
        upgradeData[Upgrade.lifeTotal] = new UpgradeInfo(lifeTotals, costs8);
    }


    Upgrade[] allUpgrades = {Upgrade.meleeSize, Upgrade.meleeDamage, Upgrade.meleeSlow, Upgrade.rangedAttackSpeed, Upgrade.rangedAmmo,
                             Upgrade.rangedPoison, Upgrade.critChance, Upgrade.lifeTotal };
    public void UpdateAllUpgrades() {
        foreach(Upgrade upgrade in allUpgrades) {
            UpdateUpgrade(upgrade);
        }
    }

    public void AttemptUpgrade(Upgrade upgrade) {
        int cost = GetUpgradeCost(upgrade);
        if (cost <= PlayerController.gold) {
            PlayerController.gold -= cost;
            LevelUpUpgrade(upgrade);
        } else {
            Debug.Log("Not enough gold!");
        }
    }

    public void LevelUpUpgrade(Upgrade upgrade) {
        upgradeData[upgrade].LevelUp();
        UpdateUpgrade(upgrade);
    }

    public int GetUpgradeLevel(Upgrade upgrade) {
        return upgradeData[upgrade].CurrentLevel;
    }

    public float GetUpgradeMagnitude(Upgrade ugprade) {
        return upgradeData[ugprade].CurrentMagnitude;
    }

    public float GetUpgradeNextMagnitude(Upgrade ugprade) {
        return upgradeData[ugprade].NextMagnitude;
    }

    public int GetUpgradeNumLevels(Upgrade upgrade) {
        return upgradeData[upgrade].MaxLevel + 1;
    }

    public int GetUpgradeCost(Upgrade upgrade) {
        return upgradeData[upgrade].Cost;
    }

    public void UpdateUpgrade(Upgrade upgrade) {
        switch(upgrade) {
            case Upgrade.meleeSize: { SIZE_MODIFIER = upgradeData[upgrade].CurrentMagnitude; } break;
            case Upgrade.meleeDamage: { DAMAGE_MODIFIER = upgradeData[upgrade].CurrentMagnitude; } break;
            case Upgrade.meleeSlow: { SWORD_SLOW_CHANCE = upgradeData[upgrade].CurrentMagnitude;} break;
            case Upgrade.rangedAttackSpeed: { AS_MODIFIER = upgradeData[upgrade].CurrentMagnitude; } break;
            case Upgrade.rangedAmmo: { MAX_AMMO = (int)upgradeData[upgrade].CurrentMagnitude; } break;
            case Upgrade.rangedPoison: { BOW_POISON_CHANCE = upgradeData[upgrade].CurrentMagnitude; } break;
            case Upgrade.critChance: { CRIT_MODIFIER = upgradeData[upgrade].CurrentMagnitude; } break;
            case Upgrade.lifeTotal: { MAX_HP = upgradeData[upgrade].CurrentMagnitude; } break;
        }
    }

    public enum Upgrade {
        meleeSize,
        meleeDamage,
        meleeSlow,
        rangedAttackSpeed,
        rangedAmmo,
        rangedPoison,
        critChance,
        lifeTotal,
    }
}
