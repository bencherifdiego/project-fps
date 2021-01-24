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
    public GameObject muzzleFlash;

    health health;

    [Client]
    void Start()
    {
        gun = gunOBJ.GetComponent<gun>();

        if (!hasAuthority) { return; }

        health = this.GetComponent<health>();

        gun.bulletsLeft = gun.magazineSize;
        gun.readyToShoot = true;

        GetComponent<health>().ammoText.text = gun.bulletsLeft.ToString() + " / " + gun.magazineSize.ToString();

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
            CmdShoot(gun.readyToShoot, gun.reloading, gun.bulletsLeft, gun.damage, cam.transform.position, cam.transform.forward, gun.range, this.gameObject.GetComponent<NetworkIdentity>().netId, gun.horizontalSpread, gun.verticalSpread, gun.bulletsPerTap, gun.timeBetweenShots, gun.isShotgun, gun.timeBetweenShooting, gun.shootPoint);

            gun.readyToShoot = false;
        }
    }

    [Command]
    void CmdShoot(bool readyToShoot, bool reloading, int bulletsLeft, float damage, Vector3 firePoint, Vector3 dir, float range, uint id, float horizontalSpread, float verticalSpread, int bulletsPerTap, float timeBetweenShots, bool isShotgun, float timeBetweenShooting, Transform shootPoint)
    {
        if (GetComponent<health>().isDead) { return; }

        StartCoroutine(serverShoot(readyToShoot, reloading, bulletsLeft, damage, firePoint, dir, range, id, horizontalSpread, verticalSpread, bulletsPerTap, timeBetweenShots, isShotgun, timeBetweenShooting, shootPoint));
    }
    [Server]
    IEnumerator serverShoot(bool readyToShoot, bool reloading, int bulletsLeft, float damage, Vector3 firePoint, Vector3 dir, float range, uint id, float horizontalSpread, float verticalSpread, int bulletsPerTap, float timeBetweenShots, bool isShotgun, float timeBetweenShooting, Transform shootPoint)
    {
        if (isShotgun)
        {
            damage = damage / bulletsPerTap;
        }
        if (readyToShoot && !reloading && bulletsLeft > 0)
        {
            RpcSetReadyToShoot(false);

            for (int i = 0; i < bulletsPerTap; i++)
            {
                RaycastHit rayHit;

                float x = Random.Range(-horizontalSpread, horizontalSpread);
                float y = Random.Range(-verticalSpread, verticalSpread);

                dir = dir + new Vector3(x, y, 0);

                Debug.Log(gameObject.layer);
                int layer = ~(1 << gameObject.layer);

                if (Physics.Raycast(firePoint, dir, out rayHit, range, layer))
                {
                    Debug.Log(rayHit.collider);

                    int enemyLayer = 11;
                    switch (gameObject.layer)
                    {
                        case 13:
                            enemyLayer = 14;
                            break;
                        case 14:
                            enemyLayer = 13;
                            break;
                    }


                    if (rayHit.collider.gameObject.layer == enemyLayer)
                    {
                        NetworkIdentity.spawned[rayHit.collider.transform.root.gameObject.GetComponent<NetworkIdentity>().netId].SendMessage("gotShot", damage);
                    }
                    else
                    {
                        Quaternion hitRotation = Quaternion.FromToRotation(Vector3.forward, rayHit.normal);
                        GameObject bulletHole = Instantiate(bulletHoleGraphic, rayHit.point, hitRotation);

                        //GameObject muzleFlase = Instantiate(muzzleFlash, shootPoint);

                        NetworkServer.Spawn(bulletHole);
                        //NetworkServer.Spawn(muzleFlase);
                    }
                }
                if (!isShotgun)
                {
                    yield return new WaitForSeconds(timeBetweenShots);
                    RpcShoot();
                }
                
            }
            if (isShotgun)
            {
                RpcShoot();
            }

            yield return new WaitForSeconds(timeBetweenShooting);

            RpcSetReadyToShoot(true);
        }
    }
    [ClientRpc]
    void RpcSetReadyToShoot(bool change)
    {
        if (!hasAuthority) { return; }
        gun.readyToShoot = change;
    }
    [ClientRpc]
    void RpcShoot()
    {
        gun.muzzleflash.Play();
        if (!hasAuthority) { return; }
        gun.bulletsLeft--;

        GetComponent<health>().ammoText.text = gun.bulletsLeft.ToString() + " / " + gun.magazineSize.ToString();
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
        gun.readyToShoot = true;
        gun.Animator.SetBool("isReloading", false);

        if (hasAuthority)
        {
            GetComponent<health>().ammoText.text = gun.bulletsLeft.ToString() + " / " + gun.magazineSize.ToString();
        }
    }
}
