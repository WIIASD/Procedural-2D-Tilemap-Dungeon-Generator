# 2D Procedural Tilemap Dungeon Generator
**Coded by:** Ziyang Li (WIIASD)  
**Last Edit:** 2/14/2022  
**Directory Structure:**
- Assets/
	- Scripts/
		- Camera/
			- [CameraController.cs](#cameracontroller.cs)
		- Room/
			- [RoomBase.cs](#roombase.cs)
			- [RectangleRoom.cs](#rectangleroom.cs)
			- Generators/
				- [RoomGenerator.cs](#roomgenerator.cs)
				- [DungeonGenerator.cs](#dungeongenerator.cs)
			- TilePalette/
				- [RoomTilePalette.cs](#roomtilepalette.cs)
				- [RoomTilePaletteManager.cs](#roomtilepalettemanager.cs)
		- UI/
			- [ControlPanel.cs](#controlpanel.cs)
		- Util/
			- [ZMonoSingleton.cs](#zmonosingleton.cs)
			- [ZTools.cs](#ztools.cs)
### CameraController.cs
Component attached to the main camera, responsible for its movement.
### RoomBase.cs
Base class for room structure
### RectangleRoom.cs
Inherit from RoomBase.cs, structure of rectangular rooms
### RoomGenerator.cs
Component exist in the scene, responsible for generating rooms with provided tile palette in corresponding tilemaps
### DungeonGenerator.cs
Component exist in the scene, responsible for generating the whole dungeon using a room generator
### RoomTilePalette.cs
Scriptable object containing data for individual tile palette
### RoomTilePaletteManager.cs
Singleton Manager class that manages palette scriptable objects
### ControlPanel.cs
This class is responsiblel for the UI panel in the Dungeon Scene
### ZMonoSingleton.cs
Base class for singleton design pattern
### ZTools.cs
Static class that provides various mathematical/structural tools

