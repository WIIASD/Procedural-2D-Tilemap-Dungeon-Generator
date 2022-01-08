using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ZTools
{
    public static bool InRangeInclusive(float value, float min, float max)
    {
        float realMin = Mathf.Min(min, max);
        float realMax = Mathf.Max(min, max);
        return (value >= realMin && value <= realMax);
    }
}
