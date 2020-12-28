using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_TileDamage : MonoBehaviour
{
    public bool damageActive;
    public Material damageMaterial;
    public Material neutralMaterial;
    GameObject model;
    void Start()
    {
        S_World.events.Add(new Event(gameObject, 30, DamageActivate));
        model = this.gameObject.transform.GetChild(0).gameObject;
    }

    void Update()
    {
        
    }

    void DamageActivate() {
        damageActive = true;
        model.GetComponent<MeshRenderer> ().material = damageMaterial;
        S_World.events.Add(new Event(gameObject, 5, Eliminate));
    }

    void Eliminate() {
        Destroy(gameObject);
    }
}
