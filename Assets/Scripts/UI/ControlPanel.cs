using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class ControlPanel : MonoBehaviour
{
    public static int StartRoomWidth_DEFAULT = 10, 
                      StartRoomHeight_DEFAULT = 10,
                      RoomMinWidth_DEFAULT = 10,
                      RoomMinHeight_DEFAULT = 10,
                      RoomMaxWidth_DEFAULT = 10,
                      RoomMaxHeight_DEFAULT = 10,
                      RoomCount_DEFAULT = 30;

    public TMP_InputField StartRoomWidth, StartRoomHeight, RoomMinWidth, RoomMinHeight,
               RoomMaxWidth, RoomMaxHeight, RoomCount;

    public Button Generate;

    private DungeonGenerator dungeonGenerator;

    private void Awake()
    {
        dungeonGenerator = FindObjectOfType<DungeonGenerator>();
    }

    private void Start()
    {
        Generate.onClick.AddListener(GenerateDungeon);
    }

    public void GenerateDungeon()
    {
        if (dungeonGenerator == null) throw new Exception("No Dungeon Generator Found!");

        int startRoomWidth = getPosIntInput(StartRoomWidth),
            startRoomHeight = getPosIntInput(StartRoomHeight),
            roomMinWidth = getPosIntInput(RoomMinWidth),
            roomMinHeight = getPosIntInput(RoomMinHeight),
            roomMaxWidth = getPosIntInput(RoomMaxWidth),
            roomMaxHeight = getPosIntInput(RoomMaxHeight),
            roomCount = getPosIntInput(RoomCount);

        dungeonGenerator.StartWidth = startRoomWidth == -1 ? StartRoomWidth_DEFAULT : startRoomWidth;
        dungeonGenerator.StartHeight = startRoomHeight == -1 ? StartRoomHeight_DEFAULT : startRoomHeight;
        dungeonGenerator.MinRoomWidth = roomMinWidth == -1 ? RoomMinWidth_DEFAULT : roomMinWidth;
        dungeonGenerator.MinRoomHeight = roomMinHeight == -1 ? RoomMinHeight_DEFAULT : roomMinHeight;
        dungeonGenerator.MaxRoomWidth = roomMaxWidth == -1 ? RoomMaxWidth_DEFAULT : roomMaxWidth;
        dungeonGenerator.MaxRoomHeight = roomMaxHeight == -1 ? RoomMaxHeight_DEFAULT : roomMaxHeight;
        dungeonGenerator.RoomCount = roomCount == -1 ? RoomCount_DEFAULT : roomCount;

        dungeonGenerator.generateDungeon();
    }

    public void InitInputFields(TMP_InputField inputField)
    {
        
    }

    private int getPosIntInput(TMP_InputField inputField)
    {
        int value;
        if (int.TryParse(inputField.text, out value))
        {
            return (value > 0 ? value : -1);
        }
        return -1;
    }
}
