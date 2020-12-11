using UnityEngine;

namespace Lightbug.CharacterControllerPro.Core
{

/// <summary>
/// This (very important) abstract class represents the root behaviour that will define all the properties of the CharacterActor. 
/// It can be also defined as the main interface between the core and the implementation parts. For instance, the CharacterStateController is a CharacterActorBehaviour component. 
/// The CharacterActor component will execute its CharacterActorBehaviour instance before anything else. 
/// 
/// Implement this abstract class with your own behaviour code to define the character. 
/// </summary>
public abstract class CharacterActorBehaviour : MonoBehaviour
{
    /// <summary>
    /// The CharacterActor instance associated with this class.
    /// </summary>
	protected CharacterActor characterActor = null;

    /// <summary>
    /// Gets the character actor associated with this behaviour.
    /// </summary>
    public CharacterActor CharacterActor
    {
        get
        {
            return characterActor;
        }
    }

	public virtual void Initialize( CharacterActor characterActor )
    {
        this.characterActor = characterActor;
        
    }

	/// <summary>
	/// Main update method for the CharacterActorBehaviour component.
	/// </summary>
	public abstract void UpdateBehaviour( float dt );
}


}
