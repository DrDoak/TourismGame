using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRZone : Transition
{
    public string ZoneName = "noZone";
    public bool TriggerWhenInZone = true;

    private float m_nextCheck;
    private const float CHECK_INTERVAL = 1.0f;

    private void Start()
    {
        m_nextCheck = Random.RandomRange(Time.timeSinceLevelLoad, Time.timeSinceLevelLoad + CHECK_INTERVAL);
    }
    public override void OnUpdate()
    {
        if (Time.timeSinceLevelLoad > m_nextCheck)
        {
            if (ZoneName == "noZone")
                return; //TODO, check for No Zone.

            bool inZone = ZoneManager.IsHaveObject(MasterAI.GetComponent<AICharacter>(),ZoneName);
            //Debug.Log("Trigger condition: " + " inZone?: " + inZone + " TWIZ: " + TriggerWhenInZone + " pos: " + transform.position);

            if (inZone && TriggerWhenInZone)
            {
                Zone z = ZoneManager.GetZone(ZoneName);
                if (z != null)
                {
                    TargetTask.Target = z.gameObject;
                    TriggerTransition();
                }
            }
            if (!inZone && !TriggerWhenInZone)
            {
                Zone z = ZoneManager.GetZone(ZoneName);
                if (z != null)
                {
                    TargetTask.Target = z.gameObject;
                    TriggerTransition();
                }
            }
            m_nextCheck += CHECK_INTERVAL;
        }
    }
}
