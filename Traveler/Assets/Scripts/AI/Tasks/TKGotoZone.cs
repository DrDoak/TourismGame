using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TKGotoZone : Task
{
    // Start is called before the first frame update
    void Start() { }

    public override void OnTransition()
    {
        /*Debug.Log("On transition. Target is: " + Target + " point : " + 
            Target.GetComponent<Zone>().NearestPointToZone(MasterAI.transform.position));*/
        if (GetTargetObj() != null)
        {
            MasterAI.GetComponent<MovementBase>().SetTargetPoint(GetTargetObj().GetComponent<Zone>().transform.position);
            /*MasterAI.GetComponent<MovementBase>().SetTargetPoint(
                Target.GetComponent<Zone>().NearestPointToZone(MasterAI.transform.position));*/
        }
    }
}
