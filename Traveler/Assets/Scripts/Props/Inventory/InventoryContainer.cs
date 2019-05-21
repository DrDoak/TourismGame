using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItemData
{
    public string itemName;
    public Vector2 inventoryLocation;
}
public class InventoryContainer : MonoBehaviour
{
    public Vector2 size;
    public List<InventoryItemData> initItemData;
    public Dictionary<Vector2, Item> items;
    public string InventoryName;
    public InventoryHolder Holder;

    private bool m_displaying;

    // Start is called before the first frame update
    void Start()
    {
        items = new Dictionary<Vector2, Item>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool CanFit(Vector2 itemPos, Vector2 itemSize)
    {
        foreach (Vector2 coord in items.Keys)
        {
            if (isOverlap(coord,items[coord].baseSize,
                itemPos, itemSize))
                return false;
        }
        return true;
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
        Debug.Log("Adding item: " + i + " to inventory: " + pos);
        i.OnEnterInventory(this);
        items[pos] = i;
        InventoryItemData newItem = new InventoryItemData();
        newItem.itemName = i.UIPrefabName;
        newItem.inventoryLocation = pos;
        initItemData.Add(newItem);
    }
    public void ClearItem(Vector2 v)
    {
        Debug.Log("Attempting to clear inventory" +  v);
        items[v].OnExitInventory(this);
        items.Remove(v);
        InventoryItemData toRemove = null;
        foreach (InventoryItemData id in initItemData) {
            if (id.inventoryLocation == v)
            {
                toRemove = id;
                break;
            }
        }
        if (toRemove != null)
            initItemData.Remove(toRemove);
    }

}
