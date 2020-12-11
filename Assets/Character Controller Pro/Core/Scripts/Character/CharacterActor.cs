using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

public enum CharacterActorState
{
	NotGrounded ,
	StableGrounded ,
	UnstableGrounded
}


/// <summary>
/// This class represents a character actor. It contains all the character information, collision flags, collision events, and so on. It also responsible for the execution order 
/// of everything related to the character, such as movement, rotation, teleportation, rigidbodies interactions, body size, etc. Since the character can be 2D or 3D, this abstract class must be implemented in the 
/// two formats, one for 2D and one for 3D.
/// </summary>
[AddComponentMenu("Character Controller Pro/Core/Character Actor")]
[RequireComponent(typeof( CharacterBody ))]
public class CharacterActor : MonoBehaviour
{  
	

	[Header("Tags & Layers")]

	[Tooltip("Important, assign this scriptable object in order to define the layers to work with.")]
	[SerializeField]
	protected CharacterTagsAndLayersProfile tagsAndLayersProfile = null;
	
	[Header("Slopes")]

	[Tooltip("The slope limit set the maximum angle considered as stable.")]
	[SerializeField]
	[Range(1f, 85f)]
	protected float slopeLimit = 60f;


	[Header("Grounding")]

	[Tooltip("The offset distance applied to the bottom of the character. A higher offset means more walkable surfaces")]
	[Min( 0f )]
	[SerializeField]
	protected float stepOffset = 0.5f;

	[Tooltip("How fast the step up action is going to be. A value of 1 equals 1 frame (instantly), any value below 1 will produce a smooth transition.")]
	[SerializeField]
	[Range( 0.1f , 1f )]
	float stepUpSpeed = 0.5f;

	[Space(10f)]

	[Tooltip("The distance the character is capable of detecting ground. Use this value to clamp (or not) the character to the ground.")]
	[SerializeField]
	[Min( 0f )]
	protected float stepDownDistance = 0.5f;

	[Tooltip("How fast the step down action is going to be. A value of 1 equals 1 frame (instantly), any value below 1 will produce a smooth transition.")]
	[SerializeField]
	[Range( 0.1f , 1f )]
	float stepDownSpeed = 0.1f;

	[Space(10f)]

	[Tooltip("This toggle will enable the \"edge compensation\" feature, simulating a box collider when the character is standing on a edge." + 
	"TIP: Use it for 2D platformers if you want to imitate a box collider for the bottom.")]
	[SerializeField]
	bool edgeCompensation = false;

	[Space(10f)]


	[Tooltip("Enable this flag if you want to completely ignore the grounded state.")]
	[SerializeField]
	bool alwaysNotGrounded = false;

	[Tooltip("If enabled the character will do an initial ground check (at \"Start\"). If the test fails the starting state will be \"Not grounded\".")]
	[SerializeField]
	bool forceGroundedAtStart = true;
	
	
	[Header("Vertical Alignment")]
	[SerializeField]
	VerticalAlignmentSettings verticalAlignmentSettings = new VerticalAlignmentSettings();
	

	[Header("Dynamic Ground")]

	[Tooltip("Should the character be affected by the movement of the ground (only for kinematic rigidbodies).")]
	[SerializeField]
	protected bool supportDynamicGround = true;
	
	[Tooltip("If this toggle is enabled the forward direction will be affected by the rotation of the dynamic ground (only yaw motion allowed).")]
	[SerializeField]
	bool rotateForwardDirection = true;

	[Header("Weight")]

	[SerializeField]
	bool applyWeightToGround = true;

	[SerializeField]
	float weightGravity = CharacterConstants.DefaultGravity;

	[SerializeField]
	bool filterWeightRigidbodiesByTag = true;

	[Header("Size")]

	[Min(0f)]
	[SerializeField]
	float sizeLerpSpeed = 8f;

	[Header("External Velocity")]

	[Tooltip("Should the velocities produced by the physics simulation affect the character velocity?")]
	[SerializeField]
	bool addExternalVelocity = true;

	[Tooltip("Enable this if you want to prevent static colliders (non-rigidbodies) from adding external velocity to your character.")]
	[SerializeField]
	bool ignoreStaticObstacles = true;

	[Tooltip("Enable this if you want to prevent kinematic rigidbodies from adding external velocity to your character.")]
	[SerializeField]
	bool ignoreKinematicRigidbodies = true;
	

	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	
	
	
	CharacterBody characterBody = null;	

	/// <summary>
	/// Gets the CharacterBody component associated with this character actor.
	/// </summary>
	public CharacterBody CharacterBody
	{
		get
		{
			if( characterBody == null)
				characterBody = GetComponent<CharacterBody>();
			
			return characterBody;
		}
	}

	CharacterActorBehaviour characterActorBehaviour = null;	

	/// <summary>
	/// Gets the CharacterActorBehaviour component associated with this character actor.
	/// </summary>
	public CharacterActorBehaviour CharacterActorBehaviour
	{
		get
		{	
			return characterActorBehaviour;
		}
	}

	PhysicsComponent physicsComponent = null;

	/// <summary>
	/// Gets the physics component from the character.
	/// </summary>
	public PhysicsComponent PhysicsComponent
	{
		get
		{
			return physicsComponent;
		}
	}

	

	/// <summary>
	/// Returns the current character actor state. This enum variable contains the information about the grounded and stable state, all in one.
	/// </summary>
	public CharacterActorState CurrentState
	{
		get
		{
			if( IsGrounded )
				return IsStable ? CharacterActorState.StableGrounded : CharacterActorState.UnstableGrounded;
			else			
				return CharacterActorState.NotGrounded;
		}
	}

	/// <summary>
	/// Returns the character actor state from the previous frame.
	/// </summary>
	public CharacterActorState PreviousState
	{
		get
		{
			if( WasGrounded )
				return WasStable ? CharacterActorState.StableGrounded : CharacterActorState.UnstableGrounded;
			else			
				return CharacterActorState.NotGrounded;
		}
	}

	#region Collision Properties
	
	/// <summary>
	/// Returns true if the character is standing on an edge.
	/// </summary>
	public bool IsOnEdge
	{
		get
		{
			return characterCollisionInfo.isOnEdge;
		}
	}

	/// <summary>
	/// Gets the grounded state, true if the ground object is not null, false otherwise.
	/// </summary>
	public bool IsGrounded
	{
		get
		{
			return characterCollisionInfo.groundObject != null;
		}
	}

	
	/// <summary>
	/// Gets the angle between the up vector and the stable normal.
	/// </summary>
	public float GroundSlopeAngle
	{
		get
		{
			return characterCollisionInfo.stableSlopeAngle;
		}
	}

	/// <summary>
	/// Gets the contact point obtained directly from the ground test (sphere cast).
	/// </summary>
	public Vector3 GroundContactPoint
	{ 
		get
		{ 
			return characterCollisionInfo.groundContactPoint; 
		} 
	}

	/// <summary>
	/// Gets the normal vector obtained directly from the ground test (sphere cast).
	/// </summary>
	public Vector3 GroundContactNormal
	{ 
		get
		{ 
			return characterCollisionInfo.groundContactNormal; 
		} 
	}

	/// <summary>
	/// Gets the normal vector used to determine stability. This may or may not be the normal obtained from the ground test.
	/// </summary>
	public Vector3 GroundStableNormal
	{ 
		get
		{ 
			return characterCollisionInfo.groundStableNormal; 
		} 
	}

	/// <summary>
	/// Gets the GameObject component of the current ground.
	/// </summary>
	public GameObject GroundObject 
	{ 
		get
		{ 
			return characterCollisionInfo.groundObject; 
		} 
	}

	/// <summary>
	/// Gets the Transform component of the current ground.
	/// </summary>
	public Transform GroundTransform
	{ 
		get
		{ 
			return characterCollisionInfo.groundObject.transform; 
		} 
	}
	
	/// <summary>
	/// Gets the Collider2D component of the current ground.
	/// </summary>
	public Collider2D GroundCollider2D
	{ 
		get
		{ 
			return characterCollisionInfo.groundCollider2D; 
		} 
	}

	/// <summary>
	/// Gets the Collider3D component of the current ground.
	/// </summary>
	public Collider GroundCollider3D
	{ 
		get
		{ 
			return characterCollisionInfo.groundCollider3D; 
		} 
	}

	

	

	/// <summary>
	/// Gets the wall collision flag, true if the character hit a wall, false otherwise.
	/// </summary>
	public bool WallCollision
	{ 
		get
		{ 
			return characterCollisionInfo.wallCollision; 
		} 
	}

	/// <summary>
	/// Gets the angle of the wall. A 90 degrees wall will return a wall angle of 0 degrees.
	/// </summary>
	public float WallAngle
	{ 
		get
		{ 
			return characterCollisionInfo.wallAngle; 
		} 
	}

