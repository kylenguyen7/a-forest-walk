using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Textbox : MonoBehaviour {
    // Textbox singleton
    public static Textbox instance;
    private DialogueObject currentDialogue;

    public GameObject dialogueArea;
    public GameObject responsesArea;

    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI speakerDialogue;
    public Image speakerSprite;

    public AudioSource textboxSounds;

    string currentText = "";
    int currentTextIndex = 0;

    public float typeDelay = 0.05f;
    public float typeDelayLong = 0.1f;

    public delegate void OnDialogueFinished();
    public OnDialogueFinished OnDialogueFinishedCallback;
    // public DialogueObject lastDialogue;

    enum states {
        typing,
        paused,
        inactive
    }
    states state = states.inactive;

    private void Awake() {
        if (instance != null) {
            Debug.LogError("More than one instance of Textbox found!");
            return;
        }

        instance = this;
    }

    private void Update() {
        switch (state) {
            case states.typing: {
                    if (Input.GetMouseButtonDown(0)) {
                        SkipToDialogueEnd();
                    }
                    break;
                }
            case states.paused: {
                    if (Input.GetMouseButtonDown(0)) {

                        // If there are responses, dialogue is ended by clicking,
                        // not by normal advance text method.
                        if (currentTextIndex == currentDialogue.lines.Length - 1 && currentDialogue.responses.Length == 0
                            && state == states.paused) {
                            EndDialogue(true);
                        } else {
                            currentTextIndex++;
                            StartCoroutine(TypeText());
                        }
                    }
                    break;
                }
        }

    }

    string mutedSounds = " ";
    string pauseSounds = ",!.?";
    IEnumerator TypeText() {
        state = states.typing;

        int i = 0;
        string line = currentDialogue.lines[currentTextIndex];
        while (i < line.Length) {

            string currentChar = line.Substring(i, 1);
            // Skipping over tags
            if (i != line.Length && currentChar == "<") {
                int closeBracket = line.IndexOf(">", i + 1);

                if (closeBracket != -1) {
                    i = closeBracket + 1;
                }
            }

            speakerDialogue.text = line.Substring(0, i + 1);

            // Exit condition, skip sounds
            if (i == line.Length - 1) {
                SkipToDialogueEnd();
                break;
            }

            // Mute when SFX is null or a space is being typed
            if (currentDialogue.talkingSFX != null && mutedSounds.IndexOf(currentChar) == -1) {
                textboxSounds.PlayOneShot(currentDialogue.talkingSFX);
            }

            // Delay longer for .,!
            if (pauseSounds.IndexOf(currentChar) == -1) {
                yield return new WaitForSeconds(typeDelay);
            } else {
                yield return new WaitForSeconds(typeDelayLong);
            }

            i++;
        }
    }

    // Called when one dialogue is finished, waits until input
    // To start next dialogue
    void SkipToDialogueEnd() {
        StopAllCoroutines();
        speakerDialogue.text = currentDialogue.lines[currentTextIndex];
        textboxSounds.PlayOneShot(currentDialogue.talkingSFX);
        state = states.paused;

        if (currentTextIndex == currentDialogue.lines.Length - 1) {
            Invoke("CreateResponses", 0f); // 0.1f second delay?
        }
    }

    void CreateResponses() {
        int i = 0;
        GameObject responsePrefab = Resources.Load("Prefabs/UI/Response", typeof(GameObject)) as GameObject;

        foreach (ResponseObject response in currentDialogue.responses) {
            GameObject currentResponse = Instantiate(responsePrefab);
            currentResponse.transform.parent = responsesArea.transform;
            var resp = currentResponse.GetComponent<RespManager>();
            resp.myResponse = response;
            resp.index = i;
            i++;
        }
    }

    // Called by RespManager, on click, which passes back the index
    // of which response was clicked
    public void ReceiveResponse(int index) {

        foreach (RespManager response in FindObjectsOfType<RespManager>()) {
            Destroy(response.gameObject);
        }

        if (index < 0 || index >= currentDialogue.responses.Length) {
            Debug.LogError($"Received an invalid response index: {index}");
            return;
        }

        DialogueObject toStart = currentDialogue.responses[index].result;
        
        

        // a null result field just closes the textbox
        if(toStart != null) {
            StartDialogue(toStart);
            EndDialogue(false);
        } else {
            EndDialogue(true);
        }
    }

    public void StartDialogue(DialogueObject newDialogue) {
        dialogueArea.GetComponent<Animator>().SetBool("isOpen", true);

        speakerName.text = newDialogue.speakerName;
        speakerSprite.sprite = newDialogue.portrait;
        currentDialogue = newDialogue;

        currentText = "";
        currentTextIndex = 0;

        StartCoroutine(TypeText());

        Debug.Log("Starting dialogue that begins " + newDialogue.lines[0].Substring(0, 10));
    }

    public void EndDialogue(bool finished) {
        state = states.inactive;

        if (currentDialogue.nextDialogue != null) {
            StartDialogue(currentDialogue.nextDialogue);
        }

        if (finished) {
            dialogueArea.GetComponent<Animator>().SetBool("isOpen", false);

            if(OnDialogueFinishedCallback != null)
                OnDialogueFinishedCallback.Invoke();
        } 
    }
}
