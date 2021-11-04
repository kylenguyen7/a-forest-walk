using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MagnitudeLabelController : MonoBehaviour
{
    [SerializeField] string suffix;
    
    public string GetSuffix() {
        return suffix;
    }
}
