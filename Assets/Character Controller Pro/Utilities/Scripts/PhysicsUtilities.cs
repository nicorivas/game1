using UnityEngine;

namespace Lightbug.Utilities
{

public static class PhysicsUtilities
{
    
    public static int CapsuleCast( Vector3 bottom , Vector3 top , float radius  , Vector3 castDisplacement , LayerMask layerMask , RaycastHit[] raycastHits , bool ignoreTrigger = true )
    {        
        int hits = Physics.CapsuleCastNonAlloc(
            bottom ,
            top ,  
            radius ,         
            castDisplacement.normalized ,
            raycastHits ,
            castDisplacement.magnitude ,
            layerMask ,
            ignoreTrigger ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
        );

        return hits;
    }

    public static int CapsuleCast( Vector3 bottom , Vector3 top , float radius  , Vector3 castDisplacement , LayerMask layerMask , RaycastHit2D[] raycastHits , bool ignoreTrigger = true )
    {    
        Vector3 bottomToTop = top - bottom;
        Vector3 center = bottom + 0.5f * bottomToTop.magnitude * bottomToTop.normalized;
        Vector2 size = new Vector2( 2f * radius , bottomToTop.magnitude + 2f * radius );

        float castAngle = Vector2.SignedAngle( bottomToTop.normalized , Vector2.up );

        Physics2D.queriesHitTriggers = !ignoreTrigger;

        int hits = Physics2D.CapsuleCastNonAlloc(
            center ,
            size ,
            CapsuleDirection2D.Vertical ,
            castAngle ,
            castDisplacement.normalized ,
            raycastHits ,
            castDisplacement.magnitude ,
            layerMask 
        );

        return hits;
    }

    
    public static bool SphereCast( Vector3 center , float radius , Vector3 castDisplacement , LayerMask layerMask , out RaycastHit raycastHit , bool ignoreTrigger = true )
    {
        bool hit = Physics.SphereCast(
            center ,
            radius ,
            castDisplacement.normalized ,
            out raycastHit ,
            castDisplacement.magnitude ,
            layerMask ,
            ignoreTrigger ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
        );

        return hit;
    }

    public static bool SphereCast( Vector3 center , float radius , Vector3 castDisplacement , LayerMask layerMask , out RaycastHit2D raycastHit , bool ignoreTrigger = true )
    {
        Physics2D.queriesHitTriggers = !ignoreTrigger;

        raycastHit = Physics2D.CircleCast(
            center ,
            radius ,
            castDisplacement.normalized ,
            castDisplacement.magnitude ,
            layerMask
        );

        return raycastHit.collider != null;
    }

    public static int SphereCast( Vector3 center , float radius , Vector3 castDisplacement , LayerMask layerMask , RaycastHit[] raycastHits , bool ignoreTrigger = true )
    {
        int hits = Physics.SphereCastNonAlloc(
            center ,
            radius ,
            castDisplacement.normalized ,
            raycastHits ,
            castDisplacement.magnitude ,
            layerMask ,
            ignoreTrigger ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
        );

        return hits;
    }


    public static int SphereCast( Vector3 center , float radius , Vector3 castDisplacement , LayerMask layerMask , RaycastHit2D[] raycastHits , bool ignoreTrigger = true )
    {    
        Physics2D.queriesHitTriggers = !ignoreTrigger;

        int hits = Physics2D.CircleCastNonAlloc(
            center ,
            radius ,
            castDisplacement.normalized ,
            raycastHits ,
            castDisplacement.magnitude ,
            layerMask 
        );

        return hits;
    }

    // Overlaps ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    
    public static int OverlapSphere( Vector3 center , float radius , LayerMask layerMask , Collider[] overlappedColliders , bool ignoreTrigger = true )
    {        
        
        int hits = Physics.OverlapSphereNonAlloc(
            center ,
            radius ,
            overlappedColliders ,
            layerMask ,
            ignoreTrigger ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
        );

        return hits;
    }

    public static int OverlapSphere( Vector3 center , float radius , LayerMask layerMask , Collider2D[] overlappedColliders , bool ignoreTrigger = true )
    {        
        Physics2D.queriesHitTriggers = !ignoreTrigger;
        
        int hits = Physics2D.OverlapCircleNonAlloc(
            center ,
            radius ,
            overlappedColliders ,
            layerMask
        );

        return hits;
    }

    public static int OverlapCapsule( Vector3 bottom , Vector3 top , float radius , LayerMask layerMask , Collider[] overlappedColliders , bool ignoreTrigger = true )
    {  

        int hits = Physics.OverlapCapsuleNonAlloc(
            bottom ,
            top ,  
            radius ,
            overlappedColliders ,
            layerMask ,
            ignoreTrigger ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
        );

        return hits;
    }

    public static int OverlapCapsule( Vector3 bottom , Vector3 top , float radius , LayerMask layerMask , Collider2D[] overlappedColliders , bool ignoreTrigger = true )
    {  
        Vector3 bottomToTop = top - bottom;
        Vector3 center = bottom + 0.5f * bottomToTop;
        Vector2 size = new Vector2( 2f * radius , bottomToTop.magnitude + 2f * radius );

        float castAngle = Vector2.SignedAngle( bottomToTop.normalized , Vector2.up );

        Physics2D.queriesHitTriggers = !ignoreTrigger;
        
        int hits = Physics2D.OverlapCapsuleNonAlloc(
            center ,
            size ,
            CapsuleDirection2D.Vertical ,
            castAngle ,
            overlappedColliders ,
            layerMask
        );

        return hits;
    }

    

}


}