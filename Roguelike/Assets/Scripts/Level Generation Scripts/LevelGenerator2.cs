using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;

public class LevelGenerator2 : MonoBehaviour {

    // Mechanics
    enum TileType {
        wall,
        ground,
        door
    }
    public int gridSize = 120;

    // Keeps track of where centers have been generated
    List<(int, int)> roomCenters;

    // Keeps track of room indices and lastConnection for Room Managers
    Vector2Int indices;
    RoomManager.connections lastConnection;

    // Chance to spawn an offshoot room
    float offshootChance = 0f;

    public static int roomWidth = 22;
    public static int roomHeight = 12;
    public static int roomBuffer = 5;

    IntRange numRooms = new IntRange(3, 4); // new IntRange(4, 5);
    int circularRoomRadius = 5;
    TileType[,] wallTiles;
    bool[,] grassTiles;

    public GameObject player;

    // Tile resources
    public Tilemap canopyNoCollisions;
    public RuleTile canopyTile;

    public Tilemap canopyCollisions;
    public Tile unitCollisionTile;

    public Tilemap grass;
    public RandomTile grassTile;
    public RuleTile dirtTile;

    // C.A. variables for dirt
    public float initialGroundChance = 0.48f;
    public int birthLimit = 4;
    public int deathLimit = 4;

    // RoomManager lastRoom;
    void Start() {

        wallTiles = new TileType[gridSize, gridSize];
        grassTiles = new bool[gridSize, gridSize];
        roomCenters = new List<(int, int)>();

        for (int i = 0; i < wallTiles.GetLength(0); i++) {
            for (int j = 0; j < wallTiles.GetLength(1); j++) {
                wallTiles[i, j] = TileType.wall;
            }
        }

        //RandomizeDirt(initialGroundChance);
        for (int i = 0; i < 7; i++) {
            // SmoothDirt();
        }

        GenerateRooms();
        PostGeneration();
        MakeRooms();
    }

    // Update is called once per frame
    void Update() {
        /*
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SmoothWalls();
        }
        */
    }

    void PostGeneration() {
        // Update all connections, adding door tiles
        foreach(RoomManager room in FindObjectsOfType<RoomManager>()) {
            room.UpdateConnections();
        }

        foreach (RoomManager room in FindObjectsOfType<RoomManager>()) {
            AddDoorTiles(room);
        }

        // Add doors
        foreach (RoomManager room in FindObjectsOfType<RoomManager>()) {
            room.CreateDoors();
        }

        // Generate level end objects
        // lastRoom.isLastRoom = true;
    }

