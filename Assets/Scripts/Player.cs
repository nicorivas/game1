using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    static GameObject objectHeld;
    public static float shootRange = 40f;
    public static List<GameObject> targets;
    public float health;
    public static Player instance;
    Vector3 originalPos;
    void Start()
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = 2;
        targets = new List<GameObject>();
        health = 5f;
        instance = this;
        originalPos = gameObject.transform.position;
    }

    public float GetHealth() {
        return health;
    }

    public void Hit(GameObject enemy)
    {
        //enemy.GetComponent<Spiky>().Hurt();
    }

    public void Hurt(float damage)
    {
        health -= damage;
        if (health <= 0) {
            GameOver();
        }
    }

    public void GameOver() {
        S_UI.GameOver();
    }

    static public bool IsHoldingObject() {
        return objectHeld != null;
    }

    static public GameObject GetHeldObject() {
        return objectHeld;
    }

    static public void DropHeldObject() {
        objectHeld = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy") {
            Hit(other.gameObject);
        } else if (other.tag == "Range") {
            targets.Add(other.transform.parent.gameObject);
            GetComponent<LineRenderer>().positionCount = 2;
        } else if (other.tag == "Portal") {
            S_Director.NextLevel();
            Destroy(other.gameObject);
        }
    }

    public void RemoveTarget(GameObject target) {
        targets.Remove(target);
        GetComponent<LineRenderer>().positionCount = 0;

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Range") {
            RemoveTarget(other.transform.parent.gameObject);
        }
    }

    static public void Pickup(GameObject gameObject) {
        objectHeld = gameObject;
    }

    static public void Release() {
        objectHeld = null;
    }

    public GameObject GetObjectHeld() {
        return objectHeld;
    }

    void Update() {
        for (int i=targets.Count-1; i>=0; i--) {
            
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, new Vector3(gameObject.transform.position.x, 1.0f, gameObject.transform.position.z));
            lineRenderer.SetPosition(1, new Vector3(targets[i].transform.position.x, 1.0f, targets[i].transform.position.z));
            targets[i].GetComponent<S_Enemy>().Hit();
        }
    }

    void FixedUpdate()
    {
        
    }
}
