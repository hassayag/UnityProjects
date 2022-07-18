using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : LivingEntity
{
    public float speed = 1.0f; 
    public float damage = 1.0f;

    private Vector3 targetPos, targetDir;
    private Rigidbody2D myRigidBody;

    // Start is called before the first frame update
    void Start()
    {
        GameObject home = GameObject.FindGameObjectWithTag("Home");

        targetPos = home.transform.position;
        targetDir = targetPos - transform.position;

        // Set velocity
        myRigidBody = gameObject.GetComponent<Rigidbody2D>();
        myRigidBody.velocity = targetDir.normalized * speed;

        // Assign Death function
        gameObject.GetComponent<LivingEntity>().OnDeath += OnMyDeath;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Planet")
        {
            OnMyDeath();
            collision.collider.gameObject.GetComponent<LivingEntity>().TakeDamage(damage);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveToTarget(targetPos);
    }

    void MoveToTarget(Vector3 targetPos)
    {
        var lookRotation = Quaternion.FromToRotation(transform.position, targetDir);

        transform.rotation = lookRotation;
    }

    void OnMyDeath()
    {
        transform.parent.GetComponent<WaveSpawner>().EnemyDied();
        Destroy(gameObject);
    }
}
