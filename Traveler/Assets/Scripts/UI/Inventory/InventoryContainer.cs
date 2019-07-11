using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InitialItemData
{
    public string itemName;
    public Vector2 inventoryLocation;
}
[System.Serializable]
public class EquipmentSlot
{
    public string SlotName;
    public InventorySlotType SlotType;
    public Vector2 coordinate;
}
public enum InventorySlotType
{
    NORMAL, EQUIPMENT, CLOTHES
}

public class InventoryItemData
{
    public string prefabName;
    [HideInInspector]
    public string itemName;
    [HideInInspector]
    public Vector2 size;
    [HideInInspector]
    public delegate void OnExitReturnFunc(InventoryContainer i, EquipmentSlot es);
    [HideInInspector]
    public OnExitReturnFunc exitFunc;

    public InventoryItemData(Item i) {
        itemName = i.displayname;
        size = i.baseSize;
        prefabName = i.PrefabName;
        exitFunc = i.OnExitInventory;
    }

    /*public Item ToItem()
    {

    }*/
}
public class InventoryContainer : MonoBehaviour
{
    public Vector2 size;
    public List<InitialItemData> initItemData;
    public Dictionary<Vector2, InventoryItemData> items;
    public Dictionary<Vector2, EquipmentSlot> eqpSlotInfo;
    public List<EquipmentSlot> slotData;
    public string InventoryName;
    public InventoryHolder Holder;

    private bool m_displaying;
    private List<Vector2> m_freeSlots;
    private bool m_inventoryInitialized = false;

    internal void Awake()
    {
        if (GetComponent<PersistentItem>() != null)
            GetComponent<PersistentItem>().InitializeSaveLoadFuncs(storeData, loadData);
    }
    // Start is called before the first frame update
    void Start()
    {
        eqpSlotInfo = new Dictionary<Vector2, EquipmentSlot>();
        foreach (EquipmentSlot es in slotData)
        {
            eqpSlotInfo[es.coordinate] = es;
        }
        items = new Dictionary<Vector2, InventoryItemData>();

        m_freeSlots = new List<Vector2>();
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                m_freeSlots.Add(new Vector2(x, y));
            }
        }
        if (!m_inventoryInitialized)
            InitInventory();
    }

    public bool CanFit(Vector2 itemPos, Vector2 itemSize)
    {
        if (!hasSpace(itemPos, itemSize))
            return false;
        foreach (Vector2 coord in items.Keys)
        {
            if (isOverlap(coord,items[coord].size,
                itemPos, itemSize))
                return false;
        }
        return true;
    }
    private bool hasSpace(Vector2 p, Vector2 s)
    {
        return (p.x + s.x <= size.x && p.y + s.y <= size.y);
    }
    private bool isOverlap(Vector2 l1, Vector2 size1,
                          Vector2 l2, Vector2 size2)
    {
        Vector2 r1 = new Vector2(l1.x + size1.x, l1.y + size1.y);
        Vector2 r2 = new Vector2(l2.x + size2.x, l2.y + size2.y);
        if (l1.x > r2.x || l2.x > r1.x)
        {
            return false;
        }
        if (l1.y < r2.y || l2.y < r1.y)
        {
            return false;
        }
        return true;
    }
    public void ToggleDisplay()
    {
        if (m_displaying)
            CloseContainer();
        else
            DisplayContainer();
    }
    public void DisplayContainer()
    {
        m_displaying = true;
        InventoryManager.CreateInventoryGUI(this);
    }

    public void CloseContainer()
    {
        m_displaying = false;
        InventoryManager.CloseGUIPanel(this);
    }

    private void InitializeSlots()
    {

    }
    public void AddItem(Item i , Vector2 pos)
    {
        //Debug.Log("Adding item: " + i + " to inventory: " + pos + " for:" + gameObject);
        if (eqpSlotInfo.ContainsKey(pos))
        {
            i.OnEnterInventory(this, eqpSlotInfo[pos]);
        } else
        {
            i.OnEnterInventory(this, null);
        }
        i.GetComponent<PersistentItem>().StoreData();
        onItemAdded(i, pos);
        if (items.ContainsKey(pos))
            items.Remove(pos);
        items.Add(pos, new InventoryItemData(i));
        m_freeSlots.Remove(pos);
    }
    public void ClearItem(Vector2 v)
    {
        Debug.Log("Attempting to clear item");
        if (!items.ContainsKey(v))
            return;
        if (eqpSlotInfo.ContainsKey(v))
        {
            items[v].exitFunc(this, eqpSlotInfo[v]);
        } else {
            items[v].exitFunc(this, null);
        }
            
        onItemRemoved(items[v], v);
        items.Remove(v);
        m_freeSlots.Add(v);
    }
    public Vector2 findFreeSlot(Item i)
    {
        if (!canAcceptItem(i))
            return new Vector2(-1, -1);
        foreach (Vector2 v in m_freeSlots)
        {
            if (CanFit(v, i.baseSize))
                return v;
        }
        return new Vector2(-1,-1);
    }

    private void InitInventory()
    {
        List<Vector2> vList = new List<Vector2>();
        foreach (Vector2 v in items.Keys){ vList.Add(v); }
        foreach (Vector2 v in vList) { ClearItem(v); }
        items.Clear();
        foreach (InitialItemData iid in initItemData)
        {
            //Debug.Log("Attempting to load: " + iid);
            if ((GameObject)Resources.Load(iid.itemName) == null)
                continue;
            GameObject go = Instantiate((GameObject)Resources.Load(iid.itemName));
            AddItem(go.GetComponent<Item>(), iid.inventoryLocation);
            Destroy(go);
        }
        //Debug.Log("InventoryInitialized");
        m_inventoryInitialized = true;
    }

    public virtual bool canAcceptItem(Item i)
    {
        return true;
    }

    public virtual void onItemAdded(Item i, Vector2 pos)
    {
    }
    public virtual void onItemRemoved(InventoryItemData i, Vector2 pos)
    {
    }

    public virtual void EquipmentUseUpdatePlayer(string slot, Vector2 input)
    {
        Transform t = transform.Find(slot);
        if (t == null)
        {
            return;
        }
        Equipment e = t.gameObject.GetComponent<Equipment>();
        if (input.magnitude > 0.1f)
        {
            e.OnSecondaryUse(input,gameObject);
        }
        else
        {
            e.OnPrimaryUse(input, gameObject);
        }
    }

    private string convertToSaveList(Dictionary<Vector2, InventoryItemData> saveItems)
    {
        string saveList = "";
        foreach (Vector2 v in items.Keys)
        {
            InitialItemData newItem = new InitialItemData();
            //Debug.Log("Item location: : " + v);
            newItem.inventoryLocation = v;
            newItem.itemName = items[v].prefabName;

            saveList += JsonUtility.ToJson(newItem) + "\n";
        }
        return saveList;
    }
    private void storeData(CharData d)
    {
        string s = convertToSaveList(items);
        d.SetString("initItemData", s);
    }

    private void loadData(CharData d)
    {
        string savedItems = d.GetString("initItemData");
        var arr = savedItems.Split('\n');
        initItemData.Clear();
        foreach (string s in arr)
        {
            if (s.Length > 0) {
                InitialItemData newItem = JsonUtility.FromJson<InitialItemData>(s);
                initItemData.Add(newItem);
            }
        }
        InitInventory();
    }
}
