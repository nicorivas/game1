using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Lightbug.Utilities
{

/// <summary>
/// This component represents a sphere collider in a 2D world (better known as a circle).
/// </summary>
public class SphereColliderComponent2D : ColliderComponent2D
{
    CircleCollider2D circleCollider = null;

    public override Vector3 Size
    {
        get
        {
            return Vector2.one * 2f * circleCollider.radius;
        }
        set
        {
            circleCollider.radius = value.x / 2f;
        }
    }
    public override Vector3 Offset 
    {
        get
        {
            return circleCollider.offset;
        }
        set
        {
            circleCollider.offset = value;
        }
    }

    protected override void Awake()
    {        
               
        circleCollider = gameObject.GetOrAddComponent<CircleCollider2D>();	
        collider = circleCollider;
        
        base.Awake();
    }

    
}

}