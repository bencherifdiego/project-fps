using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class agentShoot1 : Agent
{
    Vector3 startingPosition;

    public Transform shootingPoint;
    public int minStepsBetweenShots = 50;
    public float damage = 100;

    bool shotAvaliable = true;
    int stepsUntilShotIsAvaliable = 0;

    Rigidbody rb;

    void Shoot()
    {
        if (!shotAvaliable) { return; }

        var layerMask = 1 << LayerMask.NameToLayer("player");
        var direction = transform.forward;

        Debug.Log("Shot");
        Debug.DrawRay(shootingPoint.position, direction * 200f, Color.green, 2f);

        if (Physics.Raycast(shootingPoint.position, direction,out var hit, 200f, layerMask))
        {
            //hit.transform.GetComponent<dummyTakeDamage>().gotShot(damage, this.gameObject);
        }

        shotAvaliable = false;
        stepsUntilShotIsAvaliable = minStepsBetweenShots;
    }

    private void FixedUpdate()
    {
        if (!shotAvaliable)
        {
            stepsUntilShotIsAvaliable--;

            if (stepsUntilShotIsAvaliable <= 0)
            {
                shotAvaliable = true;
            }
        }
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        if (Mathf.RoundToInt(vectorAction[0]) >= 1)
        {
            Shoot();
        }
    }

    public override void Initialize()
    {
        startingPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetKey(KeyCode.P) ? 1f : 0f;
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("Episode Begin");

        transform.position = startingPosition;
        rb.velocity = Vector3.zero;
        shotAvaliable = true;
    }

    public void RegisterKill()
    {
        AddReward(1.0f);
        EndEpisode();
    }
}
