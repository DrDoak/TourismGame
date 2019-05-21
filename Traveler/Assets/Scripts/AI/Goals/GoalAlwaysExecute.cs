using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalAlwaysExecute : Goal
{
    public GameObject BehaviourPrefab;
    Behaviour b;
    private void Awake()
    {
        b = new Behaviour("AutoSet", BehaviourPrefab,this,10000.0f);
    }
    public override void OnStart()
    {
        //Debug.Log("ON start, proposed behaviour");
        m_masterAI.ProposeNewBehaviour(b);
    }
    public override void OnEnterZone(Zone z)
    {
        //Debug.Log("On enter zone: " + z.Label);
    }
}
