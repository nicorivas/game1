using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Enemy : MonoBehaviour
{
    public float health;
    public int score;
    GameObject rangeCircle;
    Lightbug.CharacterControllerPro.Implementation.CharacterAIBehaviour AI;
    void Start()
    {
        rangeCircle = Instantiate(Resources.Load("RangeCircle")) as GameObject;
        rangeCircle.transform.parent = this.transform;
        rangeCircle.transform.localPosition = new Vector3(0.0f,1.0f,0.0f);
        rangeCircle.transform.localScale = new Vector3(Player.shootRange, Player.shootRange, 0f);
        S_Director.level.GetComponent<S_Level>().AddGameObject(gameObject);
        AI = gameObject.transform.Find("Actions").GetComponent<Lightbug.CharacterControllerPro.Implementation.CharacterAIBehaviour>();
    }

    public void Hurt(float damage=1.0f) {
        health -= damage;
        if (health <= 0f) {
            Die();
        }
    }

    void Die() {
        GameObject.FindWithTag("Player").GetComponent<Player>().RemoveTarget(gameObject);
        S_World.events.RemoveFromGameObject(gameObject);
        S_Director.EnemyKilled();
        S_World.AddScore(score);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        ;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Magma") {
            Hurt(1000f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        AI.OnCollisionEnter(collision);
    }
}
