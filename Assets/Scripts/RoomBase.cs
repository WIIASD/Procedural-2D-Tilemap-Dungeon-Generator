using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoomBase
{
    public static int IDCount = 0;
    public int ID;
    public Vector3Int GridPosition;// Position of topleft corner
    public Vector3 CenterWorldPosition;
    public Collider2D RoomCollder;
    public int Width, Height;
    public bool cleared = false;
    public int[,] GroundMatrix { get; protected set; }
    public int[,] WallMatrix { get; protected set; }

    protected RoomBase(Vector3Int pos, int w, int h)
    {
        initializeID();
        GridPosition = pos;
        Width = w;
        Height = h;
        GroundMatrix = GenerateRoomLayoutBaseMatrix();
        WallMatrix = GenerateRoomLayoutBaseMatrix();
        //DrawLayout();
    }

    private void initializeID()
    {
        ID = IDCount;
        IDCount++;
    }

    public virtual int[,] GenerateRoomLayoutBaseMatrix()
    {
        int[,] result = new int[Height, Width];
        ZTools.FillInt2DArray(result, -1);
        return result;
    }

    public virtual void DrawLayout(RoomTilePalette palette)
    {
        DrawGround(palette);
        DrawWall(palette);
    }

    public abstract void DrawGround(RoomTilePalette palette);
    public abstract void DrawWall(RoomTilePalette palette);

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
