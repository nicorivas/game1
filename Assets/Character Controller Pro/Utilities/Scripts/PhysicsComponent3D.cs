using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// An implementation of a PhysicsComponent for 3D physics.
/// </summary>
public sealed class PhysicsComponent3D : PhysicsComponent
{
	RaycastHit[] raycastHits = new RaycastHit[10];
	Collider[] overlappedColliders = new Collider[10];

    ContactPoint[] contactsBuffer = new ContactPoint[10];


    void OnTriggerStay( Collider other )
    {        
        bool found = false;

        Trigger trigger = new Trigger();

        for( int i = 0 ; i < Triggers.Count ; i++ )
        {
            if( Triggers[i].gameObject != other.gameObject )
                continue;
            
            found = true;

            

            // Ignore old Triggers
            if( !Triggers[i].firstContact )
                continue;
            
            // Set the firstContact field to false
            trigger = Triggers[i];
            trigger.firstContact = false;
            Triggers[i] = trigger;


            break;
            
        }

        // First contact
        if( !found )
        {            
            trigger.Set( true , other );
            Triggers.Add( trigger );
        }
        
    }

    void OnTriggerExit( Collider other )
    {
        for( int i = Triggers.Count - 1 ; i >= 0 ; i-- )
        {            
            if( Triggers[i].collider3D == other )
            {
                Triggers.RemoveAt( i );

                break;
            }
        }
    }


    
    void OnCollisionEnter( Collision collision )
    {
        int bufferHits = collision.GetContacts( contactsBuffer );
        
        // Add the contacts to the list
        for( int i = 0 ; i < bufferHits ; i++ )
        {
            ContactPoint contact = contactsBuffer[i];

            Contact outputContact = new Contact();

            outputContact.Set( true , contact );

            Contacts.Add( outputContact );
        } 
    }

    void OnCollisionStay( Collision collision )
    {
        int bufferHits = collision.GetContacts( contactsBuffer );
        
        // Add the contacts to the list
        for( int i = 0 ; i < bufferHits ; i++ )
        {
            ContactPoint contact = contactsBuffer[i];

            Contact outputContact = new Contact();

            outputContact.Set( false , contact );

            Contacts.Add( outputContact );
        }
    }

    public override void IgnoreLayerCollision( int layerA , int layerB , bool ignore )
    {
        Physics.IgnoreLayerCollision( layerA , layerB , ignore );
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

    

    // Casts ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    
    public override int Raycast(out HitInfo hitInfo, Vector3 origin, Vector3 castDisplacement, LayerMask layerMask, float minimumDistance = 0f , bool ignoreTrigger = true )
    {
        hits = Physics.RaycastNonAlloc(
			origin ,
			castDisplacement.normalized ,            
            raycastHits ,
            castDisplacement.magnitude ,
			layerMask ,
            ignoreTrigger ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
		);

        GetClosestHit( out hitInfo , castDisplacement , minimumDistance , layerMask );

        return hits;
    }


	public override int CapsuleCast( out HitInfo hitInfo , Vector3 bottom , Vector3 top , float radius  , Vector3 castDisplacement , LayerMask layerMask , float minimumDistance = 0f , bool ignoreTrigger = true )
    {        
        hits = Physics.CapsuleCastNonAlloc(
            bottom ,
            top ,  
            radius ,         
            castDisplacement.normalized ,
            raycastHits ,
            castDisplacement.magnitude ,
            layerMask ,
            ignoreTrigger ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
        );

        GetClosestHit( out hitInfo , castDisplacement , minimumDistance , layerMask );

        return hits;
    }



    public override int SphereCast( out HitInfo hitInfo , Vector3 center , float radius , Vector3 castDisplacement , LayerMask layerMask , float minimumDistance = 0f , bool ignoreTrigger = true )
    {
        hits = Physics.SphereCastNonAlloc(
            center ,
            radius ,
            castDisplacement.normalized ,
            raycastHits ,
            castDisplacement.magnitude ,
            layerMask ,
            ignoreTrigger ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
        );

        GetClosestHit( out hitInfo , castDisplacement , minimumDistance , layerMask );

        return hits;
    }


    // Overlaps ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    
    public override bool OverlapSphere( Vector3 center , float radius , LayerMask layerMask , bool ignoreTrigger = true )
    {        
        
        int hits = Physics.OverlapSphereNonAlloc(
            center ,
            radius ,
            overlappedColliders ,
            layerMask ,
            ignoreTrigger ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
        );
        
        this.hits = hits;

        return hits != 0;
    }

    public override bool OverlapCapsule( Vector3 bottom , Vector3 top , float radius , LayerMask layerMask , bool ignoreTrigger = true )
    {  

        int hits = Physics.OverlapCapsuleNonAlloc(
            bottom ,
            top ,  
            radius ,
            overlappedColliders ,
            layerMask ,
            ignoreTrigger ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
        );

        this.hits = hits;

        return hits != 0;
    }

    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    void GetHitInfo( ref HitInfo hitInfo , RaycastHit raycastHit  , Vector3 castDirection)
    {

        if( raycastHit.collider != null )
        {                    
            hitInfo.point = raycastHit.point;
            hitInfo.normal = raycastHit.normal;
            hitInfo.distance = raycastHit.distance;
            hitInfo.direction = castDirection;
            hitInfo.transform = raycastHit.transform;
            hitInfo.collider3D = raycastHit.collider;
            hitInfo.rigidbody3D = raycastHit.rigidbody;     
        }
    }

    protected override void GetClosestHit( out HitInfo hitInfo , Vector3 castDisplacement , float minimumDistance , LayerMask layerMask )
    {
        RaycastHit closestRaycastHit = new RaycastHit();
        closestRaycastHit.distance = Mathf.Infinity;

        hitInfo = new HitInfo();
        hitInfo.hit = false;

        for( int i = 0 ; i < hits ; i++ )
        {
            RaycastHit raycastHit = raycastHits[i];             

            if( raycastHit.distance == 0 )
                continue;

            if( raycastHit.distance < minimumDistance )
                continue;

            hitInfo.hit = true;

            if( raycastHit.distance < closestRaycastHit.distance )
                closestRaycastHit = raycastHit;

        }

        GetHitInfo( ref hitInfo , closestRaycastHit , castDisplacement.normalized );        

    }

    


}

}