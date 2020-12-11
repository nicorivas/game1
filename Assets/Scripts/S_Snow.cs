using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Snow : S_TerrainObject
{
    // Start is called before the first frame update
    static public Dictionary<int, string> SNOW_PREFAB_NAME = new Dictionary<int, string> {
        {1, "Snow"},
    };
    protected override void Awake() {
        Debug.Log("Snow:Awake");
        base.Awake();
        S_World.OnTick += Tick;
        levelMax = Config.Snow_Level_Max;
        grabable = true;
    }

    protected override void Start()
    {
        Debug.Log("Snow:Start");
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnMouseDown() {
        Debug.Log("S_Snow:OnMouseDown");
        base.OnMouseDown();
        if (Player.IsHoldingObject()) {
            Combine(Player.GetHeldObject());
        }
    }

    protected override void Tick(object sender, S_World.OnTickEventArgs e) {
        base.Tick(sender, e);
        /*
        if (tick%Config.Energy_Tick_Period == 0) {
            GameObject terrainTile = S_Terrain.GetRandomTileNeighbour(x,z);
            terrainTile.GetComponent<S_TerrainTile>().SetAndLoadType(S_Terrain.TERRAIN_TYPES["river"]);
        }
        */
    }
}
