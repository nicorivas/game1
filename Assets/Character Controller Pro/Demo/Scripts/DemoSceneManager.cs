using UnityEngine;
using Lightbug.Utilities;
using Lightbug.CharacterControllerPro.Core;

namespace Lightbug.CharacterControllerPro.Demo
{

public class DemoSceneManager : MonoBehaviour
{
    [Header("Character")]

    [SerializeField]
    CharacterActor characterActor = null;
    

    [Header("Scene references")]

    [SerializeField]
    CharacterReferenceObject[] references = null;

    [Header("UI")]

    [SerializeField]
    Canvas infoCanvas = null;

    [SerializeField]
    bool hideAndConfineCursor = true;

    [Header("Graphics")]

    [Tooltip("Whether or not to show the capsule shape or the animated model.")]
    [SerializeField]
    bool showCapsule = false;

    [SerializeField]
    GameObject capsuleObject = null;

    [SerializeField]
    GameObject graphicsObject = null;

    [Header("Camera")]

    [SerializeField]
    new Camera3D camera = null; 
    
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    
    NormalMovement normalMovement = null;

    void Awake()
    {
        if( characterActor != null )
            normalMovement = characterActor.GetComponentInChildren<NormalMovement>();

        HandleVisualObjects( showCapsule );

        Cursor.visible = !hideAndConfineCursor;
        Cursor.lockState = hideAndConfineCursor ? CursorLockMode.Locked : CursorLockMode.None;
        
    }

    void Update()
    {
        int index = 0;

        for( index = 0 ; index < references.Length ; index++ )
        {        
            if( references[index] == null )
                break;
            
            if( Input.GetKeyDown( KeyCode.Alpha1 + index ) || Input.GetKeyDown( KeyCode.Keypad1 + index ) )
            {
                GoTo( references[index] );
                break;
            }
        }

        if( Input.GetKeyDown( KeyCode.Tab ) )
        {
            if( infoCanvas != null )
                infoCanvas.enabled = !infoCanvas.enabled;
        }


        if( Input.GetKeyDown( KeyCode.T ) )
        {
            showCapsule = !showCapsule;

            HandleVisualObjects( showCapsule );
        }

        if( Input.GetKeyDown( KeyCode.V ) )
        {
            // If the Camera3D is present, change between First person and Third person mode.
            if( camera != null )
            {
                camera.ToggleCameraMode();

                // If the NormalMovement state is present, follow the camera forward in first person mode.
                if( normalMovement != null )
                {
                    normalMovement.FollowExternalReference = !normalMovement.FollowExternalReference;
                }
            }

            
        }
        
    }

    

    void HandleVisualObjects( bool showCapsule )
    {
        if( capsuleObject != null )
            capsuleObject.SetActive( showCapsule );
            
        if( graphicsObject != null )
            graphicsObject.SetActive( !showCapsule );
        
    }

    void GoTo( CharacterReferenceObject reference )
    {
        if( reference == null )
            return;        
        
        if( characterActor == null )
            return;
        
        characterActor.VerticalAlignmentDirection = reference.referenceTransform.up;
        characterActor.Teleport( reference.referenceTransform );

        characterActor.VerticalAlignmentReference = reference.verticalAlignmentReference;
        characterActor.VerticalReferenceMode = VerticalAlignmentSettings.VerticalReferenceMode.Away;
        
    }
}

}
