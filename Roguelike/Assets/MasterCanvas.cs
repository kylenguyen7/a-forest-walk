using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterCanvas : MonoBehaviour
{
    #region singleton
    public static MasterCanvas instance;
    private void Awake() {
        if(instance != null) {
            Destroy(gameObject);
            return;
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if(GameManager.instance.PastLastLevel) {
            Destroy(gameObject);
        }
    }
}
