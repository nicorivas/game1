using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Gun : MonoBehaviour
{
    public enum shootDirectionTypes
    {
        random, 
        towardsPlayer,
        given
    };
    public shootDirectionTypes shootDirectionType = shootDirectionTypes.random;
    public enum gunStates
    {
        idle, 
        shooting,
        reloading
    };
    protected gunStates state;
    protected Vector3 shootDirection;
    protected int lastShootTick;
    public int shootTicks;
    public float range;
    protected Lightbug.CharacterControllerPro.Core.CharacterActor characterActor;

    protected virtual void Awake() {}

    protected virtual void Start()
    {
        state = gunStates.idle;
        characterActor = GameObject.FindWithTag("Player").GetComponent<Lightbug.CharacterControllerPro.Core.CharacterActor>();
    }

    public virtual void Shoot(Vector3 direction=default(Vector3), GameObject target=default(GameObject))
    {
        if (direction != default(Vector3)) {
            shootDirection = direction;
        }
        if (S_World.tick - lastShootTick >= shootTicks) {
            lastShootTick = S_World.tick;
            state = gunStates.shooting;
        }
    }

    public virtual void StopShooting() {
        state = gunStates.idle;
    }
}