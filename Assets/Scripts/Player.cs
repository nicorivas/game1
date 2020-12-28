using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    static GameObject objectHeld;
    public static float shootRange = 40f;
    public static List<GameObject> targets;
    static public float health;
    static public int maxHearts;
    public static Player instance;
    Vector3 originalPos;
    void Awake()
    {
        maxHearts = 5;
        health = 3f;
    }
    void Start()
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = 2;
        targets = new List<GameObject>();
        instance = this;
        originalPos = gameObject.transform.position;
    }

    static public float GetHealth()
    {
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

    public void RestartLevel() {
        transform.position = originalPos;
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

    static public void Pickup(GameObject gameObject) {
        objectHeld = gameObject;
    }

    static public void Release() {
        objectHeld = null;
    }

    public GameObject GetObjectHeld() {
        return objectHeld;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Damage") {
            if (other.GetComponent<S_TileDamage>().damageActive) {
                Hurt(10f);
            };
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy") {
            Hit(other.gameObject);
        } else if (other.tag == "Range") {
            StartShoot(other.transform.parent.gameObject);
        } else if (other.tag == "Portal") {
            S_Director.NextLevel();
            Destroy(other.gameObject);
        } else if (other.tag == "Damage") {
            if (other.GetComponent<S_TileDamage>().damageActive) {
                Hurt(10f);
            };
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Range") {
            RemoveTarget(other.transform.parent.gameObject);
        }
    }

    void StartShoot(GameObject target) {
        if (!targets.Contains(target)) {
            targets.Add(target);
            GetComponent<LineRenderer>().positionCount = 2;
        }
    }

    public void RemoveTarget(GameObject target)
    {
        targets.Remove(target);
        GetComponent<LineRenderer>().positionCount = 0;

    }

    void Update() {
        for (int i=targets.Count-1; i>=0; i--) {
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            bool shootable = true;
            Vector3 originPosition = new Vector3(gameObject.transform.position.x, 2.0f, gameObject.transform.position.z);
            Vector3 targetPosition = targets[i].transform.Find("Spot").transform.position;
            float distance = (originPosition-targetPosition).magnitude;
            RaycastHit[] hits = Physics.RaycastAll(originPosition, targetPosition-originPosition, distance);
            foreach (RaycastHit hit in hits) {
                if (hit.transform.gameObject.tag == "Shield") {
                    shootable = false;
                    lineRenderer.positionCount = 0;
                }
            }
            if (shootable) {
                lineRenderer.SetPosition(0, originPosition);
                lineRenderer.SetPosition(1, targetPosition);
                targets[i].GetComponent<S_Enemy>().Hurt();
            }
        }
    }

    void FixedUpdate()
    {
        
    }
}
