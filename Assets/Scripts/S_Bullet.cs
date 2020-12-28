using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Bullet : MonoBehaviour
{
    public float moveSpeed;
    public float destroySeconds;
    public Vector3 moveDirection;
    public Quaternion initialRotation;
    public float power;
    protected Rigidbody rb;
    public bool collideWithTerrain;
    public bool bounce;
    Vector3 lastPosition;
    public GameObject owner;
    // Start is called before the first frame update
    protected void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddRelativeForce(moveDirection * moveSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        destroySeconds -= Time.deltaTime;
        if (destroySeconds < 0) {
            Eliminate();
        }
    }

    private void HitHero(GameObject other)
    {
        Player.instance.Hurt(power);
        Eliminate();
    }

    protected void Eliminate() {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") {
            HitHero(collision.gameObject);
        } else if (collision.gameObject.tag == "Outside") {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        lastPosition = transform.position;
        //rb.MovePosition(transform.position+moveDirection*MoveSpeed*Time.fixedDeltaTime);
    }

}
