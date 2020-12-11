using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Magma : S_TerrainObject
{
    // Start is called before the first frame update
    public static string MAGMA_PREFAB_NAME = "Magma";
    GameObject volcano;
    protected override void Awake() {
        base.Awake();
        name = "Magma";
        grabable = true;
        volcano = null;
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

    public void SetVolcano(GameObject volcano_) {
        volcano = volcano_;
    }

    protected virtual void OnMouseDown() {
        base.OnMouseDown();
        volcano.GetComponent<S_Volcano>().RemoveMagma(gameObject);
    }
}
