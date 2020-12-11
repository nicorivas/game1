using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Flower : S_TerrainObject
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Tick(object sender, S_World.OnTickEventArgs e)
    {
        AddLifeEnergy(1);
    }
}
