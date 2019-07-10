﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EqpDebug : Equipment
{
    public ActionInfo PrimaryAction;
    public ActionInfo SecondaryAction;
    public override void OnPrimaryUse(Vector2 input,GameObject user) {
        if (PrimaryAction != null)
            user.GetComponent<CharacterBase>().TryAction(PrimaryAction);  
    }
    public override void OnSecondaryUse(Vector2 input, GameObject user)
    {
        if (SecondaryAction != null)
            user.GetComponent<CharacterBase>().TryAction(SecondaryAction);
        else
            OnPrimaryUse(input, user);
    }
}