	/// <summary>
	/// Gets the current collided wall GameObject.
	/// </summary>
	public GameObject WallObject
	{ 
		get
		{ 
			return characterCollisionInfo.wallObject; 
		} 
	}

	/// <summary>
	/// Gets the wall contact point from the collision info.
	/// </summary>
	public Vector3 WallContactPoint
	{
		get
		{
			return characterCollisionInfo.wallContactPoint;
		}
	}

	/// <summary>
	/// Gets the wall contact normal from the collision info.
	/// </summary>
	public Vector3 WallContactNormal
	{
		get
		{
			return characterCollisionInfo.wallContactNormal;
		}
	}

	
	/// <summary>
	/// Gets the current stability state of the character. Stability is equal to "grounded + slope angle <= slope limit".
	/// </summary>
	public bool IsStable
	{
		get
		{
			return IsGrounded && characterCollisionInfo.stableSlopeAngle <= slopeLimit;
		}
	}

	/// <summary>
	/// Returns true if the character is grounded onto an unstable ground, false otherwise.
	/// </summary>
	public bool IsOnUnstableGround
	{
		get
		{
			return IsGrounded && characterCollisionInfo.stableSlopeAngle > slopeLimit;
		}
	}

	bool wasGrounded = false;
	
	/// <summary>
	/// Gets the previous grounded state.
	/// </summary>
    public bool WasGrounded
    {
        get
        {
            return wasGrounded;
        }
    }
	
	
    bool wasStable = false;

	/// <summary>
	/// Gets the previous stability state.
	/// </summary>
    public bool WasStable
    {
        get
        {
            return wasStable;
        }
    }

	

	/// <summary>
	/// Returns true if the collided wall is a Rigidbody (2D or 3D), false otherwise.
	/// </summary>
	public bool IsWallARigidbody
	{
		get
		{
			return characterBody.Is2D ? characterCollisionInfo.wallRigidbody2D != null : characterCollisionInfo.wallRigidbody3D != null;
		}
	}

	/// <summary>
	/// Returns true if the collided wall is a kinematic Rigidbody (2D or 3D), false otherwise.
	/// </summary>
	public bool IsWallAKinematicRigidbody
	{
		get
		{
			return characterBody.Is2D ? characterCollisionInfo.wallRigidbody2D.isKinematic : characterCollisionInfo.wallRigidbody3D.isKinematic;
		}
	}

	/// <summary>
	/// Returns true if the current ground is a Rigidbody (2D or 3D), false otherwise.
	/// </summary>
	public bool IsGroundARigidbody
	{
		get
		{
			return characterBody.Is2D ? characterCollisionInfo.groundRigidbody2D != null : characterCollisionInfo.groundRigidbody3D != null;
		}
	}

	/// <summary>
	/// Returns true if the current ground is a kinematic Rigidbody (2D or 3D), false otherwise.
	/// </summary>
	public bool IsGroundAKinematicRigidbody
	{
		get
		{
			return characterBody.Is2D ? characterCollisionInfo.groundRigidbody2D.isKinematic : characterCollisionInfo.groundRigidbody3D.isKinematic;
		}
	}

	/// <summary>
	/// Returns true if the current ground is a kinematic Rigidbody (2D or 3D), false otherwise.
	/// </summary>
	public Vector3 DynamicGroundPointVelocity
	{
		get
		{			
			if( !dynamicGroundInfo.IsActive )
				return Vector3.zero;
			
			return dynamicGroundInfo.GetPointVelocity( Position );			
			
			//return characterBody.Is2D ? (Vector3)characterCollisionInfo.groundRigidbody2D.velocity : characterCollisionInfo.groundRigidbody3D.velocity;
		}
	}
	
	/// <summary>
	/// Gets a concatenated string containing all the current collision information.
	/// </summary>
	public override string ToString()
	{
		const string nullString = " ---- ";
		

		string triggerString = "";

		for( int i = 0 ; i < Triggers.Count ; i++ )
		{
			triggerString += " - " + Triggers[i].gameObject.name + "\n";
		}

		return string.Concat( 
			"Ground : \n" ,
			"──────────────────\n" ,
			"Is Grounded : " , IsGrounded , '\n' ,
			"Stable Ground : " , IsStable , '\n' ,
			"Slope Angle : " , characterCollisionInfo.stableSlopeAngle , '\n' ,
			"On Edge : " , characterCollisionInfo.isOnEdge , '\n' ,
			"Edge Angle : " , characterCollisionInfo.edgeAngle , '\n' ,
			"Contact Point : " , characterCollisionInfo.groundContactPoint , '\n' ,
			"Contact Normal : " , characterCollisionInfo.groundContactNormal , '\n' ,
			"Object Name : " , characterCollisionInfo.groundObject != null ? characterCollisionInfo.groundObject.name : nullString , '\n' ,
			"Layer : " , LayerMask.LayerToName( characterCollisionInfo.groundLayer ) , '\n' , 	
			"Is a Rigidbody : " , IsGrounded ? IsGroundARigidbody.ToString() : nullString , '\n' ,
			"Is a Kinematic Rigidbody : " , IsGroundARigidbody ? IsGroundAKinematicRigidbody.ToString() : nullString , '\n' ,
			"Dynamic Ground : " , dynamicGroundInfo.IsActive ? dynamicGroundInfo.Transform.name : nullString , "\n\n" ,			
			"Walls : \n" ,
			"──────────────────\n" ,			
			"Wall Collision : " , characterCollisionInfo.wallCollision , '\n' ,	
			"Wall Angle : " , characterCollisionInfo.wallAngle , '\n' ,
			"Is a Rigidbody : " , IsGrounded ? IsWallARigidbody.ToString() : nullString , '\n' ,
			"Is a Kinematic Rigidbody : " , IsWallARigidbody ? IsWallAKinematicRigidbody.ToString() : nullString , '\n' ,
			"──────────────────\n" ,
			"Triggers : \n" ,
			"Current : " , CurrentTrigger.gameObject != null ? CurrentTrigger.gameObject.name : nullString , '\n' ,		
			triggerString	
		);
	}

#endregion

	protected CharacterCollisionInfo characterCollisionInfo = new CharacterCollisionInfo();

	protected DynamicGroundInfo dynamicGroundInfo = new DynamicGroundInfo();

	Dictionary< Transform , KinematicPlatform > kinematicPlatforms = new Dictionary< Transform , KinematicPlatform >();


	public float GroundedTime{ get; private set; }    
	public float NotGroundedTime{ get; private set; }
    



	/// <summary>
	/// Gets the alwaysNotGrounded flag.
	/// </summary>
	public bool AlwaysNotGrounded
    {
        get
        {
            return alwaysNotGrounded;
        }
		set
        {
            alwaysNotGrounded = value;
        }
    }

	/// <summary>
	/// Gets the initial body size.
	/// </summary>
	public Vector2 DefaultBodySize
	{
		get
		{
			return characterBody.BodySize;
		}
	}

	/// <summary>
	/// Gets the current body size.
	/// </summary>
	public Vector2 BodySize{ get; private set; }

	/// <summary>
	/// Gets the LayerMask with all the considered static obstacles.
	/// </summary>
	public LayerMask StaticObstaclesLayerMask
	{
		get
		{
			return tagsAndLayersProfile.staticObstaclesLayerMask;
		}
	}

	/// <summary>
	/// Gets the LayerMask with all the considered dynamic rigidbodies.
	/// </summary>
	public LayerMask DynamicRigidbodiesLayerMask
	{
		get
		{
			return tagsAndLayersProfile.dynamicRigidbodiesLayerMask;
		}
	}

	/// <summary>
	/// Gets the LayerMask with all the considered dynamic obstacles.
	/// </summary>
	public LayerMask DynamicGroundLayerMask
	{
		get
		{
			return tagsAndLayersProfile.dynamicGroundLayerMask;
		}
	}

	/// <summary>
	/// Gets the tags and layers profile asset used by the character actor.
	/// </summary>
	public CharacterTagsAndLayersProfile TagsAndLayersProfile
	{
		get
		{
			return tagsAndLayersProfile;
		}
	}    	 


	/// <summary>
	/// Gets the rigidbody velocity.
	/// </summary>
	public Vector3 Velocity
	{
		get
		{
			return RigidbodyComponent.Velocity;
		}
		set
		{
			RigidbodyComponent.Velocity = value;
		}
	}

	/// <summary>
	/// Gets the rigidbody velocity projected onto a plane formed by its up direction.
	/// </summary>
	public Vector3 PlanarVelocity
	{
		get
		{
			return Vector3.ProjectOnPlane( Velocity , Up );
		}
		set
		{
			Velocity = Vector3.ProjectOnPlane( value , Up ) + VerticalVelocity;
		}
	}

