using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class shoot2OBJ : NetworkBehaviour
{
    public GameObject gunOBJ;
    public gun gun;

    public GameObject bullet;
    public Transform shootPoint;

    [Client]
    void Start()
    {
        

        if (!hasAuthority) { return; }

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
            CmdShoot(gun.readyToShoot, gun.reloading, gun.bulletsLeft, gun.bulletSpeed, gun.damage);
        }
    }

    [Command]
    void CmdShoot(bool readyToShoot, bool reloading, int bulletsLeft, float bulletSpeed, float damage)
    {
        Debug.Log("test");
        Debug.Log(readyToShoot);
        if (readyToShoot && !reloading && bulletsLeft > 0)
        {
            //shoot
            GameObject bul = Instantiate(bullet) as GameObject;
            bul.transform.position = shootPoint.position;
            bul.transform.rotation = shootPoint.rotation;
            bullet bl = bul.GetComponent<bullet>();
            bl.bulletSpeed = bulletSpeed;
            bl.damage = damage;
            NetworkServer.Spawn(bul);

            RpcShoot();
        }
    }
    [ClientRpc]
    void RpcShoot()
    {
        if (!hasAuthority) { return; }
        Debug.Log("shoot");
        gun.readyToShoot = false;

        gun.bulletsLeft--;
        StartCoroutine(resetShot());
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
