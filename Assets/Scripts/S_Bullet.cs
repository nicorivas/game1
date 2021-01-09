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
    public bool explosive;
    Vector3 lastPosition;
    public GameObject owner;
    // Start is called before the first frame update
    void Awake() {
        gameObject.layer = 11;
        // Ignore collisions with borders of the map (invisible ones)
        GameObject[] borders = GameObject.FindGameObjectsWithTag("TerrainBorder");
        foreach (GameObject border in borders)
        {
            Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), border.GetComponent<Collider>());
        }
    }
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

    void Explode() {
        GameObject explosion = Instantiate(Resources.Load("Explosion")) as GameObject;
        explosion.transform.position = gameObject.transform.position;
        Eliminate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") {
            HitHero(collision.gameObject);
        } else if (collision.gameObject.tag == "Outside") {
            Eliminate();
        } else if (collision.gameObject.tag == "Block") {
            collision.gameObject.GetComponent<S_Block>().Hurt(power);
        } else if (collision.gameObject.tag == "TerrainTile") {
            if (explosive) {
                Explode();
            }
        }
    }

    void FixedUpdate()
    {
        lastPosition = transform.position;
        //rb.MovePosition(transform.position+moveDirection*MoveSpeed*Time.fixedDeltaTime);
    }

}
