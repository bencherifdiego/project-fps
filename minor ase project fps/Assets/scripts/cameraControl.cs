using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class cameraControl : NetworkBehaviour
{
    public Transform playerBody;
    public Camera cam;
    public AudioListener aL;
    public GameObject rP;
    public Transform shootPoint;
    public GameObject bullet;
    public float mouseSensitivity = 100;

    float xRotation = 0f;
    
    [Client]
    private void Start()
    {
        if (hasAuthority)
        {
            cam.enabled = true;
            aL.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    [Client]
    void Update()
    {
        if (hasAuthority)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            rP.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);



            if (Input.GetButtonDown("Fire1"))
            {
                Cmd_shoot();
            }
        }
    }

    [Command]
    void Cmd_shoot()
    {
        GameObject bul = Instantiate(bullet, shootPoint.position, transform.rotation);
        bullet bl = bul.GetComponent<bullet>();
        bl.bulletSpeed = 5;
        NetworkServer.Spawn(bul);
    }
}
