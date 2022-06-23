using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;
    float damage = 1;
    public float lifeTime = 1;

    private Rigidbody2D myRigidBody;

    void Start()
    {
        GameObject.Destroy(gameObject, lifeTime);
        // Collider [] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        // if (initialCollisions.Length > 0)
        // {
        //     OnHitObject(initialCollisions[0]);
        // }
    }

    public void SetVelocity(Vector3 v)
    {
        myRigidBody = gameObject.GetComponent<Rigidbody2D>();
        myRigidBody.velocity = v;
    }

    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag != "Player")
        {
            OnHitObject(collider);
        }
    }   

    void OnHitObject(Collider2D collider)
    {
        IDamageable damageableObject = collider.GetComponent<IDamageable> ();
        if (damageableObject != null) {
            damageableObject.TakeDamage(damage);
        }
        GameObject.Destroy (gameObject);
    }
}
