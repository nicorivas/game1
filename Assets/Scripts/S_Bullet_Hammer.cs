using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Bullet_Hammer : S_Bullet
{
    Quaternion deltaRotation;
    public Vector3 rotationVelocity;
    public int rotationTicks;
    void Start()
    {
        base.Start();
        S_World.events.Add(new Event(gameObject, rotationTicks, Eliminate));
        transform.rotation = initialRotation;
    }

    void FixedUpdate()
    {
        deltaRotation = Quaternion.Euler(rotationVelocity * Time.deltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }
}
