using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InteractionInfo
{
    public string InteractionPrompt = "to Interact";
    public int Priority = 1;
    public bool autoTrigger = true;
    public float TriggerRefresh = 2.0f;
    public bool oneTime = true;
}

public class Interactable : MonoBehaviour
{
    [HideInInspector]
    public Interactor Actor;

    public InteractionInfo interactableObjectInfo;

    private float lastTimeTriggered = 0.0f;

    
    [HideInInspector]
    public bool TriggerUsed = false;

    [HideInInspector]
    public bool HoldTrigger;
    [HideInInspector]
    public bool PressTrigger;

   // private UIActionText m_prompt;

    void Start()
    {
        init();
    }

    protected void init()
    {
        lastTimeTriggered = -interactableObjectInfo.TriggerRefresh;
        Actor = null;
        //Will become true if the interactor presses/holds the interaction key while in this interactable's area
        HoldTrigger = false;
        PressTrigger = false;

       /* m_prompt = new UIActionText();
        m_prompt.text = "Press Enter to Interact";
        m_prompt.textColor = Color.white;
        m_prompt.timeToDisplay = 2.0f;
        m_prompt.id = "intPrompt";*/

        /*if (GetComponent<PersistentItem>() != null)
            GetComponent<PersistentItem>().InitializeSaveLoadFuncs(storeData, loadData);*/
    }
    void Update()
    {
        destroyAfterUse();
    }
    protected void destroyAfterUse()
    {
        if (interactableObjectInfo.oneTime && TriggerUsed)
            Destroy(gameObject);
    }

    protected void TriggerWithCoolDown(GameObject Interactor)
    {
        if (Time.timeSinceLevelLoad - lastTimeTriggered >= interactableObjectInfo.TriggerRefresh)
        {
            lastTimeTriggered = Time.timeSinceLevelLoad;
            TriggerUsed = true;
            onTrigger(Interactor);
        }
    }
    protected virtual void onTrigger(GameObject interactor) { }

    internal void OnTriggerEnter(Collider other)
    {
        if (interactableObjectInfo.autoTrigger && other.gameObject.GetComponent<Interactor>())
        {
            TriggerWithCoolDown(other.gameObject);
        }
        if (other.gameObject.GetComponent<Interactor>() != null)
        {
            /*m_prompt.text = "Press " + TextboxManager.GetKeyString("Interact") + " to " + InteractionPrompt;
            FindObjectOfType<GUIHandler>().ReplaceText(m_prompt);*/
            Actor = other.gameObject.GetComponent<Interactor>();
           // Actor.PromptedInteraction = this;
        }
    }

    internal void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Interactor>() != null &&
            Actor == other.gameObject.GetComponent<Interactor>())
        {
           /* FindObjectOfType<GUIHandler>().RemoveText(m_prompt);
            Actor.PromptedInteraction = null;*/
        }
    }

    /*private void storeData(CharData d)
    {
        d.PersistentBools["TriggerUsed"] = TriggerUsed;
    }

    private void loadData(CharData d)
    {
        TriggerUsed = d.PersistentBools["TriggerUsed"];
    }*/

    public virtual void onHold(GameObject interactor)
    {
    }

    public virtual void onRelease(GameObject interactor) { }

    public virtual void onPress(GameObject interactor)
    {
        if (IsInteractable(interactor))
            TriggerWithCoolDown(interactor);
    }
    public virtual bool IsInteractable(GameObject interactor)
    {
        return true;
    }
}
