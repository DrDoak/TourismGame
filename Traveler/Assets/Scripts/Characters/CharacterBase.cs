using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    public float GravityScale = 1.0f;
    public float TimeScale = 1.0f;
    public float RunSpeed = 1.0f;
    public bool CanControl = true;
    CharCustomControl m_customControl;
    CharacterController m_controller;

    // Start is called before the first frame update
    void Start()
    {
        m_controller = GetComponent<CharacterController>();
        m_customControl = GetComponent<CharCustomControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CanControl)
        {
            Vector3 movement = new Vector3();
            movement += m_customControl.BaseMovement() * RunSpeed * Time.deltaTime;
            movement.y += -GravityScale * Time.deltaTime;
            m_controller.Move(movement);
        }

    }
}
