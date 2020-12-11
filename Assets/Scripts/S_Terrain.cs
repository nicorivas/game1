using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_Terrain : MonoBehaviour
{
    static public int tilesN = 11;
    static public float tileWidth = 2.0f;
    static public float tileDepth = 2.0f;
    int terrainUpdateTicks = 10;
    int numberOfMountains = 2;
    bool firstTick;
    static GameObject[,] tiles;
    static List<GameObject> tilesList;
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
        // Register Tick function
        S_World.OnTick += Tick;
        tiles = new GameObject[tilesN,tilesN];
        tilesList = new List<GameObject>();
        firstTick = true;
    }

    static public void SpawnEnemy(int[] coords, GameObject enemy) {
        coords = CoordsTransform(coords);
        GameObject spawner = Instantiate(Resources.Load(Config.Enemies_Dir+"/Spawner")) as GameObject;
        spawner.GetComponent<S_Spawner>().enemy = enemy;
        spawner.GetComponent<S_Spawner>().tile = tiles[coords[0],coords[1]];
        tiles[coords[0],coords[1]].GetComponent<S_TerrainTile>().PlaceEnemy(spawner);
    }

    static public void SpawnPortal(Vector2Int coords) {
        GameObject portal = Instantiate(Resources.Load("Portal")) as GameObject;
        tiles[coords.x,coords.y].GetComponent<S_TerrainTile>().PlacePortal(portal);
    }

    static public void RiseTile(Vector2Int coords, int height) {
        tiles[coords[0],coords[1]].GetComponent<S_TerrainTile>().Rise(height);
    }

    static public int[] CoordsTransform(int[] coords) {
        if (coords[0] < 0) coords[0] += tilesN;
        if (coords[1] < 0) coords[1] += tilesN;
        return coords;
    }

    void GenerateTerrain()
    {
        // Debug.Log("S_Terrain:GenerateTerrain");    
        // Create terrain tiles.
        for (int i=0; i < tilesN; i++) {
            for (int j=0; j < tilesN; j++) {
                GameObject terrainTile = CreateTerrainTile(i,j);
                terrainTile.GetComponent<S_TerrainTile>().CalculateTemperature();
                terrainTile.GetComponent<S_TerrainTile>().CalculateType();
            }
        }
        //CreateHeight();
        //CreateRivers();
        //CreateTrees();
        for (int i=0; i < tilesN; i++) {
            for (int j=0; j < tilesN; j++) {
                tiles[i,j].GetComponent<S_TerrainTile>().LoadType();
            }
        }
        //Spawn();
    }

    void CreateHeight() {
        Debug.Log("S_Terrain:CreateHeight");
        for (int ix=0; ix < tilesN; ix++) {
            for (int iz=0; iz < tilesN; iz++) {
                tiles[ix,iz].GetComponent<S_TerrainTile>().SetHeight(0);
            }
        }
        for (int i=0; i < numberOfMountains; i++) {
            if (i==0)
                CreateMountainRange(volcano:true);
            else
                CreateMountainRange(mountain:true);
        }
        // Soften
        int dxi, dxf, dzi, dzf;
        for (int ix=0; ix < tilesN; ix++) {
            for (int iz=0; iz < tilesN; iz++) {
                int currentHeight = tiles[ix,iz].GetComponent<S_TerrainTile>().GetHeight();
                dxi = -1;
                dxf = 1;
                dzi = -1;
                dzf = 1;
                if (ix==0)
                    dxi = 1;
                if (ix==tilesN-1)
                    dxf = -1;
                if (iz==0)
                    dzi = 1;
                if (iz==tilesN-1)
                    dzf = -1;
                if (tiles[ix+dxi,iz].GetComponent<S_TerrainTile>().GetHeight() > currentHeight &&
                    tiles[ix+dxf,iz].GetComponent<S_TerrainTile>().GetHeight() > currentHeight &&
                    tiles[ix,iz+dzi].GetComponent<S_TerrainTile>().GetHeight() > currentHeight &&
                    tiles[ix,iz+dzf].GetComponent<S_TerrainTile>().GetHeight() > currentHeight) {
                    tiles[ix,iz].GetComponent<S_TerrainTile>().SetHeight(currentHeight+1);
                }
            }
        }
    }

    void CreateMountainRange(bool volcano = false, bool mountain = false) {
        int[] coords;
        if (volcano)
            coords = GetRandomCoordinatesInsideRegion(Config.Volcano_Init_Region);
        else
            coords = GetRandomCoordinatesInsideRegion(Config.Mountain_Init_Region);
        S_TerrainTile tileScript = tiles[coords[0],coords[1]].GetComponent<S_TerrainTile>();
        tileScript.SetHeight(2);
        if (volcano)
            tileScript.CreateVolcano();
        if (mountain)
            tileScript.CreateMountain();
        for (int ix=coords[0]-2; ix <= coords[0]+2; ix++) {
            for (int iz=coords[1]-2; iz <= coords[1]+2; iz++) {
                if (ix==coords[0] && iz==coords[1]) continue;
                if (CoordinateInside(ix,iz)) {
                    if (Random.value < 0.5) {
                        tileScript = tiles[ix,iz].GetComponent<S_TerrainTile>();
                        tileScript.SetHeight(1);
                    }
                }
            }
        }
    }

    void CreateRivers() {
        List<GameObject> mountains = tilesList.FindAll(t => t.GetComponent<S_TerrainTile>().GetHeight() == 2);
        foreach (GameObject mountain in mountains) {
            if (!mountain.GetComponent<S_TerrainTile>().HasTerrainObjectByTag("Volcano")) {
                int riverLength = 0;
                int tries = 0;
                S_TerrainTile next;
                S_TerrainTile current = mountain.GetComponent<S_TerrainTile>();
                while (riverLength < 20 && tries < 1000) {
                    tries++;
                    current.SetAndLoadType(TERRAIN_TYPES["river"]);
                    if (current.x == 0 || current.z == 0 || current.x == tilesN-1 || current.z == tilesN-1)
                        break;
                    if (riverLength>10)
                        break;
                    //next = GetRandomTileUntilDistance(current.x, current.z, 0.7f).GetComponent<S_TerrainTile>();
                    next = GetTile(current.x, current.z-1).GetComponent<S_TerrainTile>();
                    if (next.GetTileType()==TERRAIN_TYPES["river"])
                        continue;
                    //if (next.GetHeight() > current.GetHeight())
                    //    continue;
                    current = next;
                    riverLength++;
                }
                break;
            }
        }
    }

    void CreateTrees() {
        GameObject tile =  GetRandomTileInsideRegion(
            Config.Tree_Init_Region,
            not_types:new int[]{S_Terrain.TERRAIN_TYPES["river"]},
            heights:new int[]{0});
        GameObject tree = tile.GetComponent<S_TerrainTile>().CreateTree();
        tree.GetComponent<S_Tree>().SetLifeEnergy(Config.Tree_Init_Life_Level);
    }

    static int[] GetRandomCoordinatesInsideRegion(float[] region) {
        int[] coords = new int[] {
            (int)(Random.Range(region[0],region[2])*tilesN),
            (int)(Random.Range(region[1],region[3])*tilesN)};
        return coords;
    }

    static GameObject GetRandomTileInsideRegion(float[] region, int[] not_types=null, int[] heights=null) {
        int count = 0;
        int[] coords = null;
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

    public static void CalculateWind() {
        // Wind is computed as the gradient of temperature.
        S_TerrainTile t11,t01,t21,t10,t12;
        for (int ix=1; ix < tilesN-1; ix++) {
            for (int iz=1; iz < tilesN-1; iz++) {
                // Using second symmetric differences
                t11 = tiles[ix,iz].GetComponent<S_TerrainTile>();
                t01 = tiles[ix-1,iz].GetComponent<S_TerrainTile>();
                t21 = tiles[ix+1,iz].GetComponent<S_TerrainTile>();
                t10 = tiles[ix,iz-1].GetComponent<S_TerrainTile>();
                t12 = tiles[ix,iz+1].GetComponent<S_TerrainTile>();
                t11.wind = new Vector2(
                    (t01.temperature-0f*t11.temperature-t21.temperature),
                    (t10.temperature-0f*t11.temperature-t12.temperature));
            }
        }
        // x=0, x=tilesN-1
        for (int iz=1; iz < tilesN-1; iz++) {
            t11 = tiles[0,iz].GetComponent<S_TerrainTile>();
            t21 = tiles[1,iz].GetComponent<S_TerrainTile>();
            t10 = tiles[0,iz-1].GetComponent<S_TerrainTile>();
            t12 = tiles[0,iz+1].GetComponent<S_TerrainTile>();
            t11.wind = new Vector2(
                (t11.temperature-t21.temperature),
                (t10.temperature-0f*t11.temperature-t21.temperature)
            );
            t11 = tiles[tilesN-1,iz].GetComponent<S_TerrainTile>();
            t01 = tiles[tilesN-2,iz].GetComponent<S_TerrainTile>();
            t10 = tiles[tilesN-1,iz-1].GetComponent<S_TerrainTile>();
            t12 = tiles[tilesN-1,iz+1].GetComponent<S_TerrainTile>();
            t11.wind = new Vector2(
                (t01.temperature-t11.temperature),
                (t10.temperature-0f*t11.temperature-t12.temperature)
            );
        }
        // z=0, z=tilesN-1
        for (int ix=1; ix < tilesN-1; ix++) {
            t01 = tiles[ix-1,0].GetComponent<S_TerrainTile>();
            t11 = tiles[ix,0].GetComponent<S_TerrainTile>();
            t21 = tiles[ix+1,0].GetComponent<S_TerrainTile>();
            t12 = tiles[ix,1].GetComponent<S_TerrainTile>();
            t11.wind = new Vector2(
                (t01.temperature-0f*t11.temperature-t21.temperature),
                (t11.temperature-t12.temperature)
            );
            t01 = tiles[ix-1,tilesN-1].GetComponent<S_TerrainTile>();
            t11 = tiles[ix,tilesN-1].GetComponent<S_TerrainTile>();
            t21 = tiles[ix+1,tilesN-1].GetComponent<S_TerrainTile>();
            t10 = tiles[ix,tilesN-2].GetComponent<S_TerrainTile>();
            t11.wind = new Vector2(
                (t01.temperature-0f*t11.temperature-t21.temperature),
                (t11.temperature-t10.temperature)
            );
        }
        
    }

    void MoveHumidity() {
        S_TerrainTile t11,t01,t21,t10,t12;
        float factor = 0.1f;
        float[,] humTemp = new float[tilesN,tilesN];
        for (int ix=0; ix < tilesN; ix++) {
            for (int iz=0; iz < tilesN; iz++) {
                humTemp[ix,iz] = 0f;
            }
        }
        for (int ix=1; ix < tilesN-1; ix++) {
            for (int iz=1; iz < tilesN-1; iz++) {
                t11 = tiles[ix,iz].GetComponent<S_TerrainTile>();
                t01 = tiles[ix-1,iz].GetComponent<S_TerrainTile>();
                t21 = tiles[ix+1,iz].GetComponent<S_TerrainTile>();
                t10 = tiles[ix,iz-1].GetComponent<S_TerrainTile>();
                t12 = tiles[ix,iz+1].GetComponent<S_TerrainTile>();
                humTemp[ix,iz] += t11.humidity;
                if (t11.wind.x > 0) {
                    humTemp[ix+1,iz] += t11.humidity*t11.wind.x*factor;
                    humTemp[ix,iz] -= t11.humidity*t11.wind.x*factor;
                } else {
                    humTemp[ix-1,iz] += t11.humidity*Mathf.Abs(t11.wind.x)*factor;
                    humTemp[ix,iz] -= t11.humidity*Mathf.Abs(t11.wind.x)*factor;
                }
                if (t11.wind.y > 0) {
                    humTemp[ix,iz+1] += t11.humidity*t11.wind.y*factor;
                    humTemp[ix,iz] -= t11.humidity*t11.wind.y*factor;
                } else {
                    humTemp[ix,iz-1] += t11.humidity*Mathf.Abs(t11.wind.y)*factor;
                    humTemp[ix,iz] -= t11.humidity*Mathf.Abs(t11.wind.y)*factor;
                }
            }
        }
        for (int ix=0; ix < tilesN; ix++) {
            for (int iz=0; iz < tilesN; iz++) {
                t11 = tiles[ix,iz].GetComponent<S_TerrainTile>();
                t11.humidity = humTemp[ix,iz];
            }
        }
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
        S_TerrainTile t11;
        if (firstTick) {
            firstTick = false;
            GenerateTerrain();
            for (int ix=0; ix<tilesN; ix++) {
                for (int iz=0; iz<tilesN; iz++) {
                    t11 = tiles[ix,iz].GetComponent<S_TerrainTile>();
                    t11.LoadType();
                }
            }
        }
        if (S_World.tick%terrainUpdateTicks==0) {
            for (int ix=0; ix<tilesN; ix++) {
                for (int iz=0; iz<tilesN; iz++) {
                    t11 = tiles[ix,iz].GetComponent<S_TerrainTile>();
                    t11.CalculateTemperature();
                    t11.CalculateHumidity();
                    t11.CalculateType();
                }
            }
            CalculateWind();
            //MoveHumidity();
        }
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

    void OnDestroy() {
        S_World.OnTick -= Tick;
    }

}
