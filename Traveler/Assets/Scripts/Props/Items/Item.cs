using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool Equipabble;
    public int MaxStack = 1;
    public bool Rotated = false;
    public InventorySlot CurrentSlot;
    public Vector2 baseSize;
    public string UIPrefabName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnEquip() { }

    public virtual void OnEnterInventory(InventoryContainer i) { }

    public virtual void OnExitInventory(InventoryContainer i) { }

    public virtual bool CanEnterInventory(InventoryContainer i) { return true; }
}
