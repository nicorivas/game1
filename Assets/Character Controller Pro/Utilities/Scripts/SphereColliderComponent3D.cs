using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Lightbug.Utilities
{

/// <summary>
/// This component represents a sphere collider in a 3D world.
/// </summary>
public class SphereColliderComponent3D : ColliderComponent3D
{
    SphereCollider sphereCollider = null;

    public override Vector3 Size
    {
        get
        {
            return Vector3.one * 2f * sphereCollider.radius;
        }
        set
        {
            sphereCollider.radius = value.x / 2f;
        }
    }
    public override Vector3 Offset 
    {
        get
        {
            return sphereCollider.center;
        }
        set
        {
            sphereCollider.center = value;
        }
    }

    protected override void Awake()
    {
                
        sphereCollider = gameObject.GetOrAddComponent<SphereCollider>(); 
        collider = sphereCollider;

        base.Awake();

    }

    
}

}
