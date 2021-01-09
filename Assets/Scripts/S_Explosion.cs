using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Explosion : MonoBehaviour
{
    public float destroySeconds;
    protected void Start()
    {
    }

    void Eliminate() {
        Destroy(gameObject);
    }

    void Update()
    {
        destroySeconds -= Time.deltaTime;
        if (destroySeconds <= 0) {
            Eliminate();
        }
    }
}