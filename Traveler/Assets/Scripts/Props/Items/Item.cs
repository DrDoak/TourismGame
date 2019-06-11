using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    public bool Equipabble;
    public int MaxStack = 1;
    public bool Rotated = false;
    public InventorySlot CurrentSlot;
    public Vector2 baseSize = new Vector2(1,1);
    public string UIPrefabName;
    public string displayname;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnEnterInventory(InventoryContainer s, EquipmentSlot es) { }

    public virtual void OnExitInventory(InventoryContainer s, EquipmentSlot es) { }

    public virtual bool CanEnterInventory(InventoryContainer i, InventorySlot s) {
        return (s.SlotType == InventorySlotType.NORMAL);
    }

    protected override void onTrigger(GameObject interactor) {
        Debug.Log("Item interacted by: " + interactor);
        if (interactor.GetComponent<InventoryHolder>())
        {
            bool added = interactor.GetComponent<InventoryHolder>().AddItemIfFree(this);
            Debug.Log("add attempt result: " + added);
            if (added)
                Destroy(gameObject);
        }
    }
}
