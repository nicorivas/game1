using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.CharacterControllerPro.Core;

namespace Lightbug.CharacterControllerPro.Demo
{

public enum DashMode
{
	FacingDirection , 
	InputDirection 
}

[AddComponentMenu("Character Controller Pro/Demo/Character/States/Dash")]
public class Dash : CharacterState
{    

	#region Events

	/// <summary>
	/// This event is called when the dash state is entered.
	/// 
	/// The direction of the dash is passed as an argument.
	/// </summary>
	public event System.Action<Vector3> OnDashStart;	

	/// <summary>
	/// This event is called when the dash action has ended.
	/// 
	/// The direction of the dash is passed as an argument.
	/// </summary>
	public event System.Action<Vector3> OnDashEnd;
	
	#endregion
		

	[Min( 0f )] 
	[SerializeField] 
	float initialVelocity = 12f;

	[Min( 0f )] 
	[SerializeField] 
	float duration = 0.4f;

	[SerializeField] 
	AnimationCurve movementCurve = AnimationCurve.Linear( 0,1,1,0 );	

	[Min( 0f )] 
	[SerializeField] int availableNotGroundedDashes = 1;

	[SerializeField]
	bool ignoreSpeedMultipliers = false;

	[SerializeField]
	bool forceNotGrounded = true;

	[Tooltip("Whether or not to allow the dash to be canceled by others rigidbodies.")]
	[SerializeField]
	bool cancelOnContact = true;

	[Tooltip("If the contact point velocity (magnitude) is greater than this value, the Dash will be instantly canceled.")]
	[SerializeField]
	float contactVelocityTolerance = 5f;
		

    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

	
	MaterialController materialController = null;


	int airDashesLeft;
	float dashCursor = 0;

	Vector3 dashDirection = Vector2.right;

	bool isDone = true;


	void OnEnable()
	{
		CharacterActor.OnGroundedStateEnter += OnGroundedStateEnter;
		
	}

	void OnDisable()
	{
		CharacterActor.OnGroundedStateEnter -= OnGroundedStateEnter;
	}


	public override string GetInfo()
    {
        return "This state is entirely based on particular movement, the \"dash\". This movement is normally a fast impulse along " + 
		"the forward direction. In this case the movement can be defined by using an animation curve (time vs velocity)";
    }

	void OnGroundedStateEnter( Vector3 localVelocity )
	{
		airDashesLeft = availableNotGroundedDashes;
	}

	bool CheckContacts()
	{
		if( !cancelOnContact )
			return false;
		
		for( int i = 0 ; i < CharacterActor.Contacts.Count ; i++ )
		{
			Contact contact = CharacterActor.Contacts[i];

			if( contact.pointVelocity.magnitude > contactVelocityTolerance )
			{
				return true;
			}

		}

		return false;
		
	}

	protected override void Awake()
	{
		base.Awake();

		materialController = this.GetComponentInBranch< CharacterActor , MaterialController>();
        if( materialController == null )
        {
            Debug.Log("Missing MaterialController component");
            this.enabled = false;
            return;
        }
	}
	

	public override bool CheckEnterTransition( CharacterState fromState )
    {
        if( !CharacterActor.IsGrounded && airDashesLeft <= 0 )
			return false;

		return true;
    }

	public override void CheckExitTransition()
    {
        if( isDone )
        {
			if( OnDashEnd != null )
				OnDashEnd( dashDirection );

			CharacterStateController.EnqueueTransition<NormalMovement>();
        }
    }

	
	public override void EnterBehaviour( float dt , CharacterState fromState )
	{

		if( forceNotGrounded )
			CharacterActor.ForceNotGrounded();
		

		if( CharacterActor.IsGrounded )
		{			
			
			if( !ignoreSpeedMultipliers )
				currentSpeedMultiplier = materialController.CurrentSurface.speedMultiplier * materialController.CurrentVolume.speedMultiplier;
			
		}
		else
		{			
			
			if( !ignoreSpeedMultipliers )
				currentSpeedMultiplier = materialController.CurrentVolume.speedMultiplier;

			airDashesLeft--;

			
		}

		//Set the dash direction
		dashDirection = CharacterActor.Forward;		

		ResetDash();

		//Execute the event
		if( OnDashStart != null )
			OnDashStart( dashDirection );

	}

	

	float currentSpeedMultiplier = 1f;
		

	public override void UpdateBehaviour( float dt )
	{		
		Vector3 dashVelocity = initialVelocity * currentSpeedMultiplier * movementCurve.Evaluate(dashCursor) * dashDirection;		

		CharacterActor.Velocity = dashVelocity;

		float animationDt = dt / duration;
		dashCursor += animationDt; 

		if( dashCursor >= 1 )
		{
			isDone = true;
			dashCursor = 0;
		}

	}

	public override void PostUpdateBehaviour(float dt)
	{
		isDone |= CheckContacts();
	}
	
	void ResetDash()
	{		
		CharacterActor.Velocity = Vector3.zero;
		isDone = false;
		dashCursor = 0;
	}

		
	
}

}





