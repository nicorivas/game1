using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// An implementation of a RigidbodyComponent for 3D rigidbodies.
/// </summary>
public sealed class RigidbodyComponent3D : RigidbodyComponent
{
	new Rigidbody rigidbody = null;

    protected override void Awake()
	{
        base.Awake();
		this.rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
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
			return rigidbody.useGravity;
		}
        set
        {
            rigidbody.useGravity = value;
        }
	}

	public override bool UseInterpolation
    {
		get
		{
			return rigidbody.interpolation == RigidbodyInterpolation.Interpolate;
		}
        set
        {
            rigidbody.interpolation = value ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
        }
	}

	public override bool ContinuousCollisionDetection
    {
		get
		{
			return rigidbody.collisionDetectionMode == CollisionDetectionMode.Continuous;
		}
        set
        {
            rigidbody.collisionDetectionMode = value ? CollisionDetectionMode.Continuous : CollisionDetectionMode.Discrete;
        }
	}

    public override RigidbodyConstraints Constraints
    {
        get
        {
            return rigidbody.constraints;
            
        }
        set
        {
            rigidbody.constraints = value;
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
        }
	}
	
	public override Quaternion Rotation
	{
		get
		{
			return rigidbody.rotation;
		}
        set
        {
            rigidbody.rotation = value;
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
            return rigidbody.angularVelocity;
        }
        set
        {
            rigidbody.angularVelocity = value;
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
		rigidbody.MoveRotation( rotation );

        DesiredRotation = rotation;
	}
	
    
    public override Vector3 GetPointVelocity(Vector3 point)
    {
        return rigidbody.GetPointVelocity( point );
    }

	public override void AddForceToRigidbody( Vector3 force , ForceMode forceMode = ForceMode.Force )
    {
        rigidbody.AddForce( force , forceMode );
    }

    public override void AddExplosionForceToRigidbody(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier = 0)
    {
        rigidbody.AddExplosionForce( explosionForce , explosionPosition , explosionRadius , upwardsModifier );
    }
}

}