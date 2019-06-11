using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public Dictionary<string, Zone> m_registeredZones;
    private static InventoryManager m_instance;

    public GameObject InvPrefab;
    public GameObject SlotPrefab;

    private InventorySlot m_highlightedSlot;
    private ItemUIElement m_highlightedItem;
    private ItemUIElement m_currentItem;
    private Dictionary<InventoryContainer,GameObject> m_containerPrefabs;
    private Dictionary<InventoryHolder,int> m_openHolder;

    public static InventoryManager Instance
    {
        get { return m_instance; }
        set { m_instance = value; }
    }

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
            return;
        }

    }
    public void Start()
    {
        m_containerPrefabs = new Dictionary<InventoryContainer, GameObject>();
        m_openHolder = new Dictionary<InventoryHolder, int>();
    }

    public static GameObject CreateInventoryGUI(InventoryContainer ic)
    {
        GameObject go = Instantiate(m_instance.InvPrefab,FindObjectOfType<Canvas>().transform);
        m_instance.m_containerPrefabs.Add(ic, go);
        if (m_instance.m_openHolder.ContainsKey(ic.Holder))
            m_instance.m_openHolder[ic.Holder]++;
        else
            m_instance.m_openHolder[ic.Holder] = 1;
        int invX = 1;
        foreach (InventoryHolder h in m_instance.m_openHolder.Keys)
        {
            if (h == ic.Holder)
                break;
            invX++;
        }
        int invY = m_instance.m_openHolder[ic.Holder];
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300f + (300f * (invX - 1)), 140f - (200f * (invY - 1)));
        go.GetComponent<RectTransform>().sizeDelta = new Vector2(150f + ((ic.size.x - 1 )* 50f), 150f + ((ic.size.y - 1) * 50f));

        go.transform.Find("DragBar").Find("OwnerName").GetComponent<TextMeshProUGUI>().text = ic.Holder.name;
        go.transform.Find("DragBar").Find("OwnerName").GetComponent<TextMeshProUGUI>().text = ic.name;
        Transform slotParent = go.transform.Find("Inv").Find("Slots");
        Dictionary<Vector2, InventorySlot> slots = new Dictionary<Vector2, InventorySlot>();
        for (int i = 0; i < ic.size.x * ic.size.y; i++)
        {
            GameObject newSlot = Instantiate(m_instance.SlotPrefab, slotParent);
            newSlot.GetComponent<InventorySlot>().m_container = ic;
           
            newSlot.GetComponent<InventorySlot>().m_container = ic;
            Vector2 coordinates = new Vector2(i % ic.size.x,Mathf.FloorToInt(i / ic.size.x));
            if (ic.eqpSlotInfo.ContainsKey(coordinates))
            {
                newSlot.GetComponent<InventorySlot>().SlotType = ic.eqpSlotInfo[coordinates].SlotType;
                newSlot.GetComponent<InventorySlot>().SetSlotName(ic.eqpSlotInfo[coordinates].SlotName);
            } else
            {
                newSlot.GetComponent<InventorySlot>().SlotType = InventorySlotType.NORMAL;
            }
            newSlot.GetComponent<InventorySlot>().ItemOffsetPos = new Vector2((coordinates.x) * 50f, (-coordinates.y) * 50f);
            newSlot.GetComponent<InventorySlot>().Coordinate = coordinates;
            slots.Add(coordinates, newSlot.GetComponent<InventorySlot>());
        }
        Transform itemParent = go.transform.Find("Inv").Find("Items");
        foreach (Vector2 v in ic.items.Keys)
        {
            m_instance.addItemIcon(ic.items[v],v,slots,itemParent,ic);
        }
        return go;
    }
    public static void CloseGUIPanel(InventoryContainer ic)
    {
        if (m_instance.m_containerPrefabs.ContainsKey(ic))
        {
            Destroy(m_instance.m_containerPrefabs[ic]);
            m_instance.m_containerPrefabs.Remove(ic);

            m_instance.m_openHolder[ic.Holder]--;
            if (m_instance.m_openHolder[ic.Holder] == 0)
                m_instance.m_openHolder.Remove(ic.Holder);
        }
    }

    public static bool AttemptMoveItem(ItemUIElement iue)
    {
        if (m_instance.m_highlightedSlot.CanFitItem(iue.ItemInfo))
        {
            if (!iue.ItemInfo.CanEnterInventory(iue.ItemInfo.CurrentSlot.m_container, m_instance.m_highlightedSlot))
                return false;
            iue.UpdateReturnPos(m_instance.m_highlightedSlot.ItemOffsetPos);
            iue.transform.SetParent( m_instance.m_highlightedSlot.transform.parent.parent.Find("Items") );
            iue.ReturnPos();
            iue.ItemInfo.CurrentSlot.m_container.ClearItem(iue.ItemInfo.CurrentSlot.Coordinate);
            m_instance.m_highlightedSlot.AddItem(iue.ItemInfo, iue);
            iue.ItemInfo.CurrentSlot = m_instance.m_highlightedSlot;
            return true;
        }
        iue.ReturnPos();
        return false;
    }
    public static bool AttemptSwap(ItemUIElement item1, ItemUIElement item2)
    {
        InventorySlot slot1 = item1.ItemInfo.CurrentSlot;
        InventorySlot slot2 = item2.ItemInfo.CurrentSlot;
        if (slot1.CanFitItem(item2.ItemInfo) && slot2.CanFitItem(item1.ItemInfo) &&
            item1.ItemInfo.CanEnterInventory(slot2.m_container,slot2) &&
            item2.ItemInfo.CanEnterInventory(slot1.m_container,slot1))
        {
            Debug.Log("Moving item 1");
            m_instance.MoveItemTo(item1, slot2);
            Debug.Log("Moving item 2");
            m_instance.MoveItemTo(item2, slot1);
            return true;
        }
        item1.ReturnPos();
        item2.ReturnPos();
        return false;
    }
    private void MoveItemTo(ItemUIElement iue, InventorySlot slot)
    {
        iue.UpdateReturnPos(slot.ItemOffsetPos);
        iue.transform.SetParent( slot.transform.parent.parent.Find("Items") );
        iue.ReturnPos();
        iue.ItemInfo.CurrentSlot.m_container.ClearItem(iue.ItemInfo.CurrentSlot.Coordinate);
        slot.AddItem(iue.ItemInfo,iue);
        iue.ItemInfo.CurrentSlot = slot;
    }
    private void addItemIcon(InventoryItemData i, Vector2 loc, Dictionary<Vector2,InventorySlot> slots, Transform parent,InventoryContainer c)
    {
        if ((GameObject)Resources.Load(i.prefabName) == null)
            return;
        GameObject go = Instantiate((GameObject)Resources.Load(i.prefabName), parent);
        
        go.transform.localPosition = new Vector3(50 + (loc.y - 1) * 50f,
                (-loc.x) * 50f, 3f);
        Debug.Log("instantiated at: " + go.transform.localPosition);
        go.GetComponent<Item>().CurrentSlot = slots[loc];
    }

    public static List<Vector2> GetOccupiedSlots(Item i)
    {
        return new List<Vector2>();
    }
    private Vector2 GetDrawLocation()
    {
        return new Vector2();
    }
    public static void SetHighlightedCell(InventorySlot slot)
    {
        m_instance.m_highlightedSlot = slot;
    }
    public static void SetHighlightedItem(ItemUIElement item)
    {
        m_instance.m_highlightedItem = item;
    }
    public static void ClearHighlightedItem(ItemUIElement item)
    {
        if (m_instance.m_highlightedItem == item)
            m_instance.m_highlightedItem = null;
    }
    public static void SetHeldItem(ItemUIElement item)
    {
        m_instance.m_currentItem = item;
    }
    public static void ClearHighlightedCell(InventorySlot slot)
    {
        if (m_instance.m_highlightedSlot == slot)
            m_instance.m_highlightedSlot = null;
    }
    public static InventorySlot GetCurrentCell()
    {
        return m_instance.m_highlightedSlot;
    }

    public static ItemUIElement GetCurrentItem()
    {
        return m_instance.m_currentItem;
    }
    public static ItemUIElement GetHighlightedItem()
    {
        return m_instance.m_highlightedItem;
    }
}
