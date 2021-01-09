using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_RayGun : S_Gun
{
    public GameObject ray;
    public int maxRays;
    List<GameObject> rays;
    List<GameObject> targets;

    protected override void Awake()
    {
        base.Awake();
        rays = new List<GameObject>();
        targets = new List<GameObject>();
    }

    public override void Shoot(Vector3 direction=default(Vector3), GameObject target=default(GameObject))
    {
        base.Shoot(direction, target);
        // Check which are the closest enemies
        List<GameObject> enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        enemies = enemies.OrderBy(x=>(x.transform.position - gameObject.transform.position).magnitude).ToList();
        foreach (GameObject enemy in enemies) {
            if (targets.Contains(enemy)) {
                continue;
            } else {
                Ray(enemy);
            }
        }
    }

    public override void StopShooting()
    {
        base.StopShooting();
        foreach (GameObject ray in rays) {
            Destroy(ray);
        }
    }

    void Ray(GameObject target)
    {
        // Generate rays
        if (rays.Count < maxRays) {
            bool shootable = true;
            Vector3 originPosition = new Vector3(gameObject.transform.position.x, 2.0f, gameObject.transform.position.z);
            Vector3 targetPosition = target.transform.Find("Spot").transform.position;
            float distance = (originPosition-targetPosition).magnitude;
            if (distance <= range) {
                // Check that we are not hitting something in between
                RaycastHit[] hits = Physics.RaycastAll(originPosition, targetPosition-originPosition, distance);
                foreach (RaycastHit hit in hits) {
                    if (hit.transform.gameObject.tag == "Shield") {
                        shootable = false;
                    }
                }
                if (shootable) {
                    GameObject rayInstance = Instantiate(ray) as GameObject;
                    rayInstance.GetComponent<S_Ray>().originGameObject = GameObject.FindWithTag("Player");
                    rayInstance.GetComponent<S_Ray>().targetGameObject = target;
                    rayInstance.GetComponent<S_Ray>().maxLength = range;
                    rays.Add(rayInstance);
                    targets.Add(target);
                }
            }
        }
    }

    void Update()
    {
        // Update existing rays
        for (int i=rays.Count-1;i>=0;i--) {
            GameObject ri = rays[i];
            if (ri.GetComponent<S_Ray>().GetTarget() == null) {
                rays.Remove(ri);
                Destroy(ri);
            } else if (ri.GetComponent<S_Ray>().Length() > range) {
                targets.Remove(ri.GetComponent<S_Ray>().GetTarget());
                rays.Remove(ri);
                Destroy(ri);
            }
        }
    }
}
