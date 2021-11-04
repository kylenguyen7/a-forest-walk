using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class RoomManager : MonoBehaviour
{
    public int radius = -1;
    public int width = -1;
    public int height = -1;
    public Collider2D roomCameraBounds;    

    public Vector2 indices;
    UnityEngine.Experimental.Rendering.Universal.Light2D myLight;
    public bool finished = false;

    public bool isLastRoom = false;
    public int roomIndex = -1;
    public List<Enemy> myEnemies;

    public enum connections {
        error,
        north,
        south,
        east,
        west
    }
    public List<connections> myConnections;

    public enum roomTypes {
        rectangle,
        circle
    }
    public roomTypes myType = roomTypes.rectangle;

    EnterPositionManager[] myEnterPositions;

    // Wave management variables
    public LevelScriptableObject[] possibleLevels;
    public LevelScriptableObject[] bossLevels;
    LevelScriptableObject myLevel;
    private int waveIndex = 0;
    
    private enum roomStates {
        inactive,
        active,
        finished
    }
    private roomStates state = roomStates.inactive;
    List<DoorManager> myDoors = new List<DoorManager>();

    public delegate void OnRoomFinishedCallback();
    public OnRoomFinishedCallback OnRoomFinished;

    bool displayedBossMessage = false;
    bool waitedForPlayerToLeave = false;
    bool left = false;

    void Start()
    {
        myEnterPositions = GetComponentsInChildren<EnterPositionManager>();
        myEnemies = new List<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        // If player has entered the room
        if(state == roomStates.active) {
            if(isLastRoom && !displayedBossMessage) {
                MusicPlayer.instance.PauseThenPlay(MusicPlayer.Track.bossTheme);
                GameManager.instance.CreateMessage("Boss Level!");
                displayedBossMessage = true;
            }

            // and there are no enemies remaining
            if (myEnemies.Count == 0) {
                // spawn a new wave
                if(waveIndex < myLevel.wavePaths.Length) {
                    LevelJSONReader.CreateRoom(transform.position, myLevel.wavePaths[waveIndex]);

                    PlayerController.instance.SetPlayerInvincible();

                    waveIndex++;
                } else { // or end room
                    state = roomStates.finished;
                    finished = true;
                    Debug.Log("Finishing room!");
                    
                    StartCoroutine(FinishRoom());
                }
            }

            // Debug.Log($"Enemy count: {myEnemies.Count}");
        }

        if(waitedForPlayerToLeave) {
            if(PlayerController.instance.currentRoomIndex == indices && !left) {
                if(GameManager.instance.CurrentLevel == 0) {
                    foreach (DoorManager door in myDoors) {
                        door.ShowArrow();
                    }
                }
            } else {
                left = true;
                foreach(DoorManager door in myDoors) {
                    door.HideArrow();
                }
            }
        }
    }

    IEnumerator FinishRoom() {
        yield return new WaitForFixedUpdate();

        // Destroy projectiles
        var projectiles = Physics2D.OverlapBoxAll(transform.position, new Vector2(width, height), 0, LayerMask.GetMask("Projectiles"));

        foreach (Collider2D collider in projectiles) {
            if (collider == null) continue;
            var projectile = collider.GetComponent<ProjectileController>();
            if (projectile == null) continue;
            projectile.MyDestroy();
        }

        // Activate doors
        foreach (DoorManager door in myDoors) {
            door.Activate();
        }

        // Trigger OnRoomEnd() buffs
        BuffManager.instance.OnRoomEnd();

        if (OnRoomFinished != null) {
            OnRoomFinished.Invoke();
        }

        StartCoroutine(WaitForPlayerToLeaveRoom());

        if(isLastRoom) {
            if (GameManager.instance.OnLastLevel)
                MusicPlayer.instance.Play(MusicPlayer.Track.lastLevelTheme);
            else
                MusicPlayer.instance.Play(MusicPlayer.Track.mainTheme);

            var ladder = Resources.Load("LevelGen/Ladder (Level End)", typeof(GameObject)) as GameObject;
            // var scroll = Resources.Load("LevelGen/Scroll (Level End)", typeof(GameObject)) as GameObject;
            Instantiate(ladder, transform.position + new Vector3(0, 0, 0), Quaternion.identity);
            // Instantiate(scroll, transform.position + new Vector3(1, 0, 0), Quaternion.identity);
        }
    }

    IEnumerator WaitForPlayerToLeaveRoom() {
        yield return new WaitForSeconds(8f);
        waitedForPlayerToLeave = true;
    }

    public void InitializeRoomInfo(int rad, Vector2 roomIndices, connections defaultConnection) {
        radius = rad;
        var collider = gameObject.AddComponent<PolygonCollider2D>();

        collider.points = new Vector2[] {
            new Vector2(-LevelGenerator2.roomWidth/2f, -LevelGenerator2.roomHeight/2f),
            new Vector2(-LevelGenerator2.roomWidth/2f,  LevelGenerator2.roomHeight/2f),
            new Vector2( LevelGenerator2.roomWidth/2f,  LevelGenerator2.roomHeight/2f),
            new Vector2( LevelGenerator2.roomWidth/2f, -LevelGenerator2.roomHeight/2f)
        };

        collider.isTrigger = true;
        roomCameraBounds = collider;

        myType = roomTypes.circle;

        InitializeRoom(roomIndices, defaultConnection);
    }

    public void InitializeRoomInfo(int w, int h, Vector2 roomIndices, connections defaultConnection) {
        width = w;
        height = h;

        var collider = gameObject.AddComponent<PolygonCollider2D>();
        float vMargin = 2f;
        float hMargin = 2f;
        collider.points = new Vector2[] {
            new Vector2(-width/2f - hMargin, -height/2f - vMargin),
            new Vector2(-width/2f - hMargin,  height/2f + vMargin),
            new Vector2( width/2f + hMargin,  height/2f + vMargin),
            new Vector2( width/2f + hMargin, -height/2f - vMargin)
        };

        collider.isTrigger = true;
        roomCameraBounds = collider;

        InitializeRoom(roomIndices, defaultConnection);
    }

    void InitializeRoom(Vector2 roomIndices, connections defaultConnection) {
        indices = roomIndices;
        myConnections = new List<connections>();

        if(isLastRoom) {
            myLevel = bossLevels[GameManager.instance.CurrentLevel];
        } else {
            myLevel = possibleLevels[Random.Range(0, possibleLevels.Length - 1)];
        }
        
        LevelJSONReader.CreateRoom(transform.position, myLevel.baseLevelPath);

        /*var light = Instantiate(Resources.Load("LevelGen/Room Light", typeof(GameObject)) as GameObject, transform);
        myLight = light.GetComponent<Light2D>();
        myLight.pointLightOuterRadius = rad * 1.5f;
        myLight.pointLightInnerRadius = rad * 1.5f - 2;*/

        // At the very least, this room has one connection: to the room that created it
        if (defaultConnection != connections.error) {
            myConnections.Add(defaultConnection);
        }
    }

    public void UpdateConnections() {

        RoomManager[] managers = FindObjectsOfType<RoomManager>();
        foreach(connections connection in myConnections) {

            Vector2 targetIndices = indices;
            connections connectionToAdd = connections.error;

            // set target indices and connection to add to that room based on connection
            switch (connection) {
                case connections.east: {
                        connectionToAdd = connections.west;
                        targetIndices = indices + new Vector2(1, 0);
                        break;
                    }
                case connections.west: {
                        connectionToAdd = connections.east;
                        targetIndices = indices + new Vector2(-1, 0);
                        break;
                    }
                case connections.north: {
                        connectionToAdd = connections.south;
                        targetIndices = indices + new Vector2(0, 1);
                        break;
                    }
                case connections.south: {
                        connectionToAdd = connections.north;
                        targetIndices = indices + new Vector2(0, -1);
                        break;
                    }
            }

            foreach(RoomManager manager in managers) {
                if(manager.indices == targetIndices) {
                    if(!manager.myConnections.Contains(connectionToAdd)) {
                        manager.myConnections.Add(connectionToAdd);
                    }
                }
            }
        }
    }

    public void CreateDoors() {
        foreach(connections connection in myConnections) {

            Vector2 position = transform.position;
            Quaternion rotation = Quaternion.identity;
            bool doSpawn = true;
            switch (connection) {
                case connections.east: {

                        // Circular vs. rectangular room
                        if(radius != -1) {
                            position = (Vector2)transform.position + new Vector2(radius + 0.5f, 0);
                        } else {
                            position = (Vector2)transform.position + new Vector2(width / 2, 0);
                        }
                        
                        rotation = Quaternion.Euler(0, 0, -90);
                        break;
                    }
                case connections.west: {
                        if (radius != -1) {
                            position = (Vector2)transform.position + new Vector2(-(radius + 0.5f), 0);
                        } else {
                            position = (Vector2)transform.position + new Vector2(-width / 2, 0);
                        }
                        rotation = Quaternion.Euler(0, 0, 90);
                        break;
                    }
                case connections.north: {
                        if (radius != -1) {
                            position = (Vector2)transform.position + new Vector2(0, radius + 0.5f);
                        } else {
                            position = (Vector2)transform.position + new Vector2(0, height / 2);
                        }
                        rotation = Quaternion.identity;
                        break;
                    }
                case connections.south: {
                        if (radius != -1) {
                            position = (Vector2)transform.position + new Vector2(0, -(radius + 0.5f));
                        } else {
                            position = (Vector2)transform.position + new Vector2(0, -height / 2);
                        }
                        rotation = Quaternion.Euler(0, 0, 180);
                        break;
                    }
                case connections.error: {
                        doSpawn = false;
                        break;
                    }
            }

            if (doSpawn) {
                var doorManager = Instantiate(Resources.Load("LevelGen/Door", typeof(GameObject)) as GameObject, position, rotation).GetComponent<DoorManager>();
                doorManager.myConnection = connection;
                myDoors.Add(doorManager);
            }
        }
    }

    public Vector2 GetEnterPosition(connections from) {
        foreach(EnterPositionManager enterPos in myEnterPositions) {
            if(enterPos.from == from) {
                return enterPos.transform.position;
            }
        }

        Debug.LogError("Failed to find EnterPosition to transport player into room!");
        return Vector2.zero;
    }

    public static RoomManager findRoomManagerByIndex(Vector2 indices) {
        foreach(RoomManager manager in FindObjectsOfType<RoomManager>()) {
            if(manager.indices == indices) {
                return manager;
            }
        }

        return null;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && state != roomStates.finished) {
            StartCoroutine("Activate");
        }
    }

    IEnumerator Activate() {
        yield return new WaitForSeconds(0.5f);
        state = roomStates.active;
    }
}
