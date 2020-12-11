using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// An implementation of a PhysicsComponent for 2D physics.
/// </summary>
public sealed class PhysicsComponent2D : PhysicsComponent
{
	RaycastHit2D[] raycastHits = new RaycastHit2D[10];
	Collider2D[] overlappedColliders = new Collider2D[10];

    ContactPoint2D[] contactsBuffer = new ContactPoint2D[10];


    void OnTriggerStay2D( Collider2D other )
    {        
        bool found = false;

        Trigger Trigger = new Trigger();

        for( int i = 0 ; i < Triggers.Count ; i++ )
        {
            if( Triggers[i].gameObject != other.gameObject )
                continue;
            
            found = true;

            

            // Ignore old Triggers
            if( !Triggers[i].firstContact )
                continue;
            
            // Set the firstContact field to false
            Trigger = Triggers[i];
            Trigger.firstContact = false;
            Triggers[i] = Trigger;
            

            break;
            
        }

        // First contact
        if( !found )
        {            
            Trigger.Set( true , other );
            Triggers.Add( Trigger );
        }
        
    }

    void OnTriggerExit2D( Collider2D other )
    {
        for( int i = Triggers.Count - 1 ; i >= 0 ; i-- )
        {            
            if( Triggers[i].collider2D == other )
            {
                Triggers.RemoveAt( i );

                break;
            }
        }
    }
    
    
    
    void OnCollisionEnter2D( Collision2D collision )
    {
        int bufferHits = collision.GetContacts( contactsBuffer );
        
        // Add the contacts to the list
        for( int i = 0 ; i < bufferHits ; i++ )
        {
            ContactPoint2D contact = contactsBuffer[i];    
            
            Contact outputContact = new Contact();

            outputContact.Set( true , contact );
            
            Contacts.Add( outputContact );
        }    
    }

    void OnCollisionStay2D( Collision2D collision )
    {
        int bufferHits = collision.GetContacts( contactsBuffer );
        
        // Add the contacts to the list
        for( int i = 0 ; i < bufferHits ; i++ )
        {
            ContactPoint2D contact = contactsBuffer[i];    
            
            Contact outputContact = new Contact();

            outputContact.Set( false , contact );
            
            Contacts.Add( outputContact );
        }
    }

    public override void IgnoreLayerCollision( int layerA , int layerB , bool ignore )
    {
        Physics2D.IgnoreLayerCollision( layerA , layerB , ignore );
    }

    public override void IgnoreLayerMaskCollision( LayerMask layerMask , bool ignore )
    {
        int characterLayer = gameObject.layer;
        int layerMaskValue = layerMask.value;
        int currentLayer = 1;

		for( int i = 0 ; i < 32 ; i++ )
		{
			bool exist = ( layerMaskValue & currentLayer ) > 0;

            if( exist )
                IgnoreLayerCollision( characterLayer , i , ignore );

            currentLayer <<= 1;
		}
        
    }

    
    public override int Raycast(out HitInfo hitInfo, Vector3 origin, Vector3 castDisplacement, LayerMask layerMask, float minimumDistance = 0f , bool ignoreTrigger = true)
    {
        bool previousQueriesHitTriggers = Physics2D.queriesHitTriggers;
        Physics2D.queriesHitTriggers = !ignoreTrigger;

        hits = Physics2D.RaycastNonAlloc(
			origin ,
			castDisplacement.normalized ,            
            raycastHits ,
            castDisplacement.magnitude ,
			layerMask
		);        

        Physics2D.queriesHitTriggers = previousQueriesHitTriggers;         
        
        GetClosestHit( out hitInfo , castDisplacement , minimumDistance , layerMask );
        
        return hits;
    }

