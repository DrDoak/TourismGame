using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUIElement : MonoBehaviour, IDragHandler, IEndDragHandler, 
    IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 mouseoffset = new Vector3();
    private float maxY;
    private Image m_image;
    public Item ItemInfo;
    public bool CanMove = true;

    private Vector3 m_returnPos;
    public void Start()
    {
        UpdateReturnPos(GetComponent<RectTransform>().localPosition);
        m_image = GetComponent<Image>();
        ItemInfo = GetComponent<Item>();
        m_image.color = Color.gray;
    }
    
    public void UpdateReturnPos(Vector3 pos)
    {

        m_returnPos = pos;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        mouseoffset = Input.mousePosition - transform.position;
        maxY = transform.GetComponent<RectTransform>().rect.height / 2f;
        m_image.color = Color.yellow;
        m_image.raycastTarget = false;
        InventoryManager.SetHeldItem(this);
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 newPos = Input.mousePosition - mouseoffset;
        float newX = Mathf.Min(Mathf.Max(0f, newPos.x), Screen.width);
        float newY = Mathf.Min(Mathf.Max(0f, newPos.y), Screen.height - maxY);
        transform.position = new Vector3(newX, newY, 0f);
        m_image.raycastTarget = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_image.color = Color.gray;
        m_image.raycastTarget = true;
        if (InventoryManager.GetCurrentCell() != null && 
            InventoryManager.AttemptMoveItem(this))
        {
            UpdateReturnPos(GetComponent<RectTransform>().localPosition);
        } else
        {
            ReturnPos();
        }
        InventoryManager.SetHeldItem(null);
    }

    public void MatchItemProperties(Item i)
    {

    }

    public void ReturnPos()
    {
        transform.localPosition = m_returnPos;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CanMove)
        {
            m_image.color = Color.yellow;
        }
        else
        {
            m_image.color = Color.red;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        m_image.color = Color.gray;
    }
}
