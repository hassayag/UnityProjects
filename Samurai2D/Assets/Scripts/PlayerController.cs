using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float playerSpeed, jumpForce, playerHeight;

    public LayerMask findGroundMask;

    bool isGrounded;

    Rigidbody2D rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = transform.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = FindGround();

        float leftRight = Input.GetAxisRaw("Horizontal");
        if (leftRight != 0)
        {
            transform.position += leftRight * playerSpeed * Time.deltaTime * transform.right;
        }
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rigidbody.AddForce(jumpForce * transform.up);
        }
    }

    bool FindGround()
    {
        Debug.DrawRay(transform.position, -Vector3.up * playerHeight, Color.yellow);
        if (Physics2D.Raycast(transform.position, -Vector3.up, playerHeight, findGroundMask))
        {
            return true;
        }

        return false;
    }
}
