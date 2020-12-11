using UnityEngine;

namespace Lightbug.CharacterControllerPro.Implementation
{


[RequireComponent( typeof( Animator ) )]
public class AnimatorLink : MonoBehaviour
{
        
    Animator animator = null;
    

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    bool resetIKWeightsFlag = false;

    public void ResetIKWeights()
    {
        resetIKWeightsFlag = true;
    }


    public event System.Action< Vector3 , Quaternion > OnAnimatorMoveEvent;
    public event System.Action< int > OnAnimatorIKEvent;

    void OnAnimatorMove()
    {
        
        if( OnAnimatorMoveEvent != null )
            OnAnimatorMoveEvent( animator.deltaPosition , animator.deltaRotation );
        
    }

    void OnAnimatorIK( int layerIndex )
    {
        
        if( resetIKWeightsFlag )
        {
            resetIKWeightsFlag = false;

            animator.SetIKPositionWeight( AvatarIKGoal.LeftFoot , 0f );
            animator.SetIKPositionWeight( AvatarIKGoal.RightFoot , 0f );
            animator.SetIKPositionWeight( AvatarIKGoal.LeftHand , 0f );
            animator.SetIKPositionWeight( AvatarIKGoal.RightHand , 0f );
        }     

        if( OnAnimatorIKEvent != null )
            OnAnimatorIKEvent( layerIndex );
        
    }
   

}

}