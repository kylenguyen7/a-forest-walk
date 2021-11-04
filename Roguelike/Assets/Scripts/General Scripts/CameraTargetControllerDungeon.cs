using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTargetControllerDungeon : MonoBehaviour
{
    public PlayerController player;
    public float mouseFactor = 0.5f;

    Transform focus;
    Vector2Int currentRoomIndex = Vector2Int.zero;

    [SerializeField] bool useRoomFocuses = false;

    private void Start() {
        transform.position = PlayerController.instance.transform.position;
    }

    private float maxDistancePerFrame = 1.5f;
    // Update is called once per frame
    void Update()
    {
        if(focus == null) {
            UpdateFocus();
        }
        else {
            // Vector2 toTarget = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - focus.position);
            Vector2 toTarget = (player.transform.position - focus.position);
            Vector2 goal = (Vector2)focus.position + toTarget * mouseFactor;

            transform.position = (Vector2)transform.position +
                                  Vector2.ClampMagnitude(goal - (Vector2)transform.position, maxDistancePerFrame);
        }

        // Player has entered new room, reassign focus
        if(player.currentRoomIndex != currentRoomIndex) {
            UpdateFocus();
        }

        // Debug.Log($"Camera focus: {focus.gameObject.name}");
        
    }

    void UpdateFocus() {
        if(!useRoomFocuses) {
            focus = player.transform;
            return;
        }

        StopAllCoroutines();
        StartCoroutine(TransitionSequence());
    }

    IEnumerator TransitionSequence() {

        FindObjectOfType<CinemachineConfiner>().m_ConfineScreenEdges = false;
        yield return new WaitForEndOfFrame();

        var room = RoomManager.findRoomManagerByIndex(player.currentRoomIndex);
        currentRoomIndex = player.currentRoomIndex;
        focus = room.transform;
        // FindObjectOfType<CinemachineConfiner>().m_BoundingShape2D = room.roomCameraBounds;
        
        yield return new WaitForSeconds(1f);
        FindObjectOfType<CinemachineConfiner>().m_ConfineScreenEdges = true;
        // confiner.InvalidatePathCache();
    }
}
