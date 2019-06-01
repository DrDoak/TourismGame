using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPiece : AnimatorSprite
{
    GameObject Parent;
    public string PieceType;
    public string AttachPointOther;
    public string AttachPointMe;

    private GameObject MyAttachObject;
    private GameObject OtherAttachObject;

    List<GameObject> m_allAttachPoints;
    private bool LastFlippedState = false;
    // Start is called before the first frame update
    void Start()
    {
        m_allAttachPoints = new List<GameObject>();
        findAttachmentPoint();
        GetAllAttachPoints();
    }

    // Update is called once per frame
    void Update()
    {
        // updateAttachmentPoint();
    }

    private void GetAllAttachPoints()
    {
        for (int j = 0; j < transform.childCount; j++)
        {
            Debug.Log("Adding attach point: " + transform.GetChild(j).name);
            m_allAttachPoints.Add(transform.GetChild(j).gameObject);
        }

    }

    private void SetFlip( bool flipX)
    {
        if (flipX != LastFlippedState)
        {
            foreach (GameObject g in m_allAttachPoints)
            {
                Vector3 v = g.transform.localPosition;
                v.x = -v.x;
                Debug.Log("FLippy: " + g.gameObject);
                g.transform.localPosition = v;
            }
            LastFlippedState = flipX;
        }
    }
    private void findAttachmentPoint()
    {
        MyAttachObject = transform.Find(AttachPointMe).gameObject;
        OtherAttachObject = transform.parent.parent.GetComponent<AnimatorMultiSprite>().GetAttachPoint(AttachPointOther);
        if (MyAttachObject != null && OtherAttachObject != null)
        {
            transform.localPosition = MyAttachObject.transform.position - transform.position;
            transform.parent = OtherAttachObject.transform;
        }
    }

    private void updateAttachmentPoint()
    {
        if (OtherAttachObject != null)
        {
            
            //transform.localRotation = OtherAttachObject.transform.rotation;
            transform.position = (transform.position - MyAttachObject.transform.position) + OtherAttachObject.transform.position;
        }
    }
    public override bool Play(string stateName, bool autoAlign = true, bool forceReset = false)
    {
        bool retVal = base.Play(stateName, autoAlign, forceReset);
        if (retVal && MyAttachObject != null && OtherAttachObject != null)
        {
            Vector3 offset = MyAttachObject.transform.localPosition;
            transform.localPosition = -offset;
        }
        //SetFlip(GetComponent<SpriteRenderer>().flipX);
        return retVal;
    }
    public override void SetDirection(Direction d)
    {
        string lastSuffix = suffix;
        if (d == Direction.DOWN)
        {
            suffix = "_d";
        }
        else if (d == Direction.UP)
        {
            suffix = "_u";
        }
        else
        {
            suffix = "";
        }
        if (lastSuffix != suffix)
        {
            Play(m_baseAnimName, m_autoAlign, true);
        }
    }
}
