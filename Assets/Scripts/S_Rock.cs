using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Rock : S_TerrainObject
{
    // Start is called before the first frame update
    static public Dictionary<string, int> ROCK_TYPE = new Dictionary<string, int> {
            {"GrasslandRock",0},
            {"RiverRock",1},
            {"DesertRock",2},
            {"PolarRock",3},
            {"PlainsRock",4},
        };
    static public Dictionary<int, string> ROCK_PREFAB_NAME = new Dictionary<int, string> {
            {0,"GrasslandRock"},
            {1,"RiverRock"},
            {2,"DesertRock"},
            {3,"PolarRock"},
            {4,"PlainsRock"},
        };
    int type;
    GameObject rockObject;
    
    protected override void Awake() {
        base.Awake();
        S_World.OnTick += Tick;
        grabable = true;
        name = "Rock";
        return;
    }

    protected override void Initialize(int objectType) {
        base.Initialize(objectType);
    }

    protected override void Start()
    {
        base.Start();
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static string GetPrefabName(int objectType_) {
        return "Rocks/"+ROCK_PREFAB_NAME[objectType_];
    }

    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        Debug.Log("S_Rock:OnMouseDown");
        if (Player.IsHoldingObject()) {
            if (this.beingGrabbed == false) {
                GameObject heldObject = Player.GetHeldObject();
                Combine(heldObject);
            }
        }
    }

    public override void Place()
    {
        Debug.Log("S_Rock:Place");
        if (started) {

        }
    }

    protected override void Tick(object sender, S_World.OnTickEventArgs e)
    {
        base.Tick(sender, e);
        if (tick%Config.Energy_Tick_Period == 0) {
            int terrainTileType = terrainTile.GetComponent<S_TerrainTile>().GetTileType();
            if (terrainTileType == S_Terrain.TERRAIN_TYPES["grassland"]) {
                AddLifeEnergy(Config.Rock_Grassland_Life_Energy);
            }
            AddFireEnergy(-Config.Rock_Drain_Fire_Energy);
        }
    }

    public override void Combine(GameObject gameObjectCombine)
    {
        Debug.Log("S_Rock:Combine");
        if (gameObjectCombine.tag == "Rock" && gameObjectCombine != gameObject) {
            gameObjectCombine.GetComponent<S_TerrainObject>().Delete();
            m_Animator.SetTrigger("Level2");
            level += 1;
        }
    }
}
