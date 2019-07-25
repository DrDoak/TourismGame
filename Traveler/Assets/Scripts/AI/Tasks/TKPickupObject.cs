using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TKPickupObject : Task
{
    public override void OnActiveUpdate()
    {
        if (GetTargetObj() != null)
            MasterAI.GetComponent<MovementBase>().SetTargetPoint(GetTargetObj().transform.position);
    }
}
