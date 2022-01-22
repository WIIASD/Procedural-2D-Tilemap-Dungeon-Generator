using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Data", menuName = "Room Tiles Collection", order = 51)]    
public class RoomTilesCollection : ScriptableObject
{
    public Tile Ground, GroundLeft, GroundRight, GroundLeftCorner,
                GroundRightCorner, GroundTop,
                WallFace, WallFaceCornerLeft, WallFaceCornerRight, Wallleft, Wallright, WallTop,
                WallBottom, WallTopLeftCorner, WallTopRightCorner, WallTopLeftCorner2, WallTopRightCorner2,
                WallBottomLeftOutterCorner, WallBottomRightOutterCorner,
                WallBottomLeftInnerCorner, WallBottomRightInnerCorner;
}
