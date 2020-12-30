using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class health : NetworkBehaviour
{
    public float hp = 100f;
    public Text hpText;

    private void Start()
    {
        if (hasAuthority)
        {
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
        RpcGotShot(damage);
    }

    [ClientRpc]
    void RpcGotShot(float bulDamage)
    {
        Debug.Log("test3");
        hp -= bulDamage;
        if (hasAuthority)
        {
            hpText.text = hp.ToString();
        }
    }
}