	/// <summary>
	/// Gets the rigidbody velocity projected onto its up direction.
	/// </summary>
	public Vector3 VerticalVelocity
	{
		get
		{
			return Vector3.Project( Velocity , Up );
		}
		set
		{
			Velocity = PlanarVelocity + Vector3.Project( value , Up );
		}
	}

	public Vector3 LastGroundedVelocity{ get; private set; }
	public Vector3 LastNotGroundedVelocity{ get; private set; }
	
	
	/// <summary>
	/// Gets the rigidbody local velocity.
	/// </summary>
	public Vector3 LocalVelocity
	{
		get
		{
			return transform.InverseTransformVectorUnscaled( Velocity );
		}
		set
		{
			Velocity = transform.TransformVectorUnscaled( value );
		}
	}


	/// <summary>
	/// Returns true if the character local vertical velocity is less than zero. 
	/// </summary>
	public bool IsFalling
	{
		get
		{
			return LocalVelocity.y < 0f;
		}
	}

	/// <summary>
	/// Returns true if the character local vertical velocity is greater than zero.
	/// </summary>
	public bool IsAscending
	{
		get
		{
			return LocalVelocity.y > 0f;
		}
	}
	


	#region public Body properties

	/// <summary>
	/// Gets the center of the collision shape.
	/// </summary>
	public Vector3 Center
	{
		get
		{
			return GetCenter( Position );
		}
	}

	/// <summary>
	/// Gets the center of the collision shape.
	/// </summary>
	public Vector3 Top
	{
		get
		{
			return GetTop( Position );
		}
	}

	/// <summary>
	/// Gets the center of the collision shape.
	/// </summary>
	public Vector3 Bottom
	{
		get
		{
			return GetBottom( Position );
		}
	}

	/// <summary>
	/// Gets the center of the collision shape.
	/// </summary>
	public Vector3 TopCenter
	{
		get
		{
			return GetTopCenter( Position );
		}
	}

	/// <summary>
	/// Gets the center of the collision shape.
	/// </summary>
	public Vector3 BottomCenter
	{
		get
		{
			return GetBottomCenter( Position );
		}
	}

	/// <summary>
	/// Gets the center of the collision shape.
	/// </summary>
	public Vector3 OffsettedBottomCenter
	{
		get
		{
			return GetOffsettedBottomCenter( Position );
		}
	}

#endregion

#region Internal Body functions

	/// <summary>
	/// Gets the center of the collision shape.
	/// </summary>
	protected Vector3 GetCenter( Vector3 position )
	{
		return position + Up * BodySize.y / 2f;
	}



	/// <summary>
	/// Gets the top most point of the collision shape.
	/// </summary>
	protected Vector3 GetTop( Vector3 position )
	{
		return position + Up * ( BodySize.y - CharacterConstants.SkinWidth );
	}

	/// <summary>
	/// Gets the bottom most point of the collision shape.
	/// </summary>
	protected Vector3 GetBottom( Vector3 position )
	{
		return position + Up * CharacterConstants.SkinWidth;
	}

	/// <summary>
	/// Gets the center of the top sphere of the collision shape.
	/// </summary>
	protected Vector3 GetTopCenter( Vector3 position )
	{
		return position + Up * ( BodySize.y - BodySize.x / 2f );
	}

	/// <summary>
	/// Gets the center of the top sphere of the collision shape (considering an arbitrary body size).
	/// </summary>
	protected Vector3 GetTopCenter( Vector3 position , Vector2 BodySize )
	{
		return position + Up * ( BodySize.y - BodySize.x / 2f );
	}

	/// <summary>
	/// Gets the center of the bottom sphere of the collision shape.
	/// </summary>
	protected Vector3 GetBottomCenter( Vector3 position )
	{
		return position + Up * BodySize.x / 2f;
	}

	/// <summary>
	/// Gets the center of the bottom sphere of the collision shape (considering an arbitrary body size).
	/// </summary>
	protected Vector3 GetBottomCenter( Vector3 position , Vector2 BodySize )
	{
		return position + Up * BodySize.x / 2f;
	}

	/// <summary>
	/// Gets the a vector that goes from the bottom center to the top center (topCenter - bottomCenter).
	/// </summary>
	protected Vector3 GetBottomCenterToTopCenter()
	{
		return Up * ( BodySize.y - BodySize.x );
	}

	/// <summary>
	/// Gets the a vector that goes from the bottom center to the top center (topCenter - bottomCenter).
	/// </summary>
	protected Vector3 GetBottomCenterToTopCenter( Vector2 BodySize )
	{
		return Up * ( BodySize.y - BodySize.x );
	}

	/// <summary>
	/// Gets the center of the bottom sphere of the collision shape, considering the collider bottom offset.
	/// </summary>
	protected Vector3 GetOffsettedBottomCenter( Vector3 position )
	{
		return position + Up * ( BodySize.x / 2f + stepOffset );
	}

#endregion
	
	RigidbodyConstraints initialRigidbodyConstraints;

	void Awake()
	{	
		characterBody = GetComponent<CharacterBody>();
		characterBody.Initialize();

		targetBodySize = characterBody.BodySize;

		if( tagsAndLayersProfile == null )
			Debug.Log("GameObject " + gameObject.name + " doesn't have a tags and layers profile assigned to it.");
		
		if( characterBody.Is2D )
		{
			physicsComponent = gameObject.AddComponent<PhysicsComponent2D>();			 
		}
		else
		{
			physicsComponent = gameObject.AddComponent<PhysicsComponent3D>(); 
		}
		
		BodySize = characterBody.BodySize;
				

		SetColliderSize();

		
		RigidbodyComponent.Mass = characterBody.Mass;
		RigidbodyComponent.IsKinematic = false;
		RigidbodyComponent.UseGravity = false;
		RigidbodyComponent.UseInterpolation = true;
		RigidbodyComponent.ContinuousCollisionDetection = true;
		RigidbodyComponent.Constraints = RigidbodyConstraints.FreezeRotation;

		initialRigidbodyConstraints = RigidbodyComponent.Constraints;

		
		if( SceneController.Instance == null )
		{	
			Debug.Log("Missing \"scene controller\" component, please add one (you can drag and drop a prefab called \"Scene Controller\" into the scene).");
		}
		
		StartCoroutine( LateFixedUpdate() );
		
	}
	

	protected virtual void OnEnable()
	{		
		// Add this actor to the scene controller list
		SceneController.Instance.AddActor( this );
	}

	protected virtual void OnDisable()
	{
		// Remove this actor from the scene controller list
		SceneController.Instance.RemoveActor( this );		
	}

	
	protected virtual void Start()
    {
		// Initialize the the character actor behaviour
		characterActorBehaviour = transform.GetComponentInChildren<CharacterActorBehaviour>();

		if( characterActorBehaviour == null )
			Debug.Log("GameObject " + gameObject.name + " doesn't have a CharacterActorBehaviour component associated with it.");
		else
			characterActorBehaviour.Initialize( this );

		if( forceGroundedAtStart && !alwaysNotGrounded )
			ForceGrounded();

    }

	
	/// <summary>
	/// Applies a force at the ground contact point, in the direction of the weight (mass times gravity).
	/// </summary>
	protected virtual void ApplyWeight( Vector3 contactPoint )
    {
		if( !applyWeightToGround )
			return;
		
		if( filterWeightRigidbodiesByTag && !GroundObject.CompareTag( tagsAndLayersProfile.weightRigidbodiesTag ) )
            return;
		
		if( characterBody.Is2D )
		{
			if( GroundCollider2D == null )
            	return;

			if( GroundCollider2D.attachedRigidbody == null )
				return;

        
        	GroundCollider2D.attachedRigidbody.AddForceAtPosition( - transform.up * characterBody.Mass * weightGravity , contactPoint );
		}
		else
		{
			if( GroundCollider3D == null )
            	return;

			if( GroundCollider3D.attachedRigidbody == null )
				return;

        
        	GroundCollider3D.attachedRigidbody.AddForceAtPosition( - transform.up * characterBody.Mass * weightGravity , contactPoint );
		}

        
    }

	/// <summary>
	/// Performs the movement and rotation based on the current and previous dynamic ground info.
	/// </summary>
    protected virtual Vector3 ProcessDynamicGround( ref Vector3 position , float dt )
    {
        if( !dynamicGroundInfo.IsActive )
			return Vector3.zero;

		Vector3 initialPosition = position;
			
		Quaternion deltaRotation = dynamicGroundInfo.RigidbodyRotation * Quaternion.Inverse( dynamicGroundInfo.previousRotation );
		
		Vector3 centerToCharacter = position - dynamicGroundInfo.previousPosition;
		Vector3 rotatedCenterToCharacter = deltaRotation * centerToCharacter; 
		
		if( rotateForwardDirection )
		{
			Vector3 up = Up;
			Forward = deltaRotation * Forward;
			Up = up;
		}

		position = dynamicGroundInfo.RigidbodyPosition + rotatedCenterToCharacter;				
		
		return ( position - initialPosition ) / dt;
    }
	
	

