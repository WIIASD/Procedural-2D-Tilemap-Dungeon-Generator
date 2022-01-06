using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room
{
    public static int IDCount = 0;
    public int ID;
    public Vector3Int Position;
    public int Width, Height;
    public int[] openDirection = { 0, 0, 0, 0 };
    public Vector3 AreaColliderWorldPosition;
    public float AreaColliderWidth, AreaColliderHeight;
    public int walkAbleWidth { get; private set; }
    public int walkAbleHeight { get; private set; }
    public BoxCollider2D bc;
    public bool cleared = false;

    public Room(Vector3Int pos, int w, int h)
    {
        Position = pos;
        Width = w;
        Height = h;
        walkAbleWidth = w - 2;
        walkAbleHeight = h - 1;
        ID = IDCount;
        IDCount++;
        AreaColliderWidth = w - 2;
        AreaColliderHeight = h - 1;
        AreaColliderWorldPosition = Position + Vector3.right + new Vector3(AreaColliderWidth / 2, -AreaColliderHeight / 2, 0);
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType() != this.GetType())
        {
            return false;
        }
        Room r = (Room)obj;
        return r.ID==this.ID;
    }
}
