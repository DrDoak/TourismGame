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
    private Vector3 lastMyAttachPointPos;
    // Start is called before the first frame update
    void Start()
    {
        InitPiece();
    }

    public void InitPiece()
    {
        m_allAttachPoints = new List<GameObject>();
        findAttachmentPoint();
        GetAllAttachPoints();
        lastMyAttachPointPos = new Vector3();
    }
    
    // Update is called once per frame
    void Update()
    {
        updateAttachmentPoint();
    }

    private void GetAllAttachPoints()
    {
        for (int j = 0; j < transform.childCount; j++)
        {
            //Debug.Log("Adding attach point: " + transform.GetChild(j).name);
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
                //Debug.Log("FLippy: " + g.gameObject);
                g.transform.localPosition = v;
            }
            LastFlippedState = flipX;
        }
    }
    private void findAttachmentPoint()
    {
        if (transform.parent == null || transform.parent.parent == null)
            return;
        MyAttachObject = transform.Find(AttachPointMe).gameObject;
        OtherAttachObject = transform.parent.parent.GetComponent<AnimatorMultiSprite>().GetAttachPoint(AttachPointOther);
        if (MyAttachObject != null && OtherAttachObject != null)
        {
            transform.parent = OtherAttachObject.transform;
            transform.localPosition = transform.position - MyAttachObject.transform.position;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            lastMyAttachPointPos = MyAttachObject.transform.localPosition;
        }
    }

    private void updateAttachmentPoint()
    {
        if (lastMyAttachPointPos != Vector3.zero)
        {

            //transform.localRotation = Quaternion.EulerAngles(Vector3.zero); // OtherAttachObject.transform.rotation;
            transform.position += (MyAttachObject.transform.localPosition - lastMyAttachPointPos);
            lastMyAttachPointPos = MyAttachObject.transform.localPosition;
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
