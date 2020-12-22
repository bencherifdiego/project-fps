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

    public Animator Animator;

    [Client]
    void Update()
    {
        if (hasAuthority)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            if (x != 0 || z != 0)
            {
                Animator.SetBool("isWalking", true);
            }
            else
            {
                Animator.SetBool("isWalking", false);
            }

            bool jump;
            if (Input.GetButtonDown("Jump"))
            {
                jump = true;
            }
            else
            {
                jump = false;
            }

            CmdMove(x, z, jump);
        }
    }

    [Command]
    void CmdMove(float x, float z, bool jump)
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 move = transform.right * x + transform.forward * z;

        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (isGrounded && jump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        RpcMove(move, velocity);
    }

    [ClientRpc]
    void RpcMove(Vector3 move, Vector3 velocity)
    {
        controller.Move(move * movevementSpeed * Time.deltaTime);

        controller.Move(velocity * Time.deltaTime);
    }
}
