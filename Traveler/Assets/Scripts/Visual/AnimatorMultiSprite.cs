using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorMultiSprite : AnimatorSprite
{
    Dictionary<string,AnimationPiece> m_pieces;
    Dictionary<string, GameObject> m_attachPoints;
    // Start is called before the first frame update
    new void Awake()
    {
        base.Awake();
        init();
    }

    protected void init()
    {
        m_pieces = new Dictionary<string, AnimationPiece>();
        m_attachPoints = new Dictionary<string, GameObject>();
        initializeSubSprites();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void initializeSubSprites()
    {
        AnimationPiece[] pList = transform.Find("SpritePieces").gameObject.GetComponentsInChildren<AnimationPiece>();
        m_attachPoints.Clear();
        for (int i = 0; i < pList.Length; i++)
        {
            AnimationPiece ap = pList[i];
            for (int j = 0; j < ap.transform.childCount; j++ )
            {
                if (!m_attachPoints.ContainsKey(ap.transform.GetChild(j).name))
                    m_attachPoints.Add(ap.transform.GetChild(j).name, ap.transform.GetChild(j).gameObject);
            }
            
            m_pieces[ap.PieceType] = ap;
        }
    }
    public GameObject GetAttachPoint(string ID)
    {
        if (m_attachPoints.ContainsKey(ID))
            return m_attachPoints[ID];
        return null;
    }
    public override void Play(string[] stateNames, bool autoAlign = true)
    {
        foreach (AnimationPiece ap in m_pieces.Values)
        {
            //Debug.Log("Attempting to play: " + ap.gameObject.name + " animation: " + stateNames[0]);
            ap.Play(stateNames, autoAlign);
        }
        base.Play(stateNames, autoAlign);
    }

    public override bool Play(string stateName, bool autoAlign = true, bool forceReset = false)
    {
        foreach (AnimationPiece ap in m_pieces.Values)
        {
            ap.Play(stateName, autoAlign, forceReset);
        }
        return true;
    }
    public override void SetDirection(Direction d)
    {
        base.SetDirection(d);
        Vector3 v = transform.Find("SpritePieces").localScale;
        
        if (d == Direction.LEFT)
        {
            v.x = -1f;
        } else if (d == Direction.RIGHT)
        {
            v.x = 1f;
        }
        transform.Find("SpritePieces").localScale = v;
        foreach (AnimationPiece ap in m_pieces.Values)
        {
            ap.SetDirection(d);
        }
    }

    public void ReplacePiece(string piece, GameObject spritePiece)
    {

    }

    public void AddPiece(GameObject spritePiece)
    {
        initializeSubSprites();
    }

    public void RemovePiece(string pieceType)
    {
        //Debug.Log("Found piece: " + pieceType);
        if (m_pieces.ContainsKey(pieceType))
        {
            //Debug.Log("Attempting to remove: " + pieceType);
            GameObject go = m_pieces[pieceType].gameObject;
            //Debug.Log("Removing attachpoints: " + go.transform.childCount);
            for (int j = 0; j < go.transform.childCount; j++)
            {
                //Debug.Log("Removing item: " + go.transform.GetChild(j).name);
                m_attachPoints.Remove(go.transform.GetChild(j).name);
            }
            //Debug.Log("Contains key: " + m_attachPoints.ContainsKey(go.transform.GetChild(0).name));
            m_pieces.Remove(pieceType);
            Destroy(go);
        }
    }
}
