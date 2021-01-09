using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Object : MonoBehaviour
{
    public float health;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Hurt(float damage)
    {
        health -= damage;
        if (health <= 0) {
            Eliminate();
        }
    }

    void Eliminate() {
        Destroy(gameObject);
    }
}
