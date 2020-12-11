using UnityEngine;

namespace Lightbug.CharacterControllerPro.Core
{
	
/// <summary>
/// This ScriptableObject contains of the info related to the layers and tags used by the CharacterActor component.
/// </summary>
[CreateAssetMenu( menuName = "Character Controller Pro/Core/Layers Profile" )]
public class CharacterTagsAndLayersProfile : ScriptableObject
{
	[Header("Layers")]	

    [Tooltip("Assign all the static geometry layers. These objects will be treated as infinite mass objects")]
	public LayerMask staticObstaclesLayerMask;

	[Tooltip("Same as static obstacles, but these objects will be treated as valid moving ground.")]	
	public LayerMask dynamicGroundLayerMask;
	
	[Tooltip("It's recommended to assign all the interactable dynamic rigidbodies inside this LayerMask. They will be treated as pushable objects, but also as valid ground.")]
	public LayerMask dynamicRigidbodiesLayerMask;

	[Tooltip("If you have a platform effector 2D, assign its layer in this layerMask (2D only).")]	
	public LayerMask oneWayPlatforms;

	[Header("Tags")]

	[Tooltip("This tag is used when the CharacterActor needs to apply a force towards the ground (simualting weight), and the \"filterByTag\" option is enabled.")]	
	public string weightRigidbodiesTag = "WeightRigidbody";
}

}
