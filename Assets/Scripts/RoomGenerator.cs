using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Sides{
    Left,Right,Bottom,Top
}

public class RoomGenerator : MonoBehaviour
{
    public Tile Ground, GroundLeft, GroundRight, GroundLeftCorner,
                GroundRightCorner, GroundTop,
                WallFace, Wallleft, Wallright, WallTop, 
                WallBottom, WallTopLeftCorner, WallTopRightCorner, WallTopLeftCorner2, WallTopRightCorner2,
                WallBottomLeftOutterCorner, WallBottomRightOutterCorner, 
                WallBottomLeftInnerCorner, WallBottomRightInnerCorner;

    public Tilemap GroundTilemap, WallTilemap;

    public List<Room> Rooms;
    public GameObject RoomAreaColliderHolder;

    public int MinRoomWidth, MinRoomHeight, MaxRoomWidth, MaxRoomHeight;

    private void Start()
    {
        Rooms = new List<Room>();
        RoomAreaColliderHolder = new GameObject();
        RoomAreaColliderHolder.name = "RoomAreaColliderHolder";
        //Room r = GenerateRoom(Vector3Int.zero, 10, 10, new int[] { 0, 0, 0, 0 });
        ////GenerateNeighborRoom(r, 20, 15, Sides.Bottom);
        //GenerateNeighborRoom(r, 6, 1, Sides.Left);
        //GenerateNeighborRoom(r, 6, 1, Sides.Right);
        //GenerateNeighborRoom(r, 14, 10, Sides.Top);
    }

    internal void Reset()
    {
        Rooms = new List<Room>();
        Destroy(RoomAreaColliderHolder);
        RoomAreaColliderHolder = new GameObject();
        RoomAreaColliderHolder.name = "RoomAreaColliderHolder";
        Room.IDCount = 0;
        GroundTilemap.ClearAllTiles();
        WallTilemap.ClearAllTiles();
    }

    //TODO: simplfy code
    public Room GenerateRoom(Vector3Int position, int w, int h, int[] openDirection)
    {
        if (w < MinRoomWidth) w = MinRoomWidth;
        if (w > MaxRoomWidth) w = MaxRoomWidth;
        if (h < MinRoomHeight) h = MinRoomHeight;
        if (h > MaxRoomHeight) h = MaxRoomHeight;
        Room r = new Room(position, w, h);
        r.openDirection = openDirection;
        if (!CanGenerate(r.Position, r.Width, r.Height)) return null;
        Vector3Int pos = new Vector3Int(r.Position.x + 1, r.Position.y - 2, 0);
        GeneratePlane(GroundTilemap, Ground, pos, r.Width - 2, r.Height - 2);
        GeneratePlane(GroundTilemap, GroundTop, pos, r.Width -2, 1);
        GeneratePlane(GroundTilemap, GroundLeft, pos, 1, r.Height - 2);
        GeneratePlane(GroundTilemap, GroundRight, pos + new Vector3Int(r.Width - 3, 0, 0), 1, r.Height - 2);
        GroundTilemap.SetTile(pos, GroundLeftCorner);
        GroundTilemap.SetTile(pos + new Vector3Int(r.Width - 3, 0, 0), GroundRightCorner);
        GenerateRoomWalls(WallTilemap, r.Position, r.Width, r.Height);
        if (r.openDirection[0] == 1) OpenExitVertical(r, Sides.Left);
        if (r.openDirection[1] == 1) OpenExitVertical(r, Sides.Right);
        if (r.openDirection[2] == 1) OpenExitHorizontal(r, Sides.Bottom);
        if (r.openDirection[3] == 1) OpenExitHorizontal(r, Sides.Top);
        CreateRoomCollider(r);
        Rooms.Add(r);
        return r;
    }

    private void CreateRoomCollider(Room r)
    {
        GameObject colliderObj = new GameObject();
        colliderObj.transform.position = r.AreaColliderWorldPosition;
        colliderObj.transform.parent = RoomAreaColliderHolder.transform;
        colliderObj.name = "AreaCollider_Room_" + r.ID;
        colliderObj.tag = "RoomArea";
        BoxCollider2D collider = colliderObj.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(r.AreaColliderWidth, r.AreaColliderHeight - 0.4f);
        collider.offset = Vector3.up * 0.2f;
        collider.isTrigger = true;
        r.bc = collider;
    }

