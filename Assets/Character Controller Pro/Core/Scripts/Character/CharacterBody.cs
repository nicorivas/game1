using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

public enum CharacterBodyType
{
	Sphere ,
	Capsule
}

/// <summary>
/// This class contains all the character body properties, such as width, height, body shape, physics, etc.
/// </summary>
[AddComponentMenu("Character Controller Pro/Core/Character Body")]
public class CharacterBody : MonoBehaviour
{

	[SerializeField]
	bool is2D = false;

	[SerializeField]
	CharacterBodyType bodyType = CharacterBodyType.Capsule;

	[SerializeField]
	Vector2 bodySize = new Vector2( 1f , 2f );

	[SerializeField]
	float mass = 50f;


    /// <summary>
	/// Returns true if the character is governed by 2D Physics, false otherwise.
	/// </summary>
	public bool Is2D
	{
		get
		{
			return is2D;
		}
	}
    
    RigidbodyComponent rigidbodyComponent = null;

	/// <summary>
    /// Gets the RigidbodyComponent component associated to the character.
    /// </summary>
	public RigidbodyComponent RigidbodyComponent
	{
		get
		{
			return rigidbodyComponent;
		}
	}

	ColliderComponent colliderComponent = null;

	/// <summary>
    /// Gets the ColliderComponent component associated to the character.
    /// </summary>
	public ColliderComponent ColliderComponent
	{
		get
		{
			return colliderComponent;
		}
	}

	/// <summary>
    /// Gets the mass of the character.
    /// </summary>
    public float Mass
	{
		get
		{
			return mass;
		}
	}

	/// <summary>
    /// Gets the body size of the character (width and height).
    /// </summary>
    public Vector2 BodySize
	{
		get
		{
			return bodySize;
		}
	}
	
	/// <summary>
    /// Gets the the shape of the character.
    /// </summary>
	public CharacterBodyType BodyType
	{
		get
		{
			return bodyType;
		}
	}

	/// <summary>
    /// Initializes the body properties and components.
    /// </summary>
    public void Initialize()
    {
        if( Is2D )
		{			

			switch( bodyType )
			{
				case CharacterBodyType.Sphere:
					colliderComponent = gameObject.AddComponent<SphereColliderComponent2D>();					
				break;

				case CharacterBodyType.Capsule:
					colliderComponent = gameObject.AddComponent<CapsuleColliderComponent2D>();
				break;

			}

			rigidbodyComponent = gameObject.AddComponent<RigidbodyComponent2D>();	 
		}
		else
		{
			switch( bodyType )
			{
				case CharacterBodyType.Sphere:
					colliderComponent = gameObject.AddComponent<SphereColliderComponent3D>();
				break;

				case CharacterBodyType.Capsule:
					colliderComponent = gameObject.AddComponent<CapsuleColliderComponent3D>();
				break;

			}

			rigidbodyComponent = gameObject.AddComponent<RigidbodyComponent3D>();
		}

		if( bodyType == CharacterBodyType.Sphere )
			bodySize.y = bodySize.x;
    }

}

}
