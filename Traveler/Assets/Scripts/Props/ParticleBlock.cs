using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBlock : MonoBehaviour
{
    public GameObject ParticlePrefab;
    public int EmitParticleAmount = 4;
    private List<BasicPhysics> OverlapObjects;

    private List<BoxCollider> m_childColliders;
    private int m_lastKnowChildCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_childColliders = new List<BoxCollider>();
        OverlapObjects = new List<BasicPhysics>();
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
        List<BasicPhysics> newList = new List<BasicPhysics>();
        foreach( BasicPhysics bp in OverlapObjects)
        {
            if (bp != null)
            {
                particleProcess(bp);
                newList.Add(bp);
            }
        }
        OverlapObjects = newList;
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
            if (ParticlePrefab != null)
            {
                aic.AddParticleSystem(gameObject, ParticlePrefab);
                //aic.EmitParticle(gameObject, EmitParticleAmount);
                //Instantiate(ParticlePrefab, aic.transform.position, Quaternion.identity);
            }
            OverlapObjects.Add(aic);
        }
    }

    public void OnRemoveChar(BasicPhysics aic)
    {
        if (OverlapObjects.Contains(aic))
        {
            aic.RemoveParticleSystem(gameObject);
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

    private void particleProcess(BasicPhysics physics)
    {
        if (physics.DrawParticles && ParticlePrefab != null && physics.m_trueVelocity.magnitude > 0.1f && physics.IsGrounded)
        {
            if (physics.ShouldDrawParticle())
            {
                physics.EmitParticle(gameObject, EmitParticleAmount);
                //Instantiate(ParticlePrefab, physics.transform.position, Quaternion.identity);
            }
        }
    }
}
