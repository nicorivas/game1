using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_PowerBlock : S_Power
{
    void Awake() {
        description = "A block is spawned in a random position";
    }
    public override void Action()
    {
        S_Terrain.SpawnBlock(S_Terrain.GetRandomCoordinates());
    }
}
