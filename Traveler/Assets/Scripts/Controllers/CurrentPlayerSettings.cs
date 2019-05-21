using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentPlayerSettings : MonoBehaviour
{
    private static CurrentPlayerSettings m_instance;

    public bool AutoFindPlayer;
    public GameObject CurrentPlayer;

    public static CurrentPlayerSettings Instance
    {
        get { return m_instance; }
        set { m_instance = value; }
    }
    private void Awake()
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

        if (AutoFindPlayer && CurrentPlayer != null)
        {
            SetCurrentPlayer();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentPlayer != null)
        {

        } else if (AutoFindPlayer)
        {
            SetCurrentPlayer();
        }
    }

    public void SetCurrentPlayer()
    {
        CurrentPlayer = FindCurrentPlayer();
    }

    public GameObject FindCurrentPlayer()
    {
        ControlPlayer cp = FindObjectOfType<ControlPlayer>();
        if (cp != null)
        {
            return cp.gameObject;
        }
        return null;
    }
}
