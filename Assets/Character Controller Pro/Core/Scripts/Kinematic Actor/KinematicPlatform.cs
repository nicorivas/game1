using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{
    


/// <summary>
/// This class represents a base kinematic platform with no scripted behaviour defined. It is used as a base class for all the kinematic platforms.
/// </summary>
[AddComponentMenu("Character Controller Pro/Core/Kinematic Platform")]
public class KinematicPlatform : KinematicActor
{   
    protected virtual void Start()
    {
        // If there is no sceneController in the scene create one.
		if( SceneController.Instance == null )
		{			
			GameObject sceneController = new GameObject("Scene Controller");			
			sceneController.AddComponent<SceneController>();			
		}

		// Add this actor to the scene controller list
		SceneController.Instance.AddActor( this );
	
    }
}

}