    void AddDoorTiles(RoomManager roomManager) {
        Vector2 center = roomManager.transform.position;

        // Make door tiles for room (rectangle)
        if (roomManager.myType == RoomManager.roomTypes.rectangle) {
            foreach (RoomManager.connections connection in roomManager.myConnections) {
                switch (connection) {
                    case RoomManager.connections.north: {
                            wallTiles[(int)center.x, (int)center.y + roomHeight / 2] = TileType.door;
                            wallTiles[(int)center.x - 1, (int)center.y + roomHeight / 2] = TileType.door;
                            break;
                        }
                    case RoomManager.connections.east: {
                            wallTiles[(int)center.x + roomWidth / 2, (int)center.y] = TileType.door;
                            wallTiles[(int)center.x + roomWidth / 2, (int)center.y - 1] = TileType.door;
                            break;
                        }
                    case RoomManager.connections.south: {
                            wallTiles[(int)center.x, (int)center.y - roomHeight / 2 - 1] = TileType.door;
                            wallTiles[(int)center.x - 1, (int)center.y - roomHeight / 2 - 1] = TileType.door;
                            break;
                        }
                    case RoomManager.connections.west: {
                            wallTiles[(int)center.x - roomWidth / 2 - 1, (int)center.y] = TileType.door;
                            wallTiles[(int)center.x - roomWidth / 2 - 1, (int)center.y - 1] = TileType.door;
                            break;
                        }
                }
            }
        }

        // Make door tiles for room (circle)
        if (roomManager.myType == RoomManager.roomTypes.circle) {
            // Debug.Log("Attempting to add door tiles for circular room.");
            foreach (RoomManager.connections connection in roomManager.myConnections) {
                // Debug.Log("adding circular room connection to the " + connection);
                switch (connection) {
                    case RoomManager.connections.north: {
                            wallTiles[(int)center.x + 1, (int)center.y + circularRoomRadius + 1] = TileType.door;
                            wallTiles[(int)center.x, (int)center.y + circularRoomRadius + 1] = TileType.door;
                            wallTiles[(int)center.x - 1, (int)center.y + circularRoomRadius + 1] = TileType.door;
                            break;
                        }
                    case RoomManager.connections.east: {
                            wallTiles[(int)center.x + circularRoomRadius + 1, (int)center.y + 1] = TileType.door;
                            wallTiles[(int)center.x + circularRoomRadius + 1, (int)center.y] = TileType.door;
                            wallTiles[(int)center.x + circularRoomRadius + 1, (int)center.y - 1] = TileType.door;
                            break;
                        }
                    case RoomManager.connections.south: {
                            wallTiles[(int)center.x + 1, (int)center.y - circularRoomRadius - 1] = TileType.door;
                            wallTiles[(int)center.x, (int)center.y - circularRoomRadius - 1] = TileType.door;
                            wallTiles[(int)center.x - 1, (int)center.y - circularRoomRadius - 1] = TileType.door;
                            break;
                        }
                    case RoomManager.connections.west: {
                            wallTiles[(int)center.x - circularRoomRadius - 1, (int)center.y + 1] = TileType.door;
                            wallTiles[(int)center.x - circularRoomRadius - 1, (int)center.y] = TileType.door;
                            wallTiles[(int)center.x - circularRoomRadius - 1, (int)center.y - 1] = TileType.door;
                            break;
                        }
                }
            }
        }
    }
    
    void GenerateRooms() {
        int roomCount = numRooms.Num();
        (int, int) center = (gridSize / 2, gridSize / 2);

        // Make first room
        GenerateRectRoom(center, roomWidth, roomHeight, false, 0);
        roomCenters.Add(center);

        // Place player at center of first room
        var startCenter = new Vector2(center.Item1, center.Item2);
        player.transform.position = startCenter;

        // Generate more rooms
        int roomsGenerated = 0;
        do {
            // Get valid direction for new center
            center = GetNewCenter(center);
            if(center == (-1, -1)) {
                Debug.Log("Since we failed to find a new room, exiting room generation early.");
                break;
            }

            bool isLast = roomsGenerated == roomCount - 1;
            GenerateRectRoom(center, roomWidth, roomHeight, isLast, roomsGenerated + 1);
            roomCenters.Add(center);

            // Generate an offshoot room with offshootChance, unless this is the finish room
            if(roomsGenerated + 1 < roomCount && Random.value < offshootChance) {
                // GetNewCenter changes indices in a way that we don't want...
                (int, int) offshootCenter = GetNewCenter(center);
                if (offshootCenter == (-1, -1)) {
                    Debug.Log("Since we failed to find a new room, exiting room generation early.");
                    break;
                }
                GenerateCircleRoom(offshootCenter, circularRoomRadius);
                roomCenters.Add(offshootCenter);

                // So we have to revert indices
                switch (lastConnection) {
                    case RoomManager.connections.east: indices += new Vector2Int(1, 0);  break;
                    case RoomManager.connections.west: indices += new Vector2Int(-1, 0); break;
                    case RoomManager.connections.north: indices += new Vector2Int(0, 1); break;
                    case RoomManager.connections.south: indices += new Vector2Int(0, -1); break;
                }
            }

            roomsGenerated++;

        } while (roomsGenerated < roomCount);
    }

    // Searches adjacent positions until finding a valid new center
    (int, int) GetNewCenter((int, int) oldCenter) {
        (int, int) newCenter = oldCenter;
        Vector2Int indexChange = new Vector2Int(0, 0);
        int attempts = 0;

        do {
            indexChange = GetNewDirection();

            // + roomBuffer creates buffer between rooms
            Vector2Int deltaPosition = indexChange * new Vector2Int(roomWidth + roomBuffer, roomHeight + roomBuffer);
            newCenter = (oldCenter.Item1 + deltaPosition.x, oldCenter.Item2 + deltaPosition.y);

            attempts++;
            if (attempts > 10) {
                Debug.Log("Failed to find new room");
                return (-1, -1);
            }

        } while (roomCenters.Contains(newCenter) || !IsValidCenter(newCenter));

        SetLastConnection((indexChange.x, indexChange.y));
        indices += indexChange;
        return newCenter;
    }