    public Room GenerateNeighborRoom(Room r, int w, int h, Sides dir)
    {
        if (w < MinRoomWidth) w = MinRoomWidth;
        if (w > MaxRoomWidth) w = MaxRoomWidth;
        if (h < MinRoomHeight) h = MinRoomHeight;
        if (h > MaxRoomHeight) h = MaxRoomHeight;
        Room neighbor = null;
        if (dir == Sides.Left)
        {

            Vector3Int pos = new Vector3Int(r.Position.x - w, r.Position.y + h/2 - r.Height/2, 0);
            neighbor = GenerateRoom(pos, w, h, new int[] { 0, 1, 0, 0 });
            if (neighbor != null)
            {
                OpenExitVertical(r, Sides.Left);
            }
        }
        if (dir == Sides.Right)
        {
            Vector3Int pos = new Vector3Int(r.Position.x + r.Width, r.Position.y + h/2 - r.Height/2, 0);
            neighbor = GenerateRoom(pos, w, h, new int[] { 1, 0, 0, 0 });
            if (neighbor != null)
            {
                OpenExitVertical(r, Sides.Right);
            }
        }
        if (dir == Sides.Bottom)
        {
            Vector3Int pos = new Vector3Int(r.Position.x - w / 2 + r.Width / 2, r.Position.y - r.Height, 0);
            neighbor = GenerateRoom(pos, w, h, new int[] { 0, 0, 0, 1 });
            if (neighbor != null)
            {
                OpenExitHorizontal(r, Sides.Bottom);
            }
        }
        if (dir == Sides.Top)
        {
            Vector3Int pos = new Vector3Int(r.Position.x - w / 2 + r.Width / 2, r.Position.y + h, 0);
            neighbor = GenerateRoom(pos, w, h, new int[] { 0, 0, 1, 0 });
            if (neighbor != null)
            {
                OpenExitHorizontal(r, Sides.Top);
            }
        }
        return neighbor;
    }

    public void CloseRoomExits(Room r)
    {
        if (r.openDirection[0] == 1) CloseVerticalExit(r, Sides.Left);
        if (r.openDirection[1] == 1) CloseVerticalExit(r, Sides.Right);
        if (r.openDirection[2] == 1) CloseHorizontalExit(r, Sides.Bottom);
        if (r.openDirection[3] == 1) CloseHorizontalExit(r, Sides.Top);
    }
    public void OpenRoomExits(Room r)
    {
        if (r.openDirection[0] == 1) OpenExitVertical(r, Sides.Left);
        if (r.openDirection[1] == 1) OpenExitVertical(r, Sides.Right);
        if (r.openDirection[2] == 1) OpenExitHorizontal(r, Sides.Bottom);
        if (r.openDirection[3] == 1) OpenExitHorizontal(r, Sides.Top);
    }

    private void CloseHorizontalExit(Room r, Sides dir)
    {
        if (dir != Sides.Top && dir != Sides.Bottom) return;
        int yOffset = dir == Sides.Top ? 0 : -r.Height + 1;
        Vector3Int pL = new Vector3Int(r.Position.x + r.Width / 2 - 1, r.Position.y + yOffset, 0);
        Vector3Int pR = pL + Vector3Int.right;
        if (dir == Sides.Bottom)
        {
            GeneratePlane(WallTilemap, WallBottom, pL, 2, 1);
        }
        else
        {
            WallTilemap.SetTile(pL, WallBottomLeftInnerCorner);
            WallTilemap.SetTile(pL + Vector3Int.right, WallBottomRightInnerCorner);
        }
        GeneratePlane(GroundTilemap, WallFace, pL + Vector3Int.down, 2, 1);
    }

    private void CloseVerticalExit(Room r, Sides dir)
    {
        if (dir != Sides.Left && dir != Sides.Right) return;
        int xOffset = dir == Sides.Left ? 0 : r.Width - 1;
        Vector3Int pB = new Vector3Int(r.Position.x + xOffset, r.Position.y - r.Height / 2 - 1, 0);
        Vector3Int pT = pB + Vector3Int.up;
        WallTilemap.SetTile(pB, dir == Sides.Left ? WallBottomRightInnerCorner : WallBottomLeftInnerCorner);
        WallTilemap.SetTile(pT, dir == Sides.Left ? Wallleft : Wallright);
    }

