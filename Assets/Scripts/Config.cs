using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : ScriptableObject
{
    static public Dictionary<int,float> Volcano_Rock_Distance_With_Level = new Dictionary<int,float>{
        {1,1f},
        {2,10f},
        {3,10f},
        {4,10f},
        {5,10f}
    };
    static public Dictionary<int, int> Volcano_Level_With_Fire_Energy = new Dictionary<int, int> {
        {1, 0},
        {2, 20},
        {3, 50},
        {4, 100},
        {5, 200}
    };
    static public Dictionary<int, int> Tree_Level_With_Life_Energy = new Dictionary<int, int> {
        {1, 0},
        {2, 50},
        {3, 100},
        {4, 150},
        {5, 200}
    };
    static public Dictionary<int, bool> Tree_Level_Grabable = new Dictionary<int, bool> {
        {1, true},
        {2, true},
        {3, true},
        {4, false},
        {5, false}
    };
    // Dirs
    static public string Bullets_Dir = "Bullets";
    static public string Enemies_Dir = "Enemies";
    static public string Levels_Dir = "Levels";
    //
    static public int Default_Level_Max = 5;
    static public int Energy_Tick_Period = 100;
    static public int Fire_Drain_Ticks = 50;
    static public int Life_Drain_Ticks = 50;
    // Terrain
    static public float[] Volcano_Init_Region = new float[] {0.1f,0.1f,0.3f,0.3f};
    static public float[] Mountain_Init_Region = new float[] {0.4f,0.7f,0.9f,0.9f};
    static public float[] Tree_Init_Region = new float[] {0.2f,0.65f,0.9f,0.9f};
    static public float Humidity_Max = 10.0f;
    static public float Temperature_Max = 40.0f;
    static public float Temperature_Min = -10.0f;
    //
    static public float Block_Spawn_Height = 10f;
    // Mountain
    static public int Mountain_Level_Max = 5;
    // Volcano
    static public int Volcano_Level_Max = 5;
    static public int Volcano_Eruption_Ticks = 50;
    static public int Volcano_Eruption_Ticks_Variance = 20;
    // Trees
    static public int Tree_Level_Max = 5;
    static public int Tree_Init_Life_Level = 250;
    static public int Tree_Max_Life_Level = 300;
    static public int Tree_Rain_Combine_Life = 25;
    static public int Tree_Drop_Seed_Ticks = 25;
    static public int Tree_In_Volcano_Fire_Energy = 10;
    static public int Tree_Drop_Seed_Life_Energy = -10;
    // Clouds
    static public float Cloud_Max_Probability = 0.25f;
    static public int Cloud_Creation_Ticks = 100;
    static public int Cloud_Creation_Ticks_Variance = 200;
    static public string Cloud_Prefab_Name = "Cloud";
    static public int Cloud_Humidity_Level = 8;
    static public string Cloud_Tag_Name = "Cloud";
    static public int Cloud_Rain_Ticks = 100;
    // Rain
    static public string Rain_Prefab_Name = "Rain";
    static public int Rain_Ticks = 10;
    // Water
    static public int Water_Level_Max = 5;
    static public string Water_Prefab_Name = "Water";
    static public int Water_Ticks = 10;
    // Snow
    static public int Snow_Level_Max = 5;
    static public string Snow_Prefab_Name = "Snow";
    static public int Snow_Ticks = 10;
    // Rocks
    static public Dictionary<int, int> Rock_Type_From_Terrain_Type = new Dictionary<int, int> {
        {0, 0},
        {1, 1},
        {2, 2},
        {3, 3},
        {4, 4},
    };
    static public int Rock_Grassland_Life_Energy = 1;
    static public int Rock_Drain_Fire_Energy = 1;
    static public int Rock_In_Volcano_Fire_Energy = 2;
}
