﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Sides{
    Left,Right,Bottom,Top
}

/// <summary>
/// Component that responsible of generating rooms base on their layouts
/// </summary>
public class RoomGenerator : MonoBehaviour
{
    public RoomTilePalette TilePalette;
    public Tilemap GroundTilemap, WallTilemap, WallTilemapNoCollider;
    public List<RectangleRoom> Rooms;
    public GameObject RoomAreaColliderHolder;

    private void Awake()
    {
        Rooms = new List<RectangleRoom>();
        RoomAreaColliderHolder = new GameObject();
        RoomAreaColliderHolder.name = "RoomAreaColliderHolder";
        TilePalette = RoomTilePaletteManager.Instance.GetCurrentPalette();
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
        if (!CanGenerate(r.GridPosition, r.Width, r.Height))
        {
            print($"Room (id: {r.ID}) does not have enough space to generate!");
            return null;
        }

        r.DrawLayout(TilePalette);
        int[,] groundLayout = r.GroundMatrix;
        int[,] wallLayout = r.WallMatrix;
        DrawLayoutMatrix(position, groundLayout, GroundTilemap, TilePalette);
        DrawLayoutMatrix(position, wallLayout, WallTilemap, TilePalette);

        if (r.openDirection[0] == 1) OpenExitVertical(r, Sides.Left);
        if (r.openDirection[1] == 1) OpenExitVertical(r, Sides.Right);
        if (r.openDirection[2] == 1) OpenExitHorizontal(r, Sides.Bottom);
        if (r.openDirection[3] == 1) OpenExitHorizontal(r, Sides.Top);
        CreateRoomCollider(r);
        Rooms.Add(r);
        return r;
    }

    private void DrawLayoutMatrix(Vector3Int position, int[,] layout, Tilemap tilemap,RoomTilePalette palette)
    {
        for (int y = 0; y < layout.GetLength(0); y++)
        {
            for (int x = 0; x < layout.GetLength(1); x++)
            {
                int tileID = layout[y, x];
                Vector3Int tPos = position + new Vector3Int(x, -y, 0);
                Tile t = TilePalette.GetTile(tileID);
                tilemap.SetTile(tPos, t);
            }
        }
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
            GeneratePlane(WallTilemap, TilePalette.WallBottom, pL, 2, 1);
        }
        else
        {
            WallTilemap.SetTile(pL, TilePalette.WallBottomLeftInnerCorner);
            WallTilemap.SetTile(pL + Vector3Int.right, TilePalette.WallBottomRightInnerCorner);
        }
        GeneratePlane(WallTilemapNoCollider, TilePalette.WallFace, pL + Vector3Int.down, 2, 1);
    }

    private void CloseVerticalExit(RectangleRoom r, Sides dir)
    {
        if (dir != Sides.Left && dir != Sides.Right) return;
        int xOffset = dir == Sides.Left ? 0 : r.Width - 1;
        Vector3Int pB = new Vector3Int(r.GridPosition.x + xOffset, r.GridPosition.y - r.Height / 2 - 1, 0);
        Vector3Int pT = pB + Vector3Int.up;
        WallTilemap.SetTile(pB, dir == Sides.Left ? TilePalette.WallBottomRightInnerCorner : TilePalette.WallBottomLeftInnerCorner);
        WallTilemap.SetTile(pT, dir == Sides.Left ? TilePalette.Wallleft : TilePalette.Wallright);
    }

    private bool OpenExitHorizontal(RectangleRoom r, Sides dir)
    {
        if (dir != Sides.Top && dir != Sides.Bottom) return false;
        if (dir == Sides.Bottom) r.openDirection[2] = 1;
        if (dir == Sides.Top) r.openDirection[3] = 1;
        int yOffset = dir == Sides.Top ? 0 : -r.Height + 1;
        Vector3Int pL = new Vector3Int(r.GridPosition.x + r.Width / 2 - 1, r.GridPosition.y + yOffset, 0);
        Vector3Int pR = pL + Vector3Int.right;
        WallTilemap.SetTile(pL, dir == Sides.Top ? TilePalette.WallBottomRightOutterCorner : TilePalette.WallTopRightCorner);
        WallTilemap.SetTile(pR, dir == Sides.Top ? TilePalette.WallBottomLeftOutterCorner : TilePalette.WallTopLeftCorner);
        if (dir == Sides.Top)
        {
            GeneratePlane(GroundTilemap, TilePalette.Ground, pL, 2, 3);
            WallTilemapNoCollider.SetTile(pL + Vector3Int.down, TilePalette.WallFaceCornerRight);
            WallTilemapNoCollider.SetTile(pL + Vector3Int.down + Vector3Int.right, TilePalette.WallFaceCornerLeft);
        }
        else
        {
            GeneratePlane(GroundTilemap, TilePalette.Ground, pL + Vector3Int.down, 2, 1);
        }
        return true;
    }

    private bool OpenExitVertical(RectangleRoom r, Sides dir)
    {
        if (dir != Sides.Left && dir != Sides.Right) return false;
        if (dir == Sides.Left) r.openDirection[0] = 1;
        if (dir == Sides.Right) r.openDirection[1] = 1;
        int xOffset = dir == Sides.Left ? 0 : r.Width - 1;
        Tile btmCorner = dir == Sides.Left ? TilePalette.WallBottomRightInnerCorner : TilePalette.WallBottomLeftInnerCorner;
        Vector3Int pB = new Vector3Int(r.GridPosition.x + xOffset, r.GridPosition.y - r.Height / 2 - 1, 0);
        Vector3Int pT = pB + new Vector3Int(0,3,0);
        WallTilemap.SetTile(pB, (dir == Sides.Left ? TilePalette.WallTopRightCorner2 : TilePalette.WallTopLeftCorner2));
        GroundTilemap.SetTile(pB, TilePalette.Ground);
        GroundTilemap.SetTile(pB - (dir == Sides.Left ? Vector3Int.left : Vector3Int.right), TilePalette.Ground);
        GroundTilemap.SetTile(pB + Vector3Int.up - (dir == Sides.Left ? Vector3Int.left : Vector3Int.right), TilePalette.Ground);
        WallTilemapNoCollider.SetTile(pB + new Vector3Int(0, 2, 0), TilePalette.WallFace);
        GroundTilemap.SetTile(pB + Vector3Int.up, TilePalette.GroundTop);
        WallTilemap.SetTile(pB + Vector3Int.up, null);
        WallTilemap.SetTile(pB + new Vector3Int(0, 2, 0), null);
        WallTilemap.SetTile(pT, pT.y == r.GridPosition.y ? TilePalette.WallBottom : btmCorner);
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
