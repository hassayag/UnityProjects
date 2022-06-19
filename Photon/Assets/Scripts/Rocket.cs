using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float power = 1;
    public float power_rot = 1;

    private Rigidbody rigidBody;

    private float linearSpeed, angularSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float boost = Mathf.Abs(Input.GetAxisRaw("Vertical"));
        float rotation = Input.GetAxisRaw("Horizontal");

        Vector3 pos = transform.position;
        // Vector3 velocity = transform.velocity;

        linearSpeed += boost * power;
        angularSpeed = rotation * power_rot * -1;
        
        Vector3 linearVel = linearSpeed * transform.up * Time.deltaTime;
        Quaternion angularVel = Quaternion.Euler(angularSpeed * transform.forward * Time.deltaTime);
        
        rigidBody.MovePosition(pos + linearVel);
        rigidBody.MoveRotation(transform.rotation * angularVel);

    }
}
