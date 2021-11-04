using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class LevelEndController : MonoBehaviour
{
    public string targetScene;
    public string endScene;

    // Ignores collisions at the very beginning, just in case
    // this spawns on top of the player
    float bufferTime = 0.25f;
    bool active = true;

    private void Update() {
        bufferTime -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {

            // If entered in the first bufferTime, requires exiting before returning to activate
            if (bufferTime > 0) {
                active = false;
            }

            if(SceneManager.GetActiveScene().name == "menus only!") {
                TransitionController.instance.Transition(targetScene);
                MusicPlayer.instance.Pause();
                return;
            }

            if (active) {
                MusicPlayer.instance.Pause();
                if (GameManager.instance.OnLastLevel) {
                    TransitionController.instance.Transition(endScene);
                } else {
                    TransitionController.instance.Transition(targetScene);
                }

                GameManager.instance.CurrentLevel += 1;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            active = true;
        }
    }
}
