using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{

public class AIWalker : CharacterAIBehaviour
{
    public enum movementTypes // your custom enumeration
    {
        random, 
        diagonal,
        towardsPlayer
    };
    public movementTypes movementType = movementTypes.random;
    public int shootPeriodTicks, shootPeriodVarianceTicks;
    public float movementDirectionVariance;
    public int movementDirectionRefreshTicks;
    public bool shoot;
    public bool hurtOnTouch;
    public float touchDamage;
    bool moving;
    S_Gun gun;
    S_ShieldGun shieldGun;
    Vector3 initialPosition = default(Vector3);
    Vector3 direction;

    public override void EnterBehaviour( float dt )
    {
        initialPosition = transform.position;
        gun = GetComponent<S_BulletGun>();
        shieldGun = GetComponent<S_ShieldGun>();
        S_World.events.Add(new Event(gameObject, 10, Move));
        if (shoot) {
            S_World.events.Add(new Event(gameObject, 10, Shoot));
        }
        // Initial direction
        direction = new Vector3(1.0f,0.0f,1.0f);
        if (movementType == movementTypes.diagonal) {
            direction = Quaternion.AngleAxis(Random.Range(0,4)*90f,Vector3.up)*direction;
        } else {
            direction = Quaternion.AngleAxis(Random.Range(0f,360f),Vector3.up)*direction;
        }
        direction = direction.normalized;
        // Direction movement
        S_World.events.Add(new Event(gameObject, movementDirectionRefreshTicks, RefreshDirection));
    }

    void Move() {
        moving = true;
    }

    public void Stop() {
        moving = false;
        characterActions.Reset();
    }

    void Shield()
    {
        shieldGun.Shoot();
        S_World.events.Add(new Event(gameObject, 1, Move));
    }

    void Shoot()
    {
        if (gun != null)
            gun.Shoot();
        S_World.events.Add(new Event(gameObject, shootPeriodTicks + (int)(Random.value*shootPeriodVarianceTicks), Shoot));
    }

    void RefreshDirection() {
        direction = Quaternion.AngleAxis(Random.Range(-movementDirectionVariance,movementDirectionVariance),Vector3.up)*direction;
        direction = direction.normalized;
        S_World.events.Add(new Event(gameObject, movementDirectionRefreshTicks, RefreshDirection));
    }
    
    public override void UpdateBehaviour( float dt )
    {
        if (moving) {
            if (movementType == movementTypes.towardsPlayer) {
                GameObject player = GameObject.FindWithTag("Player");
                Vector3 toPlayerDirection = (player.transform.position - gameObject.transform.position).normalized;
                direction = toPlayerDirection;
            }
            SetMovementAction(direction);
        }
    }

    public override void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "TerrainBorder" || collision.gameObject.tag == "Block") {
            Bounce(collision.contacts[0].normal);
        } else if (collision.gameObject.tag == "Player") {
            if (hurtOnTouch) {
                GameObject player = GameObject.FindWithTag("Player");
                player.GetComponent<Player>().Hurt(touchDamage);
            }
        }
    }
    
    private void Bounce(Vector3 collisionNormal)
    {
        direction = Vector3.Reflect(direction, collisionNormal);
    }
}

}
