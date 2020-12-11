using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// This component is a wrapper for the Rigidbody and Rigidbody2D components, containing not only the most commonly used 
/// properties and methods, but also some extra features.
/// </summary>
public abstract class RigidbodyComponent : MonoBehaviour
{	
	// WIP
	// [Tooltip("Enable this toggle if the rigidbody is controlled by an animation clip.")]
	// [SerializeField]
	// bool animatedRigidbody = false;

	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	public abstract float Mass{ get; set; }

	public abstract float Drag{ get; set; }

	public abstract bool IsKinematic{ get; set; }

	public abstract bool UseGravity{ get; set; }

	public abstract bool UseInterpolation{ get; set; }

	public abstract bool ContinuousCollisionDetection{ get; set; }

	public abstract RigidbodyConstraints Constraints{ get; set; }

	 /// <summary>
    /// Gets the current up direction based on the rigidbody rotation (not necessarily transform.up).
    /// </summary>
	public Vector3 Up
	{
		get
		{
			return Rotation * Vector3.up;
		}
	}

	/// <summary>
    /// Gets the current forward direction based on the rigidbody rotation (not necessarily transform.forward).
    /// </summary>
	public Vector3 Forward
	{
		get
		{
			return Rotation * Vector3.forward;
		}
	}

	/// <summary>
    /// Gets the current up direction based on the rigidbody rotation (not necessarily transform.right)
    /// </summary>
	public Vector3 Right
	{
		get
		{
			return Rotation * Vector3.right;
		}
	}

	/// <summary>
    /// Gets the rigidbody position.
    /// </summary>
    public abstract Vector3 Position{ get; set; }	

    /// <summary>
    /// Gets the rigidbody rotation.
    /// </summary>
	public abstract Quaternion Rotation{ get; set; }

	/// <summary>
	/// Gets the rigidbody linear velocity.
	/// </summary>
	public abstract Vector3 Velocity{ get; set; }

	/// <summary>
	/// Gets the rigidbody angular velocity.
	/// </summary>
	public abstract Vector3 AngularVelocity{ get; set; }
	
	/// <summary>
	/// Gets the desired position. This value is being updated after either an Interpolate call or a Velocity change.
	/// </summary>
	public Vector3 DesiredPosition{ get; protected set; }

	/// <summary>
	/// Gets the desired rotation. This value is being updated after either an Interpolate call or a AngularVelocity change.
	/// </summary>
	public Quaternion DesiredRotation{ get; protected set; }

    /// <summary>
    /// Sets the rigidbody position and rotation.
    /// </summary>
    public void SetPositionAndRotation( Vector3 position , Quaternion rotation )
	{
		Position = position;
		Rotation = rotation;
	}  

	/// <summary>
	/// Interpolates the rigidbody position (equivalent to MovePosition).
	/// </summary>
    public abstract void Interpolate( Vector3 position );

	/// <summary>
	/// Interpolates the rigidbody rotation (equivalent to MoveRotation).
	/// </summary>
	public abstract void Interpolate( Quaternion rotation );

	/// <summary>
	/// Interpolates the rigidbody position and rotation (equivalent to MovePosition and MoveRotation).
	/// </summary>
	/// <param name="position">the target position</param>
	/// <param name="rotation">the target rotation</param>
	public void Interpolate( Vector3 position, Quaternion rotation )
	{
		Interpolate( position );
        Interpolate( rotation );
	}

	/// <summary>
	/// Automatically moves the rigidbody based on its type. If the rigidbody is kinematic MovePosition will be used. 
	/// If the rigidbody is dynamic the velocity will be set.
	/// </summary>
	/// <param name="position">the target position</param>
	public void Move( Vector3 position )
    {
        if( IsKinematic )
            Interpolate( position );
        else     
            Velocity = ( position - Position ) / Time.deltaTime;
    }

	/// <summary>
	/// Automatically rotates the rigidbody based on its type. If the rigidbody is kinematic MoveRotation will be used. 
	/// If the rigidbody is dynamic the angular velocity will be set.
	/// </summary>
	/// <param name="rotation">the target rotation</param>
	public void Rotate( Quaternion rotation )
    {
        if( IsKinematic )
		{
            Interpolate( rotation );
		}
        else 
		{
			Vector3 angularDisplacement = Mathf.Deg2Rad * ( rotation * Quaternion.Inverse( Rotation ) ).eulerAngles;
            AngularVelocity = angularDisplacement / Time.deltaTime;
		}
      
    }

	/// <summary>
	/// Uses both Move and Rotate method.
	/// </summary>
	public void MoveAndRotate( Vector3 position , Quaternion rotation )
	{
		Move( position );
		Rotate( rotation );
	}

    /// <summary>
    /// Gets the rigidbody velocity at a specific point in space.
    /// </summary>
    public abstract Vector3 GetPointVelocity( Vector3 point );

	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

	public abstract void AddForceToRigidbody( Vector3 force , ForceMode forceMode = ForceMode.Force );
	public abstract void AddExplosionForceToRigidbody( float explosionForce , Vector3 explosionPosition , float explosionRadius , float upwardsModifier = 0f );

	// Velocity-based methods ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	
	/// <summary>
	/// Adds a velocity vector to the rigidbody (simulating AddForce) calculated from a force value.
	/// </summary>
	public void AddForce( Vector3 force , bool ignoreMass = false )
	{			
		Vector3 acceleration = force / ( ignoreMass ? 1f : Mass );
		Velocity += acceleration * Time.fixedDeltaTime;
	}

	/// <summary>
	/// Adds a velocity vector to the rigidbody (simulating AddExplosionForce) calculated from a force value.
	/// </summary>
	public void AddExplosionForce( float explosionForce , Vector3 explosionPosition , float explosionRadius , float upwardsModifier = 0f )
	{		
		Vector3 displacementToTarget = Position - explosionPosition;
		float displacementToTargetMagnitude = displacementToTarget.magnitude;

		if( displacementToTargetMagnitude > explosionRadius )
			return;
		
		// Upwards modifier
		explosionPosition -= Vector3.up * upwardsModifier;
		displacementToTarget = Position - explosionPosition;

		// Magnitude based on the radius (linear).
		float forceMagnitude = explosionForce * ( ( explosionRadius - displacementToTargetMagnitude ) / explosionRadius );
		
		Vector3 force = displacementToTarget.normalized * forceMagnitude;
		Vector3 velocity = force / Mass;

		Velocity += velocity;
	}

	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

	protected virtual void Awake()
	{
		this.hideFlags = HideFlags.HideInInspector;

		DesiredPosition = transform.position;
		DesiredRotation = transform.rotation;
	}

	
	// WIP
	// void FixedUpdate()
	// {
	// 	// If the body is kinematic then MovePosition/Rotation will set the desiredPosition and deriredRotation. Otherwise,
	// 	// it's required to update these values frame by frame.
	// 	if( !IsKinematic || animatedRigidbody )		
	// 	{
	// 		DesiredPosition = Position + Velocity * Time.fixedDeltaTime;
	// 		DesiredRotation = Rotation * Quaternion.Euler( AngularVelocity );
	// 	}
	// }

	
	
}

}
