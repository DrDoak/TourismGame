using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHolder : Interactable
{
    private const float MAX_INSPECT_DISTANCE = 3.0f;
    public string InventoryHolderName;

    private Dictionary<string, InventoryContainer> m_containers;
    private InventoryHolder m_currentInspector = null;

    // Start is called before the first frame update
    void Start()
    {
        m_containers = new Dictionary<string, InventoryContainer>();
        initializeContainers();
    }

    private void Update()
    {
        if (m_currentInspector != null)
        {
            if (Vector3.Distance(transform.position, m_currentInspector.transform.position) > MAX_INSPECT_DISTANCE)
                SetInventoryGUIOn(false);
        }
    }
    private void initializeContainers()
    {
        InventoryContainer[] iList = GetComponents<InventoryContainer>();
        for (int i = 0; i < iList.Length; i++)
        {
            InventoryContainer container = iList[i];
            container.Holder = this;
            m_containers.Add(container.InventoryName, container);
        }
    }

    public void ToggleAllInventory()
    {
        foreach (InventoryContainer m_container in m_containers.Values)
        {
            m_container.ToggleDisplay();
        }
    }

    public void SetInventoryGUIOn(bool on)
    {
        if (!on)
        {
            m_currentInspector = null;
        }
        foreach (InventoryContainer m_container in m_containers.Values)
        {
            if (on)
            {
                m_container.DisplayContainer();
            } else
            {
                m_container.CloseContainer();
            }
        }
    }
    public bool AddItemIfFree(Item i)
    {
        Vector2 badV = new Vector2(-1, -1);
        foreach (InventoryContainer m_container in m_containers.Values)
        {
            Vector2 v = m_container.findFreeSlot(i);
            if (v != badV)
            {
                m_container.AddItem(i, v);
                return true;
            }
        }
        return false;
    }
    public override bool IsInteractable(GameObject interactor)
    {
        return (m_currentInspector != null);
    }
    protected override void onTrigger(GameObject interactor)
    {
        if (interactor.GetComponent<InventoryHolder>())
        {
            SetInventoryGUIOn(true);
            m_currentInspector = interactor.GetComponent<InventoryHolder>();
        }
    }
}
