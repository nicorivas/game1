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
        diagonal
    };
    public movementTypes movementType = movementTypes.random;
    public int shootPeriodTicks, shootPeriodVarianceTicks;
    public float angleVariance;
    public bool shoot;
    bool moving;
    S_Gun gun;
    S_ShieldGun shieldGun;
    Vector3 initialPosition = default(Vector3);
    Vector3 direction;

    public override void EnterBehaviour( float dt )
    {
        initialPosition = transform.position;
        gun = GetComponent<S_Gun>();
        shieldGun = GetComponent<S_ShieldGun>();
        S_World.events.Add(new Event(gameObject, 10, Move));
        direction = new Vector3(1f,0f,1f);
        if (shoot) {
            S_World.events.Add(new Event(gameObject, 10, Shoot));
        }
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
        shieldGun.StartBatch();
        S_World.events.Add(new Event(gameObject, 1, Move));
    }

    void Shoot()
    {
        gun.StartBatch();
        S_World.events.Add(new Event(gameObject, shootPeriodTicks + (int)(Random.value*shootPeriodVarianceTicks), Shoot));
    }
    
    public override void UpdateBehaviour( float dt )
    {
        if (moving) {
            SetMovementAction(direction);
            if (movementType == movementTypes.random) {
                direction = Quaternion.AngleAxis(Random.Range(-angleVariance,angleVariance),Vector3.up)*direction;
            }
        }
    }

    public override void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "TerrainBorder" || collision.gameObject.tag == "Block") {
            Bounce(collision.contacts[0].normal);
        }
    }
    
    private void Bounce(Vector3 collisionNormal)
    {
        direction = Vector3.Reflect(direction, collisionNormal);
    }
}

}
