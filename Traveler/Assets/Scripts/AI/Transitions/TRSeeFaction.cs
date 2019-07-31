using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRSeeFaction : Transition
{
    public FactionType TriggeringFaction;
    public string IfSeenInZone = "NONE";
    public bool InvertInZoneCondition = false;

    public override void OnSight(Observable o)
    {
        Debug.Log("On sight of : " + o);
        if (o.GetComponent<Attackable>() && o.GetComponent<Attackable>().Faction == TriggeringFaction)
        {
            Debug.Log("Faction is: " + TriggeringFaction);
            if (IfSeenInZone != "NONE")
            {
                bool inZone = ZoneManager.IsPointInZone(o.transform.position, IfSeenInZone);
                if (inZone && InvertInZoneCondition)
                {
                    return;
                }
                if (!inZone && InvertInZoneCondition)
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
        if (g.ContainsKey("IfSeenInZone", this))
            IfSeenInZone = g.GetVariable("IfSeenInZone", this);
        if (g.ContainsKey("InvertInZoneCondition", this))
            InvertInZoneCondition = (g.GetVariable("InvertInZoneCondition", this) == "TRUE");
        if (g.ContainsKey("TriggeringFaction", this))
            TriggeringFaction = (FactionType)int.Parse(g.GetVariable("TriggeringFaction", this));
    }

    public override void OnSave(Goal g)
    {
        g.SetVariable("IfSeenInZone", IfSeenInZone, this);
        g.SetVariable("InvertInZoneCondition", InvertInZoneCondition ? "TRUE" : "FALSE", this);
        g.SetVariable("TriggeringFaction", ((int)TriggeringFaction).ToString(), this);
    }
}
