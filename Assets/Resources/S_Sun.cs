using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Sun : MonoBehaviour
{
    // Start is called before the first frame update
    static public float z;

    void Awake()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.x,65f,transform.rotation.z);
    }

    void Start()
    {
        transform.rotation = Quaternion.Euler(
            (Mathf.Cos(S_World.yearFloat*2.0f*Mathf.PI)+1.0f)/2.0f*140.0f+20.0f,
            65f,
            transform.rotation.z);
        z = (Mathf.Cos(S_World.yearFloat*2.0f*Mathf.PI)+1.0f)/2.0f*10.0f; 
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(
            (Mathf.Cos(S_World.yearFloat*2.0f*Mathf.PI)+1.0f)/2.0f*140.0f+20.0f,
            65f,
            transform.rotation.z);
        z = (Mathf.Cos(S_World.yearFloat*2.0f*Mathf.PI)+1.0f)/2.0f*10.0f;
    }
}
