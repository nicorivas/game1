using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public class S_Terrain : MonoBehaviour
{
    static public int tilesN = 9;
    static public float tileWidth = 2.0f;
    static public float tileDepth = 2.0f;
    bool firstTick;
    static GameObject[,] tiles;
    static List<GameObject> tilesList;
    static List<GameObject> blocks;
    public GameObject terrainTile;
    static public int[,] DIRECTIONS = {{1,0},{-1,0},{0,1},{0,-1}};
    static public Dictionary<string, int> TERRAIN_TYPES = new Dictionary<string, int> {
            {"grassland",0},
            {"river",1},
            {"desert",2},
            {"polar",3},
            {"plains",4},
        };

    void Start()
    {
        S_World.OnTick += Tick;
        Initialize();
    }

    void Initialize() {
        tiles = new GameObject[tilesN,tilesN];
        tilesList = new List<GameObject>();
        blocks = new List<GameObject>();
        firstTick = true;
        GenerateTerrain();
        S_TerrainTile t11;
        for (int ix=0; ix<tilesN; ix++) {
            for (int iz=0; iz<tilesN; iz++) {
                t11 = tiles[ix,iz].GetComponent<S_TerrainTile>();
                t11.LoadType();
            }
        }
    }

    public void ResetTerrain()
    {
        foreach (GameObject tile in tilesList) {
            Destroy(tile);
        }
        foreach (GameObject block in blocks) {
            Destroy(block);
        }
        Initialize();
    }

    static public void SpawnBlock(Vector2Int coords)
    {
        GameObject block = Instantiate(Resources.Load("Block")) as GameObject;
        block.transform.position = tiles[coords[0],coords[1]].transform.position;
        block.transform.position += Vector3.up*Config.Block_Spawn_Height;
        block.GetComponent<Rigidbody>().velocity = new Vector3(0f, -10f, 0f);
        blocks.Add(block);
    }

    static public void SpawnEnemy(Vector2Int coords, string enemyName) {
        coords = CoordsTransform(coords);
        GameObject spawner = Instantiate(Resources.Load(Path.Combine(Config.Enemies_Dir,"Spawner"))) as GameObject;
        spawner.GetComponent<S_Spawner>().enemyName = enemyName;
        spawner.GetComponent<S_Spawner>().tile = tiles[coords[0],coords[1]];
        tiles[coords[0],coords[1]].GetComponent<S_TerrainTile>().PlaceEnemy(spawner);
    }

    static public void SpawnPortal(Vector2Int coords) {
        GameObject portal = Instantiate(Resources.Load("Portal")) as GameObject;
        tiles[coords.x,coords.y].GetComponent<S_TerrainTile>().PlacePortal(portal);
    }

    static public void SpawnPower(Vector2Int coords, string powerName) {
        GameObject power = Instantiate(Resources.Load(Path.Combine(Config.Powers_Dir,powerName))) as GameObject;
        tiles[coords.x,coords.y].GetComponent<S_TerrainTile>().PlacePower(power);
    }

    static public void RiseTile(Vector2Int coords, int height) {
        tiles[coords[0],coords[1]].GetComponent<S_TerrainTile>().Rise(height);
    }

    static public void SinkTile(Vector2Int coords) {
        tiles[coords[0],coords[1]].GetComponent<S_TerrainTile>().Sink();
    }

    static public void LevelHeight() {
        //
    }

    static public void BurnTile(Vector2Int coords) {
        tiles[coords[0],coords[1]].GetComponent<S_TerrainTile>().Burn();
    }

    static public Vector2Int CoordsTransform(Vector2Int coords) {
        if (coords[0] < 0) coords[0] += tilesN;
        if (coords[1] < 0) coords[1] += tilesN;
        return coords;
    }

    void GenerateTerrain()
    {
        for (int i=0; i < tilesN; i++) {
            for (int j=0; j < tilesN; j++) {
                GameObject terrainTile = CreateTerrainTile(i,j);
                terrainTile.GetComponent<S_TerrainTile>().CalculateTemperature();
                terrainTile.GetComponent<S_TerrainTile>().CalculateType();
            }
        }
        for (int i=0; i < tilesN; i++) {
            for (int j=0; j < tilesN; j++) {
                tiles[i,j].GetComponent<S_TerrainTile>().LoadType();
            }
        }
    }

    GameObject CreateTerrainTile(int x, int z) {
        float px = (x-tilesN/2.0f)*tileWidth+tileWidth/2.0f;
        float pz = (z-tilesN/2.0f)*tileDepth+tileDepth/2.0f;
        tiles[x,z] = Instantiate(terrainTile);
        tiles[x,z].transform.parent = transform;
        tiles[x,z].transform.position = new Vector3(px,0.0f,pz);
        S_TerrainTile tileScript = tiles[x,z].GetComponent<S_TerrainTile>();
        tileScript.SetPosition(x,z);
        tilesList.Add(tiles[x,z]);
        return tiles[x,z];

    }

    public static Vector2Int GetRandomCoordinates() {
        Vector2Int coords = GetRandomCoordinatesInsideRegion(new float[]{0f,0f,1f,1f});
        return coords;
    }

    static Vector2Int GetRandomCoordinatesInsideRegion(float[] region) {
        Vector2Int coords = new Vector2Int(
            (int)(Random.Range(region[0],region[2])*tilesN),
            (int)(Random.Range(region[1],region[3])*tilesN));
        return coords;
    }

    static GameObject GetRandomTileInsideRegion(float[] region, int[] not_types=null, int[] heights=null) {
        int count = 0;
        Vector2Int coords = default(Vector2Int);
        while (count < 100) {
            coords = GetRandomCoordinatesInsideRegion(region);
            if (!heights.Contains(tiles[coords[0],coords[1]].GetComponent<S_TerrainTile>().GetHeight()))
                continue;
            if (!not_types.Contains(tiles[coords[0],coords[1]].GetComponent<S_TerrainTile>().GetTileType()))
                break;
            count++;
        }
        if (coords != null) 
            return tiles[coords[0],coords[1]];
        else
            return null;
    }

    bool CoordinateInside(int x, int z) {
        return x >= 0 && z >= 0 && x < tilesN && z < tilesN;
    }

    public GameObject GetTile(int x, int z) {
        return tiles[x,z];
    }

    void SwapTerrain(int i, int j) {
        //
    }

    void Update()
    {
        //
    }

    static public GameObject GetRandomTileUntilDistance(int x, int z, float distance) {
        int dx = -1;
        int dz = -1;
        float direction;
        while (dx < 0 || dz < 0 || dx >= tilesN || dz >= tilesN) {
            direction = ((Random.value-0.5f)/0.5f)*2f*Mathf.PI;
            distance = distance*Random.value;
            Vector2 destination = new Vector2(x+distance*Mathf.Cos(direction), z+distance*Mathf.Sin(direction));
            dx = (int)Mathf.Round(destination.x);
            dz = (int)Mathf.Round(destination.y);
        }
        return tiles[dx,dz];
    }

    static public GameObject GetRandomTileNeighbour(int x, int z) {
        int dir = (int)(Random.value*4.0f);
        return tiles[x+S_Terrain.DIRECTIONS[dir,0],z+S_Terrain.DIRECTIONS[dir,1]];
    }

    static public float DistanceToWater(GameObject gameObject) {
        GameObject[] waterGameObjects = GameObject.FindGameObjectsWithTag("Water");
        GameObject minDistanceObject;
        float minDistance = 10000.0f;
        float distance = 0.0f;
        foreach (GameObject waterGameObject in waterGameObjects) {
            distance = (waterGameObject.transform.position - gameObject.transform.position).sqrMagnitude;
            if (distance < minDistance) {
                minDistance = distance;
                minDistanceObject = waterGameObject;
            }
        }
        return Mathf.Sqrt(minDistance)/S_Terrain.tileWidth;
    }

    private void Tick(object sender, S_World.OnTickEventArgs e)
    {
        //
    }

    static public bool PositionInside(Vector3 pos) {
        if (pos.x < -(float)(tilesN/2f)*tileWidth ||
            pos.x >  (float)(tilesN/2f)*tileWidth ||
            pos.z < -(float)(tilesN/2f)*tileDepth ||
            pos.z >  (float)(tilesN/2f)*tileDepth) {
                return false;
        }
        return true;
    }

    static public Vector2Int ParseCoords(string str) {
        if (str.Contains(",")) {
            string[] coordsstr = str.Split(',');
            Vector2Int coords = new Vector2Int(0,0);
            for (int i=0; i < coordsstr.Length; i++) {
                if (coordsstr[i] == "m") {
                    coords[i] = (tilesN-1)/2;
                } else {
                    int c;
                    if (int.TryParse(coordsstr[i], out c)) {
                        coords[i] = c;
                    }
                }
            }
            return CoordsTransform(coords);
        } else if (str[0] == 't') {
            int ix;
            int iy;
            int.TryParse(str[1].ToString(), out ix);
            int.TryParse(str[2].ToString(), out iy);
            float ixf = 0.0f+(ix-1)*1.0f/3.0f;
            float iyf = 0.0f+(iy-1)*1.0f/3.0f;
            float fxf = 0.0f+(ix)*1.0f/3.0f;
            float fyf = 0.0f+(iy)*1.0f/3.0f;
            return GetRandomCoordinatesInsideRegion(new float[]{ixf,iyf,fxf,fyf});
        }
        return new Vector2Int(0,0);
    }

    void OnDestroy() {
        S_World.OnTick -= Tick;
    }

}
