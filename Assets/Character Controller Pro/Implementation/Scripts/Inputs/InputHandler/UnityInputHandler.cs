using UnityEngine;

namespace Lightbug.CharacterControllerPro.Implementation
{

/// <summary>
/// This input handler implements the input detection following the Unity's Input Manager convention. This scheme is used for desktop games.
/// </summary>
public class UnityInputHandler : InputHandler
{
	public override bool GetBool( string actionName )
	{
		bool output = false;
		try
		{
			output = Input.GetButton( actionName );
		}
		catch ( System.Exception )
		{
			PrintInputWarning( actionName );
		}

		return output;
	}

    public override float GetFloat( string actionName )
	{
		float output = default( float );
		try
		{
			output = Input.GetAxis( actionName );
		}
		catch ( System.Exception )
		{
			PrintInputWarning( actionName );
		}

		return output;		
	}

	public override Vector2 GetVector2( string actionName )
	{
		// Not officially supported	
		// Example : "Movement"  splits into "Movement X" and "Movement Y"

		Vector2 output = default( Vector2 );
		try
		{
			output = new Vector2( Input.GetAxis( actionName + " X" ) , 	Input.GetAxis( actionName + " Y" ) );
		}
		catch ( System.Exception )
		{
			PrintInputWarning( actionName + " X" , actionName + " Y" );
		}

		return output;
	}

	void PrintInputWarning( string actionName )
	{
		Debug.LogWarning( $"{actionName} action not found! Please make sure this action is included in your input settings (axis). If you're only testing the demo scenes from " + 
		"Character Controller Pro please load the input preset included at \"Character Controller Pro/OPEN ME/Presets/.");
	}

	void PrintInputWarning( string actionXName , string actionYName )
	{
		Debug.LogWarning( $"{actionXName} and/or {actionYName} actions not found! Please make sure both of these actions are included in your input settings (axis). If you're only testing the demo scenes from " + 
		"Character Controller Pro please load the input preset included at \"Character Controller Pro/OPEN ME/Presets/." );
	}
}

}
