using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Sides{
    Left,Right,Bottom,Top
}

public class RoomGenerator : MonoBehaviour
{
    public RoomTilesCollection AllTiles;

    public Tilemap GroundTilemap, WallTilemap, WallTilemapNoCollider;

    public List<RectangleRoom> Rooms;
    public GameObject RoomAreaColliderHolder;

    private void Awake()
    {
        Rooms = new List<RectangleRoom>();
        RoomAreaColliderHolder = new GameObject();
        RoomAreaColliderHolder.name = "RoomAreaColliderHolder";
       
    }

    private void Start()
    {
        //int[,] array = new int[5, 10];
        //array = ZTools.FillInt2DArray(array, 1);
        //ZTools.PrintInt2DArray(array);
        RectangleRoom room = new RectangleRoom(Vector3Int.zero, 10, 10);
        RectangleRoom room1 = new RectangleRoom(Vector3Int.zero, 10, 10);
        //print(room.Equals(room));
    }

    internal void Reset()
    {
        Rooms = new List<RectangleRoom>();
        Destroy(RoomAreaColliderHolder);
        RoomAreaColliderHolder = new GameObject();
        RoomAreaColliderHolder.name = "RoomAreaColliderHolder";
        RectangleRoom.IDCount = 0;
        GroundTilemap.ClearAllTiles();
        WallTilemap.ClearAllTiles();
        WallTilemapNoCollider.ClearAllTiles();
    }

    //TODO: simplfy code
    public RectangleRoom GenerateRoom(Vector3Int position, int w, int h, int[] openDirection)
    {
        RectangleRoom r = new RectangleRoom(position, w, h);
        r.openDirection = openDirection;
        if (!CanGenerate(r.GridPosition, r.Width, r.Height)) return null;
        Vector3Int pos = new Vector3Int(r.GridPosition.x + 1, r.GridPosition.y - 2, 0);
        GeneratePlane(GroundTilemap, AllTiles.Ground, pos, r.Width - 2, r.Height - 2);
        GeneratePlane(GroundTilemap, AllTiles.GroundTop, pos, r.Width -2, 1);
        GeneratePlane(GroundTilemap, AllTiles.GroundLeft, pos, 1, r.Height - 2);
        GeneratePlane(GroundTilemap, AllTiles.GroundRight, pos + new Vector3Int(r.Width - 3, 0, 0), 1, r.Height - 2);
        GroundTilemap.SetTile(pos, AllTiles.GroundLeftCorner);
        GroundTilemap.SetTile(pos + new Vector3Int(r.Width - 3, 0, 0), AllTiles.GroundRightCorner);
        GenerateRoomWalls(WallTilemap, r.GridPosition, r.Width, r.Height);
        if (r.openDirection[0] == 1) OpenExitVertical(r, Sides.Left);
        if (r.openDirection[1] == 1) OpenExitVertical(r, Sides.Right);
        if (r.openDirection[2] == 1) OpenExitHorizontal(r, Sides.Bottom);
        if (r.openDirection[3] == 1) OpenExitHorizontal(r, Sides.Top);
        CreateRoomCollider(r);
        Rooms.Add(r);
        return r;
    }

