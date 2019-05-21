using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool AutoFindTarget = true;

    public GameObject Target;
    public Vector3 Offset;
    public float CamAngle;
    // Start is called before the first frame update
    void Start()
    {
        if (AutoFindTarget && Target == null)
        {
            Target = FindObjectOfType<ControlPlayer>().gameObject;
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
}
