using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIDragBar : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    , IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 mouseoffset = new Vector3();
    private float maxY;
    private Image m_barBackground;

    void Start()
    {
        m_barBackground = GetComponent<Image>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        mouseoffset = Input.mousePosition - transform.parent.position;
        maxY = transform.parent.GetComponent<RectTransform>().rect.height/2f;
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 newPos = Input.mousePosition - mouseoffset;
        float newX = Mathf.Min(Mathf.Max(0f, newPos.x), Screen.width);
        float newY = Mathf.Min(Mathf.Max(0f, newPos.y), Screen.height - maxY);
        transform.parent.position = new Vector3(newX,newY,0f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        m_barBackground.color = new Color(0.8f, 0.8f, 1.0f); 
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        m_barBackground.color = new Color(0.8f, 0.8f, 0.8f);
    }
}
