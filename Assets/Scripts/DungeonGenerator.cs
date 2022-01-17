﻿using System;
using System.Collections;
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
    [Range(0, 1)]
    public float PNeighborSpawnMin, PNeighborSpawnMax;
    public List<Room> Rooms;
    public Room StartRoom;
    public List<Room> EndRooms;
    public RoomGenerator roomGenerator;
    public TilemapDungeonManager dungeonManager;

    private void Awake()
    {
        roomGenerator = GetComponent<RoomGenerator>();
        dungeonManager = GetComponent<TilemapDungeonManager>();
        EndRooms = new List<Room>();
    }
    void Start()
    {
        ConstrainMinAndMax();
        roomGenerator.Reset();
        StartRoom = roomGenerator.GenerateRoom(Vector3Int.zero, StartWidth, StartHeight, new int[] { 0, 0, 0, 0 });
        StartRoom.cleared = true;
        GenerateNeighborsForRoomNumbers(StartRoom, RoomCount-1);
        EndRooms = FindEndRooms(roomGenerator.Rooms);
        Rooms = roomGenerator.Rooms;
        /*int tries = 100;
        while (tries > 0 && !ZTools.InRangeInclusive(roomGenerator.Rooms.Count, MinRoomCount, MaxRoomCount))
        {
            tries--;
            roomGenerator.Reset();
            StartRoom = roomGenerator.GenerateRoom(Vector3Int.zero, StartWidth, StartHeight, new int[] { 0, 0, 0, 0 });
            StartRoom.cleared = true;
            foreach (Room r in GenerateRandomNeighbors(StartRoom, 1f, 1f))
            {
                GenerateNeighborsForGenerations(r, generations - 1);
            }
        }
        if (tries <= 0)
        {
            print("Cannot Generate. Please change Min/Max Room Count or generations.");
        }
        EndRooms = FindEndRooms(roomGenerator.Rooms);
        Rooms = roomGenerator.Rooms;*/
    }

    private List<Room> FindEndRooms(List<Room> rooms)
    {
        List<Room> result = new List<Room>(); 
        foreach (Room r in rooms)
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
        roomGenerator.MinRoomWidth = MinRoomWidth;
        roomGenerator.MinRoomHeight = MinRoomHeight;
        roomGenerator.MaxRoomWidth = MaxRoomWidth;
        roomGenerator.MaxRoomHeight = MaxRoomHeight;
    }

    private void GenerateNeighborsForRoomNumbers(Room r, int roomNum)
    {
        int generatedRoomCount = 0;
        //List<Room> currentRoomList = new List<Room>() { r };
        Queue<Room> currentRoomQueue = new Queue<Room>();
        currentRoomQueue.Enqueue(r);
        while (generatedRoomCount<roomNum)
        {
            Room currentRoom = currentRoomQueue.Dequeue();
            Enum.GetValues(typeof(Sides));
            List<Sides> AllSides = new List<Sides>((Sides[])Enum.GetValues(typeof(Sides))).OrderBy(a=>rnd.Next()).ToList();
            foreach (Sides s in AllSides)
            {
                Vector2Int rndSize = RandomDimension(r.Width, r.Height);
                Room n = roomGenerator.GenerateNeighborRoom(currentRoom, rndSize.x, rndSize.y, s);
                if (n != null)
                {
                    generatedRoomCount++;
                    if (generatedRoomCount == roomNum) return;
                    currentRoomQueue.Enqueue(n);
                }
            }
        }
        
    }

    private void GenerateNeighborsForGenerations(Room r, int generations)
    {
        if (generations <= 0)
        {
            return;
        }
        foreach (Room rr in GenerateNeighbors(r, PNeighborSpawnMin, PNeighborSpawnMax))
        {
            GenerateNeighborsForGenerations(rr, generations-1);
        }
    }

    private List<Room> GenerateNeighbors(Room r, float minProb, float maxProb)
    {
        List<Room> result = new List<Room>();
        foreach (Sides s in Enum.GetValues(typeof(Sides)))
        {
            Vector2Int rndSize = RandomDimension(r.Width, r.Height);
            Room n = roomGenerator.GenerateNeighborRoom(r, rndSize.x, rndSize.y, s);
            if (n != null)
            {
                result.Add(n);
            }
        }
        return result;
    }

    private List<Room> GenerateRandomNeighbors(Room r, float minProb, float maxProb)
    {
        float Pleft = Random.Range(minProb, maxProb);
        float Pright = Random.Range(minProb, maxProb);
        float Pbottom = Random.Range(minProb, maxProb);
        float Ptop = Random.Range(minProb, maxProb);
        bool left = Random.Range(0f,1f) <= Pleft;
        bool right = Random.Range(0f, 1f) <= Pright;
        bool bottom = Random.Range(0f, 1f) <= Pbottom;
        bool top = Random.Range(0f, 1f) <= Ptop;
        List<Room> result = new List<Room>();
        if (left)
        {
            Vector2Int rndSize = RandomDimension(r.Width, r.Height);
            Room n = roomGenerator.GenerateNeighborRoom(r, rndSize.x, rndSize.y, Sides.Left);
            if (n != null)
            {
                result.Add(n);
            }
        }
        if (right)
        {
            Vector2Int rndSize = RandomDimension(r.Width, r.Height);
            Room n = roomGenerator.GenerateNeighborRoom(r, rndSize.x, rndSize.y, Sides.Right);
            if (n != null)
            {
                result.Add(n);
            }
        }
        if (bottom)
        {
            Vector2Int rndSize = RandomDimension(r.Width, r.Height);
            Room n = roomGenerator.GenerateNeighborRoom(r, rndSize.x, rndSize.y, Sides.Bottom);
            if (n != null)
            {
                result.Add(n);
            }
        }
        if (top)
        {
            Vector2Int rndSize = RandomDimension(r.Width, r.Height);
            Room n = roomGenerator.GenerateNeighborRoom(r, rndSize.x, rndSize.y, Sides.Top);
            if (n != null)
            {
                result.Add(n);
            }
        }
        return result;
    }

    private Vector2Int RandomDimension(int w, int h)
    {
        int rndw = Mathf.FloorToInt(Random.Range(/*w * ShrinkFactor*/MinRoomWidth, /*w * GrowFactor*/MaxRoomWidth));
        int rndh = Mathf.FloorToInt(Random.Range(/*h * ShrinkFactor*/MinRoomHeight, /*h * GrowFactor*/ MaxRoomHeight));
        return new Vector2Int(rndw, rndh);
    }
}
