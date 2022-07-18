using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Planet : LivingEntity
{
    private float G = 1f;
    public float density = 1f;
    private float planetRadius;

    GameObject[] pullableObjects;

    void Start()
    {
        List<GameObject> list = new List<GameObject>();
        list.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        // list.AddRange(GameObject.FindGameObjectsWithTag("Planet"));

        pullableObjects = list.ToArray() as GameObject[];

        planetRadius = transform.lossyScale.x;

        // Assign Death function
        gameObject.GetComponent<LivingEntity>().OnDeath += OnMyDeath;
    }

    void Update()
    {
        for (int i=0; i<pullableObjects.Length; i++)
        {
            ApplyForce(pullableObjects[i]);
        }
    }

    void ApplyForce(GameObject item)
    {
        // Skip if selected object is itself
        if (item == gameObject)
        {
            return;
        }

        Vector3 direction = transform.position - item.transform.position;
        direction.z = 0;
        Rigidbody2D rigidBody = item.GetComponent<Rigidbody2D>();
        
        // F = GMm/r^2
        float m = rigidBody.mass; 
        rigidBody.AddForce(direction.normalized * G * density * Mathf.Pow(planetRadius, 2) * m / Mathf.Pow(direction.magnitude, 2));
    }

    void OnMyDeath()
    {
        // Debug.Log("YOU LOSE");
    }
}
