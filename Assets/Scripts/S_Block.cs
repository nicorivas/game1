using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Block : S_Object
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Player") {
            float blockBottom = gameObject.GetComponent<Collider>().bounds.min.y;
            float enemyMiddle = (collision.gameObject.GetComponent<Collider>().bounds.min.y+collision.gameObject.GetComponent<Collider>().bounds.max.y)/2f;
            if (blockBottom > enemyMiddle) {
                if (collision.gameObject.tag == "Enemy") {
                    
                    collision.gameObject.GetComponent<S_Enemy>().Hurt(1000f);
                } else {
                    collision.gameObject.GetComponent<Player>().Hurt(1000f);
                }
            }
        }
    }
}
