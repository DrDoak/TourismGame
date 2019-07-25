using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRSeeLogicalObject : Transition
{
    public string ObjectSeen = "none";
    public string IfSeenInZone = "NONE";
    public bool TriggerIfInZone = true;

    public override void OnSight(Observable o)
    {
        if (o.GetComponent<LogicalObject>() && o.GetComponent<LogicalObject>().Label == ObjectSeen)
        {
            if (IfSeenInZone != "NONE")
            {
                bool inZone = ZoneManager.IsPointInZone(o.transform.position, IfSeenInZone);
                if (inZone && !TriggerIfInZone)
                {
                    return;
                }
                if (!inZone && TriggerIfInZone)
                {
                    return;
                }
            }
            TargetTask.SetTargetObj(o.gameObject);
            TriggerTransition();
        }
    }
    public override void OnLoad(Goal g)
    {
        if (g.ContainsKey("ObjectSeen", this))
            ObjectSeen = g.GetVariable("ObjectSeen", this);
        if (g.ContainsKey("IfSeenInZone", this))
            IfSeenInZone = g.GetVariable("IfSeenInZone", this);
        if (g.ContainsKey("TriggerIfInZone", this))
        {
            string s = g.GetVariable("TriggerIfInZone", this);
            TriggerIfInZone = (s == "TRUE");
        }
            
    }

    public override void OnSave(Goal g)
    {
        g.SetVariable("ObjectSeen", ObjectSeen, this);
        g.SetVariable("IfSeenInZone", IfSeenInZone, this);
        g.SetVariable("TriggerIfInZone", (TriggerIfInZone)?"TRUE":"FALSE", this);
    }

}
