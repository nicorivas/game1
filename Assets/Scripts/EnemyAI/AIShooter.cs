using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{

[AddComponentMenu("Character Controller Pro/Demo/Character/AI/Shooter Behaviour")]
public class AIShooter : CharacterAIBehaviour
{
    bool moving;
    S_Gun gun;
    S_ShieldGun shieldGun;
    public enum movementTypes // your custom enumeration
    {
        random, 
        nextTiles,
        nextTilesOrigin,
        insideCircle,
        diagonal
    };
    public movementTypes movementType = movementTypes.random;

    public float moveSpeedTop;
    public int moveTicks, moveTicksVariance, directionVariance;
    public int shootToMoveTicks, shieldToMoveTicks, stopToShootTicks;
    public float shieldProb;

    float minRandomMagnitude = 1f;
    float maxRandomMagnitude = 1f;

    Vector3 initialPosition = default( Vector3 );

    Vector3 target = default( Vector3 );

    public override void EnterBehaviour( float dt )
    {
        initialPosition = transform.position;
        gun = GetComponent<S_BulletGun>();
        shieldGun = GetComponent<S_ShieldGun>();
        S_World.events.Add(new Event(
            gameObject, 
            10, 
            Move, 
            recurrent_: false, 
            variance_: 0));
    }

    void Move() {
        moving = true;
        SetTarget();
    }

    public void Stop() {
        moving = false;
        characterActions.Reset();
        if (Random.value < shieldProb) {
            S_World.events.Add(new Event(gameObject, stopToShootTicks, Shield));
        } else {
            S_World.events.Add(new Event(gameObject, stopToShootTicks, Shoot));
        }
    }

    void Shield()
    {
        shieldGun.Shoot();
        S_World.events.Add(new Event(
            gameObject, 
            shieldToMoveTicks, 
            Move, 
            recurrent_: false, 
            variance_: 0));
    }

    void Shoot()
    {
        gun.Shoot();
        S_World.events.Add(new Event(
            gameObject, 
            shootToMoveTicks, 
            Move, 
            recurrent_: false, 
            variance_: 0));
    }
    
    public override void UpdateBehaviour( float dt )
    {
        if (moving) {
            Vector2 targetOnPlane = new Vector2(target.x,target.z);
            Vector2 characterOnPlane = new Vector2(characterActor.Position.x,characterActor.Position.z);
            float distanceToTarget = (targetOnPlane - characterOnPlane).magnitude;
            if (distanceToTarget > 0.5f)
                SetMovementAction(target - characterActor.Position);
            else
                Stop();
        }
    }

    void OnWallHit( CollisionInfo collisionInfo )
	{
        //Stop();
    }


    
    void SetTarget()
    {
        int tries = 0;
        while (tries < 50) {
            if (movementType == movementTypes.random) {
                target = initialPosition + transform.forward * Random.Range( minRandomMagnitude , maxRandomMagnitude );
            } else if (movementType == movementTypes.nextTilesOrigin) {
                target = initialPosition + Quaternion.AngleAxis(Random.Range(0,4)*90f,Vector3.up) * Vector3.left * S_Terrain.tileWidth;
            } else if (movementType == movementTypes.insideCircle) {
                target = initialPosition + Quaternion.AngleAxis(Random.Range(0f,360f),Vector3.up) * Vector3.left * Random.Range( minRandomMagnitude , maxRandomMagnitude );
            } else if (movementType == movementTypes.nextTiles) {
                target = characterActor.Position + Quaternion.AngleAxis(Random.Range(0,4)*90f,Vector3.up) * Vector3.left * S_Terrain.tileWidth;
            }
            if (S_Terrain.PositionInside(target)) {
                break;
            }
            tries += 1;
        }
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Block") {
            // Notice here that collision could also come from block from the top,
            // but that one is handelled on block and kills the enemy immediately.
            Stop();
        }
    }
}

}
