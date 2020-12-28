using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class S_Spawner : MonoBehaviour
{
    // Start is called before the first frame update
    public string enemyName;
    public int ticksToSpawn;
    public GameObject tile;
    
    void Awake() {
        enemyName = null;
        ticksToSpawn = 5;
        tile = null;
        S_Director.level.GetComponent<S_Level>().AddGameObject(gameObject);
    }

    void Start()
    {
        S_World.events.Add(new Event(gameObject, ticksToSpawn, Spawn));
    }

    void Spawn() {
        GameObject enemy = Instantiate(Resources.Load<GameObject>(Path.Combine(Config.Enemies_Dir,enemyName)));
        tile.GetComponent<S_TerrainTile>().PlaceEnemy(enemy);
        Destroy(gameObject);
    }

    void OnDestroy() {
        S_World.events.RemoveFromGameObject(gameObject);
    }
}