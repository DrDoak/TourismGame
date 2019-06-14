﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;

    public GameObject Canvas;
    public GameObject PauseCanvas;

    public static GameManager Instance
    {
        get { return m_instance; }
        set { m_instance = value; }
    }
    void Awake()
    {

        if (m_instance == null)
        {
            m_instance = this;
            SceneManager.sceneLoaded += InitCanvasOnSceneLoad;
        }
        else if (m_instance != this)
        {
            Debug.Log("Destroying GM");
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Reset();

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void Reset()
    {
        SaveObjManager.charContainer = new CharacterSaveContainer();
        Instance.GetComponent<SaveObjManager>().SetDirectory("AutoSave");
        Instance.GetComponent<SaveObjManager>().resetRoomData();
    }
    public void InitCanvasOnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Init canvas on scene load");
        InitializeCanvas();
    }
    public void InitializeCanvas()
    {
        Debug.Log("INit canvas");
        Instantiate(Canvas);
        //Instantiate(PauseCanvas);
    }
}
