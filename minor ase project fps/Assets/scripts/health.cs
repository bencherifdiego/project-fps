 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class health : NetworkBehaviour
{
    public float hp = 100f;
    public Text hpText;

    public List<GameObject> guns;
    public NetworkTransformChild tChild;
    public NetworkAnimator NAnimator;
    public List<GameObject> spawns;

    public Animator animator = null;
    public List<Rigidbody> ragdollBodies;
    public List<Collider> ragdollColliders;
    public bool isDead = false;

    private void Start()
    {
        Rigidbody[] rb = GetComponentsInChildren<Rigidbody>();
        Collider[] col = GetComponentsInChildren<Collider>();

        foreach (Rigidbody RB in rb)
        {
            ragdollBodies.Add(RB);
        }
        foreach (Collider COL in col)
        {
            ragdollColliders.Add(COL);
        }

        ragdollColliders.RemoveAt(0);

        if (hasAuthority)
        {
            gameObject.name = "player";
            hpText.text = hp.ToString();
        }
    }

    [Command]
    void CmdToggleRagdoll(bool state)
    {
        isDead = state;
        RpcToggleRagdoll(state);
    }

    [ClientRpc]
    void RpcToggleRagdoll(bool state)
    {
        animator.enabled = !state;

        foreach(Rigidbody rb in ragdollBodies)
        {
            rb.isKinematic = !state;
        }
    }

    void Update()
    {
        
    }

    [Server]
    public void gotShot(float damage)
    {
        hp -= damage;
        mode1v1 mode = GameObject.FindGameObjectWithTag("mode").GetComponent<mode1v1>();
        if (hp <= 0 && !mode.RoundOver)
        {
            RpcGotShot(hp);
            RpcStartRagdoll(true);
            mode.onPlayerDeath(gameObject.GetComponent<NetworkIdentity>().netId);
        }
        else if (hp > 0)
        {
            RpcGotShot(hp);
        }
    }
    [ClientRpc]
    void RpcStartRagdoll(bool state)
    {
        if (!hasAuthority) { return; }
        CmdToggleRagdoll(state);
    }

    [ClientRpc]
    void RpcGotShot(float hp)
    {
        if (hasAuthority)
        {
            hpText.text = hp.ToString();
        }
    }

    [ClientRpc]
    void RpcNewGun(int gun)
    {
        if (!hasAuthority) { return; }

        tChild.target = guns[gun].transform;
        NAnimator.animator = guns[gun].GetComponent<Animator>();

        shoot2RayCast src = GetComponent<shoot2RayCast>();

        src.gunOBJ.active = false;
        src.gunOBJ = guns[gun];
        src.gunOBJ.active = true;

        

        src.gun = guns[gun].GetComponent<gun>();

        src.gun.bulletsLeft = src.gun.magazineSize;
        src.gun.readyToShoot = true;
    }

    [ClientRpc]
    void RpcRespawn(uint id)
    {
        RpcStartRagdoll(false);

        hp = 100f;

        if (hasAuthority)
        {
            hpText.text = hp.ToString();
        }

        NetworkIdentity.spawned[id].gameObject.GetComponent<CharacterController>().enabled = false;
        switch(gameObject.layer)
        {
            case 13:
                NetworkIdentity.spawned[id].gameObject.transform.position = new Vector3(15, 1.2f, -15);
                break;
            case 14:
                NetworkIdentity.spawned[id].gameObject.transform.position = new Vector3(-15, 1.2f, 15);
                break;
        }
        
        NetworkIdentity.spawned[id].gameObject.GetComponent<CharacterController>().enabled = true;
    }
}
