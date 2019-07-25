using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemUseMode {PRIMARY,SECONDARY}
public class TKUseItem : Task
{
    public string UseItemSlot = "Item1";
    public ItemUseMode UseSecondary;

    public override void OnActiveUpdate()
    {
        Vector2 input = (UseSecondary == ItemUseMode.PRIMARY) ? (new Vector2()) : (new Vector2(1, 1));
        MasterAI.GetComponent<MovementBase>().UseItem(UseItemSlot, input);
    }

    public override void OnLoad(Goal g)
    {
        if (g.ContainsKey("ItemSlot", this))
            UseItemSlot = g.GetVariable("ItemSlot", this);
        if (g.ContainsKey("UseSecondary", this))
            UseSecondary = (ItemUseMode)int.Parse(g.GetVariable("UseSecondary", this));
    }

    public override void OnSave(Goal g)
    {
        g.SetVariable("ItemSlot", UseItemSlot, this);
        g.SetVariable("UseSecondary", ((int)UseSecondary).ToString(), this);
    }
}