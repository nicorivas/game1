using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class PowerData {
    public string name;
    public float prob;
}

public class PowersData {
    public PowerData[] powers;
}

public class S_Director : MonoBehaviour
{
    static int levelN;
    static public GameObject level;
    static public GameObject terrain;
    static public PowersData powersData;
    // Start is called before the first frame update
    void Awake() {
        // Configure physics
        Physics.IgnoreLayerCollision(10, 11, true);
        Physics.IgnoreLayerCollision(11, 11, true);
        // Read power-up data
        TextAsset powersJson = Resources.Load<TextAsset>(Path.Combine(Config.Powers_Dir,"Powers"));
        Debug.Log(powersJson);
        powersData = JsonUtility.FromJson<PowersData>(powersJson.text);
    }

    void Start()
    {
        levelN = 3;
        S_World.events.Add(new Event(gameObject, 10, LoadLevel));
        terrain = GameObject.Find("Terrain");
    }

    static public void ReloadLevel()
    {
        level.GetComponent<S_Level>().filename = GenerateLevelFilename();
        level.GetComponent<S_Level>().Reset();
        Player.instance.RestartLevel();
    }
    
    static public void LoadLevel()
    {
        Destroy(level);
        level = Instantiate(Resources.Load(Config.Levels_Dir+"/Level0")) as GameObject;
        level.GetComponent<S_Level>().filename = GenerateLevelFilename();
        level.GetComponent<S_Level>().Build();
    }

    static public void SpawnPowers()
    {
        int[,] coords = new int[,] {{3, 4}, {5, 4}};
        int i = 0;
        int powerIndex = 0;
        int count = 0;
        int powerUps = 2;
        while (i<powerUps && count < 100) {
            powerIndex = Random.Range(0,powersData.powers.Length);
            if (Random.value < powersData.powers[powerIndex].prob) {
                S_Terrain.SpawnPower(
                    new Vector2Int(coords[i,0],coords[i,1]),
                    powersData.powers[powerIndex].name);
                i++;
            }
            powerIndex++;
        }
    }

    static public string GenerateLevelFilename()
    {
        int variation = Random.Range(0,2);
        string filename = Path.Combine(Config.Levels_Dir,"L"+levelN+"_"+variation);
        return filename;
    }

    static public void NextLevel()
    {
        levelN += 1;
        SpawnPowers();
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

    void Update()
    {
        if (Input.GetKeyDown("r")) {
            terrain.GetComponent<S_Terrain>().ResetTerrain();
            ReloadLevel();
        }
    }
}
