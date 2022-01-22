using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Sides{
    Left,Right,Bottom,Top
}

public class RoomGenerator : MonoBehaviour
{
    public RoomTilesCollection allTiles;

    public Tilemap GroundTilemap, WallTilemap, WallTilemapNoCollider;

    public List<Room> Rooms;
    public GameObject RoomAreaColliderHolder;

    private void Awake()
    {
        Rooms = new List<Room>();
        RoomAreaColliderHolder = new GameObject();
        RoomAreaColliderHolder.name = "RoomAreaColliderHolder";
       
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
        WallTilemapNoCollider.ClearAllTiles();
    }

    //TODO: simplfy code
    public Room GenerateRoom(Vector3Int position, int w, int h, int[] openDirection)
    {
        Room r = new Room(position, w, h);
        r.openDirection = openDirection;
        if (!CanGenerate(r.GridPosition, r.Width, r.Height)) return null;
        Vector3Int pos = new Vector3Int(r.GridPosition.x + 1, r.GridPosition.y - 2, 0);
        GeneratePlane(GroundTilemap, allTiles.Ground, pos, r.Width - 2, r.Height - 2);
        GeneratePlane(GroundTilemap, allTiles.GroundTop, pos, r.Width -2, 1);
        GeneratePlane(GroundTilemap, allTiles.GroundLeft, pos, 1, r.Height - 2);
        GeneratePlane(GroundTilemap, allTiles.GroundRight, pos + new Vector3Int(r.Width - 3, 0, 0), 1, r.Height - 2);
        GroundTilemap.SetTile(pos, allTiles.GroundLeftCorner);
        GroundTilemap.SetTile(pos + new Vector3Int(r.Width - 3, 0, 0), allTiles.GroundRightCorner);
        GenerateRoomWalls(WallTilemap, r.GridPosition, r.Width, r.Height);
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
        colliderObj.transform.position = r.CenterWorldPosition;
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
        Room neighbor = null;
        if (dir == Sides.Left)
        {
            Vector3Int pos = new Vector3Int(r.GridPosition.x - w, r.GridPosition.y + h/2 - r.Height/2, 0);
            neighbor = GenerateRoom(pos, w, h, new int[] { 0, 1, 0, 0 });
            if (neighbor != null)
            {
                OpenExitVertical(r, Sides.Left);
            }
        }
        if (dir == Sides.Right)
        {
            Vector3Int pos = new Vector3Int(r.GridPosition.x + r.Width, r.GridPosition.y + h/2 - r.Height/2, 0);
            neighbor = GenerateRoom(pos, w, h, new int[] { 1, 0, 0, 0 });
            if (neighbor != null)
            {
                OpenExitVertical(r, Sides.Right);
            }
        }
        if (dir == Sides.Bottom)
        {
            Vector3Int pos = new Vector3Int(r.GridPosition.x - w / 2 + r.Width / 2, r.GridPosition.y - r.Height, 0);
            neighbor = GenerateRoom(pos, w, h, new int[] { 0, 0, 0, 1 });
            if (neighbor != null)
            {
                OpenExitHorizontal(r, Sides.Bottom);
            }
        }
        if (dir == Sides.Top)
        {
            Vector3Int pos = new Vector3Int(r.GridPosition.x - w / 2 + r.Width / 2, r.GridPosition.y + h, 0);
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
        if (r.openDirection[0] == 0) CloseVerticalExit(r, Sides.Left);
        if (r.openDirection[1] == 0) CloseVerticalExit(r, Sides.Right);
        if (r.openDirection[2] == 0) CloseHorizontalExit(r, Sides.Bottom);
        if (r.openDirection[3] == 0) CloseHorizontalExit(r, Sides.Top);
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
        Vector3Int pL = new Vector3Int(r.GridPosition.x + r.Width / 2 - 1, r.GridPosition.y + yOffset, 0);
        Vector3Int pR = pL + Vector3Int.right;
        if (dir == Sides.Bottom)
        {
            GeneratePlane(WallTilemap, allTiles.WallBottom, pL, 2, 1);
        }
        else
        {
            WallTilemap.SetTile(pL, allTiles.WallBottomLeftInnerCorner);
            WallTilemap.SetTile(pL + Vector3Int.right, allTiles.WallBottomRightInnerCorner);
        }
        GeneratePlane(WallTilemapNoCollider, allTiles.WallFace, pL + Vector3Int.down, 2, 1);
    }

    private void CloseVerticalExit(Room r, Sides dir)
    {
        if (dir != Sides.Left && dir != Sides.Right) return;
        int xOffset = dir == Sides.Left ? 0 : r.Width - 1;
        Vector3Int pB = new Vector3Int(r.GridPosition.x + xOffset, r.GridPosition.y - r.Height / 2 - 1, 0);
        Vector3Int pT = pB + Vector3Int.up;
        WallTilemap.SetTile(pB, dir == Sides.Left ? allTiles.WallBottomRightInnerCorner : allTiles.WallBottomLeftInnerCorner);
        WallTilemap.SetTile(pT, dir == Sides.Left ? allTiles.Wallleft : allTiles.Wallright);
    }

    private bool OpenExitHorizontal(Room r, Sides dir)
    {
        if (dir != Sides.Top && dir != Sides.Bottom) return false;
        if (dir == Sides.Bottom) r.openDirection[2] = 1;
        if (dir == Sides.Top) r.openDirection[3] = 1;
        int yOffset = dir == Sides.Top ? 0 : -r.Height + 1;
        Vector3Int pL = new Vector3Int(r.GridPosition.x + r.Width / 2 - 1, r.GridPosition.y + yOffset, 0);
        Vector3Int pR = pL + Vector3Int.right;
        WallTilemap.SetTile(pL, dir == Sides.Top ? allTiles.WallBottomRightOutterCorner : allTiles.WallTopRightCorner);
        WallTilemap.SetTile(pR, dir == Sides.Top ? allTiles.WallBottomLeftOutterCorner : allTiles.WallTopLeftCorner);
        if (dir == Sides.Top)
        {
            GeneratePlane(GroundTilemap, allTiles.Ground, pL, 2, 3);
            WallTilemapNoCollider.SetTile(pL + Vector3Int.down, allTiles.WallFaceCornerRight);
            WallTilemapNoCollider.SetTile(pL + Vector3Int.down + Vector3Int.right, allTiles.WallFaceCornerLeft);
        }
        else
        {
            GeneratePlane(GroundTilemap, allTiles.Ground, pL + Vector3Int.down, 2, 1);
        }
        return true;
    }

    private bool OpenExitVertical(Room r, Sides dir)
    {
        if (dir != Sides.Left && dir != Sides.Right) return false;
        if (dir == Sides.Left) r.openDirection[0] = 1;
        if (dir == Sides.Right) r.openDirection[1] = 1;
        int xOffset = dir == Sides.Left ? 0 : r.Width - 1;
        Tile btmCorner = dir == Sides.Left ? allTiles.WallBottomRightInnerCorner : allTiles.WallBottomLeftInnerCorner;
        Vector3Int pB = new Vector3Int(r.GridPosition.x + xOffset, r.GridPosition.y - r.Height / 2 - 1, 0);
        Vector3Int pT = pB + new Vector3Int(0,3,0);
        WallTilemap.SetTile(pB, (dir == Sides.Left ? allTiles.WallTopRightCorner2 : allTiles.WallTopLeftCorner2));
        GroundTilemap.SetTile(pB, allTiles.Ground);
        GroundTilemap.SetTile(pB - (dir == Sides.Left ? Vector3Int.left : Vector3Int.right), allTiles.Ground);
        GroundTilemap.SetTile(pB + Vector3Int.up - (dir == Sides.Left ? Vector3Int.left : Vector3Int.right), allTiles.Ground);
        WallTilemapNoCollider.SetTile(pB + new Vector3Int(0, 2, 0), allTiles.WallFace);
        GroundTilemap.SetTile(pB + Vector3Int.up, allTiles.GroundTop);
        WallTilemap.SetTile(pB + Vector3Int.up, null);
        WallTilemap.SetTile(pB + new Vector3Int(0, 2, 0), null);
        WallTilemap.SetTile(pT, pT.y == r.GridPosition.y ? allTiles.WallBottom : btmCorner);
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
        GeneratePlane(wallTilemap, allTiles.WallTop, position, w, 1);
        GeneratePlane(WallTilemapNoCollider, allTiles.WallFace, position - new Vector3Int(-1, 1, 0), w - 2, 1);//wallFace
        GeneratePlane(wallTilemap, allTiles.WallBottom, position - new Vector3Int(0,h - 1, 0) , w, 1);
        GeneratePlane(wallTilemap, allTiles.Wallleft, position - new Vector3Int(0, 1, 0), 1, h - 2);
        GeneratePlane(wallTilemap, allTiles.Wallright, position - new Vector3Int(-w+1, 1, 0), 1, h - 2);
        wallTilemap.SetTile(new Vector3Int(position.x, position.y, 0), allTiles.WallTopLeftCorner);
        wallTilemap.SetTile(new Vector3Int(position.x + w - 1, position.y, 0), allTiles.WallTopRightCorner);
        wallTilemap.SetTile(new Vector3Int(position.x, position.y - h + 1, 0), allTiles.WallBottomLeftOutterCorner);
        wallTilemap.SetTile(new Vector3Int(position.x + w - 1, position.y - h + 1, 0), allTiles.WallBottomRightOutterCorner);

    }

    private void GeneratePlane(Tilemap tilemap, Tile tile, Vector3Int position, int w, int h)
    {
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                Vector3Int pos = new Vector3Int(position.x + i, position.y - j, 0);
                tilemap.SetTile(pos, tile);
            }
        }
    }
}
