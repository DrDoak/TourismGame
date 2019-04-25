using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool Equipabble;
    public int MaxStack = 1;
    public Sprite InventoryIcon;
    public Vector2 baseSize;
    public bool Rotated = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnEquip() { }

    public virtual void OnEnterInventory(Inventory i) { }

    public virtual void OnExitInventory(Inventory i) { }

    public virtual bool CanEnterInventory(Inventory i) { return true; }
}
