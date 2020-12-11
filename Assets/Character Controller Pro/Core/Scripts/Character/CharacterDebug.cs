using UnityEngine;
using UnityEngine.UI;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

/// <summary>
/// This class is used for debug purposes, mainly to print information on screen about the collision flags, values and triggered events.
/// </summary>
[AddComponentMenu("Character Controller Pro/Core/Character Debug")]
public class CharacterDebug : MonoBehaviour
{
	[SerializeField]
	Text text = null;

	[SerializeField]
    CharacterActor characterActor = null;

	[SerializeField]
    bool debugCollisionFlags = true;

	[SerializeField]
    bool debugEvents = true;

	float time = 0f;

    void Awake()
    {
		if( debugCollisionFlags )
		{
			if( text == null || characterActor == null )
				this.enabled = false;				
		}
        
    }

	void Update()
	{
		if( debugCollisionFlags )
		{
			if( time > 0.2f )
			{
				text.text = characterActor.ToString();
				
				time = 0f;
			}
			else
			{
				time += Time.deltaTime;
			}

		}
			
	}

	void OnEnable()
    {
		if( !debugEvents )
			return;
		
        characterActor.OnWallHit += OnWallHit;
		characterActor.OnGroundedStateEnter += OnGroundedStateEnter;
		characterActor.OnGroundedStateExit += OnGroundedStateExit;
		characterActor.OnHeadHit += OnHeadHit;
		characterActor.OnTeleport += OnTeleportation;
    }

    void OnDisable()
    {
		if( !debugEvents )
			return;
		
		characterActor.OnWallHit -= OnWallHit;
		characterActor.OnGroundedStateEnter -= OnGroundedStateEnter;
		characterActor.OnGroundedStateExit -= OnGroundedStateExit;
		characterActor.OnHeadHit -= OnHeadHit;
		characterActor.OnTeleport -= OnTeleportation;
    }

    void OnWallHit( CollisionInfo collisionInfo )
    {
        Debug.Log( "OnWallHit" );
    }

	void OnGroundedStateEnter( Vector3 localVelocity )
	{
		Debug.Log( "OnEnterGroundedState, localVelocity : " + localVelocity.ToString("F3") ); 
	}

	void OnGroundedStateExit()
	{
		Debug.Log( "OnExitGroundedState" );
	}

	void OnHeadHit( CollisionInfo collisionInfo )
	{
		Debug.Log( "OnHeadHit" );
	}

	void OnTeleportation( Vector3 position , Quaternion rotation )
	{
		Debug.Log( "OnTeleportation, position : " + position.ToString("F3") + " and rotation : " + rotation.ToString("F3") );
	}

	

}

}
