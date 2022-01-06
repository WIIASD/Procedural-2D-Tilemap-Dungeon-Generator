using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapDungeonManager : MonoBehaviour
{
    //public TilemapDungeonGenerator DungeonGenerator;
    //public TilemapRoom PlayerRoom;
    //public List<TilemapRoom> VisitedRooms;
    //private bool playerSpawned = false;
    ////private bool currentRoomCleared = true;


    //private void Start()
    //{
    //    VisitedRooms = new List<TilemapRoom>();
    //    DungeonGenerator = GetComponent<TilemapDungeonGenerator>();
    //}
    //private void Update()
    //{
    //    if (!playerSpawned)
    //    {
    //        playerSpawned = true;
    //        SpawnPlayer();
    //    }
    //    PlayerRoom = getPlayerRoom();
    //    if (PlayerRoom == null)//in hallway
    //    {

    //    }
    //    else if (!VisitedRooms.Contains(PlayerRoom))
    //    {   
    //        if (!PlayerRoom.cleared)
    //        {
    //            DungeonGenerator.roomGenerator.CloseRoomExits(PlayerRoom);
    //        }
    //        else
    //        {
    //            DungeonGenerator.roomGenerator.OpenRoomExits(PlayerRoom);
    //            VisitedRooms.Add(PlayerRoom);
    //        }
    //        //TODO:
    //        //do something to change playerRoom.cleared
    //        //For now press SPACE to clear the room for testing purposes
    //        if (Input.GetKeyDown(KeyCode.Space))
    //        {
    //            PlayerRoom.cleared = true;
    //        }
    //    }

    //}

    //private void SpawnPlayer()
    //{

    //}

    //public bool setPlayerRoomByID(int RoomID)
    //{
    //    TilemapRoom r = FindRoomByID(RoomID);
    //    if (r == null) return false;
    //    //Player.transform.position = r.AreaColliderWorldPosition;
    //    return true;
    //}

    //public TilemapRoom FindRoomByID(int ID)
    //{
    //    foreach (TilemapRoom r in DungeonGenerator.Rooms)
    //    {
    //        if (ID == r.ID)
    //        {
    //            return r;
    //        }
    //    }
    //    return null;
    //}

    ////public TilemapRoom getPlayerRoom()
    ////{
    ////    BoxCollider2D playerBc = Player.GetComponent<BoxCollider2D>();
    ////    foreach (TilemapRoom r in DungeonGenerator.Rooms)
    ////    {
    ////        if (r.bc.bounds.Contains(playerBc.bounds.max) && r.bc.bounds.Contains(playerBc.bounds.min))
    ////        {
    ////            return r;
    ////        }
    ////    }
    ////    return null;
    ////}
}
