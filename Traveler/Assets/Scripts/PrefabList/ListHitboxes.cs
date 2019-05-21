using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListHitboxes : MonoBehaviour
{
    private static ListHitboxes m_instance;

    public GameObject InteractBox;
    public GameObject Hitbox;
    public GameObject HitboxLine;
    public GameObject HitboxMulti;
    public GameObject HitboxDoT;
    public GameObject StandardProjectile;

    public static ListHitboxes Instance
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
