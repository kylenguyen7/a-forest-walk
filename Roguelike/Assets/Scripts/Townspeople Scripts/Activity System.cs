using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This is a data-handling system, which defines a Routine, which manages
 * an ordered list of Activities. These Activities each handle data for what
 * is necessary for that activity to execute. And they define what happens
 * during those Activities in an OnUpdate() method.
 * 
 * The OnUpdate() method should be called from an NPC each frame. The OnUpdate()
 * is also responsible for modifying the Routine's current Activity with
 * the Advance() method.
 */

public class Routine {
    List<Activity> myActivities;
    public Activity currentActivity;
    int index = -1;

    public NPC myNPC;

    // Stores the activity that is interrupted by a call
    // to Interrupt()
    Activity interruptedActivity = null;

    public Routine(List<Activity> routine, NPC npc) {
        myNPC = npc;
        myActivities = routine;

        foreach(Activity act in myActivities) {
            act.MyRoutine = this;
        }

        StartNextActivity();
    }

    public void Advance() {
        currentActivity.OnActivityEnd();
        StartNextActivity();
    }

    void StartNextActivity() {
        index += 1;

        // Reached end of Routine
        if(index >= myActivities.Count) {
            currentActivity = null;
            Debug.LogError($"{myNPC.gameObject.name} reached end of their Routine! They don't know what to do next :(");
            return;
        }

        currentActivity = myActivities[index];
        currentActivity.OnActivityStart();
        // Debug.Log($"Starting new activity of type: {currentActivity}");
    }

    // A method that starts a new Activity.
    // The new Activity is repsonsible for invoking the Resume() method
    // when it is done. Together, Interrupt() and Reusme() call the
    // OnActivityStart/End methods for interrupting and interrupted activities.
    public void Interrupt(Activity newActivity) {
        interruptedActivity = currentActivity;

        currentActivity.OnActivityEnd();
        currentActivity = newActivity;
        currentActivity.OnActivityStart();

        Debug.Log($"Interrupting a {interruptedActivity} activity.");
    }

    public void Resume() {

        if (interruptedActivity == null) return;

        currentActivity.OnActivityEnd();
        currentActivity = interruptedActivity;
        currentActivity.OnActivityStart();

        interruptedActivity = null;

        Debug.Log($"Resuming a {currentActivity} activity.");
    }
}

public interface Activity
{
    Routine MyRoutine { get; set; }

    void OnActivityStart();
    void OnActivityEnd();

    void OnUpdate();
}

public class Talk : Activity {
    public Routine MyRoutine { get; set; }
    DialogueObject myDialogue;

    public Talk(DialogueObject dialogue, Routine routine) {
        myDialogue = dialogue;
        MyRoutine = routine;
    }

    public void OnActivityEnd() {
        CameraTargetControllerManual.instance.ResetFocus();
    }

    public void OnActivityStart() {
        Textbox.instance.StartDialogue(myDialogue);
        Textbox.instance.OnDialogueFinishedCallback += Finish;

        Vector2 move = (PlayerController.instance.transform.position - MyRoutine.myNPC.transform.position).normalized;
        MyRoutine.myNPC.GetComponent<Animator>().SetFloat("lookingHorizontal", move.x);
        MyRoutine.myNPC.GetComponent<Animator>().SetFloat("lookingVertical", move.y);

        CameraTargetControllerManual.instance.SetFocus(MyRoutine.myNPC.transform, new Vector2(0, -3));
    }

    void Finish() {
        MyRoutine.Resume();
    }

    public void OnUpdate() {
        // Do nothing
    }
}


/* An Activity that sets an animation and waits for a certain
 * ClockTime to pass, at which point the Routine is advanced.
 */
public class Wait : Activity {
    ClockTime myEndTime;
    string myAnimTrigger;
    public Routine MyRoutine { get; set; }

    // TODO: implement Idle object which waits durationMinutes instead
    // of for a certain ClockTime
    // int myDurationMinutes;
    // myDurationMinutes = durationMinutes;
    public Wait(ClockTime endTime, string animTrigger) {
        myEndTime = endTime;
        myAnimTrigger = animTrigger;
    }

    public void OnActivityStart() {
        if (myAnimTrigger == null) return;

        MyRoutine.myNPC.GetComponent<Animator>().SetTrigger(myAnimTrigger);
    }

    public void OnActivityEnd() {
        MyRoutine.myNPC.GetComponent<Animator>().SetTrigger("reset");
    }

    public void OnUpdate() {
        if(GlobalTime.instance.currentTime.hour >= myEndTime.hour
            && GlobalTime.instance.currentTime.minute >= myEndTime.minute) {

            MyRoutine.Advance();
            // Debug.Log("Advancing past wait activity.");
        }
    }
}

/* An Activity that follows an ordered List of Transforms,
 * calling NPC's MoveTowards() to each Transform until reaching
 * the final Transform, at which point the Routine is advanced.
 */
public class Head : Activity {
    List<Transform> myDestinations;
    Transform currentDestination;
    int index = 0;
    public Routine MyRoutine { get; set; }

    public Head(List<Transform> destinations) {
        myDestinations = destinations;
        currentDestination = destinations[index];
    }

    public void OnActivityEnd() {
        MyRoutine.myNPC.GetComponent<Animator>().SetBool("isWalking", false);
    }

    public void OnActivityStart() {
        MyRoutine.myNPC.GetComponent<Animator>().SetBool("isWalking", true);
    }

    public void OnUpdate() {
        if (currentDestination == null || Vector3.Distance(MyRoutine.myNPC.transform.position, currentDestination.position) < 0.1) {
            index += 1;

            if (index >= myDestinations.Count) {
                MyRoutine.Advance();
                return;
            }
            currentDestination = myDestinations[index];
        }
        MyRoutine.myNPC.Move(currentDestination);
    }
}