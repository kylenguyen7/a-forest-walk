using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownDoorController : MonoBehaviour
{
    [SerializeField] Transform destination;
    [SerializeField] PolygonCollider2D destinationBounds;


    private void OnTriggerEnter2D(Collider2D collision) {

        // Debug.Log($"{gameObject}: Collided with {collision.gameObject.name}");

        if (collision.gameObject.CompareTag("Player")) {
            TransportPlayer();
        } else if(collision.gameObject.CompareTag("NPC")) {
            TransportNPC(collision.gameObject);
        }
    }

    void TransportPlayer() {
        // Start animation
        PlayerController.instance.Stop();
        var transition = Utility.Transition();
        transition.OnHalfwayPointCallback += FinishTransition;
    }

    void FinishTransition() {
        PlayerController.instance.transform.position = destination.position;
        CameraTargetControllerManual.instance.SetBounds(destinationBounds);
        PlayerController.instance.Resume();
    }



    void TransportNPC(GameObject obj) {
        // Debug.Log($"Transporting {obj.name}");
        NPC npc = obj.GetComponent<NPC>();

        if(npc.CurrentActivity.GetType() != typeof(Head)) {
            Debug.LogError("NPC entered a door area without being in 'Head' mode. Undefined behavior.");
            return;
        }

        npc.gameObject.transform.position = destination.position;
        npc.SkipCurrentActivity();
    }
}
