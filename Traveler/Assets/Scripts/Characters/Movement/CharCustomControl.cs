using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCustomControl : MonoBehaviour
{
    public virtual InputPacket BaseMovement() { return new InputPacket();}
}
