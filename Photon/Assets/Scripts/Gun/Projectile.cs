using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;
    float damage = 1;

    float lifeTime = 5f;

    private Rigidbody2D myRigidBody;

    public void Init(float range, Vector3 velocity, float baseDamage)
    {
        myRigidBody = gameObject.GetComponent<Rigidbody2D>();
        myRigidBody.velocity = velocity;

        lifeTime = range / velocity.magnitude;
        
        damage = baseDamage;
    }

    void Start()
    {
        GameObject.Destroy(gameObject, lifeTime);
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
        GameObject.Destroy (gameObject);

        // Do not allow damage to our own planet
        if (collider.gameObject.tag == "Planet")
        {
            return;
        }

        IDamageable damageableObject = collider.GetComponent<IDamageable> ();
        if (damageableObject != null) {
            damageableObject.TakeDamage(damage);
        }
    }
}
