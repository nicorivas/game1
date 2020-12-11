using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyCoords {
    public Vector2Int coords;
    public GameObject enemy;
}

[System.Serializable]
public class HeightCoords {
    public Vector2Int coords;
    public int height;
    public int enemies, initialEnemies;
}
public class S_Level : MonoBehaviour
{
    public int level;
    public int enemies, initialEnemies;
    public EnemyCoords[] enemyLocations;
    public HeightCoords[] heightCoords;
    void Start()
    {
        foreach (EnemyCoords enemyCoords in enemyLocations) {
            S_Terrain.SpawnEnemy(new int[]{ enemyCoords.coords.x, enemyCoords.coords.y}, enemyCoords.enemy);
            initialEnemies += 1;
        }
        enemies = initialEnemies;

        foreach (HeightCoords heightCoords in heightCoords) {
            S_Terrain.RiseTile(heightCoords.coords, heightCoords.height);
        }

    }

    public void Finish() {
        S_Terrain.SpawnPortal(new Vector2Int(5,5));
    }
}
