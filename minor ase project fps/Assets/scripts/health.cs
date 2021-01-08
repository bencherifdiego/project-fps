 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class health : NetworkBehaviour
{
    public float hp = 100f;
    public Text hpText;

    public GameObject[] spawns;

    private void Start()
    {
        if (hasAuthority)
        {
            gameObject.name = "player";
            spawns = GameObject.FindGameObjectsWithTag("Respawn");
            hpText.text = hp.ToString();
        }
    }

    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "bullet")
        {
            bullet bul = other.GetComponent<bullet>();
            float bulDamage = bul.damage;

            //CmdGotShot(bulDamage);
        }
    }

    [Server]
    public void gotShot(float damage)
    {
        hp -= damage;
        mode1v1 mode = GameObject.FindGameObjectWithTag("mode").GetComponent<mode1v1>();
        if (hp <= 0)
        {
            mode.onPlayerDeath(gameObject.GetComponent<NetworkIdentity>().netId);
            RpcRespawn(gameObject.GetComponent<NetworkIdentity>().netId);
        }
        else
        {
            RpcGotShot(hp);
        }
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
    void RpcRespawn(uint id)
    {
        hp = 100f;

        if (hasAuthority)
        {
            hpText.text = hp.ToString();
        }

        int rnd = Random.Range(0, spawns.Length);

        //transform.position = new Vector3(0, 1.2f, 0);
        NetworkIdentity.spawned[id].gameObject.GetComponent<CharacterController>().enabled = false;
        NetworkIdentity.spawned[id].gameObject.transform.position = new Vector3(0, 1.2f, 0);
        NetworkIdentity.spawned[id].gameObject.GetComponent<CharacterController>().enabled = true;
        //gameObject.transform.position = new Vector3(0, 1.2f, 0);
        //this.transform.position.Set(0, 1.2f, 0);
    }
}
