using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Water : S_TerrainObject
{
    // Start is called before the first frame update
    public static string WATER_PREFAB_NAME = "Water";
    GameObject mountain;
    protected override void Awake() {
        base.Awake();
        name = "Water";
        grabable = true;
        mountain = null;
    }
    protected override void Start()
    {
        base.Start();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Tick(object sender, S_World.OnTickEventArgs e) {
        base.Tick(sender, e);
    }

    public void SetMountain(GameObject mountain_) {
        mountain = mountain_;
    }

    protected virtual void OnMouseDown() {
        base.OnMouseDown();
        mountain.GetComponent<S_Mountain>().RemoveWater(gameObject);
    }
}
