using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalButtonsController : MonoBehaviour
{
    public bool CanPause = true;
    public bool DebugSave = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CanPause && Input.GetButton("Pause"))
        {

        }
        if (DebugSave)
        {
            if (Input.GetButton("QuickSave"))
            {

            } else if (Input.GetButton("QuickLoad"))
            {

            }
        }
    }
}
