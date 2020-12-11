using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// An implementation of a ColliderComponent for 2D colliders.
/// </summary>
public abstract class ColliderComponent2D : ColliderComponent
{
    protected new Collider2D collider = null;
    
    public PhysicsMaterial2D Material
    {
        get
        {
            return collider.sharedMaterial;
        }
        set
        {
            collider.sharedMaterial = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        PhysicsMaterial2D material = new PhysicsMaterial2D("Frictionless 2D");
        material.friction = 0f;
        material.bounciness = 0f;

        collider.sharedMaterial = material;
        this.collider.hideFlags = HideFlags.NotEditable;
    }

    protected override void OnEnable()
    {
        collider.enabled = true;
    }
    
    protected override void OnDisable()
    {
        collider.enabled = false;
    }
}

}
