using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoxMaterialObj : MonoBehaviour
{ 

    public Sprite SpriteTop;
    public Vector2 SizeOfTopSprite = new Vector2(1f,1f);
    public Sprite SpriteFront;
    public Vector2 SizeOfFrontSprite = new Vector2(1f, 1f);
    public Sprite SpriteLeft;
    public Vector2 SizeOfLeftSprite = new Vector2(1f, 1f);
    public Sprite SpriteRight;
    public Vector2 SizeOfRightSprite = new Vector2(1f, 1f);
    private GameObject PanelTop;
    private GameObject PanelFront;
    private GameObject PanelLeft;
    private GameObject PanelRight;
    private GameObject NavCube;
    public GameObject RepeatingQuadPrefab;

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
        int cleared = 0;
        int OldChildCount = transform.childCount;
        int timesClear = 0;
        while (timesClear < 5 && transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                cleared++;
                GameObject.DestroyImmediate(child.gameObject);
            }
            timesClear++;
        }
    }

    private void InitiateSprites()
    {
        if (RepeatingQuadPrefab != null)
        {

            PanelTop = Instantiate(RepeatingQuadPrefab, transform);
            PanelTop.GetComponent<RepeatingQuadTexture>().RepeatingSprite = SpriteTop;
            PanelTop.GetComponent<RepeatingQuadTexture>().SizeOfRepeatTexture = SizeOfTopSprite;
            PanelTop.name = "Top";

            PanelTop.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            PanelFront = Instantiate(RepeatingQuadPrefab, transform);
            PanelFront.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            PanelFront.name = "Front";
            PanelFront.GetComponent<RepeatingQuadTexture>().RepeatingSprite = SpriteFront;
            PanelFront.GetComponent<RepeatingQuadTexture>().SizeOfRepeatTexture = SizeOfFrontSprite;

            PanelLeft = Instantiate(RepeatingQuadPrefab, transform);
            PanelLeft.name = "Left";
            PanelLeft.GetComponent<RepeatingQuadTexture>().RepeatingSprite = SpriteLeft;
            PanelLeft.GetComponent<RepeatingQuadTexture>().SizeOfRepeatTexture = SizeOfLeftSprite;

            PanelLeft.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            PanelRight = Instantiate(RepeatingQuadPrefab, transform);
            PanelRight.name = "Right";
            PanelRight.GetComponent<RepeatingQuadTexture>().RepeatingSprite = SpriteRight;
            PanelRight.GetComponent<RepeatingQuadTexture>().SizeOfRepeatTexture = SizeOfRightSprite;

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
        }
        if (PanelFront != null)
        {
            PanelFront.transform.localPosition = new Vector3(0f, 0f, -m_box.center.z - m_box.size.z / 2f);
        }
        if (PanelLeft != null)
        {
            PanelLeft.transform.localPosition = new Vector3(-m_box.center.x - m_box.size.x / 2f, 0f, 0f);
        }
        if (PanelRight != null)
        {
            PanelRight.transform.localPosition = new Vector3(m_box.center.x + m_box.size.x / 2f, 0f, 0f);
        }
        if (NavCube != null)
        {
            NavCube.transform.localScale = m_box.size;
            NavCube.transform.localPosition = m_box.center;
        }
    }
}
