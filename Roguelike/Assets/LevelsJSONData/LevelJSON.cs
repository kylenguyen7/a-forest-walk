using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelJSON : MonoBehaviour
{
    [System.Serializable]
    protected class LevelObjects {
        public List<LevelObject> objects = new List<LevelObject>();
    }

    [System.Serializable]
    protected class LevelObject {
        public string prefabPath;
        public Vector2 position;

        public LevelObject(string path, Vector2 pos) {
            prefabPath = path;
            position = pos;
        }
    }
}
