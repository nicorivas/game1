using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Director : MonoBehaviour
{
    static int levelN;
    static GameObject level;
    // Start is called before the first frame update
    void Start()
    {
        levelN = 1;
        S_World.events.Add(new Event(gameObject, 5, LoadLevel));
    }
    
    static public void LoadLevel() {
        level = Instantiate(Resources.Load(Config.Levels_Dir+"/Level"+levelN.ToString())) as GameObject;
    }

    static public void NextLevel() {
        levelN += 1;
        LoadLevel();
    }
    static public void EnemyKilled() {
        S_Level levelScript = level.GetComponent<S_Level>();
        levelScript.enemies -= 1;
        if (levelScript.enemies <= 0) {
            levelScript.Finish();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
