using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RectangleRoom : RoomBase
{
    
    public int[] openDirection = { 0, 0, 0, 0 };
    
    public float AreaColliderWidth, AreaColliderHeight;
    public int walkAbleWidth { get; private set; }
    public int walkAbleHeight { get; private set; }
    public bool cleared = false;

    public RectangleRoom(Vector3Int pos, int w, int h) : base(pos, w, h)
    {
        walkAbleWidth = w - 2;
        walkAbleHeight = h - 1;
        ID = IDCount;
        IDCount++;
        AreaColliderWidth = w - 2;
        AreaColliderHeight = h - 1;
        CenterWorldPosition = GridPosition + Vector3.right + new Vector3(AreaColliderWidth / 2, -AreaColliderHeight / 2, 0);
    }

    public override int[,]  GenerateGroundMatrix()
    {
        int[,] result = new int[Height, Width];
        ZTools.FillInt2DArray(result, 1);
        return result;
    }

    public override int[,] GenerateWallMatrix()
    {
        int[,] result = new int[Height, Width];
        return result;
    }
}