    public override int CapsuleCast( out HitInfo hitInfo , Vector3 bottom , Vector3 top , float radius  , Vector3 castDisplacement , LayerMask layerMask , float minimumDistance = 0f , bool ignoreTrigger = true )
    {    
        Vector3 bottomToTop = top - bottom;
        Vector3 center = bottom + 0.5f * bottomToTop.magnitude * bottomToTop.normalized;
        Vector2 size = new Vector2( 2f * radius , bottomToTop.magnitude + 2f * radius );

        float castAngle = Vector2.SignedAngle( bottomToTop.normalized , Vector2.up );

        bool previousQueriesHitTriggers = Physics2D.queriesHitTriggers;
        Physics2D.queriesHitTriggers = !ignoreTrigger;

        hits = Physics2D.CapsuleCastNonAlloc(
            center ,
            size ,
            CapsuleDirection2D.Vertical ,
            castAngle ,
            castDisplacement.normalized ,
            raycastHits ,
            castDisplacement.magnitude ,
            layerMask 
        );

        Physics2D.queriesHitTriggers = previousQueriesHitTriggers;

        GetClosestHit( out hitInfo , castDisplacement , minimumDistance , layerMask );

        return hits;
    }

    
    public override int SphereCast( out HitInfo hitInfo , Vector3 center , float radius , Vector3 castDisplacement , LayerMask layerMask , float minimumDistance = 0f , bool ignoreTrigger = true )
    {    
        bool previousQueriesHitTriggers = Physics2D.queriesHitTriggers;
        Physics2D.queriesHitTriggers = !ignoreTrigger;

        hits = Physics2D.CircleCastNonAlloc(
            center ,
            radius ,
            castDisplacement.normalized ,
            raycastHits ,
            castDisplacement.magnitude ,
            layerMask 
        );

        Physics2D.queriesHitTriggers = previousQueriesHitTriggers;

        
        GetClosestHit( out hitInfo , castDisplacement , minimumDistance , layerMask );

        return hits;
    }

    // Overlaps ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    
  
    public override bool OverlapSphere( Vector3 center , float radius , LayerMask layerMask , bool ignoreTrigger = true )
    {        
        bool previousQueriesHitTriggers = Physics2D.queriesHitTriggers;
        Physics2D.queriesHitTriggers = !ignoreTrigger;
        
        hits = Physics2D.OverlapCircleNonAlloc(
            center ,
            radius ,
            overlappedColliders ,
            layerMask
        );

        Physics2D.queriesHitTriggers = previousQueriesHitTriggers;

        return hits != 0;
    }

 

    public override bool OverlapCapsule( Vector3 bottom , Vector3 top , float radius , LayerMask layerMask , bool ignoreTrigger = true )
    {  
        Vector3 bottomToTop = top - bottom;
        Vector3 center = bottom + 0.5f * bottomToTop;
        Vector2 size = new Vector2( 2f * radius , bottomToTop.magnitude + 2f * radius );

        float castAngle = Vector2.SignedAngle( bottomToTop.normalized , Vector2.up );

        bool previousQueriesHitTriggers = Physics2D.queriesHitTriggers;
        Physics2D.queriesHitTriggers = !ignoreTrigger;
        
        hits = Physics2D.OverlapCapsuleNonAlloc(
            center ,
            size ,
            CapsuleDirection2D.Vertical ,
            castAngle ,
            overlappedColliders ,
            layerMask
        );

        Physics2D.queriesHitTriggers = previousQueriesHitTriggers;
        
        return hits != 0;
    }

    // ---------------------------------------------------------------------------------------------------------------------------------

    

    void GetHitInfo( ref HitInfo hitInfo , RaycastHit2D raycastHit , Vector3 castDirection )
    {
        if( raycastHit.collider != null )
        {                    
            hitInfo.point = raycastHit.point;
            hitInfo.normal = raycastHit.normal;
            hitInfo.distance = raycastHit.distance;
            hitInfo.direction = castDirection;
            hitInfo.transform = raycastHit.transform;
            hitInfo.collider2D = raycastHit.collider;
            hitInfo.rigidbody2D = raycastHit.rigidbody;     
        }
    }

    protected override void GetClosestHit( out HitInfo hitInfo , Vector3 castDisplacement , float minimumDistance , LayerMask layerMask )
    {
        RaycastHit2D closestRaycastHit = new RaycastHit2D();
        closestRaycastHit.distance = Mathf.Infinity;

        hitInfo = new HitInfo();
        hitInfo.hit = false;

        for( int i = 0 ; i < hits ; i++ )
        {
            RaycastHit2D raycastHit = raycastHits[i];             

            if( raycastHit.distance == 0 )
                continue;

            if( raycastHit.distance < minimumDistance )
                continue;

            hitInfo.hit = true;

            if( raycastHit.distance < closestRaycastHit.distance )
                closestRaycastHit = raycastHit;

        }

        if( hitInfo.hit )
            GetHitInfo( ref hitInfo , closestRaycastHit , castDisplacement.normalized );        

    }

     
    }

}
