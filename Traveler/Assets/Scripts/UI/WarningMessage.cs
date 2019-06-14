using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class WarningMessage : MonoBehaviour
{
    private static WarningMessage m_instance;
    GameObject m_warningScreen;
    GameObject m_warningPrevious;
    public delegate void ButtonClickEvent();
    ButtonClickEvent m_buttonEvent;
    ButtonClickEvent m_cancelEvent;


    public static WarningMessage Instance
    {
        get { return m_instance; }
        set { m_instance = value; }
    }

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        m_warningScreen = transform.Find("Warning").gameObject;
    }
    public static void DisplayWarning(string warningMessage, GameObject oldMenu, ButtonClickEvent func, string title = "Warning", ButtonClickEvent cancelFunc = null)
    {
        Instance.m_warningScreen.SetActive(true);
        oldMenu.SetActive(false);
        Instance.m_warningPrevious = oldMenu;
        Instance.m_warningScreen.transform.Find("Message").GetComponent<TextMeshProUGUI>().SetText(warningMessage);
        Instance.m_warningScreen.transform.Find("Title").GetComponent<TextMeshProUGUI>().SetText(title);
        Instance.m_buttonEvent = func;
        EventSystem.current.SetSelectedGameObject(Instance.m_warningScreen.transform.Find("Cancel").gameObject);
        if (cancelFunc == null)
        {
            Instance.m_cancelEvent = Instance.genericCancel;
        }
        else
        {
            Instance.m_cancelEvent = cancelFunc;
        }
    }
    public void OnCancelWarning()
    {
        m_warningScreen.SetActive(false);
        m_warningPrevious.SetActive(true);
        m_cancelEvent();
    }
    public void OnConfirmWarning()
    {
        m_warningScreen.SetActive(false);
        m_warningPrevious.SetActive(true);
        m_buttonEvent();
    }
    void genericCancel() { }
}
