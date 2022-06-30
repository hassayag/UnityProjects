using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    private float G = 1f;
    public float density = 1f;

    private float planetRadius;

    GameObject[] pullableObjects;

    // Start is called before the first frame update
    void Start()
    {
        pullableObjects = GameObject.FindGameObjectsWithTag("Player");

        planetRadius = transform.localScale.x;
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
        float m = 1; // This should become a variable based on the object being pulled
        rigidBody.AddForce(direction.normalized * G * density * Mathf.Pow(planetRadius, 2) * m / Mathf.Pow(direction.magnitude, 2));
    }
}
