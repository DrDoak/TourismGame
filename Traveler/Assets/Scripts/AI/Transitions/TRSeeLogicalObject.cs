using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRSeeLogicalObject : Transition
{
    public string ObjectSeen = "none";
    public string ZoneName = "NONE";
    public bool TriggerIfInZone = true;

    public override void OnSight(Observable o)
    {
        if (o.GetComponent<LogicalObject>() && o.GetComponent<LogicalObject>().Label == ObjectSeen)
        {
            if (ZoneName != "NONE")
            {
                bool inZone = ZoneManager.IsPointInZone(o.transform.position, ZoneName);
                if (inZone && !TriggerIfInZone)
                {
                    return;
                }
                if (!inZone && TriggerIfInZone)
                {
                    return;
                }
            }
            TargetTask.Target = o.gameObject;
            TriggerTransition();
        }
    }
}
