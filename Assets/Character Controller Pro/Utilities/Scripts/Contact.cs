using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// Struct that contains the information of the contact, gathered from the collision message ("enter" and "stay").
/// </summary>
public struct Contact
{
    
    /// <summary>
    /// Flag that indicates if this contact was created in the OnCollisionEnter (2D/3D) message.
    /// </summary>
	public bool firstContact;

    /// <summary>
    /// The contact point.
    /// </summary>
	public Vector3 point;

    /// <summary>
    /// The contact normal.
    /// </summary>
    public Vector3 normal;

    /// <summary>
    /// The 2D collider component associated with the collided object.
    /// </summary>
    public Collider2D collider2D;

    /// <summary>
    /// The 3D collider component associated with the collided object.
    /// </summary>
    public Collider collider3D;

    /// <summary>
    /// Flag that indicates if the collided object is a rigidbody or not.
    /// </summary>
	public bool isRigidbody;

    /// <summary>
    /// Flag that indicates if the collided object is a kinematic rigidbody or not.
    /// </summary>
	public bool isKinematicRigidbody;

    /// <summary>
    /// The point velocity of the rigidbody associated at the contact point.
    /// </summary>
    public Vector3 pointVelocity;

    /// <summary>
    /// The gameObject representing the collided object.
    /// </summary>
    public GameObject gameObject;

    /// <summary>
    /// Sets all the structs fields, based on the callback ("enter" or "stay") and the 3D contact.
    /// </summary>
    public void Set( bool firstContact , ContactPoint contact )
    {
        this.firstContact = firstContact;
        this.collider3D = contact.otherCollider;            
        this.point = contact.point;
        this.normal = contact.normal;            
        this.gameObject = this.collider3D.gameObject;

        Rigidbody contactRigidbody = this.collider3D.attachedRigidbody;

        if( this.isRigidbody = contactRigidbody != null )
        {
            this.isKinematicRigidbody = contactRigidbody.isKinematic;
            this.pointVelocity = contactRigidbody.GetPointVelocity( this.point );  
        }  
    }

    /// <summary>
    /// Sets all the structs fields, based on the callback ("enter" or "stay") and the 2D contact.
    /// </summary>
    public void Set( bool firstContact , ContactPoint2D contact )
    {

        this.firstContact = firstContact;
        this.collider2D = contact.collider;            
        this.point = contact.point;
        this.normal = contact.normal;            
        this.gameObject = this.collider2D.gameObject;

        Rigidbody2D contactRigidbody = this.collider2D.attachedRigidbody;

        if( this.isRigidbody = contactRigidbody != null )
        {
            this.isKinematicRigidbody = contactRigidbody.isKinematic;
            this.pointVelocity = contactRigidbody.GetPointVelocity( this.point );  
        }  
    }


}

}

