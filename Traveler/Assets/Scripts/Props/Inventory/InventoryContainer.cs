using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InitialItemData
{
    public string itemName;
    public Vector2 inventoryLocation;
}

public class InventoryItemData
{
    public string prefabName;
    [HideInInspector]
    public string itemName;
    [HideInInspector]
    public Vector2 size;
    [HideInInspector]
    public delegate void OnExitReturnFunc(InventoryContainer i);
    [HideInInspector]
    public OnExitReturnFunc exitFunc;

    public InventoryItemData(Item i) {
        itemName = i.displayname;
        size = i.baseSize;
        Debug.Log("PRefab name set to: " + i.UIPrefabName);
        prefabName = i.UIPrefabName;
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
    public string InventoryName;
    public InventoryHolder Holder;

    private bool m_displaying;
    private List<Vector2> m_freeSlots;

    // Start is called before the first frame update
    void Start()
    {
        items = new Dictionary<Vector2, InventoryItemData>();
        m_freeSlots = new List<Vector2>();
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                m_freeSlots.Add(new Vector2(x, y));
            }
        }
        InitInventory();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        Debug.Log("Adding item: " + i + " to inventory: " + pos + " for:" + gameObject);
        i.OnEnterInventory(this);
        onItemAdded(i, pos);
        if (items.ContainsKey(pos))
            items.Remove(pos);
        items.Add(pos, new InventoryItemData(i));
        m_freeSlots.Remove(pos);
    }
    public void ClearItem(Vector2 v)
    {
        items[v].exitFunc(this);
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
        items.Clear();
        foreach (InitialItemData iid in initItemData)
        {
            if ((GameObject)Resources.Load(iid.itemName) == null)
                continue;
            GameObject go = Instantiate((GameObject)Resources.Load(iid.itemName));
            AddItem(go.GetComponent<Item>(), iid.inventoryLocation);
            Destroy(go);
        }
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
}
