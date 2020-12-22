using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class shoot : NetworkBehaviour
{
    public Transform shootPoint;
    public GameObject bullet;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    [Client]
    void Update()
    {
        if (!hasAuthority) { return; }

        if (Input.GetButtonDown("Fire1"))
        {
            CmdShoot();
        }
    }

    [Command]
    void CmdShoot()
    {
        GameObject bul = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
        bullet bl = bul.GetComponent<bullet>();
        bl.bulletSpeed = 5;
        NetworkServer.Spawn(bul);
    }
}
