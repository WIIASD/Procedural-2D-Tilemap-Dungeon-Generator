using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoomBase
{
    public static int IDCount = 0;
    public int ID;
    public Vector3Int GridPosition; // Position of topleft corner
    public Vector3 CenterWorldPosition;
    public Collider2D RoomCollder; 
    public int Width, Height;

    protected RoomBase(Vector3Int pos, int w, int h)
    {
        GridPosition = pos;
        Width = w;
        Height = h;
    }

    public abstract int[,] GenerateGroundMatrix();
    public abstract int[,] GenerateWallMatrix();

    public override bool Equals(object obj)
    {
        if (obj.GetType() != this.GetType())
        {
            return false;
        }
        RoomBase r = (RoomBase)obj;
        return r.ID == this.ID;
    }
}
