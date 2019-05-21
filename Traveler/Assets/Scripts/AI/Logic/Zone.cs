using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : LogicalObject
{

    public List<AICharacter> OverlapCharacters;

    private List<BoxCollider> m_childColliders;
    private int m_lastKnowChildCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_childColliders = new List<BoxCollider>();
        initializeColliders();
        ZoneManager.RegisterZone(this);
    }
    private void OnDestroy()
    {
        ZoneManager.DeRegisterZone(this);
    }
    private void Update()
    {
        if (transform.childCount != m_lastKnowChildCount)
            initializeColliders();
    }
    internal void initializeColliders ()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject go = transform.GetChild(i).gameObject;
            m_childColliders.Add(go.GetComponent<BoxCollider>());
            if (go.GetComponent<ZoneComponent>() == null &&
                go.GetComponent<BoxCollider>())
            {
                go.GetComponent<BoxCollider>().isTrigger = true;
                go.AddComponent<ZoneComponent>();
            }
        }
        m_lastKnowChildCount = transform.childCount;
    }
    internal void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<AICharacter>() != null)
        {
            OnAddChar(other.GetComponent<AICharacter>());
        }
    }
    internal void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<AICharacter>() != null)
        {
            OnRemoveChar(other.GetComponent<AICharacter>());
        }
    }

    public void OnAddChar(AICharacter aic)
    {
        if (!OverlapCharacters.Contains(aic))
        {
            aic.OnEnterZone(this);
            OverlapCharacters.Add(aic);
        }
    }

    public void OnRemoveChar(AICharacter aic)
    {
        if (OverlapCharacters.Contains(aic))
        {
            aic.OnExitZone(this);
            OverlapCharacters.Remove(aic);
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
    public bool IsHaveObject(AICharacter aic)
    {
        return OverlapCharacters.Contains(aic);
    }
    public Vector3 NearestPointToZone(Vector3 point)
    {
        
        Vector3 closest = GetComponent<BoxCollider>().bounds.ClosestPoint(point);
        float dist = Vector3.Distance(point, closest);
        foreach (BoxCollider bc in m_childColliders)
        {
            Vector3 newPoint = bc.bounds.ClosestPoint(point);
            float newDist = Vector3.Distance(point, newPoint);
            if (newDist < dist)
            {
                dist = newDist;
                closest = newPoint;
            }
        }
        return closest;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, .15f);
        Gizmos.DrawCube(transform.position, transform.lossyScale);
    }
}
