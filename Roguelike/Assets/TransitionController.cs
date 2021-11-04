using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(Animator))]
public class TransitionController : MonoBehaviour {
    [SerializeField] Texture2D defaultCursor;

    #region singleton
    public static TransitionController instance;

    // For menu and final screen music
    public MusicPlayer.Track playOnStart;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }

        animator = GetComponent<Animator>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        introText.text = "";
        introDeathCount.text = "";
        instance = this;
        DontDestroyOnLoad(transform.parent);
    }
    #endregion

    private void Start() {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    string[] messages = {
        "Stay safe!",
        "Hurry home!",
        "Down this path again...",
        "For pinky...",
        "Down this path again...",
        "Gotta get home",
        "Another run"
    };

    Animator animator;
    [SerializeField] TextMeshProUGUI introText;
    [SerializeField] TextMeshProUGUI introDeathCount;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        animator.SetTrigger("restart");

        if(SceneManager.GetActiveScene().name == "menus only!") {
            StartCoroutine(MenuSequence());
        }
        else if(SceneManager.GetActiveScene().name == "people who finish their games only!") {
            StartCoroutine(EndSequence());
        }
        else {
            if(GameManager.instance.StatTotalDeaths == 0) {
                StartCoroutine(IntroSequence("Good luck!", GameManager.instance.OnFirstLevel));
            } else {
                StartCoroutine(IntroSequence(messages[Random.Range(0, messages.Length - 1)], GameManager.instance.OnFirstLevel));
            }
        }
    }

    IEnumerator MenuSequence() {
        yield return new WaitForSecondsRealtime(0.5f);

        if (animator != null) {
            animator.SetTrigger("fade out");
        }
    }

    IEnumerator EndSequence() {
        yield return new WaitForSecondsRealtime(2f);

        if (GameManager.instance.OnLastLevel) {
            MusicPlayer.instance.Play(MusicPlayer.Track.lastLevelTheme);
        } else if (GameManager.instance.PastLastLevel) {
            MusicPlayer.instance.Play(MusicPlayer.Track.credits);
        } else {
            MusicPlayer.instance.Play(MusicPlayer.Track.mainTheme);
        }


        if (animator != null) {
            animator.SetTrigger("fade out");
        }
    }

    IEnumerator IntroSequence(string message, bool doMessage) {
        introText.text = "";
        yield return new WaitForEndOfFrame();

        Time.timeScale = 0f;
        int index = 0;

        if(doMessage) {
            yield return new WaitForSecondsRealtime(2f);

            string currentText = "";
            while (index < message.Length) {
                currentText += message[index];
                index += 1;
                introText.text = currentText;
                yield return new WaitForSecondsRealtime(0.05f);
            }

            if (GameManager.instance.StatTotalDeaths > 0) {
                yield return new WaitForSecondsRealtime(0.15f);

                string deathMessage = $"Total deaths: {GameManager.instance.StatTotalDeaths}";
                index = 0;

                currentText = "";
                while (index < deathMessage.Length) {
                    currentText += deathMessage[index];
                    index += 1;
                    introDeathCount.text = currentText;
                    yield return new WaitForSecondsRealtime(0.02f);
                }
            }
        }
        

        yield return new WaitForSecondsRealtime(2f);

        if (animator != null) {
            animator.SetTrigger("fade out");
        }

        if (GameManager.instance.OnLastLevel) {
            MusicPlayer.instance.Play(MusicPlayer.Track.lastLevelTheme);
        }
        else if (GameManager.instance.PastLastLevel) {
            MusicPlayer.instance.Play(MusicPlayer.Track.credits);
        }
        else {
            MusicPlayer.instance.Play(MusicPlayer.Track.mainTheme);
        }

        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1f;

        yield return new WaitForSecondsRealtime(1f);
        GameManager.instance.CreateLevelMessage();
    }

    public void Transition(string targetScene) {
        StartCoroutine(TransitionSequence(targetScene));
    }

    IEnumerator TransitionSequence(string targetScene) {
        PlayerController.instance.Stop();
        introText.text = "";
        introDeathCount.text = "";
        animator.SetTrigger("fade in");
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadScene(targetScene);
    }
}
