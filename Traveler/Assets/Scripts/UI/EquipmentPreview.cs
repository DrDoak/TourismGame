using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPreview : MonoBehaviour
{
    public Image EquipmentImage;
    public InventoryContainer TargetContainer;
    public Vector2 SlotCoordinate;
    public Sprite EmptySprite;

    private InventoryItemData m_lastItem;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TargetContainer != null)
        {
            InventoryItemData newItem = TargetContainer.GetItem(SlotCoordinate);
            if (newItem != m_lastItem)
            {
                m_lastItem = newItem;
                if (m_lastItem == null)
                {
                    EquipmentImage.sprite = EmptySprite;
                } else
                {
                    EquipmentImage.sprite = m_lastItem.InvIcon;
                }
            }
        } else
        {
            EquipmentImage.sprite = EmptySprite;
        }
    }
}
