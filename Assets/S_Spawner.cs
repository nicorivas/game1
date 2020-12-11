using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Spawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject enemy;
    public int ticksToSpawn;
    public GameObject tile;
    
    void Awake() {
        enemy = null;
        ticksToSpawn = 5;
        tile = null;
    }

    void Start()
    {
        S_World.events.Add(new Event(
                gameObject, 
                ticksToSpawn, 
                Spawn, 
                recurrent_: false, 
                variance_: 0));
    }

    void Spawn() {
        Instantiate(enemy);
        tile.GetComponent<S_TerrainTile>().PlaceEnemy(enemy);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}