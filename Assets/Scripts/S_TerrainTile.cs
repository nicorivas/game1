using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class S_TerrainTile : MonoBehaviour
{
    Dictionary<string, int> STATES = new Dictionary<string, int> {
        {"Idle",0},
        {"Removing",1},
        {"Adding",2},
    };
    float speed;
    public AnimationCurve Humidity_Temperature;
    public AnimationCurve Cloud_Probability_With_Humidity;
    float HEIGHT_DELTA = 2.0f;
    bool debug_ = false;
    public float temperature = 0;
    public float humidity = 0;
    public Vector2 wind = new Vector2(0.0f,0.0f);
    public int x, z;
    int height;
    StateManager states;
    GameObject tile, tile_new, text;
    GameObject eruptionMarker;
    List<GameObject> objects;
    List<string> actions;
    int type;
    int tick;
    int eruptionLength;
    GameObject lift;
    Vector3 originalPosition;
    void Awake() {
        // We use Awake because TerrainTiles are instantiated by Terrain, 
        // and also Terrain creates Objects (like trees) which need the Tile
        // and the list of objects.
        objects = new List<GameObject>();
        tick = 0;
        eruptionLength = 100;
        speed = 0.0f;
    }
    void Start() {
        //Debug.Log("S_TerrainTile:Start");
        S_World.OnTick += Tick;
        actions = new List<string>();
        states = new StateManager(STATES);
        states.Add(STATES["Idle"]);
        if (debug_ == true) {
            text = Instantiate(Resources.Load("FloatingText")) as GameObject;
            text.GetComponent<TextMesh>().transform.position = transform.position;
            text.GetComponent<TextMesh>().transform.rotation = Camera.main.transform.rotation;
        }
        /*
        S_World.events.Add(new Event(
            gameObject, 
            Config.Cloud_Creation_Ticks, 
            TryToCreateCloud, 
            recurrent_: true, 
            variance_:Config.Cloud_Creation_Ticks_Variance));
        */
    }

    public void SetPosition(int x_, int z_) {
        x = x_;
        z = z_;
    }

    public void SetTileType(int type_) {
        //Debug.Log("S_TerrainTile:SetType");
        type = type_;
    }

    public int GetTileType() {
        return type;
    }

    public bool IsWater() {
        return type == S_Terrain.TERRAIN_TYPES["river"];
    }

    public void SetAndLoadType(int type_) {
        SetTileType(type_);
        LoadType();
    }

    public void LoadType() {
        //Debug.Log("S_TerrainTile:LoadType");
        if (tile != null) Destroy(tile);
        if (type == S_Terrain.TERRAIN_TYPES["grassland"]) {
            tile = Instantiate(Resources.Load("Grass")) as GameObject;
        } else if (type == S_Terrain.TERRAIN_TYPES["river"]) {
            tile = Instantiate(Resources.Load("Water")) as GameObject;
        } else if (type == S_Terrain.TERRAIN_TYPES["desert"]) {
            tile = Instantiate(Resources.Load("Desert")) as GameObject;
        } else if (type == S_Terrain.TERRAIN_TYPES["polar"]) {
            tile = Instantiate(Resources.Load("Polar")) as GameObject;
        } else if (type == S_Terrain.TERRAIN_TYPES["plains"]) {
            tile = Instantiate(Resources.Load("Plains")) as GameObject;
        }
        tile.transform.parent = transform;
        gameObject.transform.position = new Vector3(
            gameObject.transform.position.x,
            height*HEIGHT_DELTA,
            gameObject.transform.position.z);
        tile.transform.localPosition = new Vector3(0f,0f,0f);
        originalPosition = gameObject.transform.position;
    }

    public void Rise(int height) {
        lift = Instantiate(Resources.Load("Lift")) as GameObject;
        lift.transform.position = gameObject.transform.position;
        SetHeight(height);
        speed = 1.0f;
    }

    public void Sink() {
        lift = Instantiate(Resources.Load("Lift")) as GameObject;
        lift.transform.position = gameObject.transform.position;
        SetHeight(-1);
        speed = -1.0f;
    }

    public void Stop() {
        speed = 0.0f;
        DestroyLift();
        gameObject.transform.position = new Vector3(
            gameObject.transform.position.x,
            height*HEIGHT_DELTA,
            gameObject.transform.position.z);
    }

    public void DestroyLift() {
        if (lift != null) {
            Lightbug.CharacterControllerPro.Core.SceneController.Instance.RemoveActor(lift.GetComponent<Lightbug.CharacterControllerPro.Demo.ActionBasedPlatform>());
            Destroy(lift);
        }
    }

    public int GetHeight() {
        return height;
    }

    public void SetHeight(int height_) {
        height = height_;
    }

    public void CalculateTemperature() {
        temperature = (1.0f-Mathf.Abs(z*1.0f - S_Sun.z)/11.0f)*40.0f-5.0f;
        if (height == 1) {
            temperature -= 10.0f;
        }else if (height == 2) {
            temperature -= 30.0f;
        }
    }

    public void CalculateHumidity() {
        // Distance to water
        float distanceToWater = S_Terrain.DistanceToWater(gameObject);
        humidity = (10.0f/Mathf.Exp(distanceToWater))*(1.0f-(temperature/350f));
        humidity += Humidity_Temperature.Evaluate((temperature-Config.Temperature_Min)/(Config.Temperature_Max-Config.Temperature_Min))*10.0f;
        //if (temperature > 10.0f && temperature < 25.0f)
        //    humidity += 10.0f;
    }

    public void CalculateType() {
        if (type != S_Terrain.TERRAIN_TYPES["river"]) {
            if (temperature > 30.0f) {
                if (humidity < 1.0f) {
                    SetTileType(S_Terrain.TERRAIN_TYPES["desert"]);
                } else {
                    SetTileType(S_Terrain.TERRAIN_TYPES["plains"]);
                }
            } else if (temperature > 20.0f) {
                SetTileType(S_Terrain.TERRAIN_TYPES["grassland"]);
            } else if (temperature > 10.0f) {
                SetTileType(S_Terrain.TERRAIN_TYPES["plains"]);
            } else if (temperature > 0.0f) {
                SetTileType(S_Terrain.TERRAIN_TYPES["plains"]);
            } else if (temperature < 0.0f) {
                SetTileType(S_Terrain.TERRAIN_TYPES["polar"]);
            }
        }
    }

    public void PlaceObject(GameObject objectHeld) {
        if (objectHeld.tag == "Tree") {
            PlaceTree(objectHeld);
        } else if (objectHeld.tag == "Rock") {
            PlaceRock(objectHeld);
        } else if (objectHeld.tag == "Seed") {
            PlaceSeed(objectHeld);
        } else if (objectHeld.tag == "Magma") {
            PlaceMagma(objectHeld);
        } else if (objectHeld.tag == "Snow") {
            PlaceSnow(objectHeld);
        } else if (objectHeld.tag == "Water") {
            PlaceWater(objectHeld);
        } else if (objectHeld.tag == Config.Cloud_Tag_Name) {
            PlaceCloud(objectHeld);
        }
    }

    private void CreateLogBridge() {
        Debug.Log("S_TerrainTile:CreateLogBridge");
        //GameObject logBridge = Instantiate(logBridgeGameObject);
        //PlaceTerrainObject(logBridge);
    }

    public GameObject CreateTree(int treeType = -1) {
        Debug.Log("S_TerrainTile:CreateTree");
        // If type is not given, we get it from terrain type:
        if (treeType == -1) treeType = S_Tree.GetTreeTypeFromTerrainType(type);
        GameObject tree = CreateTerrainObject(S_Tree.TREE_PREFAB_NAME[treeType]);
        tree.GetComponent<S_Tree>().SetObjectType(treeType);
        PlaceTerrainObject(tree);
        return tree;
    }

    private void PlaceTree(GameObject tree) {
        Debug.Log("S_TerrainTile:PlaceTree");
        if (type == S_Terrain.TERRAIN_TYPES["grassland"] ||
            type == S_Terrain.TERRAIN_TYPES["desert"] ||
            type == S_Terrain.TERRAIN_TYPES["plains"] ) {
            PlaceTerrainObject(tree);
        } else if (type == S_Terrain.TERRAIN_TYPES["river"]) {
            tree.GetComponent<S_TerrainObject>().Delete();
            CreateLogBridge();
        }
    }

    public void CreateVolcano(int volcanoType = 1) {
        Debug.Log("S_TerrainTile:CreateVolcano");
        GameObject volcano = CreateTerrainObject(S_Volcano.VOLCANO_PREFAB_NAME[volcanoType]);
        PlaceTerrainObject(volcano);
    }

    public void CreateMountain(int mountainType = 1) {
        Debug.Log("S_TerrainTile:CreateMountain");
        GameObject mountain = CreateTerrainObject(S_Mountain.MOUNTAIN_PREFAB_NAME[mountainType]);
        PlaceTerrainObject(mountain);
    }

    public void CreateSnow(int snowType = 1) {
        Debug.Log("S_TerrainTile:CreateSnow");
        GameObject snow = CreateTerrainObject(S_Snow.SNOW_PREFAB_NAME[snowType]);
        PlaceTerrainObject(snow);
    }

    public GameObject CreateRock(int rockType = 1) {
        //Debug.Log("S_TerrainTile:CreateRock");
        GameObject rock = CreateTerrainObject(S_Rock.GetPrefabName(rockType));
        rock.GetComponent<S_Rock>().SetObjectType(rockType);
        PlaceTerrainObject(rock);
        return rock;
    }

    private void PlaceRock(GameObject rock) {
        Debug.Log("S_TerrainTile:PlaceRock");
        PlaceOrCombineTerrainObject(rock);
    }

    public GameObject CreateMagma() {
        //Debug.Log("S_TerrainTile:CreateMagma");
        GameObject magma = CreateTerrainObject(S_Magma.MAGMA_PREFAB_NAME);
        PlaceTerrainObject(magma);
        return magma;
    }

    public void PlaceMagma(GameObject objectHeld) {
        //Debug.Log("S_TerrainTile:PlaceMagma");
        Destroy(objectHeld);
        CreateRock(Config.Rock_Type_From_Terrain_Type[type]);
    }

    public void Rain() {
        GameObject rain = CreateTerrainObject(Config.Rain_Prefab_Name);
        PlaceTerrainObject(rain);
        CombineWithAll(rain);
    }

    public void Snow() {
        GameObject snow = CreateTerrainObject(Config.Snow_Prefab_Name);
        PlaceTerrainObject(snow);
        snow.transform.localPosition = new Vector3(0.0f,2.0f,0.0f);
        CombineWithAll(snow);
    }

    public void PlaceSnow(GameObject objectHeld) {
        //Debug.Log("S_TerrainTile:PlaceSnow");
        Destroy(objectHeld);
        //SetAndLoadType(S_Terrain.TERRAIN_TYPES["river"]);
        GameObject water = CreateTerrainObject(Config.Water_Prefab_Name);
        PlaceTerrainObject(water);
        CombineWithAll(water);
    }

    public void PlaceWater(GameObject objectHeld) {
        PlaceTerrainObject(objectHeld);
    }

    public GameObject CreateTerrainObject(string gameObjectName) {
        Debug.Log("S_TerrainTile:CreateTerrainObject gameObjectName="+gameObjectName);
        return Instantiate(Resources.Load(gameObjectName)) as GameObject;
    }

    public void PlaceOrCombineTerrainObject(GameObject terrainObject) {
        if (!objects.Any()) {
            // Place
            PlaceTerrainObject(terrainObject);
        } else {
            CombineWithAll(terrainObject);
        }
    }

    public void CombineWithAll(GameObject terrainObject) {
        // Combine
        foreach (GameObject obj in objects.Reverse<GameObject>()) {
            obj.GetComponent<S_TerrainObject>().Combine(terrainObject);
        }
    }

    public bool HasTerrainObjectByTag(string tag) {
        foreach (GameObject obj in objects.Reverse<GameObject>()) {
            if (obj.tag == tag) return true;
        }
        return false;
    }

    public void PlaceEnemy(GameObject enemy) {
        enemy.transform.position = new Vector3(gameObject.transform.position.x,3f,gameObject.transform.position.z);
    }

    public void PlacePortal(GameObject portal) {
        portal.transform.position = new Vector3(gameObject.transform.position.x,3f,gameObject.transform.position.z);
    }

    public void PlacePower(GameObject power) {
        power.transform.position = new Vector3(gameObject.transform.position.x,1f,gameObject.transform.position.z);
    }

    public GameObject PlaceTerrainObject(GameObject terrainObject) {
        //Debug.Log("PlaceTerrainObject: terrainObject="+terrainObject);
        terrainObject.transform.parent = this.transform;
        terrainObject.transform.localPosition = new Vector3(0.0f,2.0f,0.0f);
        if (terrainObject.GetComponent<S_TerrainObject>().terrainTile != null) {
            terrainObject.GetComponent<S_TerrainObject>().terrainTile.GetComponent<S_TerrainTile>().objects.Remove(terrainObject);
        }
        if (Player.GetHeldObject() == terrainObject) {
            Player.DropHeldObject();
        }
        terrainObject.GetComponent<S_TerrainObject>().terrainTile = gameObject;
        terrainObject.GetComponent<S_TerrainObject>().x = x;
        terrainObject.GetComponent<S_TerrainObject>().z = z;
        terrainObject.GetComponent<S_TerrainObject>().Place();
        objects.Add(terrainObject);
        return terrainObject;
    }

    public void Burn() {
        GameObject tileDamage = Instantiate(Resources.Load("TileDamage")) as GameObject;
        tileDamage.transform.position = new Vector3(gameObject.transform.position.x,1.0f,gameObject.transform.position.z);
    }

    public void EruptionStart() {
        //Debug.Log("S_TerrainTile:EruptionStart");
        eruptionMarker = Instantiate(Resources.Load("EruptionMarker")) as GameObject;
        //eruptionMarker.transform.parent = tile.transform;
        eruptionMarker.transform.position = new Vector3(gameObject.transform.position.x,0.1f,gameObject.transform.position.z);
        S_World.events.Add(new Event(gameObject, eruptionLength,EruptionHit));
    }

    public void EruptionHit() {
        //Debug.Log("S_TerrainTile:EruptionHit");
        Destroy(eruptionMarker);
        foreach (GameObject obj in objects) {
            if (obj.tag == "Tree") {
                obj.GetComponent<S_Tree>().Burn();
            }
        }
    }

    public void RemoveObject(GameObject gameObject) {
        objects.Remove(gameObject);
    }

    public void PlaceSeed(GameObject objectHeld) {
        Destroy(objectHeld);
        CreateTree(S_Tree.GetTreeTypeFromTerrainType(type));
    }

    void TryToCreateCloud() {
        if (!HasTerrainObjectByTag(Config.Cloud_Tag_Name)) {
            if (Random.value < Cloud_Probability_With_Humidity.Evaluate(humidity/Config.Humidity_Max)*Config.Cloud_Max_Probability) {;
                CreateCloud();
            }
        }
    }

    void CreateCloud() {
        //Debug.Log("S_TerrainTile:CreateRock");
        GameObject cloud = CreateTerrainObject(Config.Cloud_Prefab_Name);
        PlaceTerrainObject(cloud);
    }

    public void PlaceCloud(GameObject objectHeld) {
        Debug.Log("S_TerrainTile:PlaceCloud");
        PlaceTerrainObject(objectHeld);
    }

    void Update() {
        if (debug_==true) {
            text.GetComponent<TextMesh>().text = "T:"+temperature.ToString("0.0")+"\nH:"+humidity.ToString("0.0");
            Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(wind.x,transform.position.y+1.0f,wind.y))*0.2f, Color.black);
        }
    }

    void OnMouseDown() {
        Debug.Log("S_TerrainTile:OnMouseDown");
        if (Player.IsHoldingObject()) {
            PlaceObject(Player.GetHeldObject());
        };
    }

    private void Tick(object sender, S_World.OnTickEventArgs e)
    {
        //
    }

    void FixedUpdate()
    {
        gameObject.transform.position += speed*Vector3.up*Time.deltaTime;
        if (speed > 0.0f) {
            if (gameObject.transform.position.y > originalPosition.y + height*HEIGHT_DELTA) {
                Stop();
            }
        } else {
            if (gameObject.transform.position.y < originalPosition.y + height*HEIGHT_DELTA) {
                Stop();
            }
        }
    }

    void OnDestroy()
    {
        DestroyLift();
    }
}