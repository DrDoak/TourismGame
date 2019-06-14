using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Luminosity.IO;

//[RequireComponent(typeof(BasicMovement))]
public class Interactor : MonoBehaviour
{

    public Interactable PromptedInteraction;
    private Collider2D m_col;
    private Text m_promptUI;
    public string InteractionKey = "Interact";

    private InteractionTrigger m_interactionHitbox;
    private Orientation m_orient;
    public Vector3 InteractionOffset;
    public List<Interactable> OverlapInteractions;

    void Start()
    {
        m_col = gameObject.GetComponent<Collider2D>();
        //m_promptUI = GameObject.Find("Interaction_prompt").GetComponentInChildren<Text>();
        OverlapInteractions = new List<Interactable>();
        m_interactionHitbox = Instantiate(ListHitboxes.Instance.InteractBox,transform).GetComponent<InteractionTrigger>();
        Debug.Log("hitbox: " + m_interactionHitbox);
        m_interactionHitbox.transform.parent = transform;
        m_interactionHitbox.MasterInteractor = this;
        m_orient = GetComponent<Orientation>();
    }

    void Update()
    {
        if (m_orient !=  null)
        {
            m_interactionHitbox.transform.localPosition = m_orient.OrientVectorToDirection(InteractionOffset);
        }
        if (PromptedInteraction != null)
        {
            //m_promptUI.text = "Press '" + InteractionKey + "' " + PromptedInteraction.InteractionString;
            /*if (GetComponent<BasicMovement>().IsCurrentPlayer)
            {
                if (InputManager.GetButton(InteractionKey))
                {
                    PromptedInteraction.onHold(gameObject);
                }
                if (InputManager.GetButtonDown(InteractionKey))
                {
                    PromptedInteraction.onPress(gameObject);
                }
                else if (InputManager.GetButtonUp(InteractionKey))
                {
                    PromptedInteraction.onRelease(gameObject);
                }
            }*/
        }
    }

    public void OnAttemptInteract()
    {
        float minDistance = 4000;
        int maxPriority = -1;
        Interactable bestInteractable = null;
        foreach(Interactable i in OverlapInteractions)
        {
            if (i == null)
                continue;
            if (i.Priority > maxPriority || 
                ( i.Priority == maxPriority && 
                Vector3.Distance(i.gameObject.transform.position,transform.position) < minDistance))
            {
                minDistance = Vector3.Distance(i.gameObject.transform.position, transform.position);
                maxPriority = i.Priority;
                bestInteractable = i;
            }
        }
        if (bestInteractable != null)
        {
            bestInteractable.onPress(gameObject);
        }
    }

    public void OnAttemptInteract(Interactable i, bool force = false)
    {
        if (force)
            i.onPress(gameObject);
        else if (OverlapInteractions.Contains(i))
            i.onPress(gameObject);
    }
}
