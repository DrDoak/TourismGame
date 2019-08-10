using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBlock : MonoBehaviour
{
    public GameObject ParticlePrefab;
    private List<BasicPhysics> OverlapObjects;

    private List<BoxCollider> m_childColliders;
    private int m_lastKnowChildCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_childColliders = new List<BoxCollider>();
        initializeColliders();
    }

    internal void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<BasicPhysics>() != null)
        {
            OnAddChar(other.GetComponent<BasicPhysics>());
        }
    }
    private void Update()
    {
        if (transform.childCount != m_lastKnowChildCount)
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
        m_lastKnowChildCount = transform.childCount;
    }
    internal void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<BasicPhysics>() != null)
        {
            OnRemoveChar(other.GetComponent<BasicPhysics>());
        }
    }

    public void OnAddChar(BasicPhysics aic)
    {
        if (!OverlapObjects.Contains(aic))
        {
            OverlapObjects.Add(aic);
        }
    }

    public void OnRemoveChar(BasicPhysics aic)
    {
        if (OverlapObjects.Contains(aic))
        {
            OverlapObjects.Remove(aic);
        }
    }

    public bool IsInZone(Vector3 point)
    {
        if (GetComponent<BoxCollider>().bounds.Contains(point))
            return true;
        else
        {
            foreach (BoxCollider bc in m_childColliders)
            {
                Debug.Log("Has point? " + bc + " = " + bc.bounds.Contains(point) + " bounds: " + bc.bounds);
                if (bc.bounds.Contains(point))
                    return true;
            }
        }
        return false;
    }
    public bool IsHaveObject(BasicPhysics aic)
    {
        return OverlapObjects.Contains(aic);
    }
}
