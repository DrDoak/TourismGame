using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : CharCustomControl
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override Vector3 BaseMovement()
    {
        Vector3 v = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        return v.normalized;
    }
}
