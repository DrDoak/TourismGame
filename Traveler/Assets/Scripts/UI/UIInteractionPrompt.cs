using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInteractionPrompt : MonoBehaviour
{
    public GameObject Target;

    private TextMeshProUGUI m_promptUI;
    private int m_lastLength = 0;
    void Start()
    {
        //m_promptUI = GameObject.Find("Interaction_prompt").GetComponentInChildren<Text>();
        m_promptUI = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (Target != null)
        {
            Interactor io = Target.GetComponent<Interactor>();
            if (io.OverlapInteractions.Count != m_lastLength)
            {
                if (io.OverlapInteractions.Count > 0)
                {
                    Interactable i = io.OverlapInteractions[io.OverlapInteractions.Count - 1];
                    m_promptUI.text = "Press ' Interact ' " + i.interactableObjectInfo.InteractionPrompt;
                } else
                {
                    m_promptUI.text = "";
                }
                m_lastLength = io.OverlapInteractions.Count;
            }
        } else
        {
            Target = CurrentPlayerSettings.Instance.CurrentPlayer;
        }
    }
}
