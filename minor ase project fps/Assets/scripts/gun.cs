using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gun : MonoBehaviour
{
    //gun stats
    public float damage;
    public float timeBetweenShooting, horizontalSpread, verticalSpread, range, reloadTime, timeBetweenShots, bulletSpeed;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold, isShotgun;
    public int bulletsLeft, bulletsShot;

    //bools
    public bool shooting, readyToShoot, reloading;

    //references
    public Transform shootPoint;
    public Animator Animator;
    public GameObject bulletHoleGraphic;
    public ParticleSystem muzzleflash;
}
