using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler , IPointerExitHandler
{
    private Image m_slotBackground;
    private bool Occupied = false;

    public InventoryContainer m_container;
    public Item OccupyingItem;
    public Vector2 Coordinate;
    public Vector3 ItemOffsetPos;
    
    // Start is called before the first frame update
    void Start()
    {
        m_slotBackground = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InventoryManager.GetCurrentItem() == null)
        {
            m_slotBackground.color = new Color(0.5f, 0.5f, 0.5f);
        } else
        {
            ItemUIElement iui = InventoryManager.GetCurrentItem();
            if (CanFitItem(iui.ItemInfo))
            {
                m_slotBackground.color = new Color(0.5f, 1.0f, 0.5f);
            } else
            {
                m_slotBackground.color = new Color(1.0f, 0.5f, 0.5f);
            }
        }
        InventoryManager.SetHighlightedCell(this);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (Occupied)
        {
            m_slotBackground.color = new Color(0.7f, 0.7f, 0.7f);
        } else
        {
            m_slotBackground.color = new Color(1.0f, 1.0f, 1.0f);
        }
        InventoryManager.ClearHighlightedCell(this);
    }

    public bool CanFitItem (Item i)
    {
        return m_container.CanFit(Coordinate,i.baseSize);
    }
    public void AddItem(Item i)
    {
        m_container.AddItem(i, Coordinate);
    }
}
