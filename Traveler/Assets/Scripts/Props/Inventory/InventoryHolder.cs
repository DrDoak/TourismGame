using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHolder : MonoBehaviour
{
    public string InventoryHolderName;

    private Dictionary<string, InventoryContainer> m_containers;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void initializeContainers()
    {
        InventoryContainer[] iList = GetComponents<InventoryContainer>();
        for (int i = 0; i < iList.Length; i++)
        {
            InventoryContainer container = iList[i];
            m_containers.Add(container.InventoryName, container);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleAllInventory()
    {
        foreach (InventoryContainer m_container in m_containers.Values)
        {
            Debug.Log("Opening Inventory");
            m_container.ToggleDisplay();
        }
    }

}
