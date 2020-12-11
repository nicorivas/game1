using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Implementation
{	


/// <summary>
/// This class represents a state, that is, a basic element used by the character state controller (finite state machine).
/// </summary>
// [RequireComponent( typeof( CharacterStateController ) )]
public abstract class CharacterState : MonoBehaviour , IUpdatable
{     
     [SerializeField]
     RuntimeAnimatorController runtimeAnimatorController = null;
  

     public int StateNameHash{ get; private set; }

     /// <summary>
     /// Gets the state runtime animator controller.
     /// </summary>
     public RuntimeAnimatorController RuntimeAnimatorController
     {
          get
          {                              
               return runtimeAnimatorController;
          }
     }
     
     /// <summary>
     /// Gets the CharacterActor component of the gameObject.
     /// </summary>
     public CharacterActor CharacterActor{ get; private set; }
     
     /// <summary>
     /// Gets the CharacterBrain component of the gameObject.
     /// </summary>
     // public CharacterBrain CharacterBrain{ get; private set; }
     CharacterBrain CharacterBrain = null;

     /// <summary>
     /// Gets the current brain actions CharacterBrain component of the gameObject.
     /// </summary>
     public CharacterActions CharacterActions
     {
          get
          {               
               return CharacterBrain == null ? new CharacterActions() : CharacterBrain.CharacterActions;
          }
     }

     /// <summary>
     /// Gets the CharacterStateController component of the gameObject.
     /// </summary>
     public CharacterStateController CharacterStateController{ get; private set; }
     // {
     //      get
     //      {               
     //           return characterStateController;
     //      }
     // }


     protected virtual void Awake()
     {          
          CharacterActor = this.GetComponentInBranch<CharacterActor>();
          CharacterStateController = this.GetComponentInBranch< CharacterActor , CharacterStateController>();          
          CharacterBrain = this.GetComponentInBranch< CharacterActor , CharacterBrain>();
     }

     
     /// <summary>
     /// This initialization method is called by the state controller, after everything else is initialized.
     /// </summary>
     public virtual void Initialize()
     {    
          StateNameHash = Animator.StringToHash( this.GetType().Name );    
     }

     // This is used to trick the component, allowing to enable/disable it via the Editor ... ;)
     void OnEnable()
     {

     }


     /// <summary>
     /// This method runs once when the state has entered the state machine.
     /// </summary>
     public virtual void EnterBehaviour( float dt , CharacterState fromState )
     {          
     }

     /// <summary>
     /// This methods runs before the main Update method.
     /// </summary>
     public virtual void PreUpdateBehaviour( float dt )
     {
     }

     /// <summary>
     /// This method runs frame by frame, and should be implemented by the derived state class.
     /// </summary>
     public abstract void UpdateBehaviour( float dt );

     /// <summary>
     /// This methods runs after the main Update method.
     /// </summary>
     public virtual void PostUpdateBehaviour( float dt )
     {
     }

     /// <summary>
     /// This method runs once when the state has exited the state machine.
     /// </summary>
     public virtual void ExitBehaviour( float dt , CharacterState toState)
     {
     }

     /// <summary>
     /// Checks if the required conditions to exit this state are true. If so it returns the desired state (null otherwise). After this the state machine will
     /// proceed to evaluate the "enter transition" condition on the target state.
     /// </summary>
     public virtual void CheckExitTransition()
     {
     }

     /// <summary>
     /// Checks if the required conditions to enter this state are true. If so the state machine will automatically change the current state to the desired one.
     /// </summary>  
     public virtual bool CheckEnterTransition( CharacterState fromState )
     {
          return true;
     }

     /// <summary>
     /// This methods runs after getting all the ik positions, rotations and their respective weights. Use it to modify the ik data of the humanoid rig.
     /// </summary>
     public virtual void UpdateIK( int layerIndex )
     {          
     }
    
     public virtual string GetInfo()
     {
          return "";
     }
	
}

}
