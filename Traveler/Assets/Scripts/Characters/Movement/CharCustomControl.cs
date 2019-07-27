using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCustomControl : MonoBehaviour
{
    protected Vector3 m_targetPoint;
    public virtual InputPacket BaseMovement() { return new InputPacket();}

    public virtual void SetTarget(Vector3 target, float tolerance = 4f)
    {
        m_targetPoint = target;
    }
}
