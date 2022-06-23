using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingBody : LivingEntity
{    
    private Utils utils;

    private Rigidbody2D myRigidBody;
    public float maxSpeed, maxSpin, childSpacing, childSpeed;
    public GameObject AsteroidChild;
    private bool finalChild;

    public void Init(float size, Vector3 initVelocity, bool randomVelocity, bool finalChild)
    {
        utils = new Utils();

        myRigidBody = gameObject.GetComponent<Rigidbody2D>();

        if (randomVelocity)
        {
            initVelocity = new Vector2 (Random.Range(0, maxSpeed), Random.Range(0,maxSpeed));
        }
        
        float torque = Random.Range(0, maxSpin);

        myRigidBody.velocity = initVelocity;
        myRigidBody.AddTorque(torque);

        // Set size and mass
        transform.localScale = new Vector3 (size,size,size);
        myRigidBody.mass = utils.Remap(size, 0, 5, 0, 20);

        // Assign Death function
        gameObject.GetComponent<LivingEntity>().OnDeath += OnMyDeath;
    }

    void Update()
    {
        
    }

    void OnMyDeath()
    {
        if (!finalChild)
        {
            SpawnChildren();
        }

        GameObject.Destroy(gameObject);
    } 

    void SpawnChildren()
    {
        // Get random line around astroid 
        float randomAngle = Random.Range(0,180);
        Vector3 randomLine = new Vector3 (Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0) * childSpacing;

        Vector3 pos1 = transform.position + randomLine;
        Vector3 pos2 = transform.position - randomLine;

        // Spawn Children
        SpawnChild(pos1);
        SpawnChild(pos2);

    }

    void SpawnChild(Vector3 pos)
    {
        GameObject child = Instantiate(AsteroidChild, pos, new Quaternion(0,0,Random.Range(0,360),1));
        child.GetComponent<WanderingBody>().Init(transform.localScale.x/2, childSpeed * (pos-transform.position) + new Vector3 (myRigidBody.velocity.x, myRigidBody.velocity.y, 0), false, false);
    }
}
