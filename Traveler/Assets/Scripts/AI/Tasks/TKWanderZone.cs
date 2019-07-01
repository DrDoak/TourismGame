﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TKWanderZone : Task
{

    public Vector2 WaitTimeRange;
    public Vector2 WanderDistanceRange;

    private float nextMoveTime = 0;
    private const int MAX_TRIES = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public override void OnActiveUpdate()
    {
        if (Time.timeSinceLevelLoad > nextMoveTime)
            pickNewDestination();
    }

    private void pickNewDestination()
    {
        int numTries = 0;
        Vector3 nextSpot = transform.position;
        while (numTries < MAX_TRIES )
        {
            nextSpot = RandomPointFromPoint(MasterAI.transform.position,
                Random.Range(WanderDistanceRange.x, WanderDistanceRange.y));
            //Debug.Log("In zone?: " + Target.GetComponent<Zone>().IsInZone(nextSpot));
            if (Target.GetComponent<Zone>().IsInZone(nextSpot))
            {
                //Debug.Log("Next Point: " + nextSpot );
                MasterAI.GetComponent<ControlAI>().SetTarget(nextSpot);
                break;
            }
            numTries++;
        }
        nextMoveTime = Time.timeSinceLevelLoad + Random.Range(WaitTimeRange.x, WaitTimeRange.y);
    }

    private Vector3 RandomPointFromPoint(Vector3 start, float dist)
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);
        //Debug.Log("Transform?: " + start);
        Vector3 newPoint = start + new Vector3(Mathf.Cos(angle) * dist, 0f, Mathf.Sin(angle) * dist);
        //Debug.Log("Next Point: " + newPoint + " angle: " + angle + " dist: " + dist + " ds: " + Mathf.Sin(angle) * dist);
        return newPoint;
    }
}
