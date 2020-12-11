using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.CharacterControllerPro.Implementation
{

/// <summary>
/// This input handler implements the input detection for UI elements (mobile UI).
/// </summary>
public class UIInputHandler : InputHandler
{    
    
    Dictionary< string , InputButton > inputButtons = new Dictionary< string , InputButton >();
    Dictionary< string , InputAxes > inputAxes = new Dictionary< string , InputAxes >();

    void Awake()
    {
        InputButton[] inputButtonsArray = GameObject.FindObjectsOfType<InputButton>();
        for( int i = 0 ; i < inputButtonsArray.Length ; i++ )
            inputButtons.Add( inputButtonsArray[i].ActionName , inputButtonsArray[i] );  


        InputAxes[] inputAxesArray = GameObject.FindObjectsOfType<InputAxes>();
        for( int i = 0 ; i < inputAxesArray.Length ; i++ )
            inputAxes.Add( inputAxesArray[i].ActionName , inputAxesArray[i] );
        
    }

    public override bool GetBool( string actionName )
	{
        InputButton inputButton;
        bool found = inputButtons.TryGetValue( actionName , out inputButton );

        if( !found )
            return false;
        else
		    return inputButton.ButtonValue;
	}

    public override float GetFloat( string actionName )
	{        
		return 0f;
	}	

    public override Vector2 GetVector2( string actionName )
	{        
		InputAxes element;
        bool found = inputAxes.TryGetValue( actionName , out element );

        if( !found )
            return Vector2.zero;
        else
		    return element.AxesValue;
	}

	
}

}
