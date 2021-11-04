using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndScreenStatsController : MonoBehaviour
{
    TextMeshProUGUI myText;

    private void Awake() {
        myText = gameObject.GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        if(GameManager.instance == null) {
            Debug.LogError("End credits stats object could not find GameManager instance!");
        }
    }

    private void Update() {
        SetText();
    }

    private void SetText() {
        int seconds = GameManager.instance.StatTotalTime;
        int minutes = seconds / 60;
        seconds = seconds % 60;

        string secondsString = seconds < 10 ? "0" + seconds.ToString() : seconds.ToString();

        int deaths = GameManager.instance.StatTotalDeaths;
        int gold = GameManager.instance.StatTotalGold;

        myText.text = $"Time: {minutes}:{secondsString} minutes\nDeaths: {deaths}\nTotal Gold: {gold}";
    }
}
