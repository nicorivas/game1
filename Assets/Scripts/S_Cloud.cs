using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Cloud : S_TerrainObject
{
    protected override void Awake()
    {
        base.Awake();
        grabable = true;
        name = "Cloud";
    }

    protected override void Start()
    {
        base.Start();
        //Debug.Log("S_Cloud:Start");
        // Register tick
        S_World.OnTick += Tick;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Snow()
    {
        //Debug.Log("S_Cloud:Rain");
        if (terrainTile != null) {
            terrainTile.GetComponent<S_TerrainTile>().Snow();
        }
        S_World.events.Add(new Event(gameObject, Config.Rain_Ticks, Delete));
    }

    void Rain()
    {
        //Debug.Log("S_Cloud:Rain");
        if (terrainTile != null) {
            terrainTile.GetComponent<S_TerrainTile>().Rain();
        }
        S_World.events.Add(new Event(gameObject, Config.Rain_Ticks, Delete));
    }

    public override void Delete()
    {
        base.Delete();
        S_World.OnTick -= Tick;
    }

    protected override void Tick(object sender, S_World.OnTickEventArgs e)
    {
        //Debug.Log("S_Cloud:Tick");
        base.Tick(sender, e);
        if (tick%Config.Cloud_Rain_Ticks==0) {
            if (terrainTile.GetComponent<S_TerrainTile>().temperature < 0) {
                Snow();
            } else {
                Rain();
            }
        }
    }

    new void OnMouseDown()
    {
        Debug.Log("S_Cloud:OnMouseDown");
        base.OnMouseDown();
    }
}
