using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Mountain : S_TerrainObject
{
    // Start is called before the first frame update
    List<GameObject> waters;
    static public Dictionary<int, string> MOUNTAIN_PREFAB_NAME = new Dictionary<int, string> {
            {1, "MOUNTAIN"},
        };
    
    protected override void Awake() {
        Debug.Log("Mountain:Awake");
        base.Awake();
        name = "Mountain";
        waters = new List<GameObject>();
        levelMax = Config.Mountain_Level_Max;
    }
    
    protected override void Start()
    {
        Debug.Log("Mountain:Start");
        base.Start();
    }

    public override void Combine(GameObject gameObjectCombine) {
        base.Combine(gameObjectCombine);
        Debug.Log("S_Volcano:Combine");
        if (gameObjectCombine.tag == "Cloud") {
            terrainTile.GetComponent<S_TerrainTile>().PlaceObject(gameObjectCombine);
        }
    }

    protected override void OnMouseDown() {
        base.OnMouseDown();
        Debug.Log("S_Mountain:OnMouseDown");
        if (Player.IsHoldingObject()) {
            Combine(Player.GetHeldObject());
        }
    }

    public void RemoveWater(GameObject water) {
        waters.Remove(water);
    }
}
