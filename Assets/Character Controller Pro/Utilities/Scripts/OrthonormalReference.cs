using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// This struct contains three orthonormal vectors. These vectors are defined as "right", "up" and "forward", and they are used for the character to determine the movement direction.
/// </summary>
public struct OrthonormalReference
{
    
    public Vector3 forward;
    public Vector3 up;
    public Vector3 right;

    public void Update( Transform transform )
    { 
        forward = transform.forward;
        up = transform.up;
        right = transform.right;
    }

    public void Update( Transform transform , Vector3 planeNormal )
    {            
        forward = Vector3.ProjectOnPlane( transform.forward , planeNormal ).normalized;
        right = Vector3.ProjectOnPlane( transform.right , planeNormal ).normalized;
        up = Vector3.Cross( forward , right ).normalized;
        
    }

    public void Update( Vector3 right , Vector3 up , Vector3 forward )
    {            
        this.forward = forward;        
        this.up = up;
        this.right = right;
    }

    public void Update( OrthonormalReference reference )
    {            
        forward = reference.forward;
        up = reference.up;
        right = reference.right;
    }

#if UNITY_EDITOR
    public void Draw( Vector3 position )
    {

        Debug.DrawRay( position , right , Color.red );
        Debug.DrawRay( position , up , Color.green );
        Debug.DrawRay( position , forward , Color.blue );

    }
#endif

}

}
