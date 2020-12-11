using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Implementation;

namespace Lightbug.CharacterControllerPro.Demo
{

[AddComponentMenu("Character Controller Pro/Demo/Camera/Camera 3D")]
public class Camera3D : MonoBehaviour
{
    [Header("Inputs")]
    
    [SerializeField]
    InputHandlerSettings inputHandlerSettings = new InputHandlerSettings();

    [SerializeField]
    string axes = "Camera";

    [SerializeField]
    string zoomAxis = "Camera Zoom";

    [Header("Target")]    

    [SerializeField]
    CharacterActor characterActor = null;

    [SerializeField]
    Vector3 offsetFromHead = Vector3.zero;

    [Header("Interpolation")]     

    [Tooltip("How fast will the camera adapt to any vertical change (using linear interpolation). " +
    "This effect is noticeable when the character is using an \"alignment reference\" (e.g. the spherical planet from the demo). Use it to smooth out the rotation.")]
    [Min( 0f )]
    [SerializeField]
    float verticalLerpSpeed = 10f;

    [Header("View")]  

    [SerializeField]
    CameraMode cameraMode = CameraMode.ThirdPerson;

    [Header("First Person")]
    
    [SerializeField]
    bool hideBody = true;

    [SerializeField]
    Renderer bodyRenderer = null;


    [Header("Pitch")]    

    [SerializeField]
    bool updatePitch = true;

    [SerializeField]
    float initialPitch = 45f;

    [SerializeField]
    float pitchSpeed = 180f;      

    [Range( 1f , 85f )]
    [SerializeField]
    float maxPitchAngle = 80f;  
    
    [Header("Yaw")]

    [SerializeField]
    bool updateYaw = true;


    [SerializeField]
    float yawSpeed = 180f;

    [Header("Zoom (Third person)")]

    [SerializeField]
    bool updateZoom = true;

    [Min(0f)]
    [SerializeField]
    float distanceToTarget = 5f;

    [Min(0f)]
    [SerializeField]
    float zoomInOutSpeed = 40f;

    [Min(0f)]
    [SerializeField]
    float zoomInOutLerpSpeed = 5f;

    [Min(0f)]
    [SerializeField]
    float minZoom = 2f;

    [Min(0.001f)]
    [SerializeField]
    float maxZoom = 12f;     
    
    


    [Header("Collision")]

    [SerializeField]
    bool collisionDetection = true;

    [SerializeField]
    bool collisionAffectsZoom = false;

    [SerializeField]
    float detectionRadius = 0.5f;    

    [SerializeField]
    LayerMask layerMask = 0;

    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    
    float currentDistanceToTarget;
    float smoothedDistanceToTarget;    

    float deltaYaw = 0f;
    float deltaPitch = 0f;
    float deltaZoom = 0f;

    Vector3 previousTargetPosition;

    Transform characterTransform = null;

    Vector3 lerpedCharacterUp = Vector3.up;

    enum CameraMode
    {
        FirstPerson ,
        ThirdPerson ,
    }

    public void ToggleCameraMode()
    {
        cameraMode = cameraMode == CameraMode.FirstPerson ? CameraMode.ThirdPerson : CameraMode.FirstPerson;
    }

    Transform viewReference = null;

    void OnValidate()
    {
        initialPitch = Mathf.Clamp( initialPitch , - maxPitchAngle , maxPitchAngle );
    }
    
    void Awake()
    {   
        inputHandlerSettings.Initialize( gameObject );

        GameObject referenceObject = new GameObject("Camera referece");
        viewReference = referenceObject.transform;
    }    
    
    void OnEnable()
    {
        if( characterActor == null )
            return;

        characterActor.OnTeleport += OnTeleport;
    }

    void OnDisable()
    {
        if( characterActor == null )
            return;
        
        characterActor.OnTeleport -= OnTeleport;
    }

    
    void Start()
    {

        if( characterActor == null || !characterActor.isActiveAndEnabled )
        {
            Debug.Log( "The target character is not active and enabled." );
            this.enabled = false;

            return;
        }


        characterTransform = characterActor.transform;
        characterPosition = characterTransform.position;

        previousLerpedCharacterUp = characterTransform.up;
        lerpedCharacterUp = previousLerpedCharacterUp;

        
        currentDistanceToTarget = distanceToTarget;
        smoothedDistanceToTarget = currentDistanceToTarget;
             

        previousTargetPosition = characterActor.Position + characterActor.transform.TransformDirection( offsetFromHead );

        viewReference.rotation = characterTransform.rotation;
        viewReference.Rotate( Vector3.right , initialPitch );
    }
    

