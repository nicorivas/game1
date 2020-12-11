using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Enemy : MonoBehaviour
{
    public float health;
    GameObject rangeCircle;
    void Start()
    {
        health = 100f;
        rangeCircle = Instantiate(Resources.Load("RangeCircle")) as GameObject;
        rangeCircle.transform.parent = this.transform;
        rangeCircle.transform.localPosition = new Vector3(0.0f,1.0f,0.0f);
        rangeCircle.transform.localScale = new Vector3(Player.shootRange, Player.shootRange, 0f);
    }
    /*
    public Vector3 GetDirectionToPlayer(float variance=0f) {
        Vector3 toPlayerDirection = (player.transform.position - gameObject.transform.position).normalized;
        toPlayerDirection = Quaternion.AngleAxis(variance*Random.Range(-1f,1f),Vector3.up)*toPlayerDirection;
        return toPlayerDirection;
    }

    public Vector3 GetDirectionAwayFromPlayer(float variance=0f) {
        Vector3 direction = -(player.transform.position - gameObject.transform.position).normalized;
        direction = Quaternion.AngleAxis(variance*Random.Range(-1f,1f),Vector3.up)*direction;
        if (onBorder && !moving) {
            direction = borderNormal;
            //Debug.Log("moveDirection="+moveDirection+" borderNormal="+borderNormal);
        }
        return direction;
    }

    public void ChangeDirection() {
        if (moveDirectionType == moveDirectionTypes.towardsPlayer) {
            moveDirection = GetDirectionToPlayer(variance:1.0f*directionVariance);
        } else if (moveDirectionType == moveDirectionTypes.awayFromPlayer) {
            moveDirection = GetDirectionAwayFromPlayer(variance:1.0f*directionVariance);
        }
        if (moving) {
            S_World.events.Add(new Event(
                    gameObject, 
                    StraightWalkTicks+(int)(Random.Range(-1f,1f)*StraightWalkTicksVariance), 
                    ChangeDirection, 
                    recurrent_: false, 
                    variance_: 0));
        }
    }
    */

    public void Hit(float damage=1.0f) {
        health -= damage;
        if (health <= 0f) {
            Die();
        }
    }

    void Die() {
        GameObject.FindWithTag("Player").GetComponent<Player>().RemoveTarget(gameObject);
        S_World.events.RemoveFromGameObject(gameObject);
        S_Director.EnemyKilled();
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        ;
    }
}
