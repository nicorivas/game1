using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Rain : S_TerrainObject
{
    // Start is called before the first frame update
    void Start()
    {
        S_World.events.Add(new Event(gameObject, Config.Rain_Ticks, Delete));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
