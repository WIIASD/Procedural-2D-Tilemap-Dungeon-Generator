using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Data", menuName = "Room Tiles Collection", order = 51)]    
public class RoomTilePalette : ScriptableObject
{
    public static int GROUND_TILE_START_ID = 0, WALL_TILE_START_ID = 50;
    public Dictionary<int, Tile> TilesDictionary = new Dictionary<int, Tile>();
    public Tile //ground
                Ground, GroundLeft, GroundRight, GroundLeftCorner,
                GroundRightCorner, GroundTop,
                //wall
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
        for (int i = 0; i < GroundTileList.Count; i++)
        {
            TilesDictionary.Add(GROUND_TILE_START_ID+i, GroundTileList[i]);
        }
        for (int i = 0; i < WallTileList.Count; i++)
        {
            TilesDictionary.Add(WALL_TILE_START_ID + i, WallTileList[i]);
        }
    }

    public int GetID(Tile tile)//only search the first occurence since there should not be duplicated tiles in the palette
    {
        foreach (KeyValuePair<int, Tile> pair in TilesDictionary)
        {
            if (pair.Value.Equals(tile))
            {
                return pair.Key;
            }
        }
        return -1;
    }

    public Tile GetTile(int ID)
    {
        if (ID == -1) return null;
        return TilesDictionary[ID];
    }

}
