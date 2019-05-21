using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TKPickupObject : Task
{
    public override void OnActiveUpdate()
    {
        if (Target != null)
            MasterAI.GetComponent<MovementBase>().SetTargetPoint(Target.transform.position);
    }
}
