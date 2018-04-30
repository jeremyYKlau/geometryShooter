using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public Map[] maps;
    public int mapIndex;

    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform mapFloor;
    public Transform navMeshFloor;
    public Transform navMeshMask;

    public Vector2 maxMapSize;

    [Range(0, 1)]
    public float outlinePercent; //how dark the outline of each squares of the map

    public float tileSize;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;
    Queue<Coord> shuffledOpenTileCoords;
    Transform[,] tileMap;

    Map currentMap;

    void Start()
    {
        FindObjectOfType<Spawner>().onNewWave += onNewWave;
    }

    void onNewWave (int waveNum)
    {
        mapIndex = waveNum - 1;
        generateMap();
    }
    public void generateMap()
    {
        currentMap = maps[mapIndex];
        tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
        System.Random rngHeight = new System.Random(currentMap.seed);
        //Generate Coords for map
        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.shuffleArray(allTileCoords.ToArray(), currentMap.seed));
 
        //Create map holder object for organization
        string placeHolder = "Generated Map";
        if (transform.Find(placeHolder))
        {
            //because called from editor we must call DestroyImmediate instead of just destroy
            DestroyImmediate(transform.Find(placeHolder).gameObject);
        }

        Transform mapHolder = new GameObject(placeHolder).transform;
        mapHolder.parent = this.transform;

        //Spawning tiles
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                Vector3 tilePosition = coordToPos(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = mapHolder;
                tileMap[x, y] = newTile;
            }
        }

        //Spawning obstacles using an 2d array of bools to work with our floodfill algorithm
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

        int obstacleToGen = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;
        List<Coord> openCoords = new List<Coord>(allTileCoords);

        for (int i = 0; i < obstacleToGen; i++)
        {
            Coord randCoord = getRandCoord();
            obstacleMap[randCoord.x, randCoord.y] = true;
            currentObstacleCount++;

            if (randCoord != currentMap.mapCenter && mapPlayable(obstacleMap, currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.MaxObstacleHeight, (float)rngHeight.NextDouble());//note cast nextdouble to float because unity works almost exclusively in floats
                Vector3 obstaclePos = coordToPos(randCoord.x, randCoord.y);

                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePos + Vector3.up * obstacleHeight / 2, Quaternion.identity) as Transform;//fix obstacles spawning half way into the ground
                newObstacle.parent = mapHolder;
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMat = new Material(obstacleRenderer.sharedMaterial);
                float colourPercent = randCoord.y / (float)currentMap.mapSize.y;
                obstacleMat.color = Color.Lerp(currentMap.foregroundColour, currentMap.backgroundColour, colourPercent);
                obstacleRenderer.sharedMaterial = obstacleMat;

                openCoords.Remove(randCoord);
            }
            else
            {
                obstacleMap[randCoord.x, randCoord.y] = false;
                currentObstacleCount--;
            }

        }
        shuffledOpenTileCoords = new Queue<Coord>(Utility.shuffleArray(openCoords.ToArray(), currentMap.seed));

        //these two transforms are to ensure the ai doesn't cut corners when navigating the map by adding a mask around the edges of the map called navMeshFloor
        Transform maskLeft = Instantiate(navMeshMask, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskRight = Instantiate(navMeshMask, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskTop = Instantiate(navMeshMask, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        Transform maskBot = Instantiate(navMeshMask, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskBot.parent = mapHolder;
        maskBot.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        navMeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
        mapFloor.localScale = new Vector3(currentMap.mapSize.x * tileSize, currentMap.mapSize.y * tileSize);
    }
    //created using a floodfill algorithm to make sure the map can be played and no blocking obstacles that block sections of map
    bool mapPlayable(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(currentMap.mapCenter);
        mapFlags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;
        int accessibleTileCount = 1;

        while (queue.Count > 0) {
            Coord tile = queue.Dequeue();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x == 0 || y == 0)
                    {
                        if ((neighbourX >= 0) && (neighbourX < obstacleMap.GetLength(0)) && (neighbourY >= 0) && (neighbourY < obstacleMap.GetLength(1)))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }

            }
        }
        int targetAccessibeCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
        return targetAccessibeCount == accessibleTileCount;
    }

    Vector3 coordToPos(int x, int y)
    {
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }

    //method used to spawn enemies on player if they are camping i personally don't like so when done tutorial remove it
    public Transform getTileFromPos(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / tileSize + (currentMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / tileSize + (currentMap.mapSize.y - 1) / 2f);
        x = Mathf.Clamp(x, 0, tileMap.GetLength(0) -1);
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1) -1);
        return tileMap[x, y];
    }


    public Coord getRandCoord()
    {
        Coord randCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randCoord);
        return randCoord;
    }

    public Transform getRandomOpenTile()
    {
        Coord randCoord = shuffledOpenTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randCoord);
        return tileMap[randCoord.x, randCoord.y];
    }

    //a struct for our 2d coordinates used in placeing obstacles
    [System.Serializable]
    public struct Coord
    {
        public int x;
        public int y;
        public Coord(int tempX, int tempY)
        {
            x = tempX;
            y = tempY;
        }
        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }
        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
    }
    //public map class for access in editor
    [System.Serializable]
    public class Map
    {
        public Coord mapSize;
        [Range(0,1)]
        public float obstaclePercent;
        public int seed;
        public float minObstacleHeight;
        public float MaxObstacleHeight;
        public Color foregroundColour;
        public Color backgroundColour;

        public Coord mapCenter
        {
            get
            {
                return new Coord(mapSize.x / 2, mapSize.y / 2);
            }
        }
    }
}