    void SetLastConnection((int, int) indexChange) {

        if(indexChange == (1, 0)) {
            lastConnection = RoomManager.connections.west;
        } else if (indexChange == (0, 1)) {
            lastConnection = RoomManager.connections.south;
        } else if (indexChange == (-1, 0)) {
            lastConnection = RoomManager.connections.east;
        } else if (indexChange == (0, -1)) {
            lastConnection = RoomManager.connections.north;
        } else {
            lastConnection = RoomManager.connections.error;
        }
    }

    // Returns north, east, south, or west
    Vector2Int GetNewDirection() {
        float chance = Random.value;

        if (chance < 0.25) {
            return new Vector2Int(0, 1);
        } else if (chance < 0.5) {
            return new Vector2Int(0, -1);
        } else if (chance < 0.75) {
            return new Vector2Int(1, 0);
        } else {
            return new Vector2Int(-1, 0);
        }
    }

    bool IsValidCenter((int, int) center) {
        // A center cannot be width/height units away from 0 or gridSize

        // X out of bounds
        if (center.Item1 + roomWidth / 2 + 4 > gridSize || center.Item1 - roomWidth / 2 - 4 < 0) {
            return false;
        }

        // Y out of bounds
        if (center.Item2 + roomHeight / 2 + 4 > gridSize || center.Item2 - roomHeight / 2 - 4 < 0) {
            return false;
        }

        return true;
    }

    void GenerateCircleRoom((int, int) center, int radius) {

        int numAngleSteps = 60;
        int numRadiusSteps = 30;
        for (float i = 0; i < 2 * Mathf.PI; i += 2 * Mathf.PI / numAngleSteps) {
            for (float j = 0; j < radius; j += (float)radius / numRadiusSteps) {

                (float, float) coord = (center.Item1 + (Mathf.Cos(i) * j),
                                        center.Item2 + (Mathf.Sin(i) * j));

                wallTiles[Mathf.RoundToInt(coord.Item1), Mathf.RoundToInt(coord.Item2)] = TileType.ground;
            }
        }

        // wallTiles[center.Item1, center.Item2] = TileType.door;

        // Create room manager and give it indices
        Vector2 position = new Vector2(center.Item1 + 0.5f, center.Item2 + 0.5f);
        var manager = Instantiate(Resources.Load("LevelGen/Room Manager (Circle)", typeof(GameObject)) as GameObject, position, Quaternion.identity);
        manager.GetComponent<RoomManager>().InitializeRoomInfo(radius, indices, lastConnection);
    }

    void GenerateRectRoom((int, int) center, int width, int height, bool isLast, int roomIndex) {

        int left = center.Item1 - width / 2;
        int right = center.Item1 + width / 2;

        int bottom = center.Item2 - height / 2;
        int top = center.Item2 + height / 2;

        for (int i = left; i < right; i++) {
            for (int j = bottom; j < top; j++) {
                wallTiles[i, j] = TileType.ground;
            }
        }

        // wallTiles[center.Item1, center.Item2] = TileType.door;

        // Create room manager and give it indices
        Vector2 position = new Vector2(center.Item1, center.Item2); // new Vector2(center.Item1 + 0.5f, center.Item2 + 0.5f);
        var room = Instantiate(Resources.Load("LevelGen/Room Manager (Rect)", typeof(GameObject)) as GameObject, position, Quaternion.identity);
        var manager = room.GetComponent<RoomManager>();
        manager.isLastRoom = isLast;
        manager.roomIndex = roomIndex;
        manager.InitializeRoomInfo(width, height, indices, lastConnection);

        // lastRoom = manager;
    }

