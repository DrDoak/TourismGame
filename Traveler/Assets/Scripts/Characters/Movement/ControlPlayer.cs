using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPlayer : CharCustomControl
{
    private float lastLeft = 0.0f;
    private float lastRight = 0.0f;
    private bool lastLeftHeld = false;
    private bool lastRightHeld = false;

    private const float SPRINT_INPUT_INTERVAL = 0.25f;

    public override InputPacket BaseMovement()
    {
        InputPacket newPacket = new InputPacket();
        Vector3 m_inputMove = new Vector3(0f, 0f);

        if (Input.GetButton("Up"))
            m_inputMove.z += 1f;
        if (Input.GetButton("Down"))
            m_inputMove.z -= 1f;
        if (Input.GetButton("Left"))
        {
            m_inputMove.x -= 1f;
            if (GetComponent<MovementBase>().IsSprinting)
                newPacket.Sprint = true;
            if (!lastLeftHeld && Time.timeSinceLevelLoad - lastLeft < SPRINT_INPUT_INTERVAL)
                newPacket.Sprint = true;
            lastLeftHeld = true;
            lastLeft = Time.timeSinceLevelLoad;
        } else
        {
            lastLeftHeld = false;
        }
            
        if (Input.GetButton("Right"))
        {
            m_inputMove.x += 1f;
            if (GetComponent<MovementBase>().IsSprinting)
                newPacket.Sprint = true;
            if (!lastRightHeld && Time.timeSinceLevelLoad - lastRight < SPRINT_INPUT_INTERVAL)
                newPacket.Sprint = true;
            lastRightHeld = true;
            lastRight = Time.timeSinceLevelLoad;
        }
        else
        {
            lastRightHeld = false;
        }


        if (Input.GetButtonDown("Interact"))
        {
            newPacket.Interact = true;
        }
        newPacket.equipmentSlotUsed = "None";
        if (Input.GetButtonDown("Item1"))
        {
            newPacket.equipmentSlotUsed = "Item1";
        }
        if (Input.GetButtonDown("Item2"))
        {
            newPacket.equipmentSlotUsed = "Item2";
        }
        if (Input.GetButtonDown("Item3"))
        {
            newPacket.equipmentSlotUsed = "Item3";
        }
        if (Input.GetButtonDown("Item4"))
        {
            newPacket.equipmentSlotUsed = "Item4";
        }
        if (Input.GetButtonDown("Inventory"))
        {
            newPacket.OpenInventory = true;
        }
        if (Input.GetButton("Sprint") && 
            (Input.GetButton("Right") || Input.GetButton("Left")))
        {
            newPacket.Sprint = true;
        }
        newPacket.InputMove = m_inputMove;
        newPacket.JumpDown = Input.GetButtonDown("Jump");
        newPacket.JumpHold = Input.GetButton("Jump");
        return newPacket;
    }
}
