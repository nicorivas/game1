using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    static GameObject objectHeld;
    public static float shootRange;
    public static List<GameObject> targets;
    static public float health;
    static public int maxHearts;
    public static Player instance;
    Vector3 originalPos;
    S_Gun gun;
    void Awake()
    {
        maxHearts = 5;
        health = 3f;
        shootRange = 5f;
    }
    void Start()
    {
        targets = new List<GameObject>();
        instance = this;
        originalPos = gameObject.transform.position;
        gun = GetComponent<S_Gun>();
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

    public void Heal(float heal) {
        health += heal;
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
        } else if (other.tag == "Portal") {
            S_Director.NextLevel();
            Destroy(other.gameObject);
        } else if (other.tag == "Damage") {
            if (other.GetComponent<S_TileDamage>().damageActive) {
                Hurt(10f);
            };
        } else if (other.tag == "Magma") {
            Hurt(1000f);
        } else if (other.tag == "Explosion") {
            Hurt(1f);
        }
    }

    void StartShoot(GameObject target)
    {
        if (!targets.Contains(target)) {
            targets.Add(target);
        }
    }

    public void RemoveTarget(GameObject target)
    {
        targets.Remove(target);
    }

    void Update()
    {
        gun.Shoot();
    }

    void FixedUpdate()
    {
        
    }
}
