using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class mode1v1 : NetworkBehaviour
{
    public int scoreBlue = 0;
    public int scoreRed = 0;
    public Text scoreBlueText;
    public Text scoreRedText;

    public int maxScore = 5;

    public List<GameObject> guns;

    public bool RoundOver = false;

    // Start is called before the first frame update
    void OnEnable()
    {
        scoreBlueText.text = ("Blue " + scoreBlue + " points");
        scoreRedText.text = ("Red " + scoreRed + " points");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Server]
    public void onPlayerDeath(uint id)
    {
        RoundOver = true;
        if (NetworkIdentity.spawned[id].gameObject.layer == 13)
        {
            scoreRed++;
        }
        else if (NetworkIdentity.spawned[id].gameObject.layer == 14)
        {
            scoreBlue++;
        }
        if (scoreBlue >= maxScore || scoreRed >= maxScore)
        {
            RpcOnGameEnd();
        }
        else
        {
            RpcUpdateScore(scoreBlue, scoreRed);
            StartCoroutine(onRoundEnd());
        }
    }

    [Server]
    public IEnumerator onRoundEnd()
    {
        yield return new WaitForSeconds(5);

        //update guns
        int rnd = Random.Range(0, GameObject.FindGameObjectWithTag("allGuns").GetComponent<allGuns>().guns.Count);

        //respawn players
        foreach (KeyValuePair<uint, NetworkIdentity> ni in NetworkIdentity.spawned)
        {
            ni.Value.SendMessage("RpcRespawn", ni.Key);
            ni.Value.SendMessage("RpcNewGun", rnd);
        }

        RoundOver = false;
    }

    [ClientRpc]
    void RpcOnGameEnd()
    {
        GameObject mg = GameObject.FindGameObjectWithTag("NetworkManager");
        mg.GetComponent<NetworkManager>().StopHost();
        Destroy(mg);

        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("menuTest");
    }

    [ClientRpc]
    void RpcUpdateScore(int sB, int sR)
    {
        scoreBlueText.text = ("Blue " + sB + " points");
        scoreRedText.text = ("Red " + sR + " points");
    }
}
