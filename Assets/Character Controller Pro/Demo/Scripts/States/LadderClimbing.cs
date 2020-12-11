using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{

[AddComponentMenu("Character Controller Pro/Demo/Character/States/Ladder Climbing")]
public class LadderClimbing : CharacterState
{       

    [Header("Animation")]

    [SerializeField]
    string bottomDownParameter = "BottomDown";

    [SerializeField]
    string bottomUpParameter = "BottomUp";

    [SerializeField]
    string topDownParameter = "TopDown";

    [SerializeField]
    string topUpParameter = "TopUp";

    [SerializeField]
    string upParameter = "Up";

    [SerializeField]
    string downParameter = "Down";

    [Header("Offset")]

    [SerializeField]
    bool useIKOffsetValues = false;

    [SerializeField]
    Vector3 rightFootOffset = Vector3.zero;

    [SerializeField]
    Vector3 leftFootOffset = Vector3.zero;

    [SerializeField]
    Vector3 rightHandOffset = Vector3.zero;

    [SerializeField]
    Vector3 leftHandOffset = Vector3.zero;

    [Header("Activation")]

    [SerializeField]
    bool filterByAngle = true;

    [SerializeField]
    float maxAngle = 70f;

    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    Dictionary< Transform , Ladder > ladders = new Dictionary<Transform, Ladder>();

    enum LadderClimbState
    {
        Entering ,
        AnimationBased ,
    }

    LadderClimbState state;

    Ladder currentLadder = null;
    
    Vector3 targetPosition = Vector3.zero;
    
    int currentClimbingAnimation = 0;

    bool forceExit = false;
      
    
	public override void CheckExitTransition()
    {         
        if( forceExit )
            CharacterStateController.EnqueueTransition<NormalMovement>();      
	}

    public Dictionary< Transform , Ladder > Ladders
	{
		get
		{
			return ladders;
		}
	}

	public override bool CheckEnterTransition( CharacterState fromState )
    {    
        if( !CharacterActor.IsGrounded )
            return false;

        for( int i = 0 ; i < CharacterActor.Triggers.Count ; i++ )
        {
            Trigger trigger = CharacterActor.Triggers[i];

            Ladder ladder = ladders.GetOrRegisterValue< Transform , Ladder >( trigger.transform );

            if( ladder != null )
            {   
                currentLadder = ladder;

                // Check if the character is closer to the top or the bottom
                float distanceToTop = Vector3.Distance( CharacterActor.Position , currentLadder.TopReference.position );
                float distanceToBottom = Vector3.Distance( CharacterActor.Position , currentLadder.BottomReference.position );

                isBottom = distanceToBottom < distanceToTop;

                if( filterByAngle )
                {
                    Vector3 ladderToCharacter = CharacterActor.Position - currentLadder.transform.position;
                    ladderToCharacter = Vector3.ProjectOnPlane( ladderToCharacter , currentLadder.transform.up );
                    
                    float angle = Vector3.Angle( currentLadder.FacingDirectionVector , ladderToCharacter );

                    if( isBottom )
                    {
                        if( angle >= maxAngle )
                            return true;
                        else
                            continue;
                    }
                    else
                    {
                        if( angle <= maxAngle )
                            return true;
                        else
                            continue;
                    }
                    

                }
                else
                {
                    return true;
                }
            }

        }   

        return false;
	}

    bool isBottom = false;

    
    public override void EnterBehaviour( float dt , CharacterState fromState )
    {
        
        CharacterActor.IsKinematic = true;
        CharacterActor.AlwaysNotGrounded = true;
        CharacterActor.Velocity = Vector3.zero;  
                
        
        currentClimbingAnimation = isBottom ? 0 : currentLadder.ClimbingAnimations;

        targetPosition = isBottom ? currentLadder.BottomReference.position : currentLadder.TopReference.position;

        CharacterActor.Forward = currentLadder.FacingDirectionVector;

        state = LadderClimbState.Entering;
        
        
    }

    public override void ExitBehaviour( float dt , CharacterState toState )
    {
        forceExit = false;
        CharacterActor.IsKinematic = false;
        CharacterStateController.UseRootMotion = false;
        CharacterActor.AlwaysNotGrounded = false;
        currentLadder = null;

        CharacterStateController.ResetIKWeights();

        CharacterActor.Velocity =  Vector3.zero;
    }

    protected override void Awake()
    {
        base.Awake();

        Ladder[] laddersArray = FindObjectsOfType<Ladder>();
        for( int i = 0 ; i < laddersArray.Length ; i++ )
            ladders.Add( laddersArray[i].transform , laddersArray[i] );
    }

    public override void Initialize()
    {
        base.Initialize();

        if( CharacterStateController.Animator == null )
        {
            Debug.Log("The LadderClimbing state needs the character to have a reference to an Animator component. Destroying this state...");
            Destroy(this);
        }     
        
    }  


    public override void UpdateBehaviour( float dt )
    {
        
        switch( state )
        {
            case LadderClimbState.Entering:                

                CharacterActor.Position = targetPosition;
                CharacterStateController.UseRootMotion = true;
                CharacterStateController.Animator.SetTrigger( isBottom ? bottomUpParameter : topDownParameter );
                
                state = LadderClimbState.AnimationBased;

                break;
            
            case LadderClimbState.AnimationBased:
                
                if( CharacterStateController.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") )
                {
                    if( CharacterActions.interact.Started )
                    {
                        forceExit = true;                        
                    }
                    else
                    {
                        if( CharacterActions.movement.Up )
                        {
                            if( currentClimbingAnimation == currentLadder.ClimbingAnimations )
                            {
                                CharacterStateController.Animator.SetTrigger( topUpParameter );
                            }
                            else
                            {
                                CharacterStateController.Animator.SetTrigger( upParameter );
                                currentClimbingAnimation++;
                            }
                        }
                        else if( CharacterActions.movement.Down )
                        {
                            if( currentClimbingAnimation == 0 )
                            {
                                CharacterStateController.Animator.SetTrigger( bottomDownParameter );
                            }
                            else
                            {
                                CharacterStateController.Animator.SetTrigger( downParameter );
                                currentClimbingAnimation--;
                            }
                        }
                        
                        
                    }

                }
                else if( CharacterStateController.Animator.GetCurrentAnimatorStateInfo(0).IsName("Entry") )
                {
                    forceExit = true;
                    CharacterActor.ForceGrounded();
                }
                
                break;

        }


        
    }
    

    
    

    public override void UpdateIK( int layerIndex )
    {
        if( !useIKOffsetValues )
            return;

        UpdateIKElement( AvatarIKGoal.LeftFoot , leftFootOffset );
        UpdateIKElement( AvatarIKGoal.RightFoot , rightFootOffset );
        UpdateIKElement( AvatarIKGoal.LeftHand , leftHandOffset );
        UpdateIKElement( AvatarIKGoal.RightHand , rightHandOffset );

    }

    void UpdateIKElement( AvatarIKGoal avatarIKGoal , Vector3 offset )
    {
        // Get the original (weight = 0) ik position.
        CharacterStateController.Animator.SetIKPositionWeight( avatarIKGoal , 0f );
        Vector3 originalRightFootPosition = CharacterStateController.Animator.GetIKPosition( avatarIKGoal );

        // Affect the original ik position with the offset.
        CharacterStateController.Animator.SetIKPositionWeight( avatarIKGoal , 1f );
        CharacterStateController.Animator.SetIKPosition( avatarIKGoal , originalRightFootPosition + offset );
    }

        
}

}
