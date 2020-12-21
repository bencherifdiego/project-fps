using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class playerMovement : NetworkBehaviour
{
    public CharacterController controller;

    public float movevementSpeed = 12f;

    Vector3 velocity;
    public float gravity = -9.81f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;

    public float jumpHeight = 3;

    [Client]
    void Update()
    {
        if (hasAuthority)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * movevementSpeed * Time.deltaTime);

            if (!isGrounded)
            {
                velocity.y += gravity * Time.deltaTime;
            }
            else if (isGrounded && Input.GetButtonDown("Jump"))
            {
                Debug.Log("test");
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            controller.Move(velocity * Time.deltaTime);
        }
    }
}
