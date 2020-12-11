namespace Lightbug.CharacterControllerPro.Core
{
	

/// <summary>
/// This class contain all the constants used for collision detection, steps detection, ground probing, etc. All the values were carefully chosen, this means that it is not recommended to modify these values at all,
/// however if you need to do it do so at your own risk (make a backup before).
/// </summary>
public class CharacterConstants
{
    /// <summary>
	/// Distance between the origins of the upper and lower edge detection rays.
	/// </summary>
	public const float EdgeRaysSeparation = 0.02f;

	/// <summary>
	/// Cast distance used for the raycasts in the edge detection algorithm.
	/// </summary>
	public const float EdgeRaysCastDistance = 2f;

    /// <summary>
	/// Distance between the collider and the inner 
	/// </summary>
	public const float SkinWidth = 0.01f;   

	/// <summary>
	/// Minimum offset applied to the bottom of the capsule (upwards) to avoid contact with the ground.
	/// </summary>
	public const float ColliderMinBottomOffset = 0.02f;
	    

    /// <summary>
	/// Minimum amount of movement tolerated. Any displacement magnitude less that this amount will not be calculated, 
	/// hence not movement will be produced.
	/// </summary>
	public const float MinMovementAmount = 0.001f;
	
	/// <summary>
	/// Minimum angle between upper and lower normals (from the edge detector) that defines an edge.
	/// </summary>
	public const float MinEdgeAngle = 0.5f;

	/// <summary>
	/// Maximum angle between upper and lower normals (from the edge detector) that defines an edge.
	/// </summary>
	public const float MaxEdgeAngle = 170f;

	/// <summary>
	/// Minimum angle between upper and lower normals (from the edge detector) that defines a step.
	/// </summary>
	public const float MinStepAngle = 85f;

	/// <summary>
	/// Maximum angle between upper and lower normals (from the edge detector) that defines a step.
	/// </summary>
	public const float MaxStepAngle = 95f;

	/// <summary>
	/// Base distance used for ground probing. The effective ground probing distance will be the maxmimum between 
	/// this value and "step down distance".
	/// </summary>
	public const float GroundCheckDistance = 0.1f;

	/// <summary>
	/// Tolerance measured in degrees (0 ± MaxWallDetectionTolerance) used to determine if the character is colliding directly towards a wall .
	/// </summary>
	public const float MaxWallDetectionTolerance = 5f;

	/// <summary>
	/// Maximum number of iterations available for the collide and slide algorithm. This is used in some cases to allow the loop to end.
	/// </summary>
	public const int MaxSlideIterations = 3;	


	/// <summary>
	/// After the "step up" movement has been calculated, if the difference in magnitude between the resulting position and the initial position is greater
	/// than this value, the step up process with be successful. 
	/// </summary>
	public const float MinStepUpDifference = 0.001f;

	/// <summary>
	/// The extra distance added to the original displacement when performing a step up. If the character has problems climbing steps try to increase this value. 
	/// A high value will produce a more noticeable effect, which is no good.
	/// </summary>
	public const float StepExtraMovement = 0.1f;//2.5f * SkinWidth;


	public const float ExtraUnstableCheckDistance = 5f * CharacterConstants.SkinWidth;


	/// <summary>
	/// Length used for the collision info containers. These are arrays used for storing the information gathered from the physics queries, 
	/// collision messages, etc.
	/// </summary>
	public const int HitCollidersBufferLength = 10;

	/// <summary>
	/// Length used for the external velocites array.
	/// </summary>
	public const int ExternalVelocitiesBufferLength = 10;

	/// <summary>
	/// The default gravity value used by the weight function.
	/// </summary>
	public const float DefaultGravity = 9.8f;

	
}


}