	void FindAndUpdateDynamicGround( Transform groundTransform , Vector3 footPosition )
    {
        KinematicPlatform kinematicPlatform;
		bool found = kinematicPlatforms.TryGetValue( groundTransform , out kinematicPlatform );

		if( found )
		{
			dynamicGroundInfo.UpdateTarget( kinematicPlatform , footPosition );
		}
		else
		{
			kinematicPlatform = GroundObject.GetComponent<KinematicPlatform>();

			if( kinematicPlatform != null )
			{
				kinematicPlatforms.Add( groundTransform , kinematicPlatform );
				dynamicGroundInfo.UpdateTarget( kinematicPlatform , footPosition );
			}
			else
			{
				dynamicGroundInfo.Reset();
			}
			
		}
    }    

	/// <summary>
	/// Checks for any dynamic ground. If the result is positive it updates the dynamic ground info.
	/// </summary>
	protected virtual void UpdateDynamicGround( Vector3 position )
	{
		if( !IsGrounded || !CustomUtilities.BelongsToLayerMask(characterCollisionInfo.groundLayer , tagsAndLayersProfile.dynamicGroundLayerMask ))
		{
            dynamicGroundInfo.Reset();
			return;
		}

		if( !IsGroundARigidbody )
		{
			dynamicGroundInfo.Reset();
			return;	
		}
		else if( !IsGroundAKinematicRigidbody )
		{
			dynamicGroundInfo.Reset();
			return;	
		}
							
		FindAndUpdateDynamicGround( GroundTransform , position );
	
	}

	

	void SetColliderSize()
    {
        float verticalOffset = IsGrounded ? 
		( Mathf.Max( CharacterConstants.ColliderMinBottomOffset , stepOffset ) ) : 
		CharacterConstants.ColliderMinBottomOffset;

        float radius = BodySize.x / 2f;
		float height = BodySize.y - verticalOffset;

        ColliderComponent.Size = new Vector2( 2f * radius , height );
		ColliderComponent.Offset = Vector2.up * ( verticalOffset + height / 2f );
    }

	Vector2 targetBodySize;

	void HandleSize( float dt )
	{
		// if( !IsStable )
		// {
		// 	BodySize = characterBody.BodySize;
		// }
		// else
		// {
        	BodySize = Vector2.Lerp( BodySize , targetBodySize , sizeLerpSpeed * dt );
		// }

		SetColliderSize();
    }


	/// <summary>
    /// Gets the current rigidbody position.
    /// </summary>
	public Vector3 Position
	{
		get
		{
			return RigidbodyComponent.Position;
		}
		set
		{
			RigidbodyComponent.Position = value;
		}
	}

	/// <summary>
    /// Gets the current rigidbody rotation.
    /// </summary>
	public Quaternion Rotation
	{
		get
		{
			return RigidbodyComponent.Rotation;
		}
		set
		{	
			RigidbodyComponent.Rotation = value;			
		}
	}
	
    /// <summary>
    /// Gets the current up direction based on the rigidbody rotation (not necessarily transform.up).
    /// </summary>
	public Vector3 Up
	{
		get
		{
			return RigidbodyComponent.Up;
		}
		set
		{			
			if( value == Vector3.zero )
				return;

			Quaternion deltaRotation = Quaternion.FromToRotation( Up , value.normalized );
			Rotation = deltaRotation * Rotation;
		}
	}

	Vector3 forward2D = Vector3.right;
	
	/// <summary>
    /// Gets the current forward direction based on the rigidbody rotation (not necessarily transform.forward).
    /// </summary>
	public Vector3 Forward
	{
		get
		{
			return characterBody.Is2D ? forward2D : Rotation * Vector3.forward;
		}
		set
		{			
			
			if( value == Vector3.zero )
				return;

			if( characterBody.Is2D )
			{
				forward2D = Vector3.Project( value , Right ).normalized;				
			}
			else
			{
				Quaternion deltaRotation = Quaternion.FromToRotation( Forward , value.normalized );
				Rotation = deltaRotation * Rotation;

			}	

			
		}
	}

	/// <summary>
    /// Gets the current up direction based on the rigidbody rotation (not necessarily transform.right).
    /// </summary>
	public Vector3 Right
	{
		get
		{
			return Rotation * Vector3.right;			
		}
	}

	/// <summary>
    /// Gets the RigidbodyComponent component associated with the character.
    /// </summary>
	public RigidbodyComponent RigidbodyComponent
	{
		get
		{
			return characterBody.RigidbodyComponent;
		}
	}

	/// <summary>
    /// Gets the ColliderComponent component associated with the character.
    /// </summary>
	public ColliderComponent ColliderComponent
	{
		get
		{
			return characterBody.ColliderComponent;
		}
	}

	// Contacts ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

	/// <summary>
    /// Gets a list with all the current contacts.
    /// </summary>
	public List<Contact> Contacts
	{
		get
		{
			if( physicsComponent == null)
				return null;
			
			return physicsComponent.Contacts;
		}
	}



	// Triggers ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

	/// <summary>
	/// Gets the most recent trigger.
	/// </summary>
	public Trigger CurrentTrigger
	{ 
		get
		{ 
			if( physicsComponent.Triggers.Count == 0 )
				return new Trigger();	// "Null trigger"
			
			return physicsComponent.Triggers[ physicsComponent.Triggers.Count - 1 ]; 
		} 
	}

	/// <summary>
	/// Gets a list with all the triggers.
	/// </summary>
	public List<Trigger> Triggers
	{ 
		get
		{			
			return physicsComponent.Triggers; 
		} 
	}
	
	

	public bool IsKinematic
	{
		get
		{
			return RigidbodyComponent.IsKinematic;
		}
		set
		{		
			// To avoid the warning	;)
			if( value )
			{
				RigidbodyComponent.ContinuousCollisionDetection = false;
				RigidbodyComponent.IsKinematic = true;
			}
			else
			{				
				RigidbodyComponent.IsKinematic = false;
				RigidbodyComponent.ContinuousCollisionDetection = true;
			}
			
		}
	}


	IEnumerator LateFixedUpdate()
	{
		YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();
		while( true )
		{
			yield return waitForFixedUpdate;	

			
			PostSimulationVelocity = Velocity;			
			ExternalVelocity = PostSimulationVelocity - preSimulationVelocity;

			
			if( addExternalVelocity )
			{
				// If nothing changes then apply the external velocity.
				bool ignoreExternalVelocity = false;

				if( ignoreStaticObstacles )
				{
					for( int i = 0 ; i < Contacts.Count ; i++ )
					{
						Contact contact = Contacts[i];	

						if( ( ignoreStaticObstacles && !contact.isRigidbody ) || ( ignoreKinematicRigidbodies && contact.isKinematicRigidbody ) )	
						{
							ignoreExternalVelocity = true;
							break;
						}

					}

				}
				

				Velocity = ignoreExternalVelocity ? inputVelocity : inputVelocity + ExternalVelocity;
			}
			else
			{	
				Velocity = inputVelocity;
			}


			
		}
	}

	
	public Vector3 PostSimulationVelocity{ get; private set; }

	/// <summary>
	/// Gets the difference between the post-simulation velocity (after the physics simulation) and the pre-simulation velocity (before the physics simulation). 
	/// This value is useful to detect any external response due to the physics simulation, such as hits from another rigidbody.
	/// </summary>
	public Vector3 ExternalVelocity{ get; private set; }

	
	
	Vector3 inputVelocity = default( Vector3 );
	Vector3 preSimulationVelocity = default( Vector3 );


	#region Vertical Alignment

	public Vector3 VerticalAlignmentDirection
	{
		get
		{
			return verticalAlignmentSettings.alignmentDirection;
		}
		set
		{
			verticalAlignmentSettings.alignmentDirection = value;
		}
	}

	public Transform VerticalAlignmentReference
	{
		get
		{
			return verticalAlignmentSettings.alignmentReference;
		}
		set
		{
			verticalAlignmentSettings.alignmentReference = value;
		}
	}

	public VerticalAlignmentSettings.VerticalReferenceMode VerticalReferenceMode
	{
		get
		{
			return verticalAlignmentSettings.referenceMode;
		}
		set
		{
			verticalAlignmentSettings.referenceMode = value;
		}
	}


	void HandleRotation( float dt )
	{
        if( verticalAlignmentSettings.alignmentReference != null )
        {               
			Vector3 targetPosition = Position + Velocity * dt;
			float sign = verticalAlignmentSettings.referenceMode == VerticalAlignmentSettings.VerticalReferenceMode.Towards ? 1f : -1f;

            verticalAlignmentSettings.alignmentDirection =  sign * ( verticalAlignmentSettings.alignmentReference.position - targetPosition ).normalized; 
               
        }

		
        Up = verticalAlignmentSettings.alignmentDirection;		
    }

