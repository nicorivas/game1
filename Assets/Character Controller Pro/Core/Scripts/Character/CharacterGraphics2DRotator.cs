using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

/// <summary>
/// This component can be used to rotate an object based on the CharacterActor forward direction (useful for 2D physics characters).
/// </summary>
[AddComponentMenu("Character Controller Pro/Core/Character Graphics 2D Rotator")]
public class CharacterGraphics2DRotator : CharacterGraphics
{       
    
    [Tooltip("Scale: it will flip the sprite along the horizontal axis (localScale). This works only with sprites!\nRotation: it will rotate the object towards the facing direction.")]
    [SerializeField]
    FacingDirectionMode facingDirectionMode = FacingDirectionMode.Rotation;

    
    /// <summary>
    /// The method used by the CharacterGraphics component to orient the graphics object towards the facing direction vector.
    /// </summary>
    public enum FacingDirectionMode
    {
        Rotation ,
        Scale
    }
 
    protected override void OnValidate()
    {
        base.OnValidate();

        CharacterBody characterBody = this.GetComponentInBranch<CharacterBody>();

        if( characterBody == null )
            Debug.Log("Warning: No CharacterBody component detected in the root object.");
        else if( !characterBody.Is2D )
            Debug.Log("Warning: CharacterBody is not 2D. This component is intended to be used with a 2D physics character.");
        
    }
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    struct GraphicsChild
    {
        public Transform transform;
        public Vector3 initialScale;

        public GraphicsChild( Transform transform , Vector3 initialScale )
        {
            this.transform = transform;
            this.initialScale = initialScale;
        }
    }


    Vector3 initialScale = Vector3.zero;
    
    
    List<GraphicsChild> graphicsObjects = new List<GraphicsChild>();

    protected override void Awake()
    {
        base.Awake();

        GraphicsChild thisGraphics = new GraphicsChild(
            transform ,
            transform.localScale
        );

        graphicsObjects.Add( thisGraphics );
        GetChildrenRecursively( transform );        
                
    }

    void GetChildrenRecursively( Transform target )
    {
        for( int i = 0 ; i < target.childCount ; i++ )
        {
            Transform child = target.GetChild( i );

            GraphicsChild graphicsChild = new GraphicsChild(
                child ,
                child.localScale
            );

            graphicsObjects.Add( graphicsChild );

            if( child.childCount != 0 )
                GetChildrenRecursively( child );
        }
    }


    void Start()
    {
        initialScale = transform.localScale;        
    }    

    void LateUpdate()
    {  
        if( characterActor == null || !characterActor.enabled )
        {
            Destroy( gameObject );
            return;
        }     

        float dt = Time.deltaTime;
        
        HandleRotation( dt );
        

    }


    void ScaleByBodySize()
    {
        Vector3 scale = new Vector3( 
            characterActor.BodySize.x / characterActor.DefaultBodySize.x , 
            characterActor.BodySize.y / characterActor.DefaultBodySize.y
        );

        scale.z = scale.x;

        transform.localScale = scale;
    }
    
     
    void HandleRotation( float dt )
    {       
        if( !characterActor.CharacterBody.Is2D )
            return;
        
        if( facingDirectionMode == FacingDirectionMode.Scale )
        {            
                        
            float signedAngle = Vector3.SignedAngle( characterActor.Forward , characterActor.Up , Vector3.forward );

            for( int i = 0 ; i < graphicsObjects.Count ; i++ )
            {
                Vector3 childLocalScale = graphicsObjects[i].transform.localScale;
                childLocalScale.x = signedAngle < 0 ? - graphicsObjects[i].initialScale.x : graphicsObjects[i].initialScale.x;
                graphicsObjects[i].transform.localScale = childLocalScale;
            }

            
        }
        else
        {
            transform.rotation = Quaternion.LookRotation( characterActor.Forward , characterActor.Up );
        }

        
    }


}

}
