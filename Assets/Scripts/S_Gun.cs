using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.CharacterControllerPro.Core
{


public class S_Gun : MonoBehaviour
{
    public GameObject bullet;
    public enum shootDirectionTypes // your custom enumeration
    {
        random, 
        towardsPlayer,
        given
    };
    public shootDirectionTypes shootDirectionType = shootDirectionTypes.random;
    public enum gunStates // your custom enumeration
    {
        idle, 
        shooting,
        reloading
    };
    gunStates state;
    Vector3 shootDirection;
    public int shellBullets;
    int lastShootTick, shootsInBatch, lastBatchTick;
    public int shootsPerBatch, shootTicks, batchTicks;
    public float shellSpreadArch;
    Lightbug.CharacterControllerPro.Core.CharacterActor characterActor;

    // Start is called before the first frame update
    void Start()
    {
        state = gunStates.idle;
        shootsInBatch = 0;
        lastBatchTick = S_World.tick;
        characterActor = GameObject.FindWithTag("Player").GetComponent<CharacterActor>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (state == gunStates.shooting)
        {
            if (shootsInBatch == shootsPerBatch) {
                EndBatch();
            } else if (S_World.tick - lastShootTick >= shootTicks && shootsInBatch < shootsPerBatch) {
                Shoot();
            }
        }
        
    }

    void EndBatch()
    {
        state = gunStates.idle;
        shootsInBatch = 0;
    }

    public void StartBatch(Vector3 direction=default(Vector3))
    {
        if (direction != default(Vector3))
            shootDirection = direction;
        if (S_World.tick - lastBatchTick >= batchTicks)
        {
            lastBatchTick = S_World.tick;
            state = gunStates.shooting;
            Shoot();
        }
    }

    void Shoot() 
    {
        shootsInBatch += 1;
        lastShootTick = S_World.tick;

        for (int i=0; i<shellBullets; i++) {

            S_Bullet bs = CreateBullet();

            if (shootDirectionType == shootDirectionTypes.random)
            {
                shootDirection = new Vector3(Random.Range(-1f,1f),0f,Random.Range(-1f,1f)).normalized;
                bs.initialRotation = Quaternion.LookRotation(characterActor.Forward).normalized;
            }
            else if (shootDirectionType == shootDirectionTypes.towardsPlayer)
            {
                GameObject player = GameObject.FindWithTag("Player");
                Vector3 horizontal = Vector3.right + Vector3.forward;
                Vector3 toPlayerDirection = (player.transform.position - gameObject.transform.position).normalized;
                float deltaAngle = 0f;
                if (shellBullets > 1)
                    deltaAngle = shellSpreadArch*2.0f/(shellBullets-1);
                toPlayerDirection = Quaternion.AngleAxis(-shellSpreadArch+deltaAngle*i,Vector3.up)*toPlayerDirection;
                shootDirection = Vector3.Scale(toPlayerDirection,horizontal);
                bs.initialRotation = Quaternion.LookRotation(characterActor.Forward).normalized;
            }
            else if (shootDirectionType == shootDirectionTypes.given)
            {
                //
            } 
            bs.moveDirection = shootDirection;
            bs.MoveSpeed = 10.0f;
        }
    }

    S_Bullet CreateBullet() {
        GameObject b = Instantiate(bullet) as GameObject;
        b.transform.position = this.transform.position;
        return b.GetComponent<S_Bullet>();
    }
}

}