    // GENERAL TOOLS
    void MakeRooms() {
        for (int i = 0; i < wallTiles.GetLength(0); i++) {
            for (int j = 0; j < wallTiles.GetLength(1); j++) {

                Vector3Int position = new Vector3Int(i, j, 0);

                if (grassTiles[i, j]) {
                    grass.SetTile(position, dirtTile);
                } else {
                    grass.SetTile(position, grassTile);
                }

                // Walls have canopy tile and collision
                if (wallTiles[i, j] == TileType.wall) {
                    canopyNoCollisions.SetTile(position, canopyTile);
                    canopyCollisions.SetTile(position, unitCollisionTile);
                } // Doors have canopy tile and no collision
                else if(wallTiles[i, j] == TileType.door) {
                    canopyNoCollisions.SetTile(position, canopyTile);
                } // Ground has no tile
                else {
                    canopyNoCollisions.SetTile(position, null);
                }
            }
        }

        PrintTiles();
    }

    void PrintTiles() {
        string output = "----------STARTING NEW PRINTTILES()----------\n";

        for (int i = 0; i < wallTiles.GetLength(0); i++) {
            for (int j = 0; j < wallTiles.GetLength(1); j++) {
                output += (int)wallTiles[i, j];
            }
            output += "\n";
        }

        Debug.Log(output);
    }


    #region deprecated
    void SmoothWalls() {
        TileType[,] newTiles = new TileType[gridSize, gridSize];

        for (int i = 0; i < wallTiles.GetLength(0); i++) {
            for (int j = 0; j < wallTiles.GetLength(1); j++) {

                int numNeighbors = CountNeighborsWalls(i, j);
                if (wallTiles[i, j] == TileType.ground) {
                    if (numNeighbors < deathLimit) {
                        newTiles[i, j] = TileType.wall;
                    } else {
                        newTiles[i, j] = TileType.ground;
                    }
                } else {
                    if (numNeighbors > birthLimit) {
                        newTiles[i, j] = TileType.ground;
                    } else {
                        newTiles[i, j] = TileType.wall;
                    }
                }
            }
        }
        wallTiles = newTiles;
    }

    void SmoothDirt() {
        bool[,] newTiles = new bool[gridSize, gridSize];

        for (int i = 0; i < grassTiles.GetLength(0); i++) {
            for (int j = 0; j < grassTiles.GetLength(1); j++) {
                int numNeighbors = CountNeighborsDirt(i, j);
                if (grassTiles[i, j]) {
                    if (numNeighbors < deathLimit) {
                        newTiles[i, j] = false;
                    } else {
                        newTiles[i, j] = true;
                    }
                } else {
                    if (numNeighbors > birthLimit) {
                        newTiles[i, j] = true;
                    } else {
                        newTiles[i, j] = false;
                    }
                }
            }
        }
        grassTiles = newTiles;
    }

    // From an online tutorial
    int CountNeighborsWalls(int x, int y) {
        int count = 0;
        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                int neighbour_x = x + i;
                int neighbour_y = y + j;
                //If we're looking at the middle point
                if (i == 0 && j == 0) {
                    //Do nothing, we don't want to add ourselves in!
                }
                //In case the index we're looking at it off the edge of the map
                else if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= wallTiles.GetLength(0) || neighbour_y >= wallTiles.GetLength(1)) {
                    count = count + 1;
                }
                //Otherwise, a normal check of the neighbour
                else if (wallTiles[neighbour_x, neighbour_y] == TileType.ground) {
                    count = count + 1;
                }
            }
        }

        return count;
    }

    int CountNeighborsDirt(int x, int y) {
        int count = 0;
        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                int neighbour_x = x + i;
                int neighbour_y = y + j;
                //If we're looking at the middle point
                if (i == 0 && j == 0) {
                    //Do nothing, we don't want to add ourselves in!
                }
                //In case the index we're looking at it off the edge of the map
                else if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= wallTiles.GetLength(0) || neighbour_y >= wallTiles.GetLength(1)) {
                    count = count + 1;
                }
                //Otherwise, a normal check of the neighbour
                else if (grassTiles[neighbour_x, neighbour_y]) {
                    count = count + 1;
                }
            }
        }

        return count;
    }

    void RandomizeDirt(float dirtChance) {
        for (int i = 0; i < wallTiles.GetLength(0); i++) {
            for (int j = 0; j < wallTiles.GetLength(1); j++) {
                if (Random.value < dirtChance)
                    grassTiles[i, j] = true;
                else
                    grassTiles[i, j] = false;
            }
        }
    }
    #endregion
}

