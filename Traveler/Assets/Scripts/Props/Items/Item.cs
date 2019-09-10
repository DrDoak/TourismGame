 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : Interactable
{
    public bool Equipabble;
    public int MaxStack = 1;
    public bool Rotated = false;
    
    public Vector2 baseSize = new Vector2(1,1);
    public string displayname;
    public Sprite InventoryIcon;
    public delegate void LoadFunction(CharData d);
    public LoadFunction m_onSave;


    public CharData ItemProperties;

    [HideInInspector]
    public InventorySlot CurrentSlot;
    protected InventoryContainer m_currentContainer;
    protected Vector2 m_slotPosition;

    [HideInInspector]
    public string prefabName;

    // Start is called before the first frame update
    void Awake()
    {
        ItemProperties = GetComponent<PersistentItem>().data;
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

    public void DestroyItem()
    {
        Debug.Log(m_currentContainer);
        Debug.Log(m_slotPosition);
        m_currentContainer.ClearItem(m_slotPosition);
    }
    public void SetSlotData(InventoryContainer container, Vector2 slotPos)
    {
        Debug.Log("Setting slot to: " + container);
        m_currentContainer = container;
        m_slotPosition = slotPos;
    }
    protected override void onTrigger(GameObject interactor) {
        if (interactor.GetComponent<InventoryHolder>())
        {
            
            bool added = interactor.GetComponent<InventoryHolder>().AddItemIfFree(this);
            if (added)
                Destroy(gameObject);
        }
    }

    public void SaveItems()
    {
        if (ItemProperties == null)
            ItemProperties = new CharData();
        if (m_onSave != null)
            m_onSave(ItemProperties);
        else
            onItemSave(ItemProperties);
    }
    public void LoadItems()
    {
        onItemLoad(ItemProperties);
    }
    public virtual void onItemSave(CharData d) { }

    public virtual void onItemLoad(CharData d) { }
}
