using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

/// <summary>
/// This component is responsible for updating the position and rotation of all the actors in the scene (character and kinematic objects), guaranteeing a nice interpolated movement.
/// </summary>
[AddComponentMenu("Character Controller Pro/Core/Scene Controller")]
[DefaultExecutionOrder(-100)]
public sealed class SceneController : MonoBehaviour
{
    static SceneController instance = null;
    public static SceneController Instance
    {
        get
        {
            return instance;
        }
    }

    public static void CreateSceneController()
    {
        GameObject sceneController = new GameObject("Scene Controller");			
		sceneController.AddComponent<SceneController>();
    }


    [Tooltip("Disable this field if you want to manually simulate the scene.")]
    [SerializeField]
    bool autoSimulation = true;

    [Tooltip("Whether or not to use the default Unity's interpolation.")]
    [SerializeField]
    bool useInterpolation = true;
    
    List<CharacterActor> characterActors = new List<CharacterActor>();
    List<KinematicPlatform> kinematicPlatforms = new List<KinematicPlatform>();
  
    #region events

    public event System.Action<float> OnSimulationStart;
    public event System.Action<float> OnSimulationEnd;
    public event System.Action<float> OnCharacterSimulationStart;
    public event System.Action<float> OnCharacterSimulationEnd;

    #endregion

    void Awake()
    {
        if( instance == null )
        {
            instance = this;
        }
        else
        {
            Destroy( gameObject );
            return;
        }
             
    }

    // Add actor -----------------------------------------------

    public void AddActor( CharacterActor characterActor )
    {
        characterActors.Add( characterActor );
    }

    public void AddActor( KinematicPlatform kinematicPlatform )
    {
        kinematicPlatforms.Add( kinematicPlatform );
    }

    // Remove actor -----------------------------------------------
    public void RemoveActor( CharacterActor characterActor )
    {
        characterActors.Remove( characterActor );
    }


    public void RemoveActor( KinematicPlatform kinematicPlatform )
    {
        kinematicPlatforms.Remove( kinematicPlatform );
    }


    void InterpolateRigidbodyComponent( RigidbodyComponent rigidbodyComponent )
    {
            
        Vector3 startPosition = rigidbodyComponent.transform.position;  
        Vector3 endPosition = rigidbodyComponent.Position;
        
        Quaternion startRotation = rigidbodyComponent.transform.rotation;
        Quaternion endRotation = rigidbodyComponent.Rotation;

        rigidbodyComponent.Position = startPosition;
        rigidbodyComponent.Rotation = startRotation;

        rigidbodyComponent.Interpolate( endPosition , endRotation );
    }

    void FixedUpdate()
    {
        if( !autoSimulation )
            return;
        
        float dt = Time.deltaTime;
        
        
        Simulate( dt );        
    }

    /// <summary>
    /// Updates and interpolates all the actors in the scene.
    /// </summary>
    public void Simulate( float dt )
    {
        if( OnSimulationStart != null )
            OnSimulationStart( dt );
        
        for( int i = 0 ; i < kinematicPlatforms.Count ; i++ )
        {
            KinematicPlatform kinematicPlatform = kinematicPlatforms[i];

            if( !kinematicPlatform.enabled )
                continue;
            
            kinematicPlatform.UpdateKinematicActor( dt );
        }

        if( OnCharacterSimulationStart != null )
            OnCharacterSimulationStart( dt );
        
        for( int i = 0 ; i < characterActors.Count ; i++ )
        {      
            CharacterActor characterActor = characterActors[i];

            
            if( !characterActor.enabled )
                continue;
            
            characterActor.UpdateCharacter( dt );            
        }   

        if( OnCharacterSimulationEnd != null )
            OnCharacterSimulationEnd( dt );     

        


        // Interpolation ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        if( useInterpolation )
        {
        
            for( int i = 0 ; i < kinematicPlatforms.Count ; i++ )
            {
                KinematicPlatform kinematicPlatform = kinematicPlatforms[i];  

                if( !kinematicPlatform.enabled )
                    continue;

                InterpolateRigidbodyComponent( kinematicPlatform.RigidbodyComponent );
            }     

            // for( int i = 0 ; i < characterActors.Count ; i++ )
            // {
            //     CharacterActor characterActor = characterActors[i];  

            //     if( !characterActor.enabled || !characterActor.IsKinematic )
            //         continue;

            //     InterpolateRigidbodyComponent( characterActor.RigidbodyComponent );
            // }  



        }


        if( OnSimulationEnd != null )
            OnSimulationEnd( dt );

    }
    
}


}