    void Update()
    {
        Vector2 cameraAxes = inputHandlerSettings.InputHandler.GetVector2( axes );

        if( updatePitch )        
            deltaPitch = - cameraAxes.y;
    
        if( updateYaw )        
            deltaYaw = cameraAxes.x;
    
        if( updateZoom )
            deltaZoom = - inputHandlerSettings.InputHandler.GetFloat( zoomAxis ); 
    }    
  

    Vector3 characterPosition = default(Vector3);  

    void LateUpdate()
    {
        float dt = Time.deltaTime;

        UpdateThirdPerson( dt );
    }

    
    

    void OnTeleport( Vector3 position , Quaternion rotation )
    {
        viewReference.rotation = rotation;

        lerpedCharacterUp = characterTransform.up;
        previousLerpedCharacterUp = lerpedCharacterUp;
        
    }

    
    Vector3 previousLerpedCharacterUp = Vector3.up;


    void UpdateThirdPerson( float dt )
    {
        // Body visibility ---------------------------------------------------------------------
        if( cameraMode == CameraMode.FirstPerson )
        {
            if( bodyRenderer != null )
                bodyRenderer.enabled = !hideBody;
        }
        else
        {
            if( bodyRenderer != null )
                bodyRenderer.enabled = true;
        }        
        
        
        // Rotation -----------------------------------------------------------------------------------------
        lerpedCharacterUp = Vector3.Lerp( lerpedCharacterUp , characterTransform.up , verticalLerpSpeed * dt );

        // Rotate the reference based on the lerped character up vector 
        Quaternion deltaRotation = Quaternion.FromToRotation( previousLerpedCharacterUp , lerpedCharacterUp );
        previousLerpedCharacterUp = lerpedCharacterUp;

        viewReference.rotation = deltaRotation * viewReference.rotation;
 
        

        // Yaw rotation -----------------------------------------------------------------------------------------
        viewReference.Rotate( lerpedCharacterUp , deltaYaw * yawSpeed * dt , Space.World ); // <---------
        

        // Pitch rotation -----------------------------------------------------------------------------------------    
        
        float angleToUp = Vector3.Angle( viewReference.forward , lerpedCharacterUp );

        float minPitch = - angleToUp + ( 90f - maxPitchAngle );
        float maxPitch = 180f - angleToUp - ( 90f - maxPitchAngle );

        float pitchAngle = Mathf.Clamp( deltaPitch * pitchSpeed * dt , minPitch , maxPitch );
        viewReference.Rotate( Vector3.right , pitchAngle );


        
        // Position of the target -----------------------------------------------------------------------
        characterPosition = characterTransform.position;

        Vector3 targetPosition = characterPosition + characterTransform.up * characterActor.BodySize.y + characterTransform.TransformDirection( offsetFromHead );


        viewReference.position = targetPosition;
        

        Vector3 finalPosition = viewReference.position;
        
        // ------------------------------------------------------------------------------------------------------
        if( cameraMode == CameraMode.ThirdPerson )
        {            
            currentDistanceToTarget += deltaZoom * zoomInOutSpeed * dt;
            currentDistanceToTarget = Mathf.Clamp( currentDistanceToTarget , minZoom , maxZoom );
            
            smoothedDistanceToTarget = Mathf.Lerp( smoothedDistanceToTarget , currentDistanceToTarget , zoomInOutLerpSpeed * dt );
            Vector3 displacement = - viewReference.forward * smoothedDistanceToTarget;
            
            if( collisionDetection )
            {
                bool hit = DetectCollisions( ref displacement , targetPosition );

                if( collisionAffectsZoom && hit )
                {
                    currentDistanceToTarget = smoothedDistanceToTarget = displacement.magnitude;
                }
            }
        
            finalPosition = targetPosition + displacement;
        }
                
         
        transform.position = finalPosition; 
        transform.rotation = viewReference.rotation; 

        previousTargetPosition = targetPosition;
        
        
    }
   
      


    bool DetectCollisions( ref Vector3 displacement , Vector3 lookAtPosition )
    {
        RaycastHit collisionInfo;
        bool hit = Physics.SphereCast(
            lookAtPosition , 
            detectionRadius ,
            displacement.normalized ,
            out collisionInfo ,
            currentDistanceToTarget ,
            layerMask ,
            QueryTriggerInteraction.Ignore
        );

        if( hit )
            displacement = displacement.normalized * collisionInfo.distance;
        
        return hit;
    }

    
    
}

}
