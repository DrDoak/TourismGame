using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EqpDebug : Equipment
{
    public ActionInfo PrimaryAction;
    public ActionInfo SecondaryAction;

    private int timesUsed = 0;

    public override void OnPrimaryUse(Vector2 input,GameObject user) {
        if (PrimaryAction != null)
        {
            user.GetComponent<CharacterBase>().TryAction(PrimaryAction);
            timesUsed++;
        }
    }
    public override void OnSecondaryUse(Vector2 input, GameObject user)
    {
        if (SecondaryAction != null)
            user.GetComponent<CharacterBase>().TryAction(SecondaryAction);
        else
            OnPrimaryUse(input, user);
    }
    public override void onItemSave(CharData d)
    {
        d.SetInt("TimesUsed", timesUsed);
    }

    public override void onItemLoad(CharData d)
    {
        timesUsed = d.GetInt("TimesUsed");
    }
}
