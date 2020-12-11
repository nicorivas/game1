using System.Collections;
using System.Collections.Generic;
using Lightbug.Utilities;
using UnityEngine;

namespace Lightbug.CharacterControllerPro.Core
{

public abstract class CharacterGraphics : MonoBehaviour
{
    protected CharacterActor characterActor = null; 

    protected virtual void OnValidate()
    {
        CharacterActor characterActor = this.GetComponentInBranch<CharacterActor>();

        if( characterActor == null )
            Debug.Log("Warning: No CharacterActor component detected in the root object.");
    }

    protected virtual void Awake()
    {
        characterActor = this.GetComponentInBranch<CharacterActor>();
    }

    
}

}
