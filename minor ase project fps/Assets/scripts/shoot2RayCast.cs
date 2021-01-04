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

    public GameObject bulletHoleGraphic;

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
            CmdShoot(gun.readyToShoot, gun.reloading, gun.bulletsLeft, gun.damage, cam.transform.position, cam.transform.forward, gun.range, this.gameObject.GetComponent<NetworkIdentity>().netId, gun.horizontalSpread, gun.verticalSpread, gun.bulletsPerTap, gun.timeBetweenShots);
        }
    }

    [Command]
    void CmdShoot(bool readyToShoot, bool reloading, int bulletsLeft, float damage, Vector3 firePoint, Vector3 dir, float range, uint id, float horizontalSpread, float verticalSpread, int bulletsPerTap, float timeBetweenShots)
    {
        StartCoroutine(serverShoot(readyToShoot, reloading, bulletsLeft, damage, firePoint, dir, range, id, horizontalSpread, verticalSpread, bulletsPerTap, timeBetweenShots));
    }
    [Server]
    IEnumerator serverShoot(bool readyToShoot, bool reloading, int bulletsLeft, float damage, Vector3 firePoint, Vector3 dir, float range, uint id, float horizontalSpread, float verticalSpread, int bulletsPerTap, float timeBetweenShots)
    {
        for (int i = 0; i < bulletsPerTap; i++)
        {
            Debug.Log(i);
            Debug.Log(bulletsPerTap);
            if (readyToShoot && !reloading && bulletsLeft > 0)
            {
                RaycastHit rayHit;

                readyToShoot = false;

                float x = Random.Range(-horizontalSpread, horizontalSpread);
                float y = Random.Range(-verticalSpread, verticalSpread);

                dir = dir + new Vector3(x, y, 0);

                if (Physics.Raycast(firePoint, dir, out rayHit, range))
                {
                    Debug.Log(rayHit.collider);

                    if (rayHit.collider.gameObject.layer == 11)
                    {
                        NetworkIdentity.spawned[rayHit.collider.gameObject.GetComponent<NetworkIdentity>().netId].SendMessage("gotShot", damage);
                    }
                    else
                    {
                        Quaternion hitRotation = Quaternion.FromToRotation(Vector3.forward, rayHit.normal);
                        GameObject bulletHole = Instantiate(bulletHoleGraphic, rayHit.point, hitRotation);
                        NetworkServer.Spawn(bulletHole);
                    }
                }
                RpcShoot();

                yield return new WaitForSeconds(timeBetweenShots);
                Debug.Log("reset");
                readyToShoot = true;
            }
        }
    }
    [ClientRpc]
    void RpcShoot()
    {
        if (!hasAuthority) { return; }
        gun.bulletsLeft--;
        //StartCoroutine(resetShot());
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
