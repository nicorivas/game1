using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

/// <summary>
/// This class represents a kinematic actor, it is used as a base class for kinematic platforms and kinematic cameras.
/// </summary>
public abstract class KinematicActor : MonoBehaviour
{
    RigidbodyComponent rigidbodyComponent = null;
    

    /// <summary>
    /// Gets the RigidbodyComponent component associated to the character.
    /// </summary>
    public RigidbodyComponent RigidbodyComponent
    {
        get
        {
            return rigidbodyComponent;
        }
    }

    protected virtual void Awake()
    {
        if( GetComponent<Collider2D>() != null || GetComponent<Rigidbody2D>() != null)
        {
            rigidbodyComponent = gameObject.AddComponent<RigidbodyComponent2D>();
        }
        else if( GetComponent<Collider>() != null || GetComponent<Rigidbody>() != null)
        {
            rigidbodyComponent = gameObject.AddComponent<RigidbodyComponent3D>();
        }

        rigidbodyComponent.IsKinematic = true;
        rigidbodyComponent.UseInterpolation = true;
        
    }

    

    /// <summary>
    /// Updates the kinematic actor.
    /// </summary>
    public virtual void UpdateKinematicActor( float dt ){}

    protected virtual void OnEnable()
    {
        rigidbodyComponent.Constraints = RigidbodyConstraints.None;
    }

    protected virtual void OnDisable()
    {
        rigidbodyComponent.Constraints = RigidbodyConstraints.FreezeAll;
    }

}

}
