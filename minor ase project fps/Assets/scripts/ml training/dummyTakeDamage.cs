using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dummyTakeDamage : MonoBehaviour
{
    public float health = 100f;
    public float healthStart = 100f;
    public List<Vector3> spawnPositions;

    public GameObject agent;

    int rnd;

    public List<GameObject> walls;

    //public List<GameObject> walls;

    private void Start()
    {
        //GameObject[] wlls = GameObject.FindGameObjectsWithTag("wall");

        //foreach (GameObject wall in wlls)
        //{
        //    walls.Add(wall);
        //}
    }

    public void gotShot(float damage)
    {
        Debug.Log("test");
        health -= damage;

        if (health <= 0)
        {
            die();
        }
    }

    Vector3 spawnPos()
    {
        float x = Random.Range(5, 36);
        float y = 1;
        float z = Random.Range(-35, -4);

        return (new Vector3(x, y, z));
    }

    void die()
    {
        GameObject.FindGameObjectWithTag("agent").GetComponent<agentShoot2>().RegisterKill();
        respawn();
    }

    public void respawn()
    {
        while (true)
        {
            Vector3 pos = spawnPos();
            bool closeToWall = false;

            foreach (GameObject wall in walls)
            {
                float dist = Vector3.Distance(wall.GetComponent<Collider>().ClosestPoint(pos), pos);
                if (dist <= 2)
                {
                    closeToWall = true;
                }
            }
            if (Vector3.Distance(pos, agent.transform.localPosition) > 4 && !closeToWall && Vector3.Distance(pos, agent.transform.localPosition) < 25)
            {
                transform.localPosition = pos;
                break;
            }
        }
        health = healthStart;
    }

    //private void OnMouseDown()
    //{
    //    gotShot(healthStart);
    //}
}
