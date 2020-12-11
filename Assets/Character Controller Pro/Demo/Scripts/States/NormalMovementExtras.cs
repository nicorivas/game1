using UnityEngine;
using Lightbug.Utilities;


namespace Lightbug.CharacterControllerPro.Demo
{

[System.Serializable]
public class PlanarMovementParameters
{
    [Tooltip("Base speed used for the planar movement.")]
    [Min( 0f )]
    public float baseSpeedLimit = 6f;    
    
    [Tooltip("\"Toggle\" will activate/deactivate the action when the input is \"pressed\". On the other hand, \"Hold\" will activate the action when the input is pressed, " + 
    "and deactivate it when the input is \"released\".")]
    public InputMode runInputMode = InputMode.Hold;

    [Min( 0f )]
    public float boostSpeedLimit = 9f;


    [CustomClassDrawer]
    public Motion stableMotion = new Motion( 50f , 40f );

    [CustomClassDrawer]
    public Motion unstableMotion = new Motion( 10f , 2f );

    [CustomClassDrawer]
    public Motion notGroundedMotion = new Motion( 10f , 5f );


    [System.Serializable]
    public struct Motion
    {
        public float acceleration;
        public float deceleration;

        public Motion( float acceleration , float deceleration )
        {
            this.acceleration = acceleration;
            this.deceleration = deceleration;
        }
    }

}


[System.Serializable]
public class VerticalMovementParameters
{    

    public enum UnstableJumpMode
    {
        Vertical ,
        GroundNormal
    }

    public enum JumpReleaseAction
    {
        Disabled ,
        StopJumping
    }
    
    
    
    [Header("Gravity")]

    [Tooltip("It enables/disables gravity. The gravity value is calculated based on the jump apex height and duration")]
    public bool useGravity = true;

    [Header("Jump")]

    public bool canJump = true;

    [Tooltip("The height reached at the apex of the jump. The maximum height will depend on the \"jumpCancellationMode\".")]
    [Min(0f)]
    public float jumpApexHeight = 1f;

    [Tooltip("The amount of time to reach the \"base height\" (apex).")]
    [Min(0f)]
    public float jumpApexDuration = 0.3f;

    [Min(0)]
    [Tooltip("Number of jumps available for the character in the air.")]
	public int availableNotGroundedJumps = 1;

    public bool canJumpOnUnstableGround = false;

    [Tooltip("If the character is not grounded, and the \"not grounded time\" is less or equal than this value, the jump action will be performed as a grounded jump.")]
    [Min(0f)]
    public float coyoteTime = 0.1f;
    
    [Tooltip("How should the character jump on unstable ground?\n\nVertical = the jump is performed considering only the up direction.\nGroundNormal = the jump is performed considering the up direction and the ground normal.")]
    public UnstableJumpMode unstableJumpMode = UnstableJumpMode.GroundNormal;

    [Tooltip("How much of the current slide velocity (unstable movement) is converted into the jump velocity.")]
    [Range( 0f , 1f )]
    public float jumpIntertiaMultiplier = 0.5f;

    [Space] 

    [Tooltip("How the release of the jump input affects the jump.\n" +         
        "Disabled = no action at all.\n" + 
        "StopJumping = the vertical velocity will be maintained as \"jumpVelocity\" for a certain duration of time. This will produce a higher jump than the base one (\"KeepJumping\" jump height >= base jump height).\n"
    )]
    public JumpReleaseAction jumpReleaseAction = JumpReleaseAction.StopJumping;

    [Tooltip("[Only for the \"StopJumping\" action] It represents the amount of seconds in which the character will maintain the vertical velocity constant (as long as the jump input is being held down).")]
	[Min( 0f )] 
    public float constantJumpDuration = 0.2f;


    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    float gravityMagnitude = 10f;
    
    public float GravityMagnitude
    {
        get
        {
            return gravityMagnitude;
        }
    }

    float jumpSpeed = 10f;

    public void UpdateParameters( float positiveGravityMultiplier )
    { 
        gravityMagnitude = positiveGravityMultiplier * ( ( 2 * jumpApexHeight ) / Mathf.Pow( jumpApexDuration , 2 ) );
        jumpSpeed = gravityMagnitude * jumpApexDuration;
    }

    public float JumpSpeed 
    {
        get
        {
            return jumpSpeed;
        }
    }

}

[System.Serializable]
public class CrouchParameters
{   

    public bool enableCrouch = true;

    [Tooltip("This multiplier represents the crouch ratio relative to the default height.")]
    [Min( 0f )]
    public float heightRatio = 0.75f;

    [Tooltip("How much the crouch action affects the movement speed?.")]
    [Min( 0f )]
    public float speedMultiplier = 0.3f;


    [Tooltip("\"Toggle\" will activate/deactivate the action when the input is \"pressed\". On the other hand, \"Hold\" will activate the action when the input is pressed, " + 
    "and deactivate it when the input is \"released\".")]
    public InputMode inputMode = InputMode.Hold;

}

    

[System.Serializable]
public class LookingDirectionParameters
{    
    public bool followExternalReference = true; 

}




public enum InputMode
{
    Toggle ,
    Hold
}


}
