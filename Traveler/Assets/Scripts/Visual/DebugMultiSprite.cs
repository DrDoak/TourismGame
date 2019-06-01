using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMultiSprite : AnimatorMultiSprite
{
    public string PlaySprite = "idle";
    public bool ResetToIdle = false;
    // Start is called before the first frame update
    void Awake()
    {
        base.Awake();
        init();
    }

    private void Update()
    {
        if (ResetToIdle)
        {
            Play("idle");
        } else
        {
            Play(PlaySprite);
        }
        
    }
   
}