    private void CreateRoomCollider(RectangleRoom r)
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
        r.RoomCollder = collider;
    }

    public RectangleRoom GenerateNeighborRoom(RectangleRoom r, int w, int h, Sides dir)
    {
        RectangleRoom neighbor = null;
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

    public void CloseRoomExits(RectangleRoom r)
    {
        if (r.openDirection[0] == 0) CloseVerticalExit(r, Sides.Left);
        if (r.openDirection[1] == 0) CloseVerticalExit(r, Sides.Right);
        if (r.openDirection[2] == 0) CloseHorizontalExit(r, Sides.Bottom);
        if (r.openDirection[3] == 0) CloseHorizontalExit(r, Sides.Top);
    }
    public void OpenRoomExits(RectangleRoom r)
    {
        if (r.openDirection[0] == 1) OpenExitVertical(r, Sides.Left);
        if (r.openDirection[1] == 1) OpenExitVertical(r, Sides.Right);
        if (r.openDirection[2] == 1) OpenExitHorizontal(r, Sides.Bottom);
        if (r.openDirection[3] == 1) OpenExitHorizontal(r, Sides.Top);
    }

    private void CloseHorizontalExit(RectangleRoom r, Sides dir)
    {
        if (dir != Sides.Top && dir != Sides.Bottom) return;
        int yOffset = dir == Sides.Top ? 0 : -r.Height + 1;
        Vector3Int pL = new Vector3Int(r.GridPosition.x + r.Width / 2 - 1, r.GridPosition.y + yOffset, 0);
        Vector3Int pR = pL + Vector3Int.right;
        if (dir == Sides.Bottom)
        {
            GeneratePlane(WallTilemap, AllTiles.WallBottom, pL, 2, 1);
        }
        else
        {
            WallTilemap.SetTile(pL, AllTiles.WallBottomLeftInnerCorner);
            WallTilemap.SetTile(pL + Vector3Int.right, AllTiles.WallBottomRightInnerCorner);
        }
        GeneratePlane(WallTilemapNoCollider, AllTiles.WallFace, pL + Vector3Int.down, 2, 1);
    }

    private void CloseVerticalExit(RectangleRoom r, Sides dir)
    {
        if (dir != Sides.Left && dir != Sides.Right) return;
        int xOffset = dir == Sides.Left ? 0 : r.Width - 1;
        Vector3Int pB = new Vector3Int(r.GridPosition.x + xOffset, r.GridPosition.y - r.Height / 2 - 1, 0);
        Vector3Int pT = pB + Vector3Int.up;
        WallTilemap.SetTile(pB, dir == Sides.Left ? AllTiles.WallBottomRightInnerCorner : AllTiles.WallBottomLeftInnerCorner);
        WallTilemap.SetTile(pT, dir == Sides.Left ? AllTiles.Wallleft : AllTiles.Wallright);
    }

    private bool OpenExitHorizontal(RectangleRoom r, Sides dir)
    {
        if (dir != Sides.Top && dir != Sides.Bottom) return false;
        if (dir == Sides.Bottom) r.openDirection[2] = 1;
        if (dir == Sides.Top) r.openDirection[3] = 1;
        int yOffset = dir == Sides.Top ? 0 : -r.Height + 1;
        Vector3Int pL = new Vector3Int(r.GridPosition.x + r.Width / 2 - 1, r.GridPosition.y + yOffset, 0);
        Vector3Int pR = pL + Vector3Int.right;
        WallTilemap.SetTile(pL, dir == Sides.Top ? AllTiles.WallBottomRightOutterCorner : AllTiles.WallTopRightCorner);
        WallTilemap.SetTile(pR, dir == Sides.Top ? AllTiles.WallBottomLeftOutterCorner : AllTiles.WallTopLeftCorner);
        if (dir == Sides.Top)
        {
            GeneratePlane(GroundTilemap, AllTiles.Ground, pL, 2, 3);
            WallTilemapNoCollider.SetTile(pL + Vector3Int.down, AllTiles.WallFaceCornerRight);
            WallTilemapNoCollider.SetTile(pL + Vector3Int.down + Vector3Int.right, AllTiles.WallFaceCornerLeft);
        }
        else
        {
            GeneratePlane(GroundTilemap, AllTiles.Ground, pL + Vector3Int.down, 2, 1);
        }
        return true;
    }

    private bool OpenExitVertical(RectangleRoom r, Sides dir)
    {
        if (dir != Sides.Left && dir != Sides.Right) return false;
        if (dir == Sides.Left) r.openDirection[0] = 1;
        if (dir == Sides.Right) r.openDirection[1] = 1;
        int xOffset = dir == Sides.Left ? 0 : r.Width - 1;
        Tile btmCorner = dir == Sides.Left ? AllTiles.WallBottomRightInnerCorner : AllTiles.WallBottomLeftInnerCorner;
        Vector3Int pB = new Vector3Int(r.GridPosition.x + xOffset, r.GridPosition.y - r.Height / 2 - 1, 0);
        Vector3Int pT = pB + new Vector3Int(0,3,0);
        WallTilemap.SetTile(pB, (dir == Sides.Left ? AllTiles.WallTopRightCorner2 : AllTiles.WallTopLeftCorner2));
        GroundTilemap.SetTile(pB, AllTiles.Ground);
        GroundTilemap.SetTile(pB - (dir == Sides.Left ? Vector3Int.left : Vector3Int.right), AllTiles.Ground);
        GroundTilemap.SetTile(pB + Vector3Int.up - (dir == Sides.Left ? Vector3Int.left : Vector3Int.right), AllTiles.Ground);
        WallTilemapNoCollider.SetTile(pB + new Vector3Int(0, 2, 0), AllTiles.WallFace);
        GroundTilemap.SetTile(pB + Vector3Int.up, AllTiles.GroundTop);
        WallTilemap.SetTile(pB + Vector3Int.up, null);
        WallTilemap.SetTile(pB + new Vector3Int(0, 2, 0), null);
        WallTilemap.SetTile(pT, pT.y == r.GridPosition.y ? AllTiles.WallBottom : btmCorner);
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
        GeneratePlane(wallTilemap, AllTiles.WallTop, position, w, 1);
        GeneratePlane(WallTilemapNoCollider, AllTiles.WallFace, position - new Vector3Int(-1, 1, 0), w - 2, 1);//wallFace
        GeneratePlane(wallTilemap, AllTiles.WallBottom, position - new Vector3Int(0,h - 1, 0) , w, 1);
        GeneratePlane(wallTilemap, AllTiles.Wallleft, position - new Vector3Int(0, 1, 0), 1, h - 2);
        GeneratePlane(wallTilemap, AllTiles.Wallright, position - new Vector3Int(-w+1, 1, 0), 1, h - 2);
        wallTilemap.SetTile(new Vector3Int(position.x, position.y, 0), AllTiles.WallTopLeftCorner);
        wallTilemap.SetTile(new Vector3Int(position.x + w - 1, position.y, 0), AllTiles.WallTopRightCorner);
        wallTilemap.SetTile(new Vector3Int(position.x, position.y - h + 1, 0), AllTiles.WallBottomLeftOutterCorner);
        wallTilemap.SetTile(new Vector3Int(position.x + w - 1, position.y - h + 1, 0), AllTiles.WallBottomRightOutterCorner);

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
