using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LevelJSONWriter : LevelJSON
{
    public string levelName = "NO_NAME";
    LevelObjects myLevelObjects = new LevelObjects();
    int index = 0;

    private void Start() {
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        // Get path
        string path = "LevelGen/" + collision.gameObject.transform.root.name.Split(' ')[0];

        // Get position rounded to nearest 0.5
        //float roundX = collision.transform.position.x - (collision.transform.position.x % 0.5f);
        //float roundY = collision.transform.position.y - (collision.transform.position.y % 0.5f);
        //Vector2 position = new Vector2(roundX, roundY);

        Vector2 position = new Vector2(collision.transform.position.x, collision.transform.position.y);

        LevelObject obj = new LevelObject(path, position);
        myLevelObjects.objects.Add(obj);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            string json = JsonUtility.ToJson(myLevelObjects);
            Debug.Log(json);

            string path = Application.dataPath + "/LevelsJSONData/";
            File.WriteAllText(path + levelName, json);
            Debug.Log("Writing \"" + levelName + "\"" + " to path: " + path);
        }
    }
}
