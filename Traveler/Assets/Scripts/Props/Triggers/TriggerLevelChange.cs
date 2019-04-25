using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLevelChange : MonoBehaviour
{
    public Color GizmosColor = Color.yellow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = GizmosColor;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
