using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// This component represents a capsule collider in a 3D world.
/// </summary>
public class CapsuleColliderComponent3D : ColliderComponent3D
{
    CapsuleCollider capsuleCollider = null;

    public override Vector3 Size
    {
        get
        {
            return new Vector2( 2f * capsuleCollider.radius , capsuleCollider.height );
        }
        set
        {
            capsuleCollider.radius = value.x / 2f;
            capsuleCollider.height = value.y;
        }
    }
    public override Vector3 Offset 
    {
        get
        {
            return capsuleCollider.center;
        }
        set
        {
            capsuleCollider.center = value;
        }
    }

    protected override void Awake()
    {
                
        capsuleCollider = gameObject.GetOrAddComponent<CapsuleCollider>(); 
        collider = capsuleCollider;

        base.Awake();
        
    }

   
    
}

}
