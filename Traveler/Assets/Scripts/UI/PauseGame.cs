using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PauseGame : MonoBehaviour
{
    private static PauseGame m_instance;
    
    public static bool isPaused = false;
    public static bool CanPause = true;

    private float m_slowingSpeed = 0.0f;
    private float m_speedingSpeed = 0.0f;

    private GameObject mMenu;
    public static PauseGame Instance
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
        mMenu = transform.Find("PauseMenu").gameObject;
        mMenu.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (CanPause && Input.GetButtonDown("Pause"))
        {
            Debug.Log("Pausing button now");
            if (isPaused)
                Resume();
            else
                Pause();
        }
        speedSlowManage();
    }


    void speedSlowManage()
    {
        if (m_slowingSpeed > 0.0f && Time.timeScale > 0f)
        {
            Time.timeScale = Mathf.Max(0f, Time.timeScale - (m_slowingSpeed * Time.unscaledDeltaTime));
            if (Time.timeScale == 0f)
            {
                m_slowingSpeed = 0f;
            }
        }
        if (m_speedingSpeed > 0.0f && Time.timeScale < 1f)
        {
            Time.timeScale = Mathf.Min(1f, Time.timeScale + (m_speedingSpeed * Time.unscaledDeltaTime));
            if (Time.timeScale == 1f)
            {
                m_speedingSpeed = 0f;
            }
        }
    }

    public static void Resume()
    {
        //m_instance.mResume();
        m_instance.m_slowingSpeed = 0f;
        m_instance.mMenu.SetActive(false);
        Time.timeScale = 1f;
        PauseGame.CanPause = true;
        isPaused = false;
        //AudioManager.instance.ResumeMusic();
    }
    public static void Pause(bool drawMenu = true)
    {
        m_instance.mPause(drawMenu);
    }
    public static void SlowToPause(float slowSpeed = 1.0f)
    {
        m_instance.m_slowingSpeed = slowSpeed;
        isPaused = true;
    }
    public static void SpeedToResume(float speedSpeed = 1.0f)
    {
        m_instance.m_speedingSpeed = speedSpeed;
        isPaused = false;
    }
    void mPause(bool drawMenu = true)
    {
        if (drawMenu)
        {
            m_instance.mMenu.SetActive(true);
            //SetFirstOption();
        }
        Time.timeScale = 0f;
        isPaused = true;
        //AudioManager.instance.PauseMusic();
    }
    
}
