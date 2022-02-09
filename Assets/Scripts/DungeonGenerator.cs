using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(RoomGenerator))]
public class DungeonGenerator : MonoBehaviour
{
    //public float GrowFactor, ShrinkFactor;
    public static System.Random rnd = new System.Random();
    public int RoomCount;
    public int StartWidth, StartHeight;
    public int MinRoomWidth, MinRoomHeight, MaxRoomWidth, MaxRoomHeight;
    public List<RectangleRoom> Rooms;
    public RectangleRoom StartRoom;
    public List<RectangleRoom> EndRooms;
    public RoomGenerator roomGenerator;

    void Awake()
    {
        roomGenerator = GetComponent<RoomGenerator>();
        EndRooms = new List<RectangleRoom>();
    }
    void Start()
    {
        ConstrainMinAndMax();
        roomGenerator.Reset();
        StartRoom = roomGenerator.GenerateRoom(Vector3Int.zero, StartWidth, StartHeight, new int[] { 0, 0, 0, 0 });
        StartRoom.cleared = true;
        print("ground matrix");
        ZTools.PrintInt2DArray(StartRoom.GroundMatrix);
        print("wall matrix");
        ZTools.PrintInt2DArray(StartRoom.WallMatrix);
        GenerateNeighborsForRoomNumbers(StartRoom, RoomCount - 1);
        EndRooms = FindEndRooms(roomGenerator.Rooms);
        Rooms = roomGenerator.Rooms;
    }

    private List<RectangleRoom> FindEndRooms(List<RectangleRoom> rooms)
    {
        List<RectangleRoom> result = new List<RectangleRoom>(); 
        foreach (RectangleRoom r in rooms)
        {
            int openCount = 0;
            foreach (int i in r.openDirection)
            {
                if (i == 1)
                {
                    openCount += 1;
                }
            }
            if (openCount == 1)
            {
                result.Add(r);
            }
        }
        return result;
    }

    private void ConstrainMinAndMax()
    {
        if (MinRoomWidth < 6) MinRoomWidth = 6;
        if (MinRoomHeight < 6) MinRoomHeight = 6;
        if (MaxRoomWidth < 6) MaxRoomWidth = 6;
        if (MaxRoomHeight < 6) MaxRoomHeight = 6;
        MaxRoomWidth = Mathf.FloorToInt(Mathf.Clamp(MaxRoomWidth, MinRoomWidth, Mathf.Infinity));
        MaxRoomHeight = Mathf.FloorToInt(Mathf.Clamp(MaxRoomHeight, MinRoomHeight, Mathf.Infinity));
        MinRoomWidth = Mathf.FloorToInt(Mathf.Clamp(MinRoomWidth, 6, MaxRoomWidth));
        MinRoomHeight = Mathf.FloorToInt(Mathf.Clamp(MinRoomHeight, 6, MaxRoomHeight));
    }

    private void GenerateNeighborsForRoomNumbers(RectangleRoom r, int roomNum)
    {
        int generatedRoomCount = 0;
        Queue<RectangleRoom> currentRoomQueue = new Queue<RectangleRoom>();
        currentRoomQueue.Enqueue(r);
        while (generatedRoomCount<roomNum)
        {
            RectangleRoom currentRoom = currentRoomQueue.Dequeue();
            Enum.GetValues(typeof(Sides));
            //randomize the the order of Sides to generate
            List<Sides> AllSides = new List<Sides>((Sides[])Enum.GetValues(typeof(Sides))).OrderBy(a=>rnd.Next()).ToList();
            foreach (Sides s in AllSides)
            {
                Vector2Int rndSize = RandomDimension(r.Width, r.Height);
                RectangleRoom n = roomGenerator.GenerateNeighborRoom(currentRoom, rndSize.x, rndSize.y, s);
                if (n != null)
                {
                    generatedRoomCount++;
                    if (generatedRoomCount == roomNum) return;
                    currentRoomQueue.Enqueue(n);
                }
            }
        }
    }

    private Vector2Int RandomDimension(int w, int h)
    {
        int rndw = Mathf.FloorToInt(Random.Range(MinRoomWidth, MaxRoomWidth));
        int rndh = Mathf.FloorToInt(Random.Range(MinRoomHeight, MaxRoomHeight));
        return new Vector2Int(rndw, rndh);
    }
}
