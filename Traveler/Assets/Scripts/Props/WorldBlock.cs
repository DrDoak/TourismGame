using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WorldBlock : MonoBehaviour
{
    public GameObject PanelTop;
    public GameObject PanelFront;
    public GameObject PanelLeft;
    public GameObject PanelRight;

    BoxCollider m_box;

    // Start is called before the first frame update
    void Start()
    {
        m_box = GetComponent<BoxCollider>();
        GetGridEdges();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGridBounds();
    }

    private void GetGridEdges()
    {

    }
    private void UpdateGridBounds()
    {
        if (PanelTop != null)
        {

            PanelTop.transform.localPosition = new Vector3(0.5f, m_box.center.y + m_box.size.y / 2f,0.5f);
        }
        if (PanelFront != null)
        {
            PanelFront.transform.localPosition = new Vector3(0.5f, 0.5f, -m_box.center.z - m_box.size.z / 2f);
        }
        if (PanelLeft != null)
        {
            PanelLeft.transform.localPosition = new Vector3(-m_box.center.x - m_box.size.x / 2f, 0.5f, 0.5f);
            PanelLeft.transform.localScale = m_box.size;
        }
        if (PanelRight != null)
        {
            PanelRight.transform.localPosition = new Vector3(-m_box.center.x - m_box.size.x / 2f, 0.5f, 0.5f);
            PanelRight.transform.localScale = m_box.size;
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 8f);

        Gizmos.DrawCube(transform.position + m_box.center, m_box.size);
    }
}
