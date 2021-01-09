using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_PowerMagma : S_Power
{
    void Awake() {
        description = "A random tile sinks";
    }
    public override void Action()
    {
        S_Terrain.SinkTile(S_Terrain.GetRandomCoordinates());
    }
}
