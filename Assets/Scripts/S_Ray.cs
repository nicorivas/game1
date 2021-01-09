using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Ray : MonoBehaviour
{
    public GameObject originGameObject, targetGameObject;
    public float maxLength;
    LineRenderer lineRenderer;

    void Awake() {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    void Start()
    {
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = 2;
    }

    public GameObject GetTarget() {
        return targetGameObject;
    }

    public float Length() {
        Vector3 originPosition = new Vector3(originGameObject.transform.position.x, 2.0f, originGameObject.transform.position.z);
        Vector3 targetPosition = targetGameObject.transform.Find("Spot").transform.position;
        return (originPosition-targetPosition).magnitude;
    }

    void Update()
    {
        if (targetGameObject != null) {
            Vector3 originPosition = new Vector3(originGameObject.transform.position.x, 2.0f, originGameObject.transform.position.z);
            Vector3 targetPosition = targetGameObject.transform.Find("Spot").transform.position;
            lineRenderer.SetPosition(0, originPosition);
            lineRenderer.SetPosition(1, targetPosition);
            targetGameObject.GetComponent<S_Enemy>().Hurt();
        }
    }
}
