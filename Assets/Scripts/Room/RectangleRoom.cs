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
        ZTools.FillInt2DArray(GroundMatrix, palette.GetID(palette.Ground));
        for (int y = 0; y < GroundMatrix.GetLength(0); y++)
        {
            GroundMatrix[y, 0] = -1;
            GroundMatrix[y, GroundMatrix.GetLength(1) - 1] = -1;
        }
        for (int x = 0; x < GroundMatrix.GetLength(1); x++)
        {
            GroundMatrix[0, x] = -1;
        }
    }

    public override void DrawWall(RoomTilePalette palette)
    {
        for(int y = 0; y < WallMatrix.GetLength(0); y++)
        {
            for(int x = 0; x < WallMatrix.GetLength(1); x++){

                if (y == 0 && x == 0)//top left corner
                {
                    WallMatrix[y, x] = palette.GetID(palette.WallTopLeftCorner);
                    continue;
                }
                if (y == 0 && x == WallMatrix.GetLength(1) - 1)//top right corner
                {
                    WallMatrix[y, x] = palette.GetID(palette.WallTopRightCorner);
                    continue;
                }
                if (x == 0 && y == WallMatrix.GetLength(0) - 1)//bottom left corner
                {
                    WallMatrix[y, x] = palette.GetID(palette.WallBottomLeftOutterCorner);
                    continue;
                }
                if (x == WallMatrix.GetLength(1) - 1 && y == WallMatrix.GetLength(0) - 1)//bottom right corner
                {
                    WallMatrix[y, x] = palette.GetID(palette.WallBottomRightOutterCorner);
                    continue;
                }
                if (x == 0)//left wall
                {
                    WallMatrix[y, x] = palette.GetID(palette.Wallleft);
                    continue;
                }
                if (x == WallMatrix.GetLength(1) - 1) //right wall
                {
                    WallMatrix[y, x] = palette.GetID(palette.Wallright);
                    continue;
                }
                if (y == 0)//top wall
                {
                    WallMatrix[y, x] = palette.GetID(palette.WallTop);
                    continue;
                }
                if (y == WallMatrix.GetLength(0) - 1)//bottom wall
                {
                    WallMatrix[y, x] = palette.GetID(palette.WallBottom);
                    continue;
                }
                if (y == 1)//wall face
                {
                    GroundMatrix[y, x] = palette.GetID(palette.WallFace);
                }

            }
        }
    }
}
