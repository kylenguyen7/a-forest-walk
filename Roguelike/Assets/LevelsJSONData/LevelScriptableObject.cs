using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Level_DESC", menuName = "ScriptableObjects/LevelScriptableObject", order = 5)]
public class LevelScriptableObject : ScriptableObject
{
    public string baseLevelPath;
    public string[] wavePaths;
}
