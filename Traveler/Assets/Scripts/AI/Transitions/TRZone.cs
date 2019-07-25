using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRZone : Transition
{
    public string ZoneName = "noZone";
    public bool InvertIfSeenInZoneCondition = false;

    private float m_nextCheck;
    private const float CHECK_INTERVAL = 1.0f;

    private void Start()
    {
        m_nextCheck = Random.Range(Time.timeSinceLevelLoad, Time.timeSinceLevelLoad + CHECK_INTERVAL);
    }
    public override void OnUpdate()
    {
        if (Time.timeSinceLevelLoad > m_nextCheck)
        {
            if (ZoneName == "noZone")
                return; //TODO, check for No Zone.

            bool IfSeenInZone = ZoneManager.IsHaveObject(MasterAI.GetComponent<AICharacter>(),ZoneName);

            if (IfSeenInZone && !InvertIfSeenInZoneCondition)
            {
                Zone z = ZoneManager.GetZone(ZoneName);
                if (z != null)
                {
                    TargetTask.SetTargetObj( z.gameObject);
                    TriggerTransition();
                }
            }
            if (!IfSeenInZone && InvertIfSeenInZoneCondition)
            {
                Zone z = ZoneManager.GetZone(ZoneName);
                if (z != null)
                {
                    TargetTask.SetTargetObj( z.gameObject );
                    TriggerTransition();
                }
            }
            m_nextCheck += CHECK_INTERVAL;
        }
    }


    public override void OnLoad(Goal g)
    {
        if (g.ContainsKey("ZoneName", this))
            ZoneName = g.GetVariable("ZoneName", this);
        if (g.ContainsKey("InvertIfSeenInZoneCondition", this))
            InvertIfSeenInZoneCondition = (g.GetVariable("InvertIfSeenInZoneCondition", this) == "TRUE");
    }

    public override void OnSave(Goal g)
    {
        g.SetVariable("ZoneName", ZoneName, this);
        g.SetVariable("InvertIfSeenInZoneCondition", InvertIfSeenInZoneCondition? "TRUE":"FALSE", this);
    }
}
