using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_BulletGun : S_Gun
{
    public GameObject bullet;
    public List<GameObject> bullets;
    public int batchsPerShoot, batchTicks, shellBullets;
    int batchsInShoot, lastBatchTick;
    public float shellSpreadArch;
    public float angleVariance;
    public bool upwards;
    public float bulletSpeed;

    void Awake()
    {
        base.Awake();
        bullets = new List<GameObject>();
    }

    void Start()
    {
        base.Start();
        batchsInShoot = 0;
        lastBatchTick = S_World.tick;
    }


    void Update()
    {
        if (state == gunStates.shooting) {
            if (batchsInShoot == batchsPerShoot) {
                StopShooting();
            } else if (S_World.tick - lastBatchTick >= batchTicks && batchsInShoot < batchsPerShoot) {
                Batch();
            }
        }   
    }

    public override void StopShooting()
    {
        base.StopShooting();
        batchsInShoot = 0;
    }

    public override void Shoot(Vector3 direction=default(Vector3), GameObject target=default(GameObject))
    {
        Debug.Log("Shoot");
        base.Shoot();
        Batch();
    }


    protected void Batch() 
    {
        batchsInShoot += 1;
        lastBatchTick = S_World.tick;

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
                if (shellBullets > 1) {
                    deltaAngle = shellSpreadArch*2.0f/(shellBullets-1);
                    toPlayerDirection = Quaternion.AngleAxis(-shellSpreadArch+deltaAngle*i,Vector3.up)*toPlayerDirection;
                }
                toPlayerDirection = Quaternion.AngleAxis(Random.Range(-1f,1f)*angleVariance,Vector3.up)*toPlayerDirection;
                shootDirection = Vector3.Scale(toPlayerDirection,horizontal);
                bs.initialRotation = Quaternion.LookRotation(characterActor.Forward).normalized;
            }
            else if (shootDirectionType == shootDirectionTypes.given)
            {
                //
            } 
            if (upwards) {
                shootDirection = new Vector3(shootDirection.x, 2.0f, shootDirection.z);
            }
            bs.moveDirection = shootDirection;
            bs.moveSpeed = bulletSpeed;
        }
    }

    protected S_Bullet CreateBullet() {
        GameObject b = Instantiate(bullet) as GameObject;
        bullets.Add(b);
        b.GetComponent<S_Bullet>().owner = gameObject;
        b.transform.position = this.transform.position;
        b.transform.position += new Vector3(0f, 1f, 0f);
        return b.GetComponent<S_Bullet>();
    }
}