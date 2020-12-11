using UnityEngine;

namespace Lightbug.CharacterControllerPro.Core
{

/// <summary>
/// This struct is a contains all the info related to the current dynamic ground.
/// </summary>
public struct DynamicGroundInfo
{
	Transform transform;

	KinematicPlatform kinematicPlatform;

	public Vector3 previousPosition;
	public Quaternion previousRotation;
	
	/// <summary>
	/// Resets the all the fields to default.
	/// </summary>
	public void Reset()
	{
		transform = null;
		
		kinematicPlatform = null;

		previousPosition = Vector3.zero;		
		previousRotation = Quaternion.identity;
	}

	/// <summary>
	/// Gets the Transfrom of the current dynamic ground.
	/// </summary>
	public Transform Transform
	{
		get
		{
			return transform;
		}
	}

	/// <summary>
	/// Gets if the character is currently standing on a dynamic ground or not.
	/// </summary>
	public bool IsActive
	{
		get
		{
			return transform != null;
		}
	}

	/// <summary>
	/// Gets the dynamic ground Rigidbody position.
	/// </summary>
	public Vector3 RigidbodyPosition
	{
		get
		{
			Vector3 position = default( Vector3 );

			if( kinematicPlatform != null )			
				position = kinematicPlatform.RigidbodyComponent.Position;
			
			return position;
		}
	}
	
	/// <summary>
	/// Gets the dynamic ground Rigidbody rotation.
	/// </summary>
	public Quaternion RigidbodyRotation
	{
		get
		{
			Quaternion rotation = default( Quaternion );

			if( kinematicPlatform != null )		
				rotation = kinematicPlatform.RigidbodyComponent.Rotation;
			
			return rotation;
		}
	}

	/// <summary>
	/// Gets the dynamic ground Rigidbody velocity.
	/// </summary>
	public Vector3 RigidbodyVelocity
	{
		get
		{
			Vector3 velocity = Vector3.zero;

			if( kinematicPlatform != null )		
				velocity = kinematicPlatform.RigidbodyComponent.Velocity;
			
			return velocity;
		}
	}

	public Vector3 GetPointVelocity( Vector3 footPosition )
	{
		Vector3 velocity = Vector3.zero;

		if( kinematicPlatform != null )		
			velocity = kinematicPlatform.RigidbodyComponent.GetPointVelocity( footPosition );
		
		return velocity;
	}

	/// <summary>
	/// Updates the dynamic ground info.
	/// </summary>
	public void UpdateTarget( KinematicPlatform kinematicPlatform , Vector3 characterPosition )
	{
		this.kinematicPlatform = kinematicPlatform;

		if( this.kinematicPlatform == null )
		{
			Reset();
			return;
		}

		this.transform = kinematicPlatform.transform;

		previousPosition = kinematicPlatform.RigidbodyComponent.Position;
		previousRotation = kinematicPlatform.RigidbodyComponent.Rotation;
	}

	

}

}