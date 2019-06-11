using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EqpDebug : Equipment
{
    public ActionInfo attackOne;
    public override void OnPrimaryUse(Vector2 input,GameObject user) {
        user.GetComponent<CharacterBase>().TryAction(attackOne);  
    }
}
