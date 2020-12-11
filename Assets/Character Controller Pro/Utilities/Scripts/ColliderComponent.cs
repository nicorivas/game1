using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// This component is an encapsulation of the Collider and Collider2D components, containing the most commonly used 
/// properties and methods from these components.
/// </summary>
public abstract class ColliderComponent : MonoBehaviour
{
    public abstract Vector3 Size{ get; set; }
    public abstract Vector3 Offset{ get; set; }

    protected virtual void Awake()
	{
		this.hideFlags = HideFlags.None;
	}
    
    protected abstract void OnEnable();
    protected abstract void OnDisable();

}

}