using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentDegradationType
{
    ONUSECONCLUDE,  ONHIT, ONHITWITHCOOLDOWN, INFINITEDURABILITY
}
public class Equipment : Item
{
    public GameObject EquipmentPiecePrefab;
    private string m_equipmentPieceType = "";
    private GameObject ItemInstance;

    public int MaxUses = 3;
    public EquipmentDegradationType EqpDegradationType;
    private int timesUsed = 0;
    private float m_timeOfLastDegrade = 0;
    private const float DEGRADE_COOLDOWN = 0.25f;


    public virtual void OnEquip(InventoryContainer i, EquipmentSlot es) { }
    public virtual void OnDeequip(InventoryContainer i, EquipmentSlot es) { }
    public virtual void OnPrimaryUse(Vector2 input, GameObject user) { }
    public virtual void OnSecondaryUse(Vector2 input, GameObject user) { OnPrimaryUse(input, user); }

    public virtual void OnRegisterHit(GameObject attackable, HitInfo hitinfo, HitResult hitResult) {
        if (hitResult != HitResult.NONE)
        {
            if (EqpDegradationType == EquipmentDegradationType.ONHIT)
            {
                degradeItem();
            } else if (EqpDegradationType == EquipmentDegradationType.ONHITWITHCOOLDOWN && Time.timeSinceLevelLoad - m_timeOfLastDegrade > DEGRADE_COOLDOWN)
            {
                
                degradeItem();
            }
        }
    }
    public override bool CanEnterInventory(InventoryContainer i, InventorySlot s)
    {
        return (s.SlotType != InventorySlotType.CLOTHES);
    }




    public override void OnEnterInventory(InventoryContainer s, EquipmentSlot es)
    {
        base.OnEnterInventory(s, es);
        if (es != null && es.SlotType == InventorySlotType.EQUIPMENT)
        {
            ItemInstance = Instantiate((GameObject)Resources.Load(ItemProperties.prefabPath),s.transform.position,Quaternion.identity);
            ItemInstance.transform.SetParent(s.gameObject.transform);
            
            ItemInstance.GetComponent<Item>().ItemProperties = ItemProperties;
            ItemInstance.GetComponent<Item>().LoadItems();
            Debug.Log("Assigning current slot to: " + es.coordinate);
            ItemInstance.GetComponent<Item>().SetSlotData(s,es.coordinate) ;
            ItemInstance.GetComponent<BasicPhysics>().enabled = false;
            ItemInstance.GetComponent<CharacterController>().enabled = false;
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

            OverrideCurrentEquipSprite(s.gameObject);
        }
    }

    protected void OverrideCurrentEquipSprite(GameObject user)
    {
        if (EquipmentPiecePrefab != null && user.GetComponent<AnimatorMultiSprite>() != null)
        {
            GameObject go = Instantiate(EquipmentPiecePrefab, user.transform.Find("SpritePieces"));
            go.GetComponent<AnimationPiece>().PieceType = "Equipment";
            user.GetComponent<AnimatorMultiSprite>().RemovePiece("Equipment");
            user.GetComponent<AnimatorMultiSprite>().AddPiece(go);
            m_equipmentPieceType = go.GetComponent<AnimationPiece>().PieceType;
        }
    }

    public override void OnExitInventory(InventoryContainer s , EquipmentSlot es)
    {
        base.OnExitInventory(s, es);
        Debug.Log(m_currentContainer);
        if (m_currentContainer.GetComponent<CharacterBase>() != null)
            m_currentContainer.GetComponent<CharacterBase>().SkipActionToEnd();

        if (es != null && es.SlotType == InventorySlotType.EQUIPMENT)
        {
            OnDeequip(s, es);
            Destroy(ItemInstance);
            if (EquipmentPiecePrefab != null && s.GetComponent<AnimatorMultiSprite>() != null)
            {
                Debug.Log("Removing Piece " + displayname);
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
    public void OnConclude()
    {
        Debug.Log("On Conclude");
        if (EqpDegradationType == EquipmentDegradationType.ONUSECONCLUDE)
        {
            degradeItem();
        }
    }
    private void degradeItem()
    {
        timesUsed++;
        m_timeOfLastDegrade = Time.timeSinceLevelLoad;
        if (timesUsed >= MaxUses)
        {
            DestroyItem();
        }
    }
    public override void onItemSave(CharData d)
    {
        d.SetInt("TimesUsed", timesUsed);
        d.SetInt("MaxUses", MaxUses);
    }

    public override void onItemLoad(CharData d)
    {
        timesUsed = d.GetInt("TimesUsed");
        //MaxUses = d.GetInt("MaxUses");
    }
}
