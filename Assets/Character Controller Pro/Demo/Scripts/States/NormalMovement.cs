using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;
using Lightbug.CharacterControllerPro.Implementation;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lightbug.CharacterControllerPro.Demo
{



[AddComponentMenu("Character Controller Pro/Demo/Character/States/Normal Movement")]
public class NormalMovement : CharacterState
{   
    
    [Space(10)]
    
    public PlanarMovementParameters planarMovementParameters = new PlanarMovementParameters();

    public VerticalMovementParameters verticalMovementParameters = new VerticalMovementParameters(); 

    public CrouchParameters crouchParameters = new CrouchParameters();
   
    public LookingDirectionParameters lookingDirectionParameters = new LookingDirectionParameters();
    
    
    [Header("Animation")]

    [SerializeField]
    string groundedParameter = "Grounded";

    [SerializeField]
    string stableParameter = "Stable";

    [SerializeField]
    string verticalSpeedParameter = "VerticalSpeed";

    [SerializeField]
    string planarSpeedParameter = "PlanarSpeed";

	[SerializeField]
    string horizontalAxisParameter = "HorizontalAxis";

	[SerializeField]
    string verticalAxisParameter = "VerticalAxis";

    [SerializeField]
    string heightParameter = "Height";

    
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    
    #region Events	

    /// <summary>
    /// Event triggered when the character jumps.
    /// </summary>
	public event System.Action OnJumpPerformed;

    /// <summary>
    /// Event triggered when the character jumps from the ground.
    /// </summary>
	public event System.Action OnGroundedJumpPerformed;

    /// <summary>
    /// Event triggered when the character jumps while.
    /// </summary>
	public event System.Action<int> OnNotGroundedJumpPerformed;
	
	#endregion

    MaterialController materialController = null;
      
    
   

    int notGroundedJumpsLeft = 0;
	float jumpTimer = 0f;

    bool isJumping = false;
    
    
    protected override void Awake()
    {
        base.Awake();
        
        notGroundedJumpsLeft = verticalMovementParameters.availableNotGroundedJumps;       

        materialController = this.GetComponentInBranch< CharacterActor , MaterialController>();
        if( materialController == null )
        {
            Debug.Log("Missing MaterialController component");
            this.enabled = false;
            return;
        }
         

    }

    void Start()
    {
        targetHeight = CharacterActor.DefaultBodySize.y;

        float minShrinkHeightRatio = CharacterActor.BodySize.x / CharacterActor.BodySize.y;
        crouchParameters.heightRatio = Mathf.Max( minShrinkHeightRatio , crouchParameters.heightRatio );        
    }

    void OnEnable()
    {
        CharacterActor.OnHeadHit += OnHeadHit;
        CharacterActor.OnTeleport += OnTeleport;
        
    }

    void OnDisable()
    {
        CharacterActor.OnHeadHit -= OnHeadHit;
        CharacterActor.OnTeleport -= OnTeleport;

        // CharacterStateController.OnVolumeEnter -= OnEnterVolume;
    }

    public override string GetInfo()
    {
        return "This state serves as a multi purpose movement based state. It is responsible for handling gravity and jump, walk and run, crouch, " + 
        "react to the different material properties, etc. Basically it covers all the common movements involved " + 
        "in a typical game, from a 3D platformer to a first person walking simulator.";
    }

    void OnHeadHit( CollisionInfo collisionInfo )
    {
        CharacterActor.VerticalVelocity = Vector3.zero;        
    }
    

    void OnTeleport( Vector3 position , Quaternion rotation )
    {
        targetLookingDirection = CharacterActor.Forward;
    }

    
    public bool FollowExternalReference
    {
        get
        {
            return lookingDirectionParameters.followExternalReference;
        }
        set
        {
            lookingDirectionParameters.followExternalReference = value;
        }
    }

    /// <summary>
    /// Gets/Sets the useGravity toggle. Use this property to enable/disable the effect of gravity on the character.
    /// </summary>
    /// <value></value>
    public bool UseGravity
    {
        get
        {
            return verticalMovementParameters.useGravity;
        }
        set
        {
            verticalMovementParameters.useGravity = value;
        }
    }

    

    public override void CheckExitTransition()
    {
        
        if( CharacterActions.jetPack.value )
        {
            CharacterStateController.EnqueueTransition<JetPack>();
        }
        else if( CharacterActions.dash.Started )
        {
            CharacterStateController.EnqueueTransition<Dash>();       
        }
        else if( CharacterActor.Triggers.Count != 0 )
        {                               

            if( CharacterActions.interact.Started )
            {   
                CharacterStateController.EnqueueTransition<LadderClimbing>(); 
            }
            // else
            // {   
            //     //  WIP
            //     // CharacterStateController.EnqueueTransition<RopeClimbing>();
            //     // CharacterStateController.EnqueueTransition<Swimming>();      
                
            // }   
            
        }
        else if( !CharacterActor.IsGrounded )
        {            
            CharacterStateController.EnqueueTransition<LedgeHanging>();
        }
        
    }
    

        
    bool wantToRun = false;
    
    float currentPlanarSpeedLimit = 0f;

    
    void SetMotionValues()
    {
        if( CharacterActor.IsGrounded )
        {
            if( CharacterActor.IsStable )
                currentMotion = planarMovementParameters.stableMotion;
            else
                currentMotion = planarMovementParameters.unstableMotion;
        }
        else
        {
            currentMotion = planarMovementParameters.notGroundedMotion;
        }


        // Material values
        if( CharacterActor.IsGrounded )
        {
            currentMotion.acceleration *= materialController.CurrentSurface.accelerationMultiplier * materialController.CurrentVolume.accelerationMultiplier;
            currentMotion.deceleration *= materialController.CurrentSurface.decelerationMultiplier * materialController.CurrentVolume.decelerationMultiplier;
        }
        else
        {            
            currentMotion.acceleration *= materialController.CurrentVolume.accelerationMultiplier;
            currentMotion.deceleration *= materialController.CurrentVolume.decelerationMultiplier;
        }
        

    }
    
    void ProcessPlanarMovement( float dt )
    {
        SetMotionValues();

        float speedMultiplier = materialController.CurrentSurface.speedMultiplier * materialController.CurrentVolume.speedMultiplier;

        bool needToAccelerate = false;

        switch( CharacterActor.CurrentState )
        {
            case CharacterActorState.NotGrounded:
                
                if( CharacterActor.WasGrounded )
                {
                    currentPlanarSpeedLimit = Mathf.Max( CharacterActor.PlanarVelocity.magnitude , planarMovementParameters.baseSpeedLimit );
                }
                

                needToAccelerate = ( CharacterStateController.InputMovementReference * currentPlanarSpeedLimit ).magnitude >= CharacterActor.PlanarVelocity.magnitude;

                
                CharacterActor.PlanarVelocity = Vector3.MoveTowards( 
                    CharacterActor.PlanarVelocity , 
                    currentPlanarSpeedLimit * CharacterStateController.InputMovementReference * speedMultiplier , 
                    ( needToAccelerate ? currentMotion.acceleration : currentMotion.deceleration ) * dt 
                );

                break;
            case CharacterActorState.StableGrounded:

                
                // Run ------------------------------------------------------------
                if( planarMovementParameters.runInputMode == InputMode.Toggle )
                {
                    if( CharacterActions.run.Started )
                        wantToRun = !wantToRun;            
                } 
                else
                {
                    wantToRun = CharacterActions.run.value;                        
                }

                if( isCrouched )
                {
                    currentPlanarSpeedLimit = planarMovementParameters.baseSpeedLimit * crouchParameters.speedMultiplier; 
                }  
                else
                {   
                    currentPlanarSpeedLimit = wantToRun ? planarMovementParameters.boostSpeedLimit : planarMovementParameters.baseSpeedLimit;  
                }   

                Vector3 targetPlanarVelocity = currentPlanarSpeedLimit * CharacterStateController.InputMovementReference * speedMultiplier;

                needToAccelerate = CharacterStateController.InputMovementReference != Vector3.zero;
                
                // Set the velocity
                CharacterActor.PlanarVelocity = Vector3.MoveTowards( 
                    CharacterActor.PlanarVelocity , 
                    targetPlanarVelocity , 
                    ( needToAccelerate ? currentMotion.acceleration : currentMotion.deceleration ) * dt 
                );
                
                
                break;
            case CharacterActorState.UnstableGrounded:                
                
                break;
        }

    }

    void ProcessSliding( float dt )
    {
        if( CharacterActor.CurrentState != CharacterActorState.UnstableGrounded )
            return;
                        
        Vector3 slidingDirection = Vector3.ProjectOnPlane( - CharacterActor.Up , CharacterActor.GroundContactNormal ).normalized;
        
        if( CharacterActor.PreviousState != CharacterActorState.UnstableGrounded )
        { 
            
            Vector3 planar = Vector3.Project( 
                CharacterActor.PlanarVelocity ,
                Vector3.Cross( slidingDirection , CharacterActor.GroundContactNormal ).normalized
            );

            Vector3 vertical = Vector3.zero;
            if( CharacterActor.IsFalling )
            {
                vertical = slidingDirection * Mathf.Abs( CharacterActor.LocalVelocity.y );
            }
            

            CharacterActor.PlanarVelocity = planar + vertical;

        }

        CharacterActor.PlanarVelocity += slidingDirection * verticalMovementParameters.GravityMagnitude * dt;
                
        
    }

    

    void ProcessGravity( float dt )
    {
        verticalMovementParameters.UpdateParameters( materialController.CurrentVolume.gravityAscendingMultiplier );

        float gravityMultiplier = CharacterActor.LocalVelocity.y >= 0 ? 
        materialController.CurrentVolume.gravityAscendingMultiplier : 
        materialController.CurrentVolume.gravityDescendingMultiplier;

        float gravity = gravityMultiplier * verticalMovementParameters.GravityMagnitude;
       
        if( CharacterActor.IsGrounded )
        {
            if( !CharacterActor.WasGrounded || !CharacterActor.WasStable )
            {
                CharacterActor.VerticalVelocity = Vector3.zero;
            }
        }
        else
        {
            if( CharacterActor.WasGrounded )
            {
                CharacterActor.VerticalVelocity += Vector3.Project( CharacterActor.Up , CharacterActor.PlanarVelocity );
            }

            CharacterActor.VerticalVelocity += - CharacterActor.Up * ( gravity * dt );
                
        }
        
        

    }
    
    bool groundedJumpAvailable = false;

    /// <summary>
    /// Apply the jump velocity to the vertical velocity vector, based on the current jump state.
    /// </summary>
    void ProcessJump( float dt )
    {     
        bool coyoteTimeValid = !CharacterActor.IsGrounded && CharacterActor.NotGroundedTime <= verticalMovementParameters.coyoteTime && groundedJumpAvailable;

        if( CharacterActor.IsGrounded )
        {
            notGroundedJumpsLeft = verticalMovementParameters.availableNotGroundedJumps;            
        }

        
        groundedJumpAvailable = CharacterActor.IsGrounded || coyoteTimeValid;


        if( !verticalMovementParameters.canJumpOnUnstableGround && CharacterActor.CurrentState == CharacterActorState.UnstableGrounded )
            groundedJumpAvailable = false;

        if( isJumping )
        {
            CharacterActor.VerticalVelocity = jumpVelocity;
            jumpTimer += dt;

            if( jumpTimer > verticalMovementParameters.constantJumpDuration || CharacterActions.jump.Canceled )
            {
                isJumping = false;
                jumpVelocity = Vector3.zero;
                jumpTimer = 0f;
            }
            
        }
        else
        {      
            // Calculate the jump velocity vector ------------------------------------------------------
            if( CharacterActions.jump.Started && verticalMovementParameters.canJump )
            {
                // Jump Down -------------------------------------------------
                if( 
                    CharacterActor.IsGrounded && 
                    CharacterActions.crouch.value && 
                    CustomUtilities.BelongsToLayerMask( CharacterActor.GroundObject.layer , CharacterActor.TagsAndLayersProfile.oneWayPlatforms )
                )
                {
                    CharacterActor.Velocity = - CharacterActor.Up * 0.5f;
                    CharacterActor.ForceNotGrounded();
                    CharacterActor.Position -= CharacterActor.Up * 0.1f;
                        
                }
                else if( groundedJumpAvailable )
                {                                  
                    groundedJumpAvailable = false;

                    SetJumpVelocity(); 
                    CharacterActor.ForceNotGrounded();

                    if( verticalMovementParameters.jumpReleaseAction == VerticalMovementParameters.JumpReleaseAction.StopJumping )
                    {
                        isJumping = true;
                        jumpTimer = 0f;
                    }

                    

                    CharacterActor.VerticalVelocity = jumpVelocity;

                }
                else
                {
                    

                    if( notGroundedJumpsLeft != 0 )
                    {
                        
                        SetJumpVelocity();                        
                        
                        notGroundedJumpsLeft--;

                        if( verticalMovementParameters.jumpReleaseAction == VerticalMovementParameters.JumpReleaseAction.StopJumping )
                        {
                            isJumping = true;
                            jumpTimer = 0f;
                        }

                        CharacterActor.VerticalVelocity = jumpVelocity;
                    }
                    
                }

                
            }
        }
                
		
    }

    Vector3 jumpVelocity = default( Vector3 );
        

    void SetJumpVelocity()
	{		
        
        Vector3 verticalJumpComponent = CharacterActor.VerticalVelocity;
        Vector3 verticalExtraComponent = verticalMovementParameters.jumpIntertiaMultiplier * CharacterActor.PlanarVelocity;


		if( CharacterActor.IsGrounded )
		{       
            if( CharacterActor.IsStable )
            {
                verticalJumpComponent = CharacterActor.Up * verticalMovementParameters.JumpSpeed;  
                verticalExtraComponent = Vector3.zero;				 

                if( OnGroundedJumpPerformed != null )
                    OnGroundedJumpPerformed();	

            }
            else
            {
                switch( verticalMovementParameters.unstableJumpMode )
                {
                    case VerticalMovementParameters.UnstableJumpMode.Vertical:

                        verticalJumpComponent = CharacterActor.Up * verticalMovementParameters.JumpSpeed;

                        break;
                    case VerticalMovementParameters.UnstableJumpMode.GroundNormal:

                        verticalJumpComponent = CharacterActor.GroundContactNormal * verticalMovementParameters.JumpSpeed;
                        
                        break;
                } 

            }
			
		}	
		else
		{
            verticalJumpComponent = CharacterActor.Up * verticalMovementParameters.JumpSpeed;             

            if( OnNotGroundedJumpPerformed != null )
                OnNotGroundedJumpPerformed( notGroundedJumpsLeft );		
			
		}
        
        jumpVelocity = verticalJumpComponent + verticalExtraComponent;

		if( OnJumpPerformed != null )
			OnJumpPerformed();
		
	}
    
    
    void ProcessVerticalMovement( float dt )
    {       
        if( verticalMovementParameters.useGravity )
            ProcessGravity( dt );
        
        ProcessJump( dt );
        
    }

    

    public override void EnterBehaviour( float dt , CharacterState fromState )
    {
        
        CharacterActor.AlwaysNotGrounded = false;

        targetLookingDirection = CharacterActor.Forward;

        jumpVelocity = Vector3.zero;     
           
        currentPlanarSpeedLimit = Mathf.Max( CharacterActor.PlanarVelocity.magnitude , planarMovementParameters.baseSpeedLimit );

        
    }      


	
    


    protected virtual void HandleRotation( float dt)
    {	
        HandleLookingDirection( dt );
        // HandleUpDirection( dt );
                
    }

    

    Vector3 targetLookingDirection = default( Vector3 );
    
    void HandleLookingDirection( float dt )
    {
        

        if( !CharacterActor.CharacterBody.Is2D && lookingDirectionParameters.followExternalReference )
        {
            targetLookingDirection = CharacterStateController.MovementReferenceForward;
        }
        else
        {
            switch( CharacterActor.CurrentState )
            {
                case CharacterActorState.NotGrounded:

                    if( CharacterActor.PlanarVelocity != Vector3.zero )
                        targetLookingDirection = CharacterActor.PlanarVelocity;
                    
                    break;
                case CharacterActorState.StableGrounded:

                    if( CharacterStateController.InputMovementReference != Vector3.zero )
                        targetLookingDirection = CharacterStateController.InputMovementReference;                   
                    else
                        targetLookingDirection = CharacterActor.Forward;
                    
                    
                    break;
                case CharacterActorState.UnstableGrounded:

                    if( CharacterActor.PlanarVelocity != Vector3.zero )
                        targetLookingDirection = CharacterActor.PlanarVelocity;
                    
                    break;
            }
            
        }  
                

        Quaternion targetDeltaRotation = Quaternion.FromToRotation( CharacterActor.Forward , targetLookingDirection );
        Quaternion currentDeltaRotation = Quaternion.Slerp( Quaternion.identity , targetDeltaRotation , 10 * dt );

        
        if( CharacterActor.CharacterBody.Is2D )
        {            
            CharacterActor.Forward = targetLookingDirection;
        }
        else
        {      
            float angle = Vector3.Angle( CharacterActor.Forward , targetLookingDirection );
            
            if( CustomUtilities.isCloseTo( angle , 180f , 0.5f ) )
            {
                
                CharacterActor.Forward = Quaternion.Euler( 0f , 1f , 0f ) * CharacterActor.Forward;
            } 
                  
            CharacterActor.Forward = currentDeltaRotation * CharacterActor.Forward;
            
        }
        
        
        
    }



    public override void UpdateBehaviour( float dt )
    {
        HandleSize( dt );
        HandleVelocity( dt );        
        HandleRotation( dt );

    }
    
    
    public override void PostUpdateBehaviour( float dt )
    {       
        if( CharacterStateController.Animator == null )
            return;

        if( CharacterStateController.Animator.runtimeAnimatorController == null )
            return;

        if( !CharacterStateController.Animator.gameObject.activeSelf )
            return;
        
        CharacterStateController.Animator.SetBool( groundedParameter , CharacterActor.IsGrounded );
        CharacterStateController.Animator.SetBool( stableParameter , CharacterActor.IsStable );
        CharacterStateController.Animator.SetFloat( verticalSpeedParameter , CharacterActor.LocalVelocity.y );
        CharacterStateController.Animator.SetFloat( planarSpeedParameter , CharacterActor.PlanarVelocity.magnitude );
        CharacterStateController.Animator.SetFloat( horizontalAxisParameter , CharacterActions.movement.value.x );
        CharacterStateController.Animator.SetFloat( verticalAxisParameter , CharacterActions.movement.value.y );	
        CharacterStateController.Animator.SetFloat( heightParameter , CharacterActor.BodySize.y );       
        
    }

    float targetHeight = 1f;

    bool wantToCrouch = false;
    bool isCrouched = false;

    
    protected virtual void HandleSize( float dt )
    {        
        // Want to crouch ---------------------------------------------------------------------    
        if( CharacterActor.IsGrounded && crouchParameters.enableCrouch )
        {
            if( crouchParameters.inputMode == InputMode.Toggle )
            {
                if( CharacterActions.crouch.Started )
                    wantToCrouch = !wantToCrouch;            
            } 
            else
            {
                wantToCrouch = CharacterActions.crouch.value;                
            }

        }
        else
        {
            wantToCrouch = false;
        }

        // Process Size ----------------------------------------------------------------------------        
        targetHeight = wantToCrouch ? CharacterActor.DefaultBodySize.y * crouchParameters.heightRatio : CharacterActor.DefaultBodySize.y;

        Vector3 targetSize = new Vector2( CharacterActor.DefaultBodySize.x , targetHeight );
        
        bool validSize = CharacterActor.SetBodySize( targetSize );
    
        if( validSize )
            isCrouched = wantToCrouch;
        
    }

    PlanarMovementParameters.Motion currentMotion = new PlanarMovementParameters.Motion();

    protected virtual void HandleVelocity( float dt )
    {
        ProcessVerticalMovement( dt );
        ProcessPlanarMovement( dt );
        ProcessSliding( dt );
    }


}
    

}






