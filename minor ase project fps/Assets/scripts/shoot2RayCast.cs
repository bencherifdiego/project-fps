using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class shoot2RayCast : NetworkBehaviour
{
    public GameObject gunOBJ;
    public gun gun;

    public GameObject bullet;
    public Transform shootPoint;
    public GameObject cam;

    health health;

    [Client]
    void Start()
    {


        if (!hasAuthority) { return; }

        health = this.GetComponent<health>();

        gun = gunOBJ.GetComponent<gun>();
        gun.bulletsLeft = gun.magazineSize;
        gun.readyToShoot = true;

        Debug.Log("Test");
    }

    [Client]
    void Update()
    {
        if (!hasAuthority) { return; }

        if (gun.allowButtonHold)
        {
            gun.shooting = Input.GetButton("Fire1");
        }
        else
        {
            gun.shooting = Input.GetButtonDown("Fire1");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            CmdReload(gun.bulletsLeft, gun.magazineSize, gun.reloading);
        }

        if (gun.shooting)
        {
            Debug.Log("shooting");
            CmdShoot(gun.readyToShoot, gun.reloading, gun.bulletsLeft, gun.damage, cam.transform.position, cam.transform.forward, gun.range);
        }
    }

    [Command]
    void CmdShoot(bool readyToShoot, bool reloading, int bulletsLeft, float damage, Vector3 firePoint, Vector3 dir, float range)
    {
        Debug.Log("test");
        Debug.Log(readyToShoot);
        if (readyToShoot && !reloading && bulletsLeft > 0)
        {
            //shoot
            //RpcShoot();

            RaycastHit rayHit;

            //if (hasAuthority)
            {
                readyToShoot = false;

                if (Physics.Raycast(firePoint, dir, out rayHit, range))
                {
                    Debug.Log(rayHit.collider);

                    if (rayHit.collider.gameObject.layer == 11)
                    {
                        //CmdShot(rayHit.collider.gameObject.GetComponent<NetworkIdentity>().netId);

                        NetworkIdentity.spawned[rayHit.collider.gameObject.GetComponent<NetworkIdentity>().netId].SendMessage("gotShot", damage);
                    }
                }
                RpcTest();
            }
        }
    }
    [ClientRpc]
    void RpcTest()
    {
        if (!hasAuthority) { return; }
        gun.bulletsLeft--;
        StartCoroutine(resetShot());
    }

    [ClientRpc]
    void RpcShoot()
    {
        RaycastHit rayHit;

        if (hasAuthority)
        {
            gun.readyToShoot = false;

            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out rayHit, gun.range))
            {
                Debug.Log(rayHit.collider);

                if (rayHit.collider.gameObject.layer == 11)
                {
                    CmdShot(rayHit.collider.gameObject.GetComponent<NetworkIdentity>().netId);
                }
            }



            gun.bulletsLeft--;
            StartCoroutine(resetShot());
        }
    }
    [Command]
    void CmdShot(uint id)
    {
        //RpcShot(id);

        Debug.Log(id);
        Debug.Log("test1");
        try
        {
            //GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            //GameObject player = null;
            //foreach (GameObject x in players)
            //{
            //    if (x.GetComponent<NetworkIdentity>().netId == id)
            //    {
            //        player = x;
            //    }
            //}
            //player.GetComponent<NetworkIdentity>().SendMessage("CmdGotShot", gun.damage);

            //player.GetComponent<health>().CmdGotShot(gun.damage);

            NetworkIdentity.spawned[id].SendMessage("gotShot", gun.damage);
        }
        catch { }
    }
    [ClientRpc]
    void RpcShot(uint id)
    {
        
    }
    [Client]
    IEnumerator resetShot()
    {
        if (hasAuthority)
        {
            yield return new WaitForSeconds(gun.timeBetweenShooting);
            Debug.Log("reset");
            gun.readyToShoot = true;
        }
    }

    [Command]
    void CmdReload(int bulletsLeft, int magazineSize, bool reloading)
    {
        if (bulletsLeft < magazineSize && !reloading)
        {
            RpcReload();
        }
    }
    [ClientRpc]
    void RpcReload()
    {
        if (!hasAuthority) { return; }
        gun.Animator.SetBool("isReloading", true);
        gun.reloading = true;
        StartCoroutine(reloadFinished());
    }
    [Client]
    IEnumerator reloadFinished()
    {
        yield return new WaitForSeconds(gun.reloadTime);

        gun.bulletsLeft = gun.magazineSize;
        gun.reloading = false;
        gun.Animator.SetBool("isReloading", false);
    }
}
