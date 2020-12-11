// ------------------------------------------------------------------------------------------
// If you want to use the new input system (using this "input handler") just uncomment this code (delete the /* and */)
// This exact same code can be found in the "How to..." section from the online documentation.
// ------------------------------------------------------------------------------------------

/*

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lightbug.CharacterControllerPro.Implementation
{

public class InputSystemHandler : InputHandler
{
    [SerializeField]
    InputActionAsset characterActionsAsset = null;

    void Awake()
    {
        characterActionsAsset.Enable();
    }

    public override float GetFloat( string actionName )
    {       
        return characterActionsAsset.FindAction( actionName ).ReadValue<float>();     
    }

    public override bool GetBool( string actionName )
    { 
        return characterActionsAsset.FindAction( actionName ).ReadValue<float>() >= InputSystem.settings.defaultButtonPressPoint;  
    }

    public override Vector2 GetVector2( string actionName )
    {
        return characterActionsAsset.FindAction( actionName ).ReadValue<Vector2>(); 
    }

}

}

*/



