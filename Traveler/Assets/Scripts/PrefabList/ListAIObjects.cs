using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListAIObjects : MonoBehaviour
{
    private static ListAIObjects m_instance;

    public GameObject GenericGoal;

    public static ListAIObjects Instance
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