	#endregion
	

	/// <summary>
	/// Updates the character size, position and rotation.
	/// </summary>
	public void UpdateCharacter( float dt )
	{
		
		if( characterActorBehaviour != null )
			characterActorBehaviour.UpdateBehaviour( dt );

		wasGrounded = IsGrounded;
        wasStable = IsStable;

		HandleSize( dt );
		HandlePosition( dt );	
		HandleRotation( dt );
		
		preSimulationVelocity = Velocity;
		// ------------------------------------------------------------
		
		if( IsGrounded )
		{
			NotGroundedTime = 0f;
			GroundedTime += dt;			
		}
		else
		{
			NotGroundedTime += dt;
			GroundedTime = 0f;

			if( wasGrounded )
				LastGroundedVelocity = PlanarVelocity;
		}
		
		
		physicsComponent.ClearContacts();
	}
	
	#region Events

		
	void OnTriggerEnterMethod( GameObject trigger )
	{
		if( OnTriggerEnter != null )
			OnTriggerEnter();
	}

	void OnTriggerExitMethod( GameObject trigger )
	{
		if( OnTriggerExit != null )
			OnTriggerExit();
	}


	/// <summary>
	/// This event is called when the character enters a trigger.
	/// </summary>
	public event System.Action OnTriggerEnter;

	/// <summary>
	/// This event is called when the character exits a trigger.
	/// </summary>
	public event System.Action OnTriggerExit;

	/// <summary>
	/// This event is called when the character hits its head (not grounded).
	/// 
	/// The related collision information struct is passed as an argument.
	/// </summary>
	public event System.Action<CollisionInfo> OnHeadHit;

	/// <summary>
	/// This event is called everytime the character is blocked by an unallowed geometry, this could be
	/// a wall or a steep slope (depending on the "slopeLimit" value).
	/// 
	/// The related collision information struct is passed as an argument.
	/// </summary>
	public event System.Action<CollisionInfo> OnWallHit;

	/// <summary>
	/// This event is called everytime the character teleports.
	/// 
	/// The teleported position and rotation are passed as arguments.
	/// </summary>
	public event System.Action<Vector3,Quaternion> OnTeleport;

	

	/// <summary>
	/// This event is called when the character enters the grounded state.
	/// 
	/// The local linear velocity is passed as an argument.
	/// </summary>
	public event System.Action<Vector3> OnGroundedStateEnter;

	/// <summary>
	/// This event is called when the character exits the grounded state.
	/// </summary>
	public event System.Action OnGroundedStateExit;
	

	#endregion
	

	/// <summary>
	/// Sets the teleportation position and rotation using an external Transform reference. 
	/// The character will move/rotate internally using its own internal logic.
	/// </summary>
	public void Teleport( Transform reference )
	{
		Teleport( reference.position , reference.rotation );
	}	

	/// <summary>
	/// Sets the teleportation position and rotation. 
	/// The character will move/rotate internally using its own internal logic.
	/// </summary>
	public void Teleport( Vector3 position , Quaternion rotation )
	{
		Position = position;
		Rotation = rotation;

		transform.SetPositionAndRotation( position , rotation );

		if( OnTeleport != null )
			OnTeleport( Position , Rotation );
	}	

	/// <summary>
	/// Sets the teleportation position. 
	/// The character will move/rotate internally using its own internal logic.
	/// </summary>
	public void Teleport( Vector3 position )
	{
		Position = position;

		transform.position = position;

		if( OnTeleport != null )
			OnTeleport( Position , Rotation );

	}
	
	/// <summary>
	/// Gets the current velocity of the ground.
	/// </summary>
	/// <value></value>
	public Vector3 GroundVelocity { get; private set; }
	

	void HandlePosition( float dt )
	{

		if( IsKinematic || !CharacterBody.ColliderComponent.enabled )
		{		
			inputVelocity = Velocity;			
			return;
		}

		if( alwaysNotGrounded )
			ForceNotGrounded();

		if( IsStable )
			VerticalVelocity = Vector3.zero;
				

		Vector3 initialPosition = Position;
		Vector3 position = Position;	

		// Calculate the displacement.
		Vector3 displacement = Velocity * dt;		
		
		
		if( IsGrounded )
		{	
			Vector3 oldInitialPosition = initialPosition;

			GroundVelocity = Vector3.zero;
			if( supportDynamicGround )
			{
				GroundVelocity = ProcessDynamicGround( ref position , dt );	
			}
			
			ApplyWeight( GroundContactPoint );
			
			GroundedMovement( ref position , displacement , dt );
		}
		else
		{			

			NotGroundedMovement( ref position , ref initialPosition , displacement , dt );

			inputVelocity = Velocity;
		}

		if( supportDynamicGround )
			UpdateDynamicGround( position );
		
		Vector3 totalDisplacement = position - initialPosition;

		
		Velocity = totalDisplacement / dt;
		
	}

	

	

	void GroundedMovement( ref Vector3 position , Vector3 displacement , float dt )
	{		
		displacement = CustomUtilities.ProjectVectorOnPlane( 
			displacement ,
            GroundStableNormal,
            Vector3.Cross( displacement , Up ).normalized 
		);
				
		CollideAndSlide( ref position , displacement , true );

		inputVelocity = Velocity;

		ProbeGround( ref position , true , dt );
		
			
	}

		
	void NotGroundedMovement( ref Vector3 position , ref Vector3 initialPosition , Vector3 displacement , float dt )
	{

        Vector3 planarDisplacement = Vector3.ProjectOnPlane( displacement , Up );
		Vector3 verticalDisplacement = Vector3.Project( displacement , Up );
		
		NotGroundedPlanarMovement( ref position , ref initialPosition , planarDisplacement );
		
		NotGroundedVerticalMovement( ref position , verticalDisplacement );

		// CollideAndSlideNotGrounded( ref position , displacement );
		
	}

	LayerMask collisionMask;

	void NotGroundedPlanarMovement( ref Vector3 position , ref Vector3 initialPosition , Vector3 planarDisplacement )
	{
		CollideAndSlide( ref position , planarDisplacement , false );

	}


	void NotGroundedVerticalMovement( ref Vector3 position , Vector3 verticalDisplacement )
	{
		float localVerticalComponent = transform.InverseTransformDirection( verticalDisplacement ).y;
		bool positiveVerticalMovement = localVerticalComponent > 0;

		// One way platforms
		bool ignoreOneWayPlatforms = IsAscending;
		
		LayerMask collisionLayerMask = ignoreOneWayPlatforms ?
		 tagsAndLayersProfile.staticObstaclesLayerMask | tagsAndLayersProfile.dynamicRigidbodiesLayerMask : 
		 tagsAndLayersProfile.staticObstaclesLayerMask | tagsAndLayersProfile.dynamicRigidbodiesLayerMask | tagsAndLayersProfile.oneWayPlatforms;
		
		
		float verticalDelta = Mathf.Sign( localVerticalComponent ) * verticalDisplacement.magnitude;

		CollisionInfo collisionInfo;
		bool hit = CastBodyVertically(
			out collisionInfo ,
			position ,
			verticalDelta ,
			collisionLayerMask
		);		

		LastNotGroundedVelocity = Velocity;

		
		position += collisionInfo.displacement;
		
		if( IsAscending )
		{			
			if( hit )
			{
				if( OnHeadHit != null )
					OnHeadHit( collisionInfo );				
			}

		}
		else
		{	
			if( hit && !alwaysNotGrounded )
			{	
				
				SetGroundCollisionInfo( collisionInfo );

				if( OnGroundedStateEnter != null )
					OnGroundedStateEnter( LocalVelocity );	
			}			
						
				
		}

		
	}

	
	void SetWallCollisionInfo( CollisionInfo collisionInfo )
	{
		bool wasCollidingWithWall = characterCollisionInfo.wallCollision;
	
		if( collisionInfo.collision )
		{			
			characterCollisionInfo.wallCollision = collisionInfo.contactSlopeAngle > slopeLimit;	
			characterCollisionInfo.wallContactNormal = collisionInfo.hitInfo.normal;
			characterCollisionInfo.wallContactPoint = collisionInfo.hitInfo.point;
			characterCollisionInfo.wallObject = collisionInfo.hitInfo.transform.gameObject;
			characterCollisionInfo.wallAngle = Vector3.Angle( - collisionInfo.hitInfo.direction , collisionInfo.hitInfo.normal );
			characterCollisionInfo.wallCollider2D = collisionInfo.hitInfo.collider2D;
			characterCollisionInfo.wallCollider3D = collisionInfo.hitInfo.collider3D;
			characterCollisionInfo.wallRigidbody2D = collisionInfo.hitInfo.rigidbody2D;
			characterCollisionInfo.wallRigidbody3D = collisionInfo.hitInfo.rigidbody3D;

		}
		else
		{
			characterCollisionInfo.ResetWallInfo();
		}

		if( characterCollisionInfo.wallCollision && !wasCollidingWithWall )
		{
			if( OnWallHit != null )
				OnWallHit( collisionInfo );
		}

	}

