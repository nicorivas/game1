using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Implementation;

namespace Lightbug.CharacterControllerPro.Demo
{

/// <summary>
/// A "KinematicPlatform" implementation whose movement and rotation are defined by a single movement or rotation action. Use this component if you want to create
/// platforms with a pendulous nature, or infinite duration actions (for instance, if the platform should rotate forever).
/// </summary>
[AddComponentMenu("Character Controller Pro/Demo/Kinematic Platform/Action Based Platform")]
public class ActionBasedPlatform : KinematicPlatform
{
    [SerializeField]
    protected MovementAction movementAction = new MovementAction();

    [SerializeField]
    protected RotationAction rotationAction = new RotationAction();

    
    public override void UpdateKinematicActor( float dt )
    {
        Vector3 position = RigidbodyComponent.Position;
        Quaternion rotation = RigidbodyComponent.Rotation;

        movementAction.Tick( dt , ref position );
        rotationAction.Tick( dt , ref position , ref rotation );
        //Debug.Log(position+" "+rotation);
        RigidbodyComponent.SetPositionAndRotation( position , rotation );
    }

    

}

}
