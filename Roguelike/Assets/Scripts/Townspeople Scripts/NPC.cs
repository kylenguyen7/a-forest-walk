using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NPC : MonoBehaviour
{
    Vector2 looking;
    BoxCollider2D myCollider;

    Routine myRoutine;
    public List<Transform> headOne;
    public List<Transform> headTwo;
    public List<Transform> headThree;


    public Activity CurrentActivity {
        get { return myRoutine.currentActivity; }
    }


    public DialogueObject myDialogue;

    List<GameObject> currentCollisions = new List<GameObject>();

    [SerializeField] GameObject collisionProjection;

    // Start is called before the first frame update
    void Start()
    {
        List<Activity> testActivities = new List<Activity>();
        testActivities.Add(new Head(headOne));
        if (Random.value < 0.5f) AddRandomWait(testActivities);
        testActivities.Add(new Head(headTwo));
        if (Random.value < 0.5f) AddRandomWait(testActivities);
        testActivities.Add(new Head(headThree));

        myRoutine = new Routine(testActivities, this);
    }

    void AddRandomWait(List<Activity> testActivities) {
        testActivities.Add(new Wait(new ClockTime {
            hour = GlobalTime.instance.currentTime.hour + 1,
            minute = Random.Range(0, 59) }, null));
    }

    // Update is called once per frame
    void Update()
    {
        // Usually only happens if the Routine runs out of Activities.
        // Which shouldn't happen, anyway
        if (myRoutine.currentActivity == null) return;

        // Debug.Log(myRoutine.currentActivity);
        myRoutine.currentActivity.OnUpdate();
    }

    public float moveSpeed;
    enum moveStates {
        moving,
        stalled,
        dashing
    }
    moveStates moveState = moveStates.moving;

    float resumeMovingTime = 0.25f;
    float resumeMovingTimer = 0;
    public void Move(Transform destination) {
        switch(moveState) {
            case moveStates.moving: {
                    if(WillCollide()) {
                        moveState = moveStates.stalled;
                        resumeMovingTimer = resumeMovingTime;
                        GetComponent<Animator>().SetBool("isWalking", false);
                        return;
                    }

                    MoveTowards(destination, 1f);

                } break;
            case moveStates.stalled: {
                    #region resume moving
                    // Counter to resume moving; no collisions have to be detected for
                    // 0.25 seconds
                    if (!WillCollide()) {
                        resumeMovingTimer -= Time.deltaTime;
                    } else {
                        resumeMovingTimer = resumeMovingTime;
                    }

                    if(resumeMovingTimer <= 0) {
                        moveState = moveStates.moving;
                        GetComponent<Animator>().SetBool("isWalking", true);
                    }
                    #endregion

                    #region start dashing
                    #endregion
                }
                break;
            case moveStates.dashing: {
                    MoveTowards(destination, 1.5f);
                }
                break;
        }

        collisionProjection.transform.position = (Vector2)transform.position + GetMovement(destination, 2f);
    }

    private void MoveTowards(Transform destination, float speedFactor) {
        Vector2 move = GetMovement(destination, speedFactor);
        transform.position = (Vector2)transform.position + move;

        GetComponent<Animator>().SetFloat("lookingHorizontal", move.x);
        GetComponent<Animator>().SetFloat("lookingVertical", move.y);
    }
    
    private Vector2 GetMovement(Transform destination, float speedFactor) {
        return moveSpeed * speedFactor * Time.deltaTime * (destination.position - transform.position).normalized;
    }

    private bool WillCollide() {
        // return currentCollisions.Count > 0;
        return collisionProjection.GetComponent<ColliderProjection>().IsColliding();
    }
    

    private void OnCollisionEnter2D(Collision2D collision) {
        currentCollisions.Add(collision.gameObject);
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if(currentCollisions.Contains(collision.gameObject)) {
            currentCollisions.Remove(collision.gameObject);
        }
    }

    public void Speak() {
        myRoutine.Interrupt(new Talk(myDialogue, myRoutine));
    }

    public void SkipCurrentActivity() {
        myRoutine.Advance();
    }
}