    private bool OpenExitHorizontal(Room r, Sides dir)
    {
        if (dir != Sides.Top && dir != Sides.Bottom) return false;
        if (dir == Sides.Bottom) r.openDirection[2] = 1;
        if (dir == Sides.Top) r.openDirection[3] = 1;
        int yOffset = dir == Sides.Top ? 0 : -r.Height + 1;
        Vector3Int pL = new Vector3Int(r.Position.x + r.Width / 2 - 1, r.Position.y + yOffset, 0);
        Vector3Int pR = pL + Vector3Int.right;
        WallTilemap.SetTile(pL, dir == Sides.Top ? WallBottomRightOutterCorner : WallTopRightCorner);
        WallTilemap.SetTile(pR, dir == Sides.Top ? WallBottomLeftOutterCorner : WallTopLeftCorner);
        if (dir == Sides.Top)
        {
            GeneratePlane(GroundTilemap, Ground, pL, 2, 3);
        }
        else
        {
            GeneratePlane(GroundTilemap, Ground, pL + Vector3Int.down, 2, 1);
        }
        return true;
    }

    private bool OpenExitVertical(Room r, Sides dir)
    {
        if (dir != Sides.Left && dir != Sides.Right) return false;
        if (dir == Sides.Left) r.openDirection[0] = 1;
        if (dir == Sides.Right) r.openDirection[1] = 1;
        int xOffset = dir == Sides.Left ? 0 : r.Width - 1;
        Tile btmCorner = dir == Sides.Left ? WallBottomRightInnerCorner : WallBottomLeftInnerCorner;
        Vector3Int pB = new Vector3Int(r.Position.x + xOffset, r.Position.y - r.Height / 2 - 1, 0);
        Vector3Int pT = pB + new Vector3Int(0,3,0);
        WallTilemap.SetTile(pB, (dir == Sides.Left ? WallTopRightCorner2 : WallTopLeftCorner2));
        GroundTilemap.SetTile(pB, Ground);
        GroundTilemap.SetTile(pB - (dir == Sides.Left ? Vector3Int.left : Vector3Int.right), Ground);
        GroundTilemap.SetTile(pB + Vector3Int.up - (dir == Sides.Left ? Vector3Int.left : Vector3Int.right), Ground);
        GroundTilemap.SetTile(pB + new Vector3Int(0, 2, 0), WallFace);
        GroundTilemap.SetTile(pB + Vector3Int.up, GroundTop);
        WallTilemap.SetTile(pB + Vector3Int.up, null);
        WallTilemap.SetTile(pB + new Vector3Int(0, 2, 0), null);
        WallTilemap.SetTile(pT, pT.y == r.Position.y ? WallBottom : btmCorner);
        return true;
    }

    private bool CanGenerate(Vector3Int position, int w, int h)
    {
        for (int i = 0; i < w; i++)
        {
            for(int j = 0; j < h; j++)
            {
                Vector3Int pos = new Vector3Int(position.x + i, position.y - j, 0);
                if (GroundTilemap.GetTile(pos) != null) return false;
                if (WallTilemap.GetTile(pos) != null) return false;
            }
        }
        return true;
    }

    private void GenerateRoomWalls(Tilemap wallTilemap, Vector3Int position, int w, int h)
    {
        GeneratePlane(wallTilemap, WallTop, position, w, 1);
        GeneratePlane(GroundTilemap, WallFace, position - new Vector3Int(-1, 1, 0), w - 2, 1);//wallFace
        GeneratePlane(wallTilemap, WallBottom, position - new Vector3Int(0,h - 1, 0) , w, 1);
        GeneratePlane(wallTilemap, Wallleft, position - new Vector3Int(0, 1, 0), 1, h - 2);
        GeneratePlane(wallTilemap, Wallright, position - new Vector3Int(-w+1, 1, 0), 1, h - 2);
        wallTilemap.SetTile(new Vector3Int(position.x, position.y, 0), WallTopLeftCorner);
        wallTilemap.SetTile(new Vector3Int(position.x + w - 1, position.y, 0), WallTopRightCorner);
        wallTilemap.SetTile(new Vector3Int(position.x, position.y - h + 1, 0), WallBottomLeftOutterCorner);
        wallTilemap.SetTile(new Vector3Int(position.x + w - 1, position.y - h + 1, 0), WallBottomRightOutterCorner);

    }

    private void GeneratePlane(Tilemap tilemap, Tile tile, Vector3Int position, int w, int h)
    {
        for (int i=0; i<w; i++)
        {
            for (int j=0; j<h; j++)
            {
                Vector3Int pos = new Vector3Int(position.x + i, position.y - j, 0);
                tilemap.SetTile(pos, tile);
            }
        }
    }
}
