using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSinkBlock : MonoBehaviour
{
    public Vector3 Offset = new Vector3();
    public List<BasicPhysics> m_overlapObjects;
    private List<BoxCollider> m_childColliders;
    private int m_lastKnownChildCount = 0;

    void Start()
    {
        m_childColliders = new List<BoxCollider>();
        m_overlapObjects = new List<BasicPhysics>();
        initializeColliders();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<BasicPhysics>() != null &&
            other.transform.Find("SpritePieces") != null)
        {
            OnAddChar(other.gameObject.GetComponent<BasicPhysics>());
        }
    }
    private void Update()
    {
        if (transform.childCount != m_lastKnownChildCount)
            initializeColliders();
    }
    internal void initializeColliders()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject go = transform.GetChild(i).gameObject;
            m_childColliders.Add(go.GetComponent<BoxCollider>());
            if (go.GetComponent<ParticleBlock>() == null &&
                go.GetComponent<BoxCollider>())
            {
                go.GetComponent<BoxCollider>().isTrigger = true;
                go.AddComponent<ParticleBlock>();
            }
        }
        m_lastKnownChildCount = transform.childCount;
    }

    public void OnAddChar(BasicPhysics aic)
    {
        if (!m_overlapObjects.Contains(aic))
        {
            aic.transform.Find("SpritePieces").transform.localPosition = aic.transform.Find("SpritePieces").transform.localPosition + Offset;
            m_overlapObjects.Add(aic);
        }
    }

    public void OnRemoveChar(BasicPhysics aic)
    {
        if (m_overlapObjects.Contains(aic))
        {
            aic.transform.Find("SpritePieces").transform.localPosition = aic.transform.Find("SpritePieces").transform.localPosition - Offset;
            m_overlapObjects.Remove(aic);
        }
    }

    internal void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<BasicPhysics>() != null)
        {
            OnRemoveChar(other.gameObject.GetComponent<BasicPhysics>());
        }
    }
}
