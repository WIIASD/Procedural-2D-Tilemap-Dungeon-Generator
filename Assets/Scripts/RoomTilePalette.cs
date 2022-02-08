using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Data", menuName = "Room Tiles Collection", order = 51)]    
public class RoomTilePalette : ScriptableObject
{
    public static int GROUND_TILE_START_ID = 0, WALL_TILE_START_ID = 50;
    public Dictionary<string, Tile> Tiles = new Dictionary<string, Tile>();
    public Tile Ground, GroundLeft, GroundRight, GroundLeftCorner,
                GroundRightCorner, GroundTop,
                WallFace, WallFaceCornerLeft, WallFaceCornerRight, Wallleft, Wallright, WallTop,
                WallBottom, WallTopLeftCorner, WallTopRightCorner, WallTopLeftCorner2, WallTopRightCorner2,
                WallBottomLeftOutterCorner, WallBottomRightOutterCorner,
                WallBottomLeftInnerCorner, WallBottomRightInnerCorner;

    public List<Tile> GroundTileList;

    public List<Tile> WallTileList;

    public void InitializeTilesLists()
    {
        GroundTileList = new List<Tile>(){Ground, GroundLeft, GroundRight, GroundLeftCorner,
                GroundRightCorner, GroundTop};
        WallTileList = new List<Tile>() {WallFace, WallFaceCornerLeft, WallFaceCornerRight, Wallleft, Wallright, WallTop,
                WallBottom, WallTopLeftCorner, WallTopRightCorner, WallTopLeftCorner2, WallTopRightCorner2,
                WallBottomLeftOutterCorner, WallBottomRightOutterCorner,
                WallBottomLeftInnerCorner, WallBottomRightInnerCorner};
    }

    public int GetID(Tile tile)
    {
        if (GroundTileList == null || WallTileList == null) throw new Exception("TilePalette ID Not Initialized!");
        int result = -1;
        result = GroundTileList.IndexOf(tile);
        if (result != -1) return GROUND_TILE_START_ID + result;
        result = WallTileList.IndexOf(tile);
        if (result != -1) return WALL_TILE_START_ID + result;
        return result;
    }

    public Tile GetTile(int ID)
    {
        if (ID == -1) return null;
        if (!ZTools.InRangeInclusive(ID, GROUND_TILE_START_ID, WALL_TILE_START_ID + WallTileList.Count)) throw new Exception("TilePalette ID Our Of Range!");
        if(ID < WALL_TILE_START_ID)
        {//ground
            if (ID > GROUND_TILE_START_ID + GroundTileList.Count) throw new Exception("TilePalette ID Our Of Range (Ground) !");
            return GroundTileList[ID];
        }
        else if(ID >= WALL_TILE_START_ID)
        {
            return WallTileList[ID-WALL_TILE_START_ID];
        }
        return null;
    }

}
