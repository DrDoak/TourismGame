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

    private float SEARCHFORPLAYER = 0.2f;
    private float aggressiveSearchEnd;
    private const float AGGRESSIVE_SEARCH_TIME = 0.5f;
    // Start is called before the first frame update
    void Awake()
    {

        /*if (AutoFindTarget && Target == null && FindObjectOfType<ControlPlayer>() != null)
        {
            SetCameraTarget();
            transform.position = Target.transform.position + Offset;
        }*/
        //SceneManager.sceneLoaded += onRoomLoad;
    }
    private void Update()
    {
        /*if (AutoFindTarget && Target == null && FindObjectOfType<ControlPlayer>() != null)
        {
            if (TimeNoPlayer < SEARCHFORPLAYER && Time.timeSinceLevelLoad > aggressiveSearchEnd)
            {
                TimeNoPlayer += Time.deltaTime;
            } else
            {
                
                SetCameraTarget();
            }
        }*/
    }
    

    public void SetCameraTarget(GameObject target)
    {
        GetComponent<Camera>().enabled = true;
        Target = target;
        transform.position = Target.transform.position + Offset;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (Target != null)
        {
            transform.position = Target.transform.position + Offset;
        }
    }

    /*private void onRoomLoad(Scene scene, LoadSceneMode mode)
    {
        ResetCamera();
        Debug.Log("On room load");
        TimeNoPlayer = 10f;
        aggressiveSearchEnd = Time.timeSinceLevelLoad + AGGRESSIVE_SEARCH_TIME;

    }
    public void ResetCamera()
    {
        TimeNoPlayer = 10f;
        aggressiveSearchEnd = Time.timeSinceLevelLoad + AGGRESSIVE_SEARCH_TIME;
    }*/
}
