using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_TerrainObject : MonoBehaviour
{

    protected Animator m_Animator;
    public GameObject terrainTile;
    public GameObject resourceObject;
    protected int tick;
    protected int level;
    protected int levelMax;
    protected bool selectable;
    protected bool selected;
    protected bool grabable;
    protected bool beingGrabbed;
    protected string name;
    public int x, z;
    public bool started = false;
    public Dictionary<string, int> energy;
    Renderer rend;
    GameObject debug_text;
    bool debug_ = true;
    protected int objectType;

    // Start is called before the first frame update
    protected virtual void Awake() {
        level = 1;
        levelMax = Config.Default_Level_Max;
        grabable = false;
        beingGrabbed = false;
        name = "TerrainObject";
        selectable = true;
        energy = new Dictionary<string, int> {
            {"fire", 0},
            {"life", 0},
            {"water", 0}
        };
    }

    protected virtual void Initialize(int objectType_) {
        objectType = objectType_;
    }
    
    protected virtual void Start()
    {
        //Debug.Log("S_TerrainObject:Start");
        tick = S_World.tick;
        S_World.OnTick += Tick;
        started = true;
    }

    public string GetName() {
        return name;
    }

    protected virtual void SetLevel(int level_) {
        level = level_;
    }

    public int GetObjectType() {
        return objectType;
    }

    public void SetObjectType(int objectType_) {
        objectType = objectType_;
    }

    public virtual int GetFireEnergy() {
        return energy["fire"];
    }

    public virtual void AddFireEnergy(int e) {
        SetFireEnergy(energy["fire"]+e);
    }

    public virtual void SetFireEnergy(int e) {
        energy["fire"] = e;
        if (energy["fire"] < 0) energy["fire"] = 0;
    }

    public virtual int GetLifeEnergy() {
        return energy["life"];
    }

    public virtual void AddLifeEnergy(int e) {
        //Debug.Log("S_TerrainObject:AddLifeEnergy e="+e);
        SetLifeEnergy(energy["life"]+e);
        S_World.AddLifeEnergy(e);
    }

    public virtual void SetLifeEnergy(int e) {
        energy["life"] = e;
        if (energy["life"] < 0) energy["life"] = 0;
    }

    protected virtual void Tick(object sender, S_World.OnTickEventArgs e) {
        tick += 1;
    }

    protected void LoadResource(string resourceName) {
        Debug.Log("S_TerrainObject:LoadResource");
        resourceObject = Instantiate(Resources.Load(resourceName)) as GameObject;
        resourceObject.transform.parent = transform;
        resourceObject.transform.localPosition = new Vector3(0.0f,0.0f,0.0f);
        m_Animator = resourceObject.GetComponent<Animator>();
    }

    public virtual void Combine(GameObject gameObject) {}

    public virtual void Place() {
        //SelectOutlineOff();
    }

    public virtual void Delete() {
        //Debug.Log("S_TerrainObject:Delete gameObject="+gameObject);
        if (terrainTile != null) {
            terrainTile.GetComponent<S_TerrainTile>().RemoveObject(gameObject);
        }
        S_World.events.RemoveFromGameObject(gameObject);
        S_World.OnTick -= Tick;
        Destroy(gameObject);
    }

    public void PickUp() {
        Debug.Log("S_TerrainObject:PickUp");
        Player.Pickup(gameObject);
        if (terrainTile != null) {
            terrainTile.GetComponent<S_TerrainTile>().RemoveObject(gameObject);
        }
        beingGrabbed = false;
    }

    protected virtual void OnMouseDown() {
        Debug.Log("S_TerrainObject:OnMouseDown - grabable="+grabable);
        if (grabable && !Player.IsHoldingObject()) {
            S_World.events.Add(new Event(gameObject, 5, PickUp));
            beingGrabbed = true;
            //SelectOutlineOn();
        } else if (selectable) {
            S_World.SelectObject(gameObject);
        }
    }

    protected virtual int CalculateLevel(string energyType, Dictionary<int,int> dictionary) {
        int lvl = level;
        while (lvl < levelMax && energy[energyType] > dictionary[lvl+1]) {
            lvl += 1;
        }
        while (lvl > 1 && energy[energyType] < dictionary[lvl]) {
            lvl -= 1;
        }
        return lvl;
    }

    void Update()
    {
        //debug_text.GetComponent<TextMesh>().text = "a";
    }

    private void SelectOutlineOn() {
        Renderer[] children = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in children) {
            rend.material.shader = Shader.Find("Unlit/Outline");
        }
    }

    private void SelectOutlineOff() {
        Renderer[] children = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in children) {
            rend.material.shader = Shader.Find("Standard");
        }
    }
}
