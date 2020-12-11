using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.CharacterControllerPro.Demo
{
public abstract class AddTorque : MonoBehaviour
{
    [SerializeField]
    protected Vector3 torque;

    [SerializeField]
    protected float maxAngularVelocity = 200f;
    
    
    protected virtual void Awake(){}

    protected abstract void AddTorqueToRigidbody();

    void FixedUpdate()
    {
        AddTorqueToRigidbody();
    }
    
    
}

}
