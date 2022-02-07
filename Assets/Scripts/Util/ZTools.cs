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

    public static int[,] FillInt2DArray(int[,] array, int value)
    {
        for (int y = 0; y < array.GetLength(0); y++)
        {
            for (int x = 0; x < array.GetLength(1); x++)
            {
                array[y, x] = value;
            }
        }
        return array;
    }

    #region Debug
    public static void PrintInt2DArray(int[,] array)
    {
        string result = "";
        for (int y = 0; y < array.GetLength(0); y++)
        {
            for (int x = 0; x < array.GetLength(1); x++)
            {
                result += array[y, x] + " ";
            }
            result += "\n";
        }
        Debug.Log(result);
    }

    #endregion
}
