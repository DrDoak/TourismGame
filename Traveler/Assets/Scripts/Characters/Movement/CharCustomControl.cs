using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCustomControl : MonoBehaviour
{
    private Vector3 targetPoint;
    public virtual InputPacket BaseMovement() { return new InputPacket();}

    public virtual void SetTarget(Vector3 v)
    {
        targetPoint = v;
    }
}
