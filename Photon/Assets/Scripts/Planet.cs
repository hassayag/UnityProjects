using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public float G=1;
    public float M=1;

    GameObject[] pullableObjects;

    // Start is called before the first frame update
    void Start()
    {
        pullableObjects = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        for (int i=0; i<pullableObjects.Length; i++)
        {
            ApplyForce(pullableObjects[i]);
        }
    }

    void ApplyForce(GameObject item)
    {
        Vector3 direction = transform.position - item.transform.position;
        direction.z = 0;
        
        Rigidbody2D rigidBody = item.GetComponent<Rigidbody2D>();
        
        // F = GMm/r^2
        rigidBody.AddForce(direction.normalized * G * M / Mathf.Pow(direction.magnitude, 2));
    }
}
