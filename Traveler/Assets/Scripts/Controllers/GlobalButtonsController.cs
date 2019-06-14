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
        if (CanPause && Input.GetButtonDown("Pause"))
        {

        }
        if (DebugSave)
        {
            if (Input.GetButtonDown("QuickSave"))
            {
                SaveObjManager.Instance.SaveProfile("QuickSave");
                TextboxManager.StartSequence("~QuickSave Successful");
            } else if (Input.GetButtonDown("QuickLoad")) { 
                bool result = SaveObjManager.Instance.LoadProfile("QuickSave");
                if (result == false)
                {
                    SaveObjManager.Instance.LoadProfile("AutoSave");
                }
        }
        }
    }
}