	void SetGroundCollisionInfoPhysics( Contact contact )
	{
		characterCollisionInfo.ResetGroundInfo();

		characterCollisionInfo.groundStableNormal = contact.normal;
		characterCollisionInfo.groundCollider2D = contact.collider2D;
		characterCollisionInfo.groundCollider3D = contact.collider3D;
		characterCollisionInfo.groundObject = contact.gameObject;
		characterCollisionInfo.groundLayer = contact.gameObject.layer;

	}

	void SetGroundCollisionInfo( CollisionInfo collisionInfo )
	{
		characterCollisionInfo.ResetGroundInfo();

		characterCollisionInfo.isOnEdge = collisionInfo.isAnEdge;

		float contactSlopeAngle =  Vector3.Angle( Up , collisionInfo.hitInfo.normal );
		characterCollisionInfo.groundContactNormal = contactSlopeAngle < 90f ? collisionInfo.hitInfo.normal : Up;
		characterCollisionInfo.groundContactPoint = collisionInfo.hitInfo.point;

		//Normal 
		if( characterCollisionInfo.isOnEdge )
		{
			characterCollisionInfo.edgeAngle = collisionInfo.edgeAngle;
			if( collisionInfo.edgeUpperSlopeAngle <= slopeLimit )
			{
				characterCollisionInfo.groundStableNormal = collisionInfo.edgeUpperNormal;
			}
			else if( collisionInfo.edgeLowerSlopeAngle <= slopeLimit )
			{
				characterCollisionInfo.groundStableNormal = collisionInfo.edgeLowerNormal;
			}
			else
			{
				characterCollisionInfo.groundStableNormal = collisionInfo.hitInfo.normal;
			}

		}
		else
		{	
			characterCollisionInfo.groundStableNormal = collisionInfo.hitInfo.normal;
		}

		characterCollisionInfo.stableSlopeAngle = Vector3.Angle( Up , characterCollisionInfo.groundStableNormal );		
		characterCollisionInfo.groundObject = collisionInfo.hitInfo.transform.gameObject;

		if( characterCollisionInfo.groundObject != null )
		{
			characterCollisionInfo.groundLayer = characterCollisionInfo.groundObject.layer;
			characterCollisionInfo.groundCollider2D = collisionInfo.hitInfo.collider2D; 
			characterCollisionInfo.groundCollider3D = collisionInfo.hitInfo.collider3D; 

			characterCollisionInfo.groundRigidbody2D = collisionInfo.hitInfo.rigidbody2D;
			characterCollisionInfo.groundRigidbody3D = collisionInfo.hitInfo.rigidbody3D;
		}

	}


	bool CheckForGround( out CollisionInfo collisionInfo , Vector3 position , bool useStepDown , LayerMask layerMask )
    {        
        collisionInfo = new CollisionInfo();

        Vector3 origin = GetOffsettedBottomCenter( position );
		float radius = BodySize.x / 2f - CharacterConstants.SkinWidth;
        float skin = stepOffset + CharacterConstants.SkinWidth;
        float extraDistance = useStepDown ? Mathf.Max( CharacterConstants.GroundCheckDistance , stepDownDistance ) : CharacterConstants.GroundCheckDistance;
        Vector3 castDisplacement = - Up * ( skin + extraDistance );
        
		HitInfo hitInfo;
        int hits = physicsComponent.SphereCast(
			out hitInfo ,
			origin ,
			radius ,
			castDisplacement ,
			layerMask
		);

		UpdateCollisionInfo( out collisionInfo , position , hitInfo , castDisplacement , skin , layerMask );     

        return collisionInfo.collision;
    }

	
	protected bool CheckForStableGround( out CollisionInfo collisionInfo , Vector3 position , Vector3 direction , LayerMask layerMask )
    {
        collisionInfo = new CollisionInfo();

        float skin = CharacterConstants.SkinWidth;
		float radius = BodySize.x / 2f - CharacterConstants.SkinWidth;
        Vector3 origin = GetBottomCenter( position );
        Vector3 castDisplacement = direction * ( CharacterConstants.GroundCheckDistance + skin + stepOffset );

		HitInfo hitInfo;
        int hits = physicsComponent.SphereCast( 
			out hitInfo ,
			origin ,
			radius ,
			castDisplacement ,
			layerMask
		);


		UpdateCollisionInfo( out collisionInfo , position , hitInfo , castDisplacement , skin , layerMask );      

        return collisionInfo.collision;
    }
	
	protected bool CastBody( out CollisionInfo collisionInfo , Vector3 position , Vector3 displacement , bool grounded , LayerMask layerMask )
    {
        collisionInfo = new CollisionInfo();
		
        float skin = CharacterConstants.SkinWidth;

        Vector3 bottom = grounded ? GetOffsettedBottomCenter( position ) : GetBottomCenter( position );   
        Vector3 top = GetTopCenter( position );      
		float radius = BodySize.x / 2f - CharacterConstants.SkinWidth;   

        Vector3 castDisplacement = displacement + displacement.normalized * skin;

		HitInfo hitInfo;
		int hits = 0;

		if( characterBody.BodyType == CharacterBodyType.Sphere )
		{
			hits = physicsComponent.SphereCast(
				out hitInfo ,
				bottom ,
				radius ,
				castDisplacement ,
				layerMask
			);
		}
		else
		{
			hits = physicsComponent.CapsuleCast(
				out hitInfo ,
				bottom ,
				top ,
				radius ,
				castDisplacement ,
				layerMask
			);
		}


		UpdateCollisionInfo( out collisionInfo , position , hitInfo , castDisplacement , skin , layerMask );     

        return collisionInfo.collision;
    }


	protected bool CastBodyVertically( out CollisionInfo collisionInfo , Vector3 position , float verticalComponent , LayerMask layerMask )
    {		
        collisionInfo = new CollisionInfo();

		float backstepDistance = BodySize.y / 2f;
		float skin = backstepDistance + CharacterConstants.SkinWidth;

		Vector3 castDirection = verticalComponent > 0 ? Up : - Up;

        Vector3 center = verticalComponent > 0 ? 
		GetTopCenter( position ) - castDirection * skin :
		GetBottomCenter( position ) - castDirection * skin;

		float castMagnitude = Mathf.Max( Mathf.Abs( verticalComponent ) + skin , 0.02f );
        Vector3 castDisplacement = castDirection * castMagnitude;

		HitInfo hitInfo;
        int hits = physicsComponent.SphereCast(
			out hitInfo ,
			center ,
			BodySize.x / 2f - CharacterConstants.SkinWidth ,
			castDisplacement ,
			layerMask
		);
		
		UpdateCollisionInfo( out collisionInfo , position , hitInfo , castDisplacement , skin , layerMask );
		
        return collisionInfo.collision;
    }

	/// <summary>
	/// Checks if the character is currently overlapping with any obstacle from a given layermask.
	/// </summary>
	public bool CheckOverlapWithLayerMask( Vector3 footPosition , bool useOffset , LayerMask layerMask )
	{
		Vector3 bottom = useOffset ? GetOffsettedBottomCenter( footPosition ) : GetBottomCenter( footPosition );   
        Vector3 top = GetTopCenter( footPosition );      
		float radius = BodySize.x / 2f - CharacterConstants.SkinWidth;  
		
		bool overlap = physicsComponent.OverlapCapsule(
			bottom ,
			top ,
			radius ,
			layerMask
		);		
		
		return overlap;
	}

	/// <summary>
	/// Checks if the character size fits at a specific location.
	/// </summary>
	public bool CheckBodySize( Vector3 size , Vector3 position , LayerMask layerMask )
    {        
        Vector3 bottom = GetBottomCenter( position , size );   
        Vector3 top = GetTopCenter( position , size ); 
		float radius = size.x / 2f - CharacterConstants.SkinWidth;

		// GetBottomCenterToTopCenter.normalized ---> Up

        Vector3 castDisplacement = GetBottomCenterToTopCenter( size ) + Up * CharacterConstants.SkinWidth;

		HitInfo hitInfo;
		physicsComponent.SphereCast( 
			out hitInfo ,
			bottom ,
			radius ,
			castDisplacement ,
			layerMask
		);


		bool overlap = hitInfo.hit;
		
		return !overlap;
    }

	/// <summary>
	/// Checks if the character size fits in place.
	/// </summary>
	public bool CheckBodySize( Vector3 size , LayerMask layerMask )
    {        
        return CheckBodySize( size , Position , layerMask );
    }

