using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCustomControl : MonoBehaviour
{
    protected Vector3 m_targetPoint;
    protected Vector3 m_facePoint;
    protected bool m_toFace = false;
    public virtual InputPacket BaseMovement() { return new InputPacket();}

    public virtual void SetTarget(Vector3 target, float tolerance = 4f)
    {
        m_targetPoint = target;
    }
    public virtual void FacePoint(Vector3 target)
    {
        m_facePoint = target;
        m_toFace = true;
    }
}
