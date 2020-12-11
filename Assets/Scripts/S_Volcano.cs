using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Volcano : S_TerrainObject
{
    // Start is called before the first frame update
    List<GameObject> magmas;
    static public Dictionary<int, string> VOLCANO_PREFAB_NAME = new Dictionary<int, string> {
            {1, "VOLCANO"},
        };
    
    protected override void Awake() {
        Debug.Log("Volcano:Awake");
        base.Awake();
        name = "Volcano";
        magmas = new List<GameObject>();
        levelMax = Config.Volcano_Level_Max;
    }
    
    protected override void Start()
    {
        Debug.Log("Volcano:Start");
        base.Start();
        S_World.events.Add(new Event(
            gameObject,
            Config.Volcano_Eruption_Ticks,
            Erupt,
            true,
            Config.Volcano_Eruption_Ticks_Variance));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Erupt() {
        //Debug.Log("S_Volcano:Erupt");
        //m_Animator.SetTrigger("Grow12");
        GameObject terrainTile = S_Terrain.GetRandomTileNeighbour(x, z);
        if (Random.value < 0.0) {
            // Fire
            terrainTile.GetComponent<S_TerrainTile>().EruptionStart();
        //} else {
        //    // Rock
        //    GameObject rock = terrainTile.GetComponent<S_TerrainTile>().CreateRock();
        //    rock.GetComponent<S_Rock>().AddFireEnergy(10);
        } else {
            // Magma
            if (magmas.Count == 0) {
                GameObject magma = terrainTile.GetComponent<S_TerrainTile>().CreateMagma();
                magma.GetComponent<S_Magma>().SetVolcano(gameObject);
                magmas.Add(magma);
            } else {
                foreach (GameObject magma in magmas) {
                    magma.GetComponent<S_TerrainObject>().AddFireEnergy(10);
                }
            }
        }
    }

    public void RemoveMagma(GameObject magma) {
        magmas.Remove(magma);
    }

    public override void AddFireEnergy(int fireLevelDelta) {
        base.AddFireEnergy(fireLevelDelta);
        SetLevel(base.CalculateLevel("fire", Config.Volcano_Level_With_Fire_Energy));
    }

    public override void Combine(GameObject gameObjectCombine) {
        base.Combine(gameObjectCombine);
        Debug.Log("S_Volcano:Combine");
        if (gameObjectCombine.tag == "Tree") {
            S_World.AddFireEnergy(Config.Tree_In_Volcano_Fire_Energy);
            AddFireEnergy(gameObjectCombine.GetComponent<S_Tree>().GetFireEnergy());
            Destroy(gameObjectCombine);
            Player.Release();
        }        
        if (gameObjectCombine.tag == "Rock") {
            S_World.AddFireEnergy(Config.Rock_In_Volcano_Fire_Energy);
            AddFireEnergy(gameObjectCombine.GetComponent<S_Rock>().GetFireEnergy());
            Destroy(gameObjectCombine);
            Player.Release();
        }
    }

    protected override void OnMouseDown() {
        Debug.Log("S_Volcano:OnMouseDown");
        base.OnMouseDown();
        if (Player.IsHoldingObject()) {
            Combine(Player.GetHeldObject());
        }
    }
}
