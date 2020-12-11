using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// This component represents a capsule collider in a 2D world.
/// </summary>
public class CapsuleColliderComponent2D : ColliderComponent2D
{
    CapsuleCollider2D capsuleCollider = null;

    public override Vector3 Size
    {
        get
        {
            return capsuleCollider.size;
        }
        set
        {
            capsuleCollider.size = value;
        }
    }
    public override Vector3 Offset 
    {
        get
        {
            return capsuleCollider.offset;
        }
        set
        {
            capsuleCollider.offset = value;
        }
    }

    protected override void Awake()
    {        
              
        capsuleCollider = gameObject.GetOrAddComponent<CapsuleCollider2D>();	
        collider = capsuleCollider;

        base.Awake();       

    }

    
}
    

    
}
