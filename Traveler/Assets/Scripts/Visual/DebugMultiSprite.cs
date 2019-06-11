using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMultiSprite : AnimatorMultiSprite
{
    public string PlaySprite = "idle";
    public bool ResetToIdle = false;
    public bool PeriodicLoop = false;

    private float nextLoop = 0.0f;
    private const float LOOP_INTERVAL = 4.0f;
    // Start is called before the first frame update
    void Awake()
    {
        base.Awake();
        init();
    }

    private void Update()
    {
        if (PeriodicLoop)
        {
            if (Time.timeSinceLevelLoad > nextLoop )
            {
                Play("idle");
                nextLoop = Time.timeSinceLevelLoad + LOOP_INTERVAL;
            } else
            {
                Play(PlaySprite);
            }
        }
        if (ResetToIdle)
        {
            Play("idle");
        } else
        {
            Play(PlaySprite);
        }
        
    }
}
