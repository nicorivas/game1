using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class EnemyCoords {
    public string coords;
    public string name;
}

[System.Serializable]
public class HeightCoords {
    public string coords;
    public int height;
}

public class LevelData {
    public int level;
    public EnemyCoords[] enemies;
    public HeightCoords[] height;
}

[System.Serializable]
public class S_Level : MonoBehaviour
{
    public LevelData levelData;
    public string filename;
    int enemiesN, initialEnemies;
    List<GameObject> gameObjects;
    void Start()
    {
        //Build();
    }
    
    public void Build()
    {
        initialEnemies = 0;

        TextAsset levelJson = Resources.Load<TextAsset>(filename);
        LevelData levelData = JsonUtility.FromJson<LevelData>(levelJson.text);

        gameObjects = new List<GameObject>();
        
        if (levelData.enemies != null) {
            foreach (EnemyCoords enemy in levelData.enemies) {
                S_Terrain.SpawnEnemy(S_Terrain.ParseCoords(enemy.coords), enemy.name);
                initialEnemies += 1;
            }
            enemiesN = initialEnemies;
        }
        
        if (levelData.height != null) {
            foreach (HeightCoords hdata in levelData.height) {
                //S_Terrain.RiseTile(hdata.coords, hdata.height);
                S_Terrain.SpawnBlock(S_Terrain.ParseCoords(hdata.coords));
            }
        }

        /*
        if (burnRandomTiles) {
            S_World.events.Add(new Event(gameObject, burnRandomTilesTicks, BurnRandomTile, variance_:burnRandomTilesTicksVariance));
        }
        */
    }

    public int GetEnemies() {
        return enemiesN;
    }

    public void RemoveEnemy() {
        enemiesN -= 1;
    }

    void BurnRandomTile() {
        /*
        S_Terrain.BurnTile(new Vector2Int(
            (int)Random.Range(0,S_Terrain.tilesN),
            (int)Random.Range(0,S_Terrain.tilesN)));
        S_World.events.Add(new Event(gameObject, burnRandomTilesTicks, BurnRandomTile, variance_:burnRandomTilesTicksVariance));
        */
    }

    void Update() {
        
    }

    public void AddGameObject(GameObject go) {
        gameObjects.Add(go);
    }

    public void RemoveGameObject(GameObject go) {
        gameObjects.Remove(go);
    }

    public void Reset()
    {
        // Remove all GameObjects in level
        foreach (GameObject go in gameObjects) {
            Destroy(go);
        }
        // Build level 
        Build();
    }


    public void Finish() {
        //S_Terrain.SpawnPortal(new Vector2Int(5,5));
        Destroy(gameObject);
    }
}
