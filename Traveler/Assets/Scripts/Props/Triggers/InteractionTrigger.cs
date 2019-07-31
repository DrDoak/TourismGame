using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    public Interactor MasterInteractor;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update() { }


    internal void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger enter other INteractor: ");
        //Debug.Log(other.gameObject.GetComponent<Interactable>());
        if (other.gameObject.GetComponent<Interactable>() != null && other.gameObject != MasterInteractor.gameObject)
        {
            //Debug.Log("Adding an INteractable");
            MasterInteractor.OverlapInteractions.Add(other.GetComponent<Interactable>());
        }
    }
    internal void OnTriggerExit(Collider other)
    {
        if (MasterInteractor.OverlapInteractions.Contains(other.gameObject.GetComponent<Interactable>()))
        {
            MasterInteractor.OverlapInteractions.Remove(other.GetComponent<Interactable>());
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, .05f);
        Gizmos.DrawCube(transform.position, transform.lossyScale);
    }
}
