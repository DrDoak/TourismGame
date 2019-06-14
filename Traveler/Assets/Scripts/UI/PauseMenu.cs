using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public bool ReturnMain = false;

    private GameObject m_pauseMenuUI;
    public GameObject m_saveScreen;
    public GameObject m_loadScreen;
    public GameObject m_deadScreen;
    public GameObject m_controlMap;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void CloseMenu()
    {
        m_saveScreen.SetActive(false);
        m_loadScreen.SetActive(false);
        m_deadScreen.SetActive(false);
        m_controlMap.SetActive(false);
    }
    public void MenuNew()
    {
        SaveObjManager.Instance.resetRoomData();
        SceneManager.LoadScene("LB_Intro");
        PauseGame.Resume();
    }
    public void MenuSave()
    {
        m_pauseMenuUI.SetActive(false);
        m_saveScreen.SetActive(true);
        m_saveScreen.GetComponent<SaveLoadMenu>().Refresh();
        m_saveScreen.GetComponent<SaveLoadMenu>().Reset();
    }
    public void MenuLoad()
    {
        m_pauseMenuUI.SetActive(false);
        m_loadScreen.SetActive(true);
        m_loadScreen.GetComponent<SaveLoadMenu>().Refresh();
        m_loadScreen.GetComponent<SaveLoadMenu>().Reset();
        PauseGame.CanPause = false;
    }
    public void MenuKeyBoardMap()
    {
        m_pauseMenuUI.SetActive(false);
        m_controlMap.SetActive(true);
        PauseGame.CanPause = false;
        EventSystem.current.SetSelectedGameObject(m_controlMap.transform.Find("main_panel").Find("back_button").gameObject);
    }
    public void MenuMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); //get rid of this hardcode
    }
    public void MenuExit()
    {
        Application.Quit();
    }
    public void ReturnToPause()
    {
        /*if (!ReturnMain)
            m_pauseMenuUI.SetActive(true);
        else
            EventSystem.current.SetSelectedGameObject(FindObjectOfType<MainMenu>().transform.Find("MainMenu").Find("New Game").gameObject);
        */
        m_saveScreen.SetActive(false);
        m_loadScreen.SetActive(false);
        m_deadScreen.SetActive(false);
        m_controlMap.SetActive(false);
        EventSystem.current.SetSelectedGameObject(m_pauseMenuUI.transform.Find("Resume Button").gameObject);
        PauseGame.CanPause = true;
    }
    public static void OnPlayerDeath()
    {
        PauseGame.SlowToPause();
        PauseGame.CanPause = false;
        /*Instance.m_pauseMenuUI.SetActive(false);
        Instance.m_saveScreen.SetActive(false);
        Instance.m_loadScreen.SetActive(false);
        Instance.m_deadScreen.SetActive(true);
        Instance.m_deadScreen.GetComponent<SaveLoadMenu>().Refresh();
        Instance.m_deadScreen.GetComponent<SaveLoadMenu>().Reset();
        AudioManager.instance.StopMusic();*/
    }

    
    void genericCancel() { }

    
    public void QuickLoad()
    {
        PauseGame.Pause(false);
        string w = "Load Last QuickSave? ";
        w += "\n All unsaved Progress will be lost.";
        WarningMessage.DisplayWarning(w, m_pauseMenuUI, quickLoad, "Warning", SetFirstOption);
    }
    private void SetFirstOption()
    {
        EventSystem.current.SetSelectedGameObject(m_pauseMenuUI.transform.Find("Resume Button").gameObject);
    }
    private void quickLoad()
    {
        bool result = SaveObjManager.Instance.LoadProfile("QuickSave");
        if (result == false)
        {
            SaveObjManager.Instance.LoadProfile("AutoSave");
        }
        PauseGame.Resume();
    }
}
