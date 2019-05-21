using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneComponent : LogicalObject
{
    Zone MasterZone;

    void Start()
    {
        if (GetComponent<BoxCollider>() != null)
            GetComponent<BoxCollider>().isTrigger = true;
        if (transform.parent != null && transform.parent.GetComponent<Zone>())
            MasterZone = transform.parent.GetComponent<Zone>();
    }

    internal void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<AICharacter>() != null)
        {
            MasterZone.OnAddChar(other.GetComponent<AICharacter>());
        }
    }
    internal void OnTriggerExit(Collider other)
    {
        if (MasterZone.OverlapCharacters.Contains(other.gameObject.GetComponent<AICharacter>()))
        {
            MasterZone.OnRemoveChar(other.GetComponent<AICharacter>());
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, .15f);
        Gizmos.DrawCube(transform.position, transform.lossyScale);
    }
}