	/// <summary>
	/// Checks if the new character size fits in place. If this check is valid then the real size of the character is changed.
	/// </summary>
	public bool SetBodySize( Vector2 size )
	{ 
        if( !CheckBodySize( size , Position , StaticObstaclesLayerMask ) )
			return false;

		targetBodySize = size;

		return true;
	}

	public void ForceGrounded()
	{
		LayerMask groundLayerMask = tagsAndLayersProfile.staticObstaclesLayerMask | tagsAndLayersProfile.dynamicRigidbodiesLayerMask | tagsAndLayersProfile.oneWayPlatforms;

		CollisionInfo collisionInfo;
		bool hit = CheckForGround( 
			out collisionInfo ,
			Position , 
			true , 
			groundLayerMask 
		);

		if( hit )
		{		
			float slopeAngle = IsAStableEdge( collisionInfo ) ? 
			Vector3.Angle( Up , collisionInfo.edgeUpperNormal ) : 
			Vector3.Angle( Up , collisionInfo.hitInfo.normal );
				
			if( slopeAngle <= slopeLimit )
			{
				Position += collisionInfo.displacement;
				
				SetGroundCollisionInfo( collisionInfo );
				
			}

			
		}
	}

	void ProbeGround( ref Vector3 position , bool grounded , float dt )
	{

		Vector3 initialFootPosition = position;

		float groundCheckDistance = edgeCompensation ? 
		BodySize.x / 2f + CharacterConstants.GroundCheckDistance :
		CharacterConstants.GroundCheckDistance;

		float groundProbingDistance = IsStable ? 
		Mathf.Max( groundCheckDistance , stepDownDistance ) : 
		CharacterConstants.GroundCheckDistance;

		// float skin = stepOffset + CharacterConstants.SkinWidth;
        // float extraDistance = IsStable ? Mathf.Max( CharacterConstants.GroundCheckDistance , stepDownDistance ) : CharacterConstants.GroundCheckDistance;
        // Vector3 castDisplacement = - Up * ( skin + extraDistance );

		Vector3 displacement = - Up * groundProbingDistance;
		
		LayerMask groundLayerMask = tagsAndLayersProfile.staticObstaclesLayerMask | tagsAndLayersProfile.dynamicRigidbodiesLayerMask | tagsAndLayersProfile.oneWayPlatforms;
		
		CollisionInfo firstCollisionInfo;
		bool hit = CheckForGround( 
			out firstCollisionInfo ,
			position , 
			grounded , 
			groundLayerMask 
		);


		if( !hit )
		{		
			ForceNotGrounded();
			return;
		}

		
		float slopeAngle = IsAStableEdge( firstCollisionInfo ) && IsStable ? 
		Vector3.Angle( Up , firstCollisionInfo.edgeUpperNormal ) : 
		Vector3.Angle( Up , firstCollisionInfo.hitInfo.normal );
			
		if( slopeAngle <= slopeLimit )
		{				
			// Stable hit ---------------------------------------------------

			// Save the ground collision info
			SetGroundCollisionInfo( firstCollisionInfo );
			
			// Calculate the final position 
			Vector3 finalPosition = position;
			finalPosition += firstCollisionInfo.displacement;				
			

			if( edgeCompensation && IsAStableEdge( firstCollisionInfo ) )
			{
				// calculate the edge compensation and apply that to the final position
				Vector3 compensation = Vector3.Project( ( firstCollisionInfo.hitInfo.point - finalPosition ) , Up );
				finalPosition += compensation;					
			}

			// Determine the edge compensation factor and the step speed.
			bool upwardsDisplacement = transform.InverseTransformVectorUnscaled( firstCollisionInfo.displacement ).y > 0f;
			float stepSpeed = upwardsDisplacement ? stepUpSpeed : stepDownSpeed;

			// Interpolate the old position with the final position
			position += ( finalPosition - position ) * stepSpeed;
			
		}
		else
		{
			
			// It hit ustable ground -> Keep checking...
			float castSkinDistance = stepOffset + CharacterConstants.ExtraUnstableCheckDistance;
			if( firstCollisionInfo.hitInfo.distance > castSkinDistance )
			{
				ForceNotGrounded();
				return;
			}

			// Backup the collision data
			CollisionInfo unstableGroundCollisionInfo = new CollisionInfo();
			unstableGroundCollisionInfo = firstCollisionInfo;

			position += unstableGroundCollisionInfo.displacement;

			Vector3 downwardsDirection = Vector3.ProjectOnPlane( - Up , unstableGroundCollisionInfo.hitInfo.normal ).normalized;
			

			CollisionInfo secondCollisionInfo;
			hit = CheckForStableGround( 
				out secondCollisionInfo , 
				position , 
				downwardsDirection , 
				groundLayerMask 
			);

			if( hit )
			{
				
				slopeAngle = Vector3.Angle( Up , secondCollisionInfo.hitInfo.normal );

				if( slopeAngle <= slopeLimit )
				{
					// It Hit a stable slope -> Set Info
					position += secondCollisionInfo.displacement;
					SetGroundCollisionInfo( secondCollisionInfo );			
					
				}
				else
				{
					SetGroundCollisionInfo( unstableGroundCollisionInfo );							
				}
				
			}
			else
			{
				SetGroundCollisionInfo( unstableGroundCollisionInfo );
			}
		}

		//EdgeCompensation( ref position );
			
	}

	void EdgeCompensation( ref Vector3 position )
	{
		if( !edgeCompensation )
			return;
		
		if( IsOnEdge && IsStable )
		{
			Vector3 compensation = Vector3.Project( ( GroundContactPoint - position ) , Up );

			position += compensation;
		}
		
	}


	/// <summary>
	/// Set the "force not grounded" internal flag only for one frame. The character will use this flag to abandon the grounded state (isGrounded = false). 
	/// 
	/// TIP: this is useful when making the character jump.
	/// </summary>
	public void ForceNotGrounded()
	{
		bool onGroundedStateExit = IsGrounded;

		characterCollisionInfo.Reset();
		GroundVelocity = Vector3.zero;

		if( onGroundedStateExit )
			if( OnGroundedStateExit != null )
				OnGroundedStateExit();	
	}


	bool IsAStableEdge( CollisionInfo collisionInfo )
	{
		return collisionInfo.isAnEdge && collisionInfo.edgeUpperSlopeAngle <= slopeLimit;
	}

	bool IsAnUnstableEdge( CollisionInfo collisionInfo )
	{
		return collisionInfo.isAnEdge && collisionInfo.edgeUpperSlopeAngle > slopeLimit;
	}	

	
	bool IsValidForStepUp( CollisionInfo collisionInfo )
	{
		if( CustomUtilities.BelongsToLayerMask( collisionInfo.hitInfo.transform.gameObject.layer , tagsAndLayersProfile.dynamicRigidbodiesLayerMask ) )
			return false;
		
		if( IsAStableEdge( collisionInfo ) )
			return true;		
		
		if( CustomUtilities.isBetween( collisionInfo.contactSlopeAngle , CharacterConstants.MinStepAngle , CharacterConstants.MaxStepAngle , true ) )
			return true;
		

		return false;
	}

	void CollideAndSlide( ref Vector3 position , Vector3 displacement , bool grounded )
	{
		Vector3 groundPlaneNormal = grounded ? GroundStableNormal : Up;

		Vector3 slidingPlaneNormal = Vector3.zero;

		Vector3 initialFootPosition = position;            

		int iteration = 0;
				
		while( iteration < CharacterConstants.MaxSlideIterations )
        {
			iteration++;
			
			Vector3 previousFootPosition = position;

			CollisionInfo collisionInfo;
			bool hit = CastBody(
				out collisionInfo ,
				position ,
				displacement ,
				grounded ,
				tagsAndLayersProfile.staticObstaclesLayerMask
			);
			
			
			if( !hit )
			{
				position += displacement;

				if( iteration == 1 )
					SetWallCollisionInfo( collisionInfo );
				
				break;
			}

			// Save the collision info
			CollisionInfo initialCollisionInfo = collisionInfo;

			position += collisionInfo.displacement;
			displacement -= collisionInfo.displacement;    

			// Slide ------------------------------------------------------------------------------------------------------------------
			bool blocked = UpdateSlidingPlanes( 
				iteration , 
				false , 
				initialCollisionInfo , 
				ref slidingPlaneNormal , 
				ref groundPlaneNormal , 
				ref displacement 
			);

			if( iteration == 1 )
				SetWallCollisionInfo( initialCollisionInfo );				
			
           	
			
		}

	}

	CollisionInfo lastHitCollisionInfo = new CollisionInfo();

