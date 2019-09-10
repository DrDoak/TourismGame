using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListUI : MonoBehaviour
{
    private static ListUI m_instance;

    public GameObject ItemIcon;
    public GameObject EquipmentPreview;

    public static ListUI Instance
    {
        get { return m_instance; }
        set { m_instance = value; }
    }

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
}
