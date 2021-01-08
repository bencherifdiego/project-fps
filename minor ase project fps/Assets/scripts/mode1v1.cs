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
        }
    }

    [ClientRpc]
    void RpcOnGameEnd()
    {
        GameObject mg = GameObject.FindGameObjectWithTag("NetworkManager");
        mg.GetComponent<NetworkManager>().StopHost();
        SceneManager.LoadScene("menuTest");
    }

    [ClientRpc]
    void RpcUpdateScore(int sB, int sR)
    {
        scoreBlueText.text = ("Blue " + sB + " points");
        scoreRedText.text = ("Red " + sR + " points");
    }
}