	void CollideAndSlideNotGrounded( ref Vector3 position , Vector3 displacement )
	{

		Vector3 initialPosition = position;            
		Vector3 groundNormal = Vector3.zero;

		int iteration = 0;
				
		while( iteration < CharacterConstants.MaxSlideIterations )
        {
			iteration++;
			
			CollisionInfo collisionInfo;
			bool hit = CastBody(
				out collisionInfo ,
				position ,
				displacement ,
				false ,
				StaticObstaclesLayerMask
			);
			
			
			if( hit )
			{
				// Update position and displacement
				position += collisionInfo.displacement;
				displacement -= collisionInfo.displacement;
				
				lastHitCollisionInfo = collisionInfo;
				
				// displacement = CustomUtilities.ProjectVectorOnPlane( 
				// 	displacement , 
				// 	collisionInfo.hitInfo.normal ,
				// 	Vector3.Cross( displacement , Up).normalized 
				// );

				groundNormal = collisionInfo.hitInfo.normal;
				displacement = Vector3.ProjectOnPlane( displacement , groundNormal );
			}
			else
			{
				position += displacement;			
				break;
			}
			
		}

		if( groundNormal != Vector3.zero && !alwaysNotGrounded )
		{
			if( lastHitCollisionInfo.contactSlopeAngle < 85f )
				SetGroundCollisionInfo( lastHitCollisionInfo );
		}
	}
			
	
	bool UpdateSlidingPlanes( 
		int iteration , 
		bool stepUpResult ,
		CollisionInfo collisionInfo , 
		ref Vector3 slidingPlaneNormal , 
		ref Vector3 groundPlaneNormal , 
		ref Vector3 displacement )
	{

		Vector3 normal = collisionInfo.hitInfo.normal;
	
		
		float slopeAngle = Vector3.Angle( normal , Up );

		if( slopeAngle > slopeLimit )
		{    
			
			if( slidingPlaneNormal != Vector3.zero )
			{
				float correlation = Vector3.Dot( normal , slidingPlaneNormal );
				
				if( correlation > 0 )
					displacement = CustomUtilities.DeflectVector( displacement , groundPlaneNormal , normal );
				else
					displacement = Vector3.zero;                            
				
			}
			else
			{
				displacement = CustomUtilities.DeflectVector( displacement , groundPlaneNormal , normal );
			}

			slidingPlaneNormal = normal;                     
		}
		else
		{
			displacement = CustomUtilities.ProjectVectorOnPlane( 
				displacement , 
				normal ,
                Vector3.Cross( displacement , Up).normalized 
			);

			groundPlaneNormal = normal;
			slidingPlaneNormal = Vector3.zero;

		}

		return displacement == Vector3.zero;
	}



	
	bool StepUp( ref Vector3 position , ref Vector3 displacement , out CollisionInfo stepUpResultInfo , CollisionInfo initialCollisionInfo )
	{		
		stepUpResultInfo = new CollisionInfo();

		Vector3 initialFootPosition = position;
		Vector3 initialDisplacement = displacement;

		// Vector3 characterToWall = 
		
		Vector3 ascendingDisplacement = Up * stepOffset;
		Vector3 stepUpDisplacement = Mathf.Max( CharacterConstants.StepExtraMovement , displacement.magnitude ) * ( - initialCollisionInfo.hitInfo.normal );//displacement.normalized; 
		Vector3 descendingDisplacement = - ascendingDisplacement;
		
		bool hit = false;


		// Ascend --------------------------------------------------------------------------
		CollisionInfo collisionInfo;
		hit = CastBody(
			out collisionInfo ,
			position ,
			ascendingDisplacement ,
			true ,
			tagsAndLayersProfile.staticObstaclesLayerMask
		);

		position += collisionInfo.displacement;
		
		
		hit = CastBody(
			out collisionInfo ,
			position ,
			stepUpDisplacement ,
			true ,
			tagsAndLayersProfile.staticObstaclesLayerMask
		);
		
		//Vector3 stepUpDisplacement = collisionInfo.displacement.normalized * ( collisionInfo.displacement.magnitude + CharacterConstants.StepExtraMovement );
		position += collisionInfo.displacement;
		displacement -= collisionInfo.displacement;
		
		// Return if the upper normal is unstable.
		if( hit )
		{			
			
			if( IsAnUnstableEdge( collisionInfo ) || collisionInfo.contactSlopeAngle > slopeLimit )
			{				
				position = initialFootPosition;
				displacement = initialDisplacement;
				
				return false;
				
			}
		}	

		// Descend -------------------------------------------------------------------------------
		
		hit = CastBody(
			out collisionInfo ,
			position ,
			descendingDisplacement ,
			true ,
			tagsAndLayersProfile.staticObstaclesLayerMask
		);

		position += collisionInfo.displacement;
		
		// Return if the upper normal is unstable.
		if( hit )
		{
			if( IsAnUnstableEdge( collisionInfo ) )
			{			
				position = initialFootPosition;
				displacement = initialDisplacement;
				return false;
			}		
		}
		
		bool stepUpPerformed = ( position - initialFootPosition ).magnitude > CharacterConstants.MinStepUpDifference;
		bool allowedHeight = Vector3.Project( collisionInfo.hitInfo.point - initialFootPosition , Up ).magnitude <= stepOffset;


		if( stepUpPerformed && allowedHeight )
		{	
			stepUpResultInfo = collisionInfo;
			return true;
		}
		else
		{
			position = initialFootPosition;
			displacement = initialDisplacement;
			return false;
		}
		
	}

	


	void UpdateCollisionInfo( out CollisionInfo collisionInfo , Vector3 position , HitInfo hitInfo , Vector3 castDisplacement , float skin , LayerMask layerMask )
    {
		collisionInfo = new CollisionInfo();

        collisionInfo.collision = hitInfo.hit;

        if( collisionInfo.collision )
        {            
            collisionInfo.displacement = castDisplacement.normalized * ( hitInfo.distance - skin );
                    
            collisionInfo.hitInfo = hitInfo;
            collisionInfo.contactSlopeAngle = Vector3.Angle( transform.up , hitInfo.normal );

            UpdateEdgeInfo( ref collisionInfo , position , layerMask );
        }
        else
        {
            collisionInfo.displacement = castDisplacement.normalized * ( castDisplacement.magnitude - skin );
        }

    }	


	void UpdateEdgeInfo( ref CollisionInfo collisionInfo , Vector3 position , LayerMask layerMask )
    {
        Vector3 center = GetOffsettedBottomCenter( position );

        Vector3 castDirection = ( collisionInfo.hitInfo.point - center ).normalized;
		Vector3 castDisplacement = castDirection * CharacterConstants.EdgeRaysCastDistance;

		Vector3 upperHitPosition = center + Up * CharacterConstants.EdgeRaysSeparation;
		Vector3 lowerHitPosition = center - Up * CharacterConstants.EdgeRaysSeparation;

		HitInfo upperHitInfo;
		physicsComponent.Raycast(
			out upperHitInfo,
			upperHitPosition ,
			castDisplacement ,
			layerMask
		);

        
        HitInfo lowerHitInfo;
		physicsComponent.Raycast(
			out lowerHitInfo,
			lowerHitPosition ,
			castDisplacement ,
			layerMask
		);
        		

		collisionInfo.edgeUpperNormal = upperHitInfo.normal;      
		collisionInfo.edgeLowerNormal = lowerHitInfo.normal;

        collisionInfo.edgeUpperSlopeAngle = Vector3.Angle( collisionInfo.edgeUpperNormal , Up );
        collisionInfo.edgeLowerSlopeAngle = Vector3.Angle( collisionInfo.edgeLowerNormal , Up );
	
		collisionInfo.edgeAngle = Vector3.Angle( collisionInfo.edgeUpperNormal , collisionInfo.edgeLowerNormal );

        collisionInfo.isAnEdge = CustomUtilities.isBetween( collisionInfo.edgeAngle , CharacterConstants.MinEdgeAngle , CharacterConstants.MaxEdgeAngle , true );
        collisionInfo.isAStep = CustomUtilities.isBetween( collisionInfo.edgeAngle , CharacterConstants.MinStepAngle , CharacterConstants.MaxStepAngle , true );
        
        
    }

	void OnDrawGizmos()
	{
		if( characterBody == null )
			characterBody = GetComponent<CharacterBody>();
		
		Vector3 top = transform.position + transform.up * characterBody.BodySize.y;
		if( verticalAlignmentSettings.alignmentReference != null )
		{
			Vector3 direction = ( verticalAlignmentSettings.alignmentReference.position - transform.position ).normalized;

			if( verticalAlignmentSettings.referenceMode == VerticalAlignmentSettings.VerticalReferenceMode.Away )
				direction *= -1;
			
			CustomUtilities.DrawArrowGizmo( top , top + direction , Color.white );
		}
		else
		{
			CustomUtilities.DrawArrowGizmo( top , top + verticalAlignmentSettings.alignmentDirection.normalized , Color.white );
		}
		
	}

	
	
}

}