using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : Item
{
    public GameObject EquipmentPiecePrefab;
    
    public virtual void OnEquip(InventoryContainer i, EquipmentSlot es) { }
    public virtual void OnDeequip(InventoryContainer i, EquipmentSlot es) { }
    public virtual void OnPrimaryUse(Vector2 input, GameObject user) { }
    public virtual void OnSecondaryUse(Vector2 input, GameObject user) { OnPrimaryUse(input, user); }
    public override bool CanEnterInventory(InventoryContainer i, InventorySlot s)
    {
        return (s.SlotType != InventorySlotType.CLOTHES);
    }


    private string m_equipmentPieceType = "";
    private GameObject ItemInstance;

    public override void OnEnterInventory(InventoryContainer s, EquipmentSlot es)
    {
        base.OnEnterInventory(s, es);
        if (es != null && es.SlotType == InventorySlotType.EQUIPMENT)
        {
            ItemInstance = Instantiate((GameObject)Resources.Load(ItemProperties.prefabPath), s.gameObject.transform);
            ItemInstance.GetComponent<Item>().ItemProperties = ItemProperties;
            ItemInstance.GetComponent<Item>().LoadItems();
            m_onSave = ItemInstance.GetComponent<Item>().onItemSave;

            Destroy(ItemInstance.GetComponent<PersistentItem>());
            ItemInstance.name = es.SlotName;

            ItemInstance.GetComponent<Equipment>().AddActionListeners(s.gameObject);
            
            if (ItemInstance.GetComponent<BasicPhysics>())
            {
                ItemInstance.GetComponent<BasicPhysics>().GravityForce = 0f;
                ItemInstance.GetComponent<SpriteRenderer>().enabled = false;
                //Debug.Log("Removing spriterenderer");
            }
            OnEquip(s, es);

            if (EquipmentPiecePrefab != null && s.GetComponent<AnimatorMultiSprite>() != null)
            {
                GameObject go = Instantiate(EquipmentPiecePrefab, s.gameObject.transform.Find("SpritePieces"));
                s.GetComponent<AnimatorMultiSprite>().AddPiece(go);
                m_equipmentPieceType = go.GetComponent<AnimationPiece>().PieceType;
            }
        }
    }

    public override void OnExitInventory(InventoryContainer s , EquipmentSlot es)
    {
        base.OnExitInventory(s, es);
        if (es != null && es.SlotType == InventorySlotType.EQUIPMENT)
        {
            OnDeequip(s, es);
            Destroy(ItemInstance);
            if (EquipmentPiecePrefab != null && s.GetComponent<AnimatorMultiSprite>() != null)
            {
//                Debug.Log("Removing Piece");
                s.GetComponent<AnimatorMultiSprite>().RemovePiece(m_equipmentPieceType);
            }
        }
    }
    public void AddActionListeners(GameObject go)
    {
        if (go.GetComponent<CharacterBase>() != null)
        {
            ActionInfo[] at1 = GetComponents<ActionInfo>();
            foreach (ActionInfo a in at1)
            {
                a.AddListener(go.GetComponent<CharacterBase>().OnActionProgressed);
                a.SetOwner(go);
            }
        }
    }
}
