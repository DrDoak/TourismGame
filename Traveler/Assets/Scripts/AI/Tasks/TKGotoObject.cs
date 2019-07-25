using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TKGotoObject : Task
{
    public override void OnActiveUpdate()
    {
        if (GetTargetObj() != null)
            MasterAI.GetComponent<MovementBase>().SetTargetPoint(GetTargetObj().transform.position);
    }
}
