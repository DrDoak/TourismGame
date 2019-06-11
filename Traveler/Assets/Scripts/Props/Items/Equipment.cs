using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : Item
{
    public GameObject EquipmentPiecePrefab;
    public GameObject ItemInstance;

    public virtual void OnEquip(InventoryContainer i, EquipmentSlot es) { }
    public virtual void OnDeequip(InventoryContainer i, EquipmentSlot es) { }
    public override bool CanEnterInventory(InventoryContainer i, InventorySlot s)
    {
        return (s.SlotType != InventorySlotType.CLOTHES);
    }
    public virtual void OnPrimaryUse(Vector2 input,GameObject user) { }
    public virtual void OnSecondaryUse(Vector2 input, GameObject user) { OnPrimaryUse(input,user); }

    private string m_equipmentPieceType = "";

    public override void OnEnterInventory(InventoryContainer s, EquipmentSlot es)
    {
        base.OnEnterInventory(s, es);
        if (es != null && es.SlotType == InventorySlotType.EQUIPMENT)
        {
            ItemInstance = Instantiate((GameObject)Resources.Load(UIPrefabName), s.gameObject.transform);
            
            ItemInstance.name = es.SlotName;

            ItemInstance.GetComponent<Equipment>().AddActionListeners(s.gameObject);
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
                Debug.Log("Removing Piece");
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
            }
        }
    }
    /*
internal void Start()
	{
		AttackInfo[] at1 = GetComponentsInChildren<AttackInfo> ();
		AttackInfo[] at2 =  GetComponents<AttackInfo>();
		var at = new AttackInfo[at1.Length + at2.Length];
		at1.CopyTo(at, 0);
		at2.CopyTo(at, at1.Length); 
		foreach (AttackInfo a in at)
		{
			if (!Attacks.ContainsKey(a.AttackName))
			{
				Attacks.Add(a.AttackName, a);
				a.AddListener(OnAttackProgressed);
			}
		}
		string[] defaultAttacks = new string[]{ "neutral", "up", "down", "side", "air", "air_up", "air_down", "air_side",
			"sneutral", "sup", "sdown", "sside", "sair", "sair_up", "sair_down", "sair_side" };
		foreach (string s in defaultAttacks) {
			Aliases ["i_" + s] = s;
		}
	}*/
}
