using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gun : MonoBehaviour
{
    //gun stats
    public float damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots, bulletSpeed;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    public int bulletsLeft, bulletsShot;

    //bools
    public bool shooting, readyToShoot, reloading;

    public Transform shootPoint;

    public Animator Animator;

    //[Client]
    //void Awake()
    //{
    //    bulletsLeft = magazineSize;
    //    readyToShoot = true;
    //}

    //[Client]
    //void Update()
    //{
    //    if (!hasAuthority) { return; }

    //    if (allowButtonHold)
    //    {
    //        shooting = Input.GetButton("Fire1");
    //    }
    //    else
    //    {
    //        shooting = Input.GetButtonDown("Fire1");
    //    }

    //    if (Input.GetKeyDown(KeyCode.R))
    //    {
    //        CmdReload();
    //    }

    //    if (shooting)
    //    {
    //        Debug.Log("shoot");
    //        CmdShoot();
    //    }
    //}

    //[Command]
    //void CmdShoot()
    //{
    //    if (readyToShoot && !reloading && bulletsLeft > 0)
    //    {
    //        readyToShoot = false;

    //        //shoot
    //        GameObject bul = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
    //        bullet bl = bul.GetComponent<bullet>();
    //        bl.bulletSpeed = 5;
    //        bl.damage = damage;
    //        NetworkServer.Spawn(bul);

    //        bulletsLeft--;
    //        Invoke("resetShot", timeBetweenShooting);
    //    }
    //}
    //[Command]
    //void CmdResetShot()
    //{
    //    readyToShoot = true;
    //}

    //[Command]
    //void CmdReload()
    //{
    //    if (bulletsLeft < magazineSize && !reloading)
    //    {
    //        reloading = true;
    //        Invoke("CmdReloadFinished", reloadTime);
    //    }
    //}
    //[Command]
    //void CmdReloadFinished()
    //{
    //    bulletsLeft = magazineSize;
    //    reloading = false;
    //}
}
