using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

/// <summary>
/// This struct contains all the character info related to collision, that is, collision flags and external components. All the internal fields are updated frame by frame, and can 
/// can be accessed by using public properties from the CharacterActor component.
/// </summary>
public struct CharacterCollisionInfo
{	
	public Vector3 groundContactPoint;
	public Vector3 groundContactNormal;
	public Vector3 groundStableNormal;
	public float stableSlopeAngle;

	// Edge
	public bool isOnEdge;	
	public float edgeAngle;

	// Wall
	public bool wallCollision;
	public Vector3 wallContactPoint;
	public Vector3 wallContactNormal;
	public float wallAngle;
	public GameObject wallObject;

	public Collider wallCollider3D;
    public Collider2D wallCollider2D;
    public Rigidbody wallRigidbody3D;
    public Rigidbody2D wallRigidbody2D;	
	
	// GameObject
	public GameObject groundObject;
	public int groundLayer;

	public Collider groundCollider3D;
    public Collider2D groundCollider2D;
	public Rigidbody groundRigidbody3D;
    public Rigidbody2D groundRigidbody2D;	
	

	/// <summary>
	/// Resets all the fields to default.
	/// </summary>
	public void Reset()
	{							
		ResetGroundInfo();
		ResetWallInfo();
	}
	
	/// <summary>
	/// Resets the ground related info.
	/// </summary>
	public void ResetGroundInfo()
	{

		groundContactPoint = Vector3.zero;
		groundContactNormal = Vector3.zero;		
		groundStableNormal = Vector3.zero;

		stableSlopeAngle = 0f;

		isOnEdge = false;
		edgeAngle = 0f;


		groundObject = null;
		groundLayer = 0;
		groundCollider3D = null;
        groundCollider2D = null;
	}

	/// <summary>
	/// Resets the wall related info.
	/// </summary>
	public void ResetWallInfo()
	{
		wallObject = null;
		wallCollision = false;
		wallContactPoint = Vector3.zero;
		wallContactNormal = Vector3.zero;
		wallAngle = 0f;
		wallCollider3D = null;
    	wallCollider2D = null;
    	wallRigidbody3D = null;
    	wallRigidbody2D = null;
	}
	

	
	
}


}
