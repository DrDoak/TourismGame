using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public bool AutoFindTarget = true;

    private static GameManager m_instance;

    public GameObject Canvas;
    public GameObject PauseCanvas;

    [SerializeField]
    private Vector3 InputMove;

    private int numUIBars;
    private GameObject CurrentPlayer;
    public float TimeNoPlayer = 0f;

    private float SEARCHFORPLAYER = 0.2f;
    private float aggressiveSearchEnd;
    private const float AGGRESSIVE_SEARCH_TIME = 0.5f;

    public static GameManager Instance
    {
        get { return m_instance; }
        set { m_instance = value; }
    }
    void Awake()
    {
        if (AutoFindTarget && CurrentPlayer == null && FindObjectOfType<ControlPlayer>() != null)
        {
            //SetCameraTarget();
            SearchForPlayer();
        }
        SceneManager.sceneLoaded += onRoomLoad;

        if (m_instance == null)
        {
            m_instance = this;
            SceneManager.sceneLoaded += InitCanvasOnSceneLoad;
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Reset();
        GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
        GraphicsSettings.transparencySortAxis = new Vector3(1f, 0f, 0f);// new Vector3(0f, 0.4f, 0.9f);// new Vector3(0.9f,0.0f, 0.1f);
        Debug.Log(GraphicsSettings.transparencySortAxis);
    }
    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void Update()
    {
        if (AutoFindTarget && CurrentPlayer == null && FindObjectOfType<ControlPlayer>() != null)
        {
            if (TimeNoPlayer < SEARCHFORPLAYER && Time.timeSinceLevelLoad > aggressiveSearchEnd)
            {
                TimeNoPlayer += Time.deltaTime;
            }
            else
            {
                SearchForPlayer();
            }
        }

        if (CurrentPlayer != null && CurrentPlayer.GetComponent<CharacterBase>() != null)
        {
            if (numUIBars != CurrentPlayer.GetComponent<CharacterBase>().GetNumUIBars())
            {
                Debug.Log("NumUIBars:" + numUIBars + " numBars: " + CurrentPlayer.GetComponent<CharacterBase>().GetNumUIBars());
                FindObjectOfType<GUIManager>().ClearAllUIBars();
                CurrentPlayer.GetComponent<CharacterBase>().DrawAllUIBars(FindObjectOfType<GUIManager>());
                numUIBars = CurrentPlayer.GetComponent<CharacterBase>().GetNumUIBars();
            }
        }

    }
    public static void Reset()
    {
        SaveObjManager.charContainer = new CharacterSaveContainer();
        Instance.GetComponent<SaveObjManager>().SetDirectory("AutoSave");
        Instance.GetComponent<SaveObjManager>().resetRoomData();
    }
    public void InitCanvasOnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        InitializeCanvas();
    }
    public void InitializeCanvas()
    {
        Instantiate(Canvas);
        //Instantiate(PauseCanvas);
    }
    private void SearchForPlayer()
    {
        ControlPlayer[] players = FindObjectsOfType<ControlPlayer>();
        foreach (ControlPlayer pl in players)
        {
            if (pl.GetComponent<MovementBase>().IsPlayerControl)
            {
                SetPlayer(pl.gameObject);
            }
        }
    }

    private void onRoomLoad(Scene scene, LoadSceneMode mode)
    {
        ResetPlayerSearch();
        TimeNoPlayer = 10f;
        aggressiveSearchEnd = Time.timeSinceLevelLoad + AGGRESSIVE_SEARCH_TIME;

    }
    public void ResetPlayerSearch()
    {
        TimeNoPlayer = 10f;
        aggressiveSearchEnd = Time.timeSinceLevelLoad + AGGRESSIVE_SEARCH_TIME;
    }
    public void SetPlayer(GameObject newPlayer)
    {
        GetComponent<CameraController>().SetCameraTarget(newPlayer);
        
        CurrentPlayer = newPlayer;
        TimeNoPlayer = 0f;
        if (FindObjectOfType<GUIManager>() == null)
            return;
        FindObjectOfType<GUIManager>().ClearAllUIBars();

        if (newPlayer == null)
            return;

        if (newPlayer.GetComponent<CharacterBase>() != null)
            newPlayer.GetComponent<CharacterBase>().DrawAllUIBars(FindObjectOfType<GUIManager>());
        CurrentPlayer = newPlayer;
        numUIBars = CurrentPlayer.GetComponent<CharacterBase>().GetNumUIBars();
        
    }

    public void ClearPlayer()
    {
        SetPlayer(null);
    }
}
/*
Mahogany
    */