using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class agentShoot2 : Agent
{
    bool shotAvaliable = true;
    public Transform shootingPoint;
    int damage = 100;

    float yrotate = 0f;

    Vector3 startPos;

    float dist;

    public GameObject dummy;
    dummyTakeDamage dum;

    [SerializeField] private Transform targetTransform;

    public RayPerceptionSensorComponent3D rps;

    bool firstEpisode = true;

    private void Start()
    {
        startPos = transform.localPosition;
        dum = dummy.GetComponent<dummyTakeDamage>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localRotation);
        sensor.AddObservation(Vector3.Distance(transform.position, targetTransform.position));
        sensor.AddObservation(rps);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        float xTran = vectorAction[0];
        float zTran = vectorAction[1];
        yrotate += vectorAction[2];

        float movementSpeed = 2;
        transform.localPosition += new Vector3(xTran, 0, zTran) * Time.deltaTime * movementSpeed;
        transform.rotation = Quaternion.Euler(0, yrotate, 0);

        float distance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);

        if (distance <= 2f)
        {
            AddReward(100f);
            EndEpisode();
        }
        //else if (distance > 15)
        //{
        //    AddReward(-200f);
        //    EndEpisode();
        //}

        AddReward(-0.05f);

        

        //if(shot > 0)
        //{
        //    shoot();
        //}
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetKey(KeyCode.P) ? 1f : 0f;
    }

    void shoot()
    {
        var layerMask = ~(1 << gameObject.layer);
        var direction = transform.forward;

        Debug.Log("Shot");
        Debug.DrawRay(shootingPoint.position, direction * 5f, Color.green, 2f);

        if (Physics.Raycast(shootingPoint.position, direction, out var hit, layerMask))
        {
            hit.transform.SendMessage("gotShot", damage);
        }
    }

    public void RegisterKill()
    {
        Debug.Log("reward added");
        AddReward(100.0f);
        
        EndEpisode();
    }

    public override void OnEpisodeBegin()
    {
        //transform.localPosition = startPos;

        if (!firstEpisode)
        {
            dum.respawn();
        }
        else if (firstEpisode)
        {
            firstEpisode = false;
        }

        base.OnEpisodeBegin();
    }
}
