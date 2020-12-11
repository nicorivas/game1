namespace Lightbug.CharacterControllerPro.Implementation
{

/// <summary>
/// This struct contains all the inputs actions available for the character to interact with.
/// </summary>
[System.Serializable]
public struct CharacterActions 
{
    
    // Bool actions
	public BoolAction @jump;
	public BoolAction @run;
	public BoolAction @interact;
	public BoolAction @jetPack;
	public BoolAction @dash;
	public BoolAction @crouch;


    // Float actions


    // Vector2 actions
	public Vector2Action @movement;


     
    /// <summary>
    /// Reset all the actions.
    /// </summary>
	public void Reset()
	{
		@jump.Reset();
		@run.Reset();
		@interact.Reset();
		@jetPack.Reset();
		@dash.Reset();
		@crouch.Reset();


		@movement.Reset();

	}

    /// <summary>
    /// Initialize the actions by instantiate them.
    /// </summary>
    public void InitializeActions()
    {
		@jump = new BoolAction();
		@run = new BoolAction();
		@interact = new BoolAction();
		@jetPack = new BoolAction();
		@dash = new BoolAction();
		@crouch = new BoolAction();


		@movement = new Vector2Action();

    }

    /// <summary>
    /// Updates the values of all the actions based on the current input handler (human).
    /// </summary>
    public void SetValues( InputHandler inputHandler )
    {
        if( inputHandler == null )
			return;
        
		@jump.value = inputHandler.GetBool( "Jump" );
		@run.value = inputHandler.GetBool( "Run" );
		@interact.value = inputHandler.GetBool( "Interact" );
		@jetPack.value = inputHandler.GetBool( "Jet Pack" );
		@dash.value = inputHandler.GetBool( "Dash" );
		@crouch.value = inputHandler.GetBool( "Crouch" );


		@movement.value = inputHandler.GetVector2( "Movement" );

    }

    /// <summary>
    /// Copies the values of all the actions from an existing set of actions.
    /// </summary>
    public void SetValues( CharacterActions characterActions )
    {	
		@jump.value = characterActions.jump.value;
		@run.value = characterActions.run.value;
		@interact.value = characterActions.interact.value;
		@jetPack.value = characterActions.jetPack.value;
		@dash.value = characterActions.dash.value;
		@crouch.value = characterActions.crouch.value;


		@movement.value = characterActions.movement.value;

    }

    /// <summary>
	/// Update all the actions internal states.
	/// </summary>
    public void Update()
    {
		@jump.Update();
		@run.Update();
		@interact.Update();
		@jetPack.Update();
		@dash.Update();
		@crouch.Update();

    }


}


}