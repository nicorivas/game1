using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Implementation
{

public abstract class CharacterAIBehaviour : MonoBehaviour
{
    protected CharacterActions characterActions = new CharacterActions();
    public CharacterActions CharacterActions
    {
        get
        {
            return characterActions;
        }
    }

    public virtual void EnterBehaviour( float dt ){}
    public abstract void UpdateBehaviour( float dt );
    public virtual void ExitBehaviour( float dt ){}

    protected CharacterActor characterActor = null;
    

    protected virtual void Awake()
    {        
        characterActor = this.GetComponentInBranch<CharacterActor>();
    }

    protected void SetMovementAction( Vector3 direction )
	{
		Vector3 inputXZ = Vector3.ProjectOnPlane( direction , characterActor.Up ).normalized;
		inputXZ.y = inputXZ.z;
		inputXZ.z = 0f;			

		characterActions.movement.value = inputXZ;
	}
}

}