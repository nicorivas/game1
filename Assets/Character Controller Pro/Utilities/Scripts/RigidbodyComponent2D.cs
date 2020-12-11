using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// An implementation of a RigidbodyComponent for 2D rigidbodies.
/// </summary>
public sealed class RigidbodyComponent2D : RigidbodyComponent
{
	new Rigidbody2D rigidbody = null;

	protected override void Awake()
	{
        base.Awake();
		this.rigidbody = gameObject.GetOrAddComponent<Rigidbody2D>();
        this.rigidbody.hideFlags = HideFlags.NotEditable;
	}

    public override float Mass
    {
		get
		{
			return rigidbody.mass;
		}
        set
        {
            rigidbody.mass = value;
        }
	}

    public override float Drag
    {
		get
		{
			return rigidbody.drag;
		}
        set
        {
            rigidbody.drag = value;
        }
	}

    public override bool IsKinematic
    {
		get
		{
			return rigidbody.isKinematic;
		}
        set
        {
            rigidbody.isKinematic = value;
        }
	}
    
    public override bool UseGravity
    {
		get
		{
			return rigidbody.gravityScale != 0f;
		}
        set
        {
            rigidbody.gravityScale = value ? 1f : 0f;
        }
	}

	public override bool UseInterpolation
    {
		get
		{
			return rigidbody.interpolation == RigidbodyInterpolation2D.Interpolate;
		}
        set
        {
            rigidbody.interpolation = value ? RigidbodyInterpolation2D.Interpolate : RigidbodyInterpolation2D.None;
        }
	}

	public override bool ContinuousCollisionDetection
    {
		get
		{
			return rigidbody.collisionDetectionMode == CollisionDetectionMode2D.Continuous;
		}
        set
        {
            rigidbody.collisionDetectionMode = value ? CollisionDetectionMode2D.Continuous : CollisionDetectionMode2D.Discrete;
        }
	}

    public override RigidbodyConstraints Constraints
    {
        get
        {
            switch( rigidbody.constraints )
            {
                case RigidbodyConstraints2D.None:
                    return RigidbodyConstraints.None;

                case RigidbodyConstraints2D.FreezeAll:
                    return RigidbodyConstraints.FreezeAll;

                case RigidbodyConstraints2D.FreezePosition:
                    return RigidbodyConstraints.FreezePosition;

                case RigidbodyConstraints2D.FreezePositionX:
                    return RigidbodyConstraints.FreezePositionX;

                case RigidbodyConstraints2D.FreezePositionY:
                    return RigidbodyConstraints.FreezePositionY;

                case RigidbodyConstraints2D.FreezeRotation:
                    return RigidbodyConstraints.FreezeRotationZ;

                default:
                    return RigidbodyConstraints.None;
            }
            
        }
        set
        {
            switch( value )
            {
                case RigidbodyConstraints.None:
                    rigidbody.constraints = RigidbodyConstraints2D.None;

                    break;

                case RigidbodyConstraints.FreezeAll:
                    rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;

                    break;

                case RigidbodyConstraints.FreezePosition:
                    rigidbody.constraints = RigidbodyConstraints2D.FreezePosition;

                    break;

                case RigidbodyConstraints.FreezePositionX:
                    rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;

                    break;

                case RigidbodyConstraints.FreezePositionY:
                    rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;

                    break;

                case RigidbodyConstraints.FreezeRotation:
                    rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

                    break;
                

                case RigidbodyConstraints.FreezeRotationZ:
                    rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

                    break;

                default:
                    rigidbody.constraints = RigidbodyConstraints2D.None;

                    break;
            }
        }
    }

	public override Vector3 Position
	{
		get
		{
			return rigidbody.position;
		}
        set
        {
            rigidbody.position = value;
            // DesiredPosition = value;
        }
	}
	
	public override Quaternion Rotation
	{
		get
		{
			return Quaternion.Euler( 0f , 0f , rigidbody.rotation );
		}
        set
        {
            rigidbody.rotation = value.eulerAngles.z;
            // DesiredRotation = value;
        }
	}

	public override Vector3 Velocity
    {
        get
        {
            return rigidbody.velocity;
        }
        set
        {
            rigidbody.velocity = value;    
            DesiredPosition = Position + value * Time.fixedDeltaTime;        
        }
    }

    public override Vector3 AngularVelocity
    {
        get
        {
            return new Vector3( 0f , 0f , rigidbody.angularVelocity );
        }
        set
        {
            rigidbody.angularVelocity = value.z;    
            DesiredRotation = Rotation * Quaternion.Euler( AngularVelocity * Time.fixedDeltaTime );        
        }
    }

    public override void Interpolate( Vector3 position )
	{
		rigidbody.MovePosition( position );

        DesiredPosition = position;

	}

    public override void Interpolate( Quaternion rotation )
	{
		rigidbody.MoveRotation( rotation.eulerAngles.z );

        DesiredRotation = rotation;
	}



    public override Vector3 GetPointVelocity(Vector3 point)
    {
        return rigidbody.GetPointVelocity( point );
    }

	public override void AddForceToRigidbody( Vector3 force , ForceMode forceMode = ForceMode.Force )
    {
        ForceMode2D forceMode2D = ForceMode2D.Force;

        if( forceMode == ForceMode.Impulse || forceMode == ForceMode.VelocityChange )
            forceMode2D = ForceMode2D.Impulse;
        
        rigidbody.AddForce( force , forceMode2D );
    }

    public override void AddExplosionForceToRigidbody(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier = 0)
    {
        Debug.LogWarning( "AddExplosionForce is not available for 2D physics" );
    }

}

}
