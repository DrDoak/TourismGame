using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoxSpriteObj : MonoBehaviour
{
    public Sprite SpriteTop;
    public Sprite SpriteFront;
    public Sprite SpriteLeft;
    public Sprite SpriteRight;
    private GameObject PanelTop;
    private GameObject PanelFront;
    private GameObject PanelLeft;
    private GameObject PanelRight;
    private GameObject NavCube;
    public GameObject SpritePanel;

    BoxCollider m_box;
    public bool DrawBox = false;
    public bool initialized = false;

    private Sprite m_old_top;
    private Sprite m_old_front;
    private Sprite m_old_left;
    private Sprite m_old_right;
    private Vector3 m_last_scale;
    // Start is called before the first frame update
    void Start()
    {
        m_box = GetComponent<BoxCollider>();
        DisableSprites();
        InitiateSprites();
    }

    private void Awake()
    {
        if (NavCube != null)
            NavCube.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (m_old_top != SpriteTop ||
             m_old_front != SpriteFront ||
             m_old_left != SpriteLeft ||
             m_old_right != SpriteRight ||
             m_last_scale != transform.localScale)
        {
            m_old_top = SpriteTop;
            m_old_front = SpriteFront;
            m_old_left = SpriteLeft;
            m_old_right = SpriteRight;
            m_last_scale = transform.localScale;
            DisableSprites();
            InitiateSprites();
        }
    }

    private void DisableSprites()
    {
        if (SpritePanel != null && PanelTop != null)
        {
            foreach (Transform child in transform)
            {
                GameObject.DestroyImmediate(child.gameObject);
            }
        }
    }

    private void InitiateSprites()
    {
        if (SpritePanel != null)
        {

            PanelTop = Instantiate(SpritePanel, transform);
            PanelTop.GetComponent<SpriteRenderer>().sprite = SpriteTop;
            PanelTop.name = "Top";
            PanelTop.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            PanelFront = Instantiate(SpritePanel, transform);
            PanelFront.name = "Front";
            PanelFront.GetComponent<SpriteRenderer>().sprite = SpriteFront;
            PanelLeft = Instantiate(SpritePanel, transform);
            PanelLeft.name = "Left";
            PanelLeft.GetComponent<SpriteRenderer>().sprite = SpriteLeft;
            PanelLeft.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            PanelRight = Instantiate(SpritePanel, transform);
            PanelRight.name = "Right";
            PanelRight.GetComponent<SpriteRenderer>().sprite = SpriteRight;
            PanelRight.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            UpdateGridBounds();
        }
    }

    private void UpdateGridBounds()
    {
        Vector3 scale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        if (PanelTop != null)
        {
            PanelTop.transform.localPosition = new Vector3(0f, m_box.center.y + m_box.size.y / 2f, 0f);
            PanelTop.transform.localScale = new Vector3(1 / scale.x, 1 / scale.z, 1 / scale.y);
            PanelTop.GetComponent<SpriteRenderer>().size = new Vector2(scale.x, scale.z);
        }
        if (PanelFront != null)
        {
            PanelFront.transform.localPosition = new Vector3(0f, 0f, -m_box.center.z - m_box.size.z / 2f);
            PanelFront.transform.localScale = new Vector3(1 / scale.x, 1 / scale.y, 1 / scale.z);
            PanelFront.GetComponent<SpriteRenderer>().size = new Vector2(scale.x, scale.y);
        }
        if (PanelLeft != null)
        {
            PanelLeft.transform.localPosition = new Vector3(-m_box.center.x - m_box.size.x / 2f, 0f, 0f);
            PanelLeft.transform.localScale = new Vector3(1 / scale.z, 1 / scale.y, 1 / scale.x);
            PanelLeft.GetComponent<SpriteRenderer>().size = new Vector2(scale.z, scale.y);
        }
        if (PanelRight != null)
        {
            PanelRight.transform.localPosition = new Vector3(m_box.center.x + m_box.size.x / 2f, 0f, 0f);
            PanelRight.transform.localScale = new Vector3(1 / scale.z, 1 / scale.y, 1 / scale.x);
            PanelRight.GetComponent<SpriteRenderer>().size = new Vector2(scale.z, scale.y);
        }
        if (NavCube != null)
        {
            NavCube.transform.localScale = m_box.size;
            NavCube.transform.localPosition = m_box.center;
        }
    }
}
