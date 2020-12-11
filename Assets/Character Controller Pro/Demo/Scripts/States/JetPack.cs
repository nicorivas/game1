using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Implementation;

namespace Lightbug.CharacterControllerPro.Demo
{


[AddComponentMenu("Character Controller Pro/Demo/Character/States/Jet Pack")]
public class JetPack : CharacterState
{
    [Header("Planar movement")]

    [SerializeField]
    float targetPlanarSpeed = 5f;    

    [Header("Planar movement")]
    
    [SerializeField]    
    float targetVerticalSpeed = 5f;

    [SerializeField]
    float duration = 1f;
    

    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────


    Vector3 smoothDampVelocity = Vector3.zero;


      
    public override string GetInfo()
    {
        return "This state allows the character to imitate a \"JetPack\" type of movement. Basically the character can ascend towards the up direction, " + 
        "but also move in the local XZ plane.";
    }

    public override void EnterBehaviour( float dt , CharacterState fromState )
    {
        CharacterActor.AlwaysNotGrounded = true;

        smoothDampVelocity = CharacterActor.VerticalVelocity;        
        
    }

    public override void ExitBehaviour(float dt, CharacterState toState)
    {
        CharacterActor.AlwaysNotGrounded = false;
    }

    public override void UpdateBehaviour(float dt)
    {        

        // Vertical movement
        CharacterActor.VerticalVelocity = Vector3.SmoothDamp( CharacterActor.VerticalVelocity , targetVerticalSpeed * CharacterActor.Up , ref smoothDampVelocity , duration );
		
        // Planar movement
        CharacterActor.PlanarVelocity = Vector3.Lerp( CharacterActor.PlanarVelocity , targetPlanarSpeed * CharacterStateController.InputMovementReference , 7f * dt );
        
        // Looking direction
        CharacterActor.Forward = CharacterActor.PlanarVelocity;
    }

    public override void CheckExitTransition()
    {
        if( CharacterActor.Triggers.Count != 0 )
        {                               

            if( CharacterActions.interact.Started )
            {   
                CharacterStateController.EnqueueTransition<LadderClimbing>();
            }
            // WIP
            // else
            // {   
            //     CharacterStateController.EnqueueTransition<RopeClimbing>();
            //     CharacterStateController.EnqueueTransition<Swimming>();
                
            // }     
            
        }
        else if( !CharacterActions.jetPack.value )
        {
            CharacterStateController.EnqueueTransition<NormalMovement>();
        }
    }
   
}

}
