using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Tree : S_TerrainObject
{
    // Start is called before the first frame update
    int growTime, seedTime, burnTime;
    int growTicks;
    bool debug_;
    GameObject flower;
    static public Dictionary<string, int> TREE_TYPE = new Dictionary<string, int> {
            {"oak",1},
            {"cactus",2},
            {"acacia",3},
        };
    static public Dictionary<int, string> TREE_PREFAB_NAME = new Dictionary<int, string> {
            {1, "Oak"},
            {2, "Cactus"},
            {3, "Acacia"}
        };
    GameObject treeObject;
    GameObject text;

    protected override void Awake() {
        base.Awake();
        grabable = true;
        name = "Tree";
        levelMax = Config.Tree_Level_Max;
    }

    protected override void Start()
    {
        base.Start();
        Debug.Log("S_Tree:Start");
        debug_ = true;
        // Register tick
        S_World.OnTick += Tick;
        // Times
        growTicks = 20;
        burnTime = 20;
        // Initial events
        m_Animator = GetComponent<Animator>();
        if (debug_ == true) {
            text = Instantiate(Resources.Load("FloatingText")) as GameObject;
            text.transform.parent = transform;
            text.GetComponent<TextMesh>().transform.position = transform.position;
            text.GetComponent<TextMesh>().transform.rotation = Camera.main.transform.rotation;
        }
        CreateFlower();
    }

    void Update()
    {
        
    }

    void DropSeed() {
        if (level == 5) {
            //Debug.Log("S_Tree:DropSeed");
            GameObject seed = Instantiate(Resources.Load("Seed")) as GameObject;
            //seed.transform.parent = transform;
            //seed.transform.localPosition = new Vector3(1.0f,0.0f,0.0f);
            int dx = 0;
            int dz = 0;
            while (dx == 0 && dz == 0) {
                dx = (int)(Random.value*2-1);
                dz = (int)(Random.value*2-1);
            }
            seed.transform.position = new Vector3(
                (float)(gameObject.transform.position.x+S_Terrain.tileWidth/4.0*dx),
                2.0f,
                (float)(gameObject.transform.position.z+S_Terrain.tileWidth/4.0*dz));
            AddLifeEnergy(Config.Tree_Drop_Seed_Life_Energy);
            S_World.events.Add(new Event(gameObject, Config.Tree_Drop_Seed_Ticks, DropSeed));
        }
    }

    protected override void SetLevel(int level_) {
        //Debug.Log("S_Tree:SetLevel:"+level_);
        level = level_;
        grabable = Config.Tree_Level_Grabable[level];
        if (m_Animator != null) {
            m_Animator.SetTrigger("Grow12");
        }
        if (level == 5) {
            S_World.events.Add(new Event(gameObject, Config.Tree_Drop_Seed_Ticks, DropSeed));
        }
    }

    public void Burn() {
        S_World.events.Add(new Event(gameObject, burnTime, Delete));
    }

    new void OnMouseDown() {
        Debug.Log("S_Tree:OnMouseDown");
        base.OnMouseDown();
        Shake();
    }

    void Shake() {
        DropSeed();
    }

    public static int GetTreeTypeFromTerrainType(int terrainType) {
        if (terrainType == S_Terrain.TERRAIN_TYPES["grassland"]) {
            return S_Tree.TREE_TYPE["oak"];
        } else if (terrainType == S_Terrain.TERRAIN_TYPES["desert"]) { 
            return S_Tree.TREE_TYPE["cactus"];
        } else if (terrainType == S_Terrain.TERRAIN_TYPES["plains"]) { 
            return S_Tree.TREE_TYPE["acacia"];
        }
        return 1;
    }

    public void AddLifeEnergy(int energyToAdd) {
        base.AddLifeEnergy(energyToAdd);
        SetLifeEnergy(energy["life"]+energyToAdd);
    }

    public override void SetLifeEnergy(int lifeEnergy) {
        if (lifeEnergy > Config.Tree_Max_Life_Level)
            lifeEnergy = Config.Tree_Max_Life_Level;
        base.SetLifeEnergy(lifeEnergy);
        int lvl = CalculateLevel("life", Config.Tree_Level_With_Life_Energy);
        Debug.Log("CalculatedLevel="+lvl+" LifeEnergy="+energy["life"]);
        if (lvl != level)
            SetLevel(lvl);
    }

    void CreateFlower()
    {
        if (objectType == TREE_TYPE["cactus"]) {
            flower = Instantiate(Resources.Load("CactusFlower")) as GameObject;
            flower.transform.parent = this.transform;
            flower.transform.localPosition = new Vector3(0.0f,5.0f,0.0f);
        }
    }

    protected override void Tick(object sender, S_World.OnTickEventArgs e) {
        base.Tick(sender, e);
        text.GetComponent<TextMesh>().text = "L:"+level+" Le:"+energy["life"];
        if (tick%growTicks==0) {
            int lifeLevelToAdd = 0;
            // Energy from distance to water
            if (S_Terrain.DistanceToWater(gameObject) < 2f) {
                if (level < 3) {
                    lifeLevelToAdd += 2;
                }
            }
            // Energy from terrain type
            if (terrainTile.GetComponent<S_TerrainTile>().GetTileType()==S_Terrain.TERRAIN_TYPES["grassland"]) {
                if (level == 1)
                    lifeLevelToAdd += 2;
            }
            if (terrainTile.GetComponent<S_TerrainTile>().GetTileType()==S_Terrain.TERRAIN_TYPES["plains"]) {
                if (level == 1)
                    lifeLevelToAdd += 1;
            }
            if (terrainTile.GetComponent<S_TerrainTile>().GetTileType()==S_Terrain.TERRAIN_TYPES["desert"]) {
                if (GetObjectType() == TREE_TYPE["cactus"]) {
                    if (level == 1)
                        lifeLevelToAdd += 2;
                } else {
                    lifeLevelToAdd -= 1;
                }
            }
            // Flower
            if (objectType == TREE_TYPE["cactus"]) {
                if (level == 3) {
                    if (Random.value < 0.05) {
                        CreateFlower();
                    }
                }
            }
            AddLifeEnergy(lifeLevelToAdd);
        }
    }

    public override void Combine(GameObject gameObjectCombine)
    {
        base.Combine(gameObjectCombine);
        Debug.Log("S_Tree:Combine");
        if (gameObjectCombine.tag == "Rain" && gameObjectCombine != gameObject) {
            AddLifeEnergy(Config.Tree_Rain_Combine_Life);
        }
    }
}
