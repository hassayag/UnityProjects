using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject target;
    public float distance;

    // Update is called once per frame
    void Start()
    {
        transform.position = new Vector3 (target.transform.position.x, target.transform.position.y, distance);
    }

    void LateUpdate()
    {
        transform.position = new Vector3 (target.transform.position.x, target.transform.position.y, transform.position.z);
    }
}
