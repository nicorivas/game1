using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_World : MonoBehaviour
{
    static public float time;

    public int year;

    static public int month;
    static public int day;
    static public int hour;

    static public float yearFloat;
    static public float monthFloat;
    static public float dayFloat;
    static public float hourFloat;

    static public GameObject selectedObject;

    static public int monthDaysLength = 5;
    static public int yearMonthsLength = 11;
    float daySecondsLength = 10.0f;
    public GameObject GO_Terrain;
    private GameObject terrain;
    public float tickTime = 0.1f;
    float lastTickTime = 0.0f;
    static public Dictionary<string, int> UI_STATES = new Dictionary<string, int> {
        {"Idle",0},
        {"Placing",1}
    };
    static public int uiState;

    public class OnTickEventArgs : EventArgs {
        public int tick;
    } 

    public static event EventHandler<OnTickEventArgs> OnTick;

    static public int score;
    static public int tick;

    static public Dictionary<string, int> energy;
    static public EventHandler events; 

    void Awake()
    {
        time = 0f;
        score = 0;
        selectedObject = null;
        events = new EventHandler();
        tick = 0;
    }
    void Start()
    {
        energy = new Dictionary<string, int> {
            {"fire",100},
            {"life",100}
        };
        terrain = GameObject.Find("Terrain");
        lastTickTime = 0.0f;
        uiState = UI_STATES["Idle"];
        events.Add(new Event(gameObject, Config.Fire_Drain_Ticks, DrainFireEnergy, true));
        events.Add(new Event(gameObject, Config.Life_Drain_Ticks, DrainLifeEnergy, true));
    }

    static public void DrainFireEnergy() {
        energy["fire"] -= 1;
    }

    static public void AddFireEnergy(int fireEnergyToAdd) {
        energy["fire"] += fireEnergyToAdd;
    }

    static public int GetFireEnergy() {
        return energy["fire"];
    }

    static public void DrainLifeEnergy() {
        energy["life"] -= 1;
    }

    static public void AddLifeEnergy(int lifeEnergyToAdd) {
        energy["life"] += lifeEnergyToAdd;
    }

    static public int GetLifeEnergy() {
        return energy["life"];
    }

    static public void SelectObject(GameObject objectToSelect) {
        selectedObject = objectToSelect;
    }

    static public void AddScore(int s) {
        score += s;
    }
 
    void Update()
    {
        time += Time.deltaTime;

        // Tick system
        if (Time.time - lastTickTime > tickTime) {
            tick += 1;
            lastTickTime = Time.time;
            if (OnTick != null) OnTick(this, new OnTickEventArgs { tick = tick});
        }  
        
        // Day / Hour

        hourFloat = ((time % daySecondsLength)/daySecondsLength)*24.0f; 
        dayFloat = time/daySecondsLength+1.0f;
        monthFloat = (dayFloat/monthDaysLength+1.0f);
        yearFloat = (monthFloat/yearMonthsLength);

        day = (int)(time/daySecondsLength+1.0f);
        hour = (int)(((time % daySecondsLength)/daySecondsLength)*24.0f);
        month = (int)(day/monthDaysLength);
    }

    
}
