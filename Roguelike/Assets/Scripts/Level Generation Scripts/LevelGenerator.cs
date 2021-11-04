using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    public Tilemap canopy;
    public RuleTile canopyTile;

    public Tilemap grass;
    public RandomTile grassTile;
    public RuleTile dirtTile;
    
    enum TileType {
        wall,
        ground,
        door
    }

    float unitSize = 1f;
    public int gridSize = 150;

    IntRange numRooms = new IntRange(3, 4);
    IntRange roomRadius = new IntRange(7, 7);
    TileType[,] wallTiles;

    bool[,] grassTiles;

    public GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        wallTiles = new TileType[gridSize, gridSize];
        grassTiles = new bool[gridSize, gridSize];

        for (int i = 0; i < wallTiles.GetLength(0); i++) {
            for(int j = 0; j < wallTiles.GetLength(1); j++) {
                wallTiles[i, j] = TileType.wall;
            }
        }

        MakeRooms();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) {
            RandomizeWalls(initialGroundChance);
            RandomizeDirt(initialGroundChance);
            MakeRooms();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SmoothDirt();
            SmoothWalls();
            MakeRooms();
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            GenerateRoomsCircular();
            MakeRooms();
        }
    }

    // C.A. variables
    public float initialGroundChance = 0.9f;
    public int birthLimit = 2;
    public int deathLimit = 8;

    void SmoothWalls() {
        TileType[,] newTiles = new TileType[gridSize, gridSize];

        for (int i = 0; i < wallTiles.GetLength(0); i++) {
            for (int j = 0; j < wallTiles.GetLength(1); j++) {

                int numNeighbors = CountNeighborsWalls(i, j);
                if (wallTiles[i,j] == TileType.ground) {
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

    void RandomizeWalls(float groundChance) {
        for (int i = 0; i < wallTiles.GetLength(0); i++) {
            for (int j = 0; j < wallTiles.GetLength(1); j++) {
                if (Random.value < groundChance)
                    wallTiles[i, j] = TileType.ground;
                else
                    wallTiles[i, j] = TileType.wall;
            }
        }
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

    // FOR CIRCULAR ROOM GENERATION
    void GenerateRoomsCircular() {
        int roomCount = numRooms.Num();
        (float, float) center = (wallTiles.GetLength(0) / 2f, wallTiles.GetLength(1) / 2f);
        int radius = roomRadius.Num();

        float angle = Random.Range(0, 2 * Mathf.PI);
        Vector2 nextRoomDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;

        // Place player at center of first room
        player.transform.position = new Vector2(center.Item1, center.Item2) * unitSize;

        for (int i = 0; i < roomCount; i++) {
            // Generate room with center and radius
            GenerateRoom(center, radius);

            // Calculate new center and radius for next room
            nextRoomDirection = GetRandomDirection(nextRoomDirection);
            int newRadius = roomRadius.Num();
            center = (center.Item1 + (nextRoomDirection * (radius + (newRadius + 2.5f))).x,
                      center.Item2 + (nextRoomDirection * (radius + (newRadius + 2.5f))).y);
            radius = newRadius;
        }
    }

    Vector2 GetRandomDirection(Vector2 oldDirection) {

        Vector2[] directions = {new Vector2(1, 1).normalized,
                                new Vector2(1, -1).normalized,
                                new Vector2(-1, 1).normalized,
                                new Vector2(-1, -1).normalized,};
        return directions[Random.Range(0, 4)];

        /*float angle = Mathf.Asin(oldDirection.y) + Random.Range(-Mathf.PI / 2, Mathf.PI / 2);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized; */
    }

    void GenerateRoom((float, float) center, int radius) {

        int numAngleSteps = 60;
        int numRadiusSteps = 30;
        for (float i = 0; i < 2 * Mathf.PI; i += 2 * Mathf.PI / numAngleSteps) {
            for(float j = 0; j < radius; j += (float)radius / numRadiusSteps) {

                (float, float) coord = (center.Item1 + (Mathf.Cos(i) * j),
                                        center.Item2 + (Mathf.Sin(i) * j));

                wallTiles[Mathf.RoundToInt(coord.Item1), Mathf.RoundToInt(coord.Item2)] = TileType.ground;
            }
        }

        wallTiles[(int)center.Item1, (int)center.Item2] = TileType.door;
    }


    // GENERAL TOOLS
    void MakeRooms() {
        for (int i = 0; i < wallTiles.GetLength(0); i++) {
            for (int j = 0; j < wallTiles.GetLength(1); j++) {

                Vector3Int position = new Vector3Int(i, j, 0);

                if(grassTiles[i,j]) {
                    grass.SetTile(position, dirtTile);
                } else {
                    grass.SetTile(position, grassTile);
                }


                if(wallTiles[i,j] == TileType.wall) {
                    canopy.SetTile(position, canopyTile);

                    //Vector2 position = new Vector3(i, j, 0) * unitSize;
                    //Instantiate(Resources.Load("LevelGen/Tree", typeof(GameObject)) as GameObject,
                    //        position, Quaternion.identity);
                } else {
                    canopy.SetTile(position, null);
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
}
