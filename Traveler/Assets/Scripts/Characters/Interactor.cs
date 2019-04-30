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

    void Start()
    {
        m_col = gameObject.GetComponent<Collider2D>();
        //m_promptUI = GameObject.Find("Interaction_prompt").GetComponentInChildren<Text>();

    }

    void Update()
    {
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
}
