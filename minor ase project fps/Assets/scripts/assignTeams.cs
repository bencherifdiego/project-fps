using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class assignTeams : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!hasAuthority) { return; }
        CmdTest();
        //NetworkIdentity.spawned[gameObject.GetComponent<NetworkIdentity>().netId].SendMessage("CmdAssign");
        //SendMessage("CmdAssign");
        //CmdAssign();
    }

    [Command]
    void CmdTest()
    {
        bool isBlue = false;
        foreach (KeyValuePair<uint, NetworkIdentity> ni in NetworkIdentity.spawned)
        {
            //try
            //{
            //    ni.Value.SendMessage("SAssign", isBlue);
            //}
            //catch
            //{

            //}

            SAssign(isBlue, ni.Key);

            if (isBlue)
            {
                isBlue = false;
            }
            else
            {
                isBlue = true;
            }
        }
    }

    [Server]
    void SAssign(bool isBlue, uint id)
    {
        if (isBlue)
        {
            RpcAssign(id, 13);
            //RpcAssign(gameObject.GetComponent<NetworkIdentity>().netId, 13);
        }
        else
        {
            RpcAssign(id, 14);
            //RpcAssign(gameObject.GetComponent<NetworkIdentity>().netId, 14);
        }
    }

    [Client]
    void assign(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            if (child == null)
            {
                continue;
            }
            assign(child.gameObject, layer);
        }
    }

    [ClientRpc]
    void RpcAssign(uint objInt, int layer)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
        GameObject obj = null;

        foreach (GameObject ob in objs)
        {
            if (ob.GetComponent<NetworkIdentity>().netId == objInt)
            {
                obj = ob;
            }
        }

        if (obj == null)
        {
            return;
        }

        assign(obj, layer);

        foreach (Transform child in obj.transform)
        {
            if (child == null)
            {
                continue;
            }
            assign(child.gameObject, layer);
        }
    }
}
