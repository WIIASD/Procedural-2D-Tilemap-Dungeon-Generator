using UnityEngine;

/// <summary>
/// This is the class handling data of rectangular rooms 
/// </summary>
[System.Serializable]
public class RectangleRoom : RoomBase
{
    /// <summary>
    /// 0 -> closed, 1 -> open
    /// </summary>
    public int[] openDirection = { 0, 0, 0, 0 };
    
    public float AreaColliderWidth, AreaColliderHeight;
    public int walkAbleWidth { get; private set; }
    public int walkAbleHeight { get; private set; }

    public RectangleRoom(Vector3Int pos, int w, int h) : base(pos, w, h)
    {
        walkAbleWidth = w - 2;
        walkAbleHeight = h - 1;
        AreaColliderWidth = w - 2;
        AreaColliderHeight = h - 1;
        CenterWorldPosition = GridPosition + Vector3.right + new Vector3(AreaColliderWidth / 2, -AreaColliderHeight / 2, 0);
    }

    public override void DrawGround(RoomTilePalette palette)
    {
        ZTools.FillInt2DArray(LayoutMatrix, palette.GetID(palette.Ground));
        for (int y = 0; y < LayoutMatrix.GetLength(0); y++)
        {
            LayoutMatrix[y, 0] = -1;
            LayoutMatrix[y, LayoutMatrix.GetLength(1) - 1] = -1;
        }
        for (int x = 0; x < LayoutMatrix.GetLength(1); x++)
        {
            LayoutMatrix[0, x] = -1;
        }
    }

    public override void DrawWall(RoomTilePalette palette)
    {
        if (LayoutMatrix.GetLength(0) < 2 || LayoutMatrix.GetLength(1) < 2) 
            throw new System.Exception("Cannot DrawWall: Dimension Error!");
        int wallFaceId = palette.GetID(palette.WallFace);
        int leftWallId = palette.GetID(palette.Wallleft);
        int rightWallId = palette.GetID(palette.Wallright);
        int topWallId = palette.GetID(palette.WallTop);
        int bottomWallId = palette.GetID(palette.WallBottom);
        int topLeftCornerId = palette.GetID(palette.WallTopLeftCorner);
        int topLeftCorner2Id = palette.GetID(palette.WallTopLeftCorner2);
        int topRightCorner2Id = palette.GetID(palette.WallTopRightCorner2);
        int topRightCornerId = palette.GetID(palette.WallTopRightCorner);
        int btmleftCornerId = palette.GetID(palette.WallBottomLeftOutterCorner);
        int btmRightCornerId = palette.GetID(palette.WallBottomRightOutterCorner);
        int btmLeftInnerCornerId= palette.GetID(palette.WallBottomLeftInnerCorner);
        int btmRightInnerCornerId = palette.GetID(palette.WallBottomRightInnerCorner);
        //horizontal walls
        for (int x = 0; x < LayoutMatrix.GetLength(1); x++)
        {
            LayoutMatrix[0, x] = topWallId;
            //goes to ground matrix because this has not collider
            LayoutMatrix[1, x] = LayoutMatrix[1, x] != -1 ? wallFaceId : -1;
            LayoutMatrix[LayoutMatrix.GetLength(0) - 1, x] = bottomWallId;
        }
        //vertical walls
        for (int y = 0; y < LayoutMatrix.GetLength(0); y++)
        {
            LayoutMatrix[y, 0] = leftWallId;
            LayoutMatrix[y, LayoutMatrix.GetLength(1) - 1] = rightWallId;
        }
        //corners
        LayoutMatrix[0, 0] = topLeftCornerId;
        LayoutMatrix[0, LayoutMatrix.GetLength(1) - 1] = topRightCornerId;
        LayoutMatrix[LayoutMatrix.GetLength(0) - 1, 0] = btmleftCornerId;
        LayoutMatrix[LayoutMatrix.GetLength(0) - 1, LayoutMatrix.GetLength(1) - 1] = btmRightCornerId;
        //entrance
        if (openDirection[0] == 1)
        {
            LayoutMatrix[LayoutMatrix.GetLength(0) / 2, 0] = -1;
            LayoutMatrix[LayoutMatrix.GetLength(0) / 2 - 1, 0] = -1;
            LayoutMatrix[LayoutMatrix.GetLength(0) / 2 - 2, 0] = btmRightInnerCornerId;
            LayoutMatrix[LayoutMatrix.GetLength(0) / 2 + 1, 0] = topRightCorner2Id;
        }
    }
}
