using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{

[AddComponentMenu("Character Controller Pro/Demo/Character/AI/Totem Behaviour")]
public class AITotem : CharacterAIBehaviour
{
    Vector3 initialPosition = default( Vector3 );
    List<Vector3> directions;
    S_Gun gun;
    public override void EnterBehaviour( float dt )
    {
        initialPosition = transform.position;
        directions = new List<Vector3>{Vector3.forward,Vector3.back,Vector3.left,Vector3.right};
        gun = GetComponent<S_Gun>();
    }

    // Update is called once per frame
    public override void UpdateBehaviour( float dt )
    {
        RaycastHit hit;
        foreach (Vector3 direction in directions)
        {
            if (Physics.Raycast(transform.position+Vector3.up, transform.TransformDirection(direction), out hit, Mathf.Infinity))
            {
                //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                if (hit.transform.gameObject.tag == "Player") {
                    gun.Shoot(direction);
                }
            }
        }
    }
}

}