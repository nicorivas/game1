using UnityEngine;

namespace Lightbug.CharacterControllerPro.Demo
{

public class LookAtTarget : MonoBehaviour
{
    [SerializeField]
    Transform target = null;

    [SerializeField]
    bool invertForwardDirection = true;

    void Start()
    {
        if( target == null )
            this.enabled = false;
    }
    
    void Update()
    {
        transform.LookAt( target );

        if( invertForwardDirection )
            transform.Rotate( Vector3.up * 180f );
    }
}

}
