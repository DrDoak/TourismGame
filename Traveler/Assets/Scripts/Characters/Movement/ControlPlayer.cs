using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPlayer : CharCustomControl
{
    public override InputPacket BaseMovement()
    {
        InputPacket newPacket = new InputPacket();
        Vector3 m_inputMove = new Vector3(0f, 0f);
        if (Input.GetButton("Left"))
            m_inputMove.x -= 1f;
        if (Input.GetButton("Right"))
            m_inputMove.x += 1f;
        if (Input.GetButton("Up"))
            m_inputMove.z += 1f;
        if (Input.GetButton("Down"))
        {
            m_inputMove.z -= 1f;
        }
        newPacket.InputMove = m_inputMove;
        newPacket.JumpDown = Input.GetButtonDown("Jump");
        newPacket.JumpHold = Input.GetButton("Jump");
        return newPacket;
    }
}
