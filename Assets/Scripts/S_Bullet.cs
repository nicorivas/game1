using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Bullet : MonoBehaviour
{
    public float MoveSpeed;
    public Vector3 moveDirection;
    public Quaternion initialRotation;
    public float power;
    protected Rigidbody rb;
    public bool collideWithTerrain;
    // Start is called before the first frame update
    protected void Start()
    {
        rb = GetComponent<Rigidbody>();
        collideWithTerrain = false;
    }

    // Update is called once per frame
    void Update()
    {
        //
    }

    private void HitHero(GameObject other)
    {
        Player.instance.Hurt(power);
        Eliminate();
    }

    protected void Eliminate() {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            HitHero(other.gameObject);
        } else if (other.tag == "Outside") {
            Destroy(gameObject);
        } else if (other.tag == "TerrainTile" && collideWithTerrain) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // For cases when the player is quiting rolling state inside bullet.
        if (other.tag == "Player") {
            HitHero(other.gameObject);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(transform.position+moveDirection*MoveSpeed*Time.fixedDeltaTime);
    }

}
