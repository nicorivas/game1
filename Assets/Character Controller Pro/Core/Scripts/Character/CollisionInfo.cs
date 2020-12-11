using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{


/// <summary>
/// Struct that contains all the gathered information from a collision test.
/// </summary>
public struct CollisionInfo
{
    public HitInfo hitInfo;

    /// <summary>
    /// Flag that indicates if the collision test was successful or not.
    /// </summary>
    public bool collision;

    // /// <summary>
    // /// Casting distance used by the collision test.
    // /// </summary>
    // public float castDistance;

    // /// <summary>
    // /// Casting direction used by the collision test.
    // /// </summary>
    // public Vector3 castDirection;

    /// <summary>
    /// Available displacement obtained as the result of the collision test. By adding this vector to the character position, the result will represent the closest possible position to the hit surface.
    /// </summary>
    public Vector3 displacement;

    // /// <summary>
    // /// The contact point obtained from the hit result of the collision test.
    // /// </summary>
    // public Vector3 contactPoint;

    // /// <summary>
    // /// The normal obtained from the hit result of the collision test.
    // /// </summary>
    // public Vector3 contactNormal; 

    /// <summary>
    /// The angle between the contact normal and the character up vector.
    /// </summary>
    public float contactSlopeAngle;

    // /// <summary>
    // /// The Transform component of the hit collider.
    // /// </summary>
    // public Transform otherTransform;

    // /// <summary>
    // /// The GameObject component of the hit collider.
    // /// </summary>
    // public GameObject otherObject;

    /// <summary>
    /// Flag that indicates if the character is standing on an edge or not.
    /// </summary>
    public bool isAnEdge;

    /// <summary>
    /// Flag that indicates if the character is standing on an step or not.
    /// </summary>
    public bool isAStep;

    /// <summary>
    /// Normal vector obtained from the edge detector upper ray.
    /// </summary>
	public Vector3 edgeUpperNormal;

    /// <summary>
    /// Normal vector obtained from the edge detector lower ray.
    /// </summary>
	public Vector3 edgeLowerNormal;

    /// <summary>
    /// Angle between the character up vector and the edge detector upper normal.
    /// </summary>
    public float edgeUpperSlopeAngle;

    /// <summary>
    /// Angle between the character up vector and the edge detector lower normal.
    /// </summary>
	public float edgeLowerSlopeAngle;	

    /// <summary>
    /// Angle between the edge detector upper normal and the edge detector lower normal.
    /// </summary>
    public float edgeAngle;
    
    // /// <summary>
    // /// The Collider component of the hit object.
    // /// </summary>
	// public Collider collider3D;

    // /// <summary>
    // /// The Collider2D component of the hit object.
    // /// </summary>
    // public Collider2D collider2D;

    // /// <summary>
    // /// The Rigidbody component of the hit object.
    // /// </summary>
    // public Rigidbody rigidbody3D;

    // /// <summary>
    // /// The Rigidbody2D component of the hit object.
    // /// </summary>
    // public Rigidbody2D rigidbody2D;
	
}



}