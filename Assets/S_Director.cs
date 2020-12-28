using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class S_Director : MonoBehaviour
{
    static int levelN;
    static public GameObject level;
    static public GameObject terrain;
    // Start is called before the first frame update
    void Start()
    {
        levelN = 3;
        S_World.events.Add(new Event(gameObject, 10, LoadLevel));
        terrain = GameObject.Find("Terrain");
    }

    static public void ReloadLevel()
    {
        terrain.GetComponent<S_Terrain>().ResetTerrain();
        level.GetComponent<S_Level>().filename = GenerateLevelFilename();
        level.GetComponent<S_Level>().Reset();
        Player.instance.RestartLevel();
    }
    
    static public void LoadLevel()
    {
        Destroy(level);
        level = Instantiate(Resources.Load(Config.Levels_Dir+"/Level0")) as GameObject;
        terrain.GetComponent<S_Terrain>().ResetTerrain();
        level.GetComponent<S_Level>().filename = GenerateLevelFilename();
        level.GetComponent<S_Level>().Build();
    }

    static public string GenerateLevelFilename()
    {
        int variation = Random.Range(0,1);
        string filename = Path.Combine(Config.Levels_Dir,"L"+levelN+"_"+variation);
        return filename;
    }

    static public void NextLevel()
    {
        levelN += 1;
        terrain.GetComponent<S_Terrain>().ResetTerrain();
        LoadLevel();
    }

    static public void EnemyKilled()
    {
        S_Level levelScript = level.GetComponent<S_Level>();
        levelScript.RemoveEnemy();
        if (levelScript.GetEnemies() <= 0) {
            levelScript.Finish();
            NextLevel();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r")) {
            ReloadLevel();
        }
    }
}
