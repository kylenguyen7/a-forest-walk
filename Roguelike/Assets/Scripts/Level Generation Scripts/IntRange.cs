using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntRange
{
    int minNum;
    int maxNum;
    public IntRange(int min, int max) {
        minNum = min;
        maxNum = max;
    }

    public int Num() {
        return Random.Range(minNum, maxNum + 1);
    }
}
