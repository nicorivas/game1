using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.CharacterControllerPro.Core
{

/// <summary>
/// This component can be used to make a Transform change its scale, based on the CharacterActor height.
/// </summary>
[AddComponentMenu("Character Controller Pro/Core/Character Graphics Scaler")]
public class CharacterGraphicsScaler : CharacterGraphics
{           
    enum VectorComponent
    {
        X ,
        Y , 
        Z
    }

    [SerializeField]
    VectorComponent scaleHeightComponent = VectorComponent.Y;


    Vector3 initialOffset = Vector3.zero;
    Vector3 initialLocalScale = Vector3.one;

    void Start()
    {
        initialOffset = transform.position - characterActor.transform.position;
        initialLocalScale = transform.localScale;
    }

    void LateUpdate()
    {
        
        if( !characterActor.enabled )
            return;
        
        ScaleByBodySize();
        

    }


    void ScaleByBodySize()
    {

        Vector3 scale = Vector3.one;
        Vector3 offset = Vector3.zero;
        
        switch( scaleHeightComponent )
        {
            case VectorComponent.X:

                scale = new Vector3(
                    initialLocalScale.x * ( characterActor.BodySize.y / characterActor.DefaultBodySize.y ) , 
                    initialLocalScale.y * ( characterActor.BodySize.x / characterActor.DefaultBodySize.x ) ,
                    initialLocalScale.z * ( characterActor.BodySize.x / characterActor.DefaultBodySize.x )
                );
                
                break;
            case VectorComponent.Y:

                scale = new Vector3(
                    initialLocalScale.x * ( characterActor.BodySize.y / characterActor.DefaultBodySize.y ) , 
                    initialLocalScale.y * ( characterActor.BodySize.y / characterActor.DefaultBodySize.y ) ,
                    initialLocalScale.z * ( characterActor.BodySize.x / characterActor.DefaultBodySize.x )
                );
                
                break;
            case VectorComponent.Z:

                scale = new Vector3(
                    initialLocalScale.x * ( characterActor.BodySize.x / characterActor.DefaultBodySize.x ) , 
                    initialLocalScale.y * ( characterActor.BodySize.x / characterActor.DefaultBodySize.x ) ,
                    initialLocalScale.z * ( characterActor.BodySize.y / characterActor.DefaultBodySize.y )
                );
                
                break;
        }

        offset = new Vector3( 
            initialOffset.x * scale.x ,
            initialOffset.y * scale.y ,
            initialOffset.z * scale.z
        );

        transform.position = characterActor.transform.position + offset;
        
        transform.localScale = scale;
    }
    
     


}

}

