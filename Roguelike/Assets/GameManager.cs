using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public partial class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject message;
    GameObject UI;

    public static float SIZE_MODIFIER = 1f;
    public static float DAMAGE_MODIFIER = 1f;
    public static float AS_MODIFIER = 1f;
    public static float CRIT_MODIFIER = 1f; 

    public static float SWORD_SLOW_CHANCE = 0f;
    public static float BOW_POISON_CHANCE = 0f;

    public static float MAX_HP = 20f;
    public static int MAX_AMMO = 1;

    private int currentLevel = 0;

    private float trueHP;

    public float TrueHP {
        get { return trueHP; }
    }


    public int CurrentLevel {
        get { return currentLevel; }
        set { currentLevel = value; }
    }

    private int statTotalGold;

    public int StatTotalGold {
        get { return statTotalGold; }
        set { statTotalGold = value; }
    }

    private int statTotalDeaths;

    public int StatTotalDeaths {
        get { return statTotalDeaths; }
        set { statTotalDeaths = value; }
    }

    private float statTotalTime;

    public int StatTotalTime {
        get { return (int)statTotalTime; }
    }


    private int numLevels = 3;

    public bool OnFirstLevel { get => CurrentLevel == 0; }
    public bool OnLastLevel { get => CurrentLevel == numLevels - 1; }
    public bool PastLastLevel { get => CurrentLevel == numLevels; }

    private void Awake() {
        if(GameManager.instance != null) {
            Debug.LogError("More than one GameManager found!");
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        GameManager.instance = this;

        InitializeUpgradeData();
        trueHP = MAX_HP;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Debug.Log($"Current level is {currentLevel}");
        UI = MasterCanvas.instance.gameObject;
        if (UI == null && !PastLastLevel) {
            Debug.LogError("GameManager failed to find UI on a non-end level.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // DevTools();

        if (Input.GetKeyDown(KeyCode.R) && PastLastLevel) {
            Debug.Log("Restarting game!");

            MusicPlayer.instance.Pause();
            currentLevel = 0;
            trueHP = MAX_HP;
            SceneManager.LoadScene("procedural generators only!");
        }

        if (!PastLastLevel) {
            statTotalTime += Time.deltaTime;
        }
        trueHP = PlayerController.instance.hp;
    }

    private void DevTools() {
        
        if (Input.GetKeyDown(KeyCode.R)) {
            Debug.Log("Restarting game!");

            MusicPlayer.instance.Pause();
            currentLevel = 0;
            trueHP = MAX_HP;
            SceneManager.LoadScene("procedural generators only!");
        }

        if(Input.GetKeyDown(KeyCode.T)) {
            foreach(var test in GameObject.FindGameObjectsWithTag("Enemy")) {
                if(test.GetComponent<Enemy>() != null) {
                    test.GetComponent<Enemy>().OnDeath(true, true);
                } else {
                    Destroy(test);
                }
            }
        }

        /*
        if (Input.GetKeyDown(KeyCode.Escape)) {
            MenuManager.instance.ToggleMenu(MenuManager.menus.inventory);
        }
        */

        /*
        if(Input.GetKeyDown(KeyCode.Q)) {
            MenuManager.instance.ToggleMenu(MenuManager.menus.shop);
        }
        */

        /*
        if (Input.GetKeyDown(KeyCode.U)) {
            MenuManager.instance.ToggleMenu(MenuManager.menus.upgrades);
        }
        */

        /*
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SceneManager.LoadScene("procedural generators only!");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SceneManager.LoadScene("townspeople only!");
        }
        */
    }

    public void CreateMessage(string text) {
        UI = MasterCanvas.instance.gameObject;

        var msg = Instantiate(message, UI.transform);
        var texts = msg.GetComponentsInChildren<TextMeshProUGUI>();

        foreach(TextMeshProUGUI txt in texts) {
            txt.text = text;
        }
    }

    public void CreateLevelMessage() {
        string msg = "";
        switch(currentLevel) {
            case 0: msg = "Forest's Edge (1/3)"; break;
            case 1: msg = "The Forest (2/3)"; break;
            case 2: msg = "The Glade (3/3)"; break;
            // case 3: msg = "Deep Forest (4/5)"; break;
            // case 4: msg = "The Glade (5/5)"; break;
        }

        CreateMessage(msg);
    }

    public void RestartSequence() {
        CreateMessage("~ YOU DIED ~");
        currentLevel = 0;
        StartCoroutine(PromptUpgrades());
    }

    IEnumerator PromptUpgrades() {
        yield return new WaitForSecondsRealtime(1.5f);
        MusicPlayer.instance.PauseThenPlay(MusicPlayer.Track.shopTheme);
        yield return new WaitForSecondsRealtime(1.5f);
        MenuManager.instance.ToggleMenu(MenuManager.menus.upgrades);
    }

    [SerializeField] GameObject skeleton;
    [SerializeField] GameObject slime;
    [SerializeField] GameObject mandrake;
    [SerializeField] GameObject archer;
    [SerializeField] GameObject lumi;
    public test.SpawnChance[] GetSpawnChance(bool melee) {
        if(melee) {
            float skeletonChance = 0f;
            float slimeChance = 0f;
            float mandrakeChance = 0f;

            slime = null;

            switch(currentLevel) {
                case 0: { skeletonChance = 0.5f; slimeChance = 0.5f; mandrakeChance = 0f; }; break;
                case 1: { skeletonChance = 0.5f; slimeChance = 0.25f; mandrakeChance = 0.25f; }; break;
                case 2: { skeletonChance = 0.33f; slimeChance = 0.33f; mandrakeChance = 0.33f; }; break;
                case 3: { skeletonChance = 0.2f; slimeChance = 0.4f; mandrakeChance = 0.4f; }; break;
                case 4: { skeletonChance = 0.1f; slimeChance = 0.25f; mandrakeChance = 0.65f; }; break;
            }

            test.SpawnChance[] chance = { new test.SpawnChance(skeleton, skeletonChance), new test.SpawnChance(slime, slimeChance),
                                          new test.SpawnChance(mandrake, mandrakeChance),  };

            return chance;
        } else {
            float archerChance = 0f;
            float lumiChance = 0f;

            archer = null;

            switch (currentLevel) {
                case 0: { archerChance = 1f; lumiChance = 0f; }; break;
                case 1: { archerChance = 0.75f; lumiChance = 0.25f; }; break;
                case 2: { archerChance = 0.5f; lumiChance = 0.5f; }; break;
                case 3: { archerChance = 0.4f; lumiChance = 0.6f; }; break;
                case 4: { archerChance = 0.3f; lumiChance = 0.7f; }; break;
            }

            test.SpawnChance[] chance = { new test.SpawnChance(archer, archerChance), new test.SpawnChance(lumi, lumiChance) };

            return chance;
        }
    }
}
