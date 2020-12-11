using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// Struct that contains the information of the contact, gathered from the collision message ("enter" and "stay").
/// </summary>
public struct Trigger
{
    
    /// <summary>
    /// Flag that indicates if this contact was created in the OnCollisionEnter (2D/3D) message.
    /// </summary>
	public bool firstContact;

    /// <summary>
    /// The 2D collider component associated with the trigger.
    /// </summary>
    public Collider2D collider2D;

    /// <summary>
    /// The 3D collider component associated with the trigger.
    /// </summary>
    public Collider collider3D;

    /// <summary>
    /// The gameObject representing the trigger.
    /// </summary>
    public GameObject gameObject;

    /// <summary>
    /// The gameObject representing the trigger.
    /// </summary>
    public Transform transform;

    /// <summary>
    /// Sets all the structs fields, based on the callback ("enter" or "stay") and the 3D collider.
    /// </summary>
    public void Set( bool firstContact , Collider collider )
    {
        this.firstContact = firstContact;
        this.collider3D = collider;
        this.gameObject = collider.gameObject;
        this.transform = collider.transform;
    }

    /// <summary>
    /// Sets all the structs fields, based on the callback ("enter" or "stay") and the 2D collider.
    /// </summary>
    public void Set( bool firstContact , Collider2D collider )
    {
        this.firstContact = firstContact;
        this.collider2D = collider;
        this.gameObject = collider.gameObject;
        this.transform = collider.transform;
    }
}

}

