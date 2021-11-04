using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public RoomManager.connections myConnection;
    public BoxCollider2D inactiveBarrier;
    bool isActive = false;
    SpriteRenderer mySprite;

    [SerializeField]
    Animator myArrow;

    // When entering door, must stay for 0.1 seconds to confirm
    float bufferTime = 0.1f;
    float bufferTimer;

    // Start is called before the first frame update
    void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();
        mySprite.enabled = false;

        bufferTimer = bufferTime;
    }

    public void Activate() {
        isActive = true;
        mySprite.enabled = true;
        inactiveBarrier.enabled = false;
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (!isActive) return;

        // wait bufferTime before transporting player
        if(collision.CompareTag("Player")) {
            if(bufferTimer <= 0) {
                TransportPlayer(collision.gameObject.GetComponent<PlayerController>());
            }
            bufferTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            bufferTimer = bufferTime;
        }
    }

    void TransportPlayer(PlayerController player) {
        switch (myConnection) {
            case RoomManager.connections.east: {
                    player.currentRoomIndex += new Vector2Int(1, 0);
                    break;
                }
            case RoomManager.connections.west: {
                    player.currentRoomIndex += new Vector2Int(-1, 0);
                    break;
                }
            case RoomManager.connections.north: {
                    player.currentRoomIndex += new Vector2Int(0, 1);
                    break;
                }
            case RoomManager.connections.south: {
                    player.currentRoomIndex += new Vector2Int(0, -1);
                    break;
                }
        }

        var newRoom = RoomManager.findRoomManagerByIndex(player.currentRoomIndex);

        player.transform.position = newRoom.GetEnterPosition(myConnection);
        PlayerController.instance.Stop();

        // coroutine resumes player control
        StartCoroutine("StartRoom");
    }

    Vector2 GetTransitionDirection(RoomManager.connections connection) {
        switch(connection) {
            case RoomManager.connections.east:  return new Vector2(1f, 0);
            case RoomManager.connections.north: return new Vector2(0, 1f);
            case RoomManager.connections.west:  return new Vector2(-1f, 0);
            case RoomManager.connections.south: return new Vector2(0, -1f);
        }

        return Vector2.zero;
    }

    public void ShowArrow() {
        myArrow.SetTrigger("ShowArrow");
    }

    public void HideArrow() {
        myArrow.SetTrigger("HideArrow");
    }

    IEnumerator StartRoom() {
        yield return new WaitForSeconds(0.4f);
        PlayerController.instance.Resume();
    }
}
