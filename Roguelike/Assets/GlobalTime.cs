using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ClockTime {
    public int hour;
    public int minute;
}

public class GlobalTime : MonoBehaviour
{
    #region singleton
    public static GlobalTime instance;
    private void Awake() {
        if(instance) {
            Debug.LogError("More than on GlobalTime object found!");
            return;
        }
        instance = this;
        currentTime = start;
    }
    #endregion

    float secondsInDayRealtime = 120;
    float realtimeSecondsPerMinute; // Calculated based on day start and end times

    ClockTime start = new ClockTime { hour = 7, minute = 0 };
    ClockTime end = new ClockTime { hour = 2, minute = 0 };
    public ClockTime currentTime;

    private void Start() {
        float hours = end.hour - start.hour;
        if(hours <= 0) {
            hours += 24;
        }

        float minutes = end.minute - start.minute;
        if(minutes < 0) {
            minutes += 60;
            hours -= 1;
        }

        Debug.Log("There are " + hours + " hours and " + minutes + " minutes in this day.");

        realtimeSecondsPerMinute = secondsInDayRealtime / (hours * 60 + minutes);
        StartCoroutine(CountTime());
    }

    IEnumerator CountTime() {
        // Debug.Log("The current time is " + currentTime.hour + ":" + currentTime.minute);
        yield return new WaitForSecondsRealtime(realtimeSecondsPerMinute);
        currentTime.minute += 1;
        if(currentTime.minute == 60) {
            currentTime.minute = 0;
            currentTime.hour += 1;
        }

        StartCoroutine(CountTime());
    }

}
