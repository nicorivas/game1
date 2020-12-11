using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Seed : S_TerrainObject
{
    // Start is called before the first frame update
    new void Start()
    {
        grabable = true;
    }

    // Update is called once per frame
    void Update()
    {
  
    }

    new void OnMouseDown() {
        Debug.Log("S_Seed:OnMouseDown");
        base.OnMouseDown();
    }
}
