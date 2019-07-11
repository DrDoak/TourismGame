using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    public bool AutoFindTarget = true;

    public GameObject Target;
    public Vector3 Offset;
    public float CamAngle;
    public float TimeNoPlayer = 0f;

    private float SEARCHFORPLAYER = 1f;
    // Start is called before the first frame update
    void Awake()
    {

        if (AutoFindTarget && Target == null && FindObjectOfType<ControlPlayer>() != null)
        {
            Target = FindObjectOfType<ControlPlayer>().gameObject;
            transform.position = Target.transform.position + Offset;
            GetComponent<Camera>().enabled = true;
        }
        TimeNoPlayer = 0f;
        SceneManager.sceneLoaded += onRoomLoad;
    }
    private void Update()
    {
        if (AutoFindTarget && Target == null && FindObjectOfType<ControlPlayer>() != null)
        {
            if (TimeNoPlayer < SEARCHFORPLAYER)
            {
                TimeNoPlayer += Time.deltaTime;
            } else
            {
                TimeNoPlayer = 0f;
                Target = FindObjectOfType<ControlPlayer>().gameObject;
            }
        }
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (Target != null)
        {
            transform.position = Target.transform.position + Offset;
        }
    }

    public void SetTarget(GameObject go)
    {
        Target = go;
    }

    private void onRoomLoad(Scene scene, LoadSceneMode mode)
    {
        TimeNoPlayer = 10f;
    }
}
