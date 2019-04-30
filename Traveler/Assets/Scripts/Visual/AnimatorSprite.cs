﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSprite : MonoBehaviour
{
	private Animator m_anim;
	private List<string> m_states;
	public string CurrentAnimation { get { return m_currentAnim; } private set { m_currentAnim = value; } }
	private string m_currentAnim = "";
    private string m_baseAnimName = "";
    private string suffix = "";
    private bool m_autoAlign = true;

	internal void Awake()
	{
		m_states = new List<string>();
		m_anim = GetComponent<Animator>();
        if (m_anim == null)
        {
            m_anim = GetComponentInChildren<Animator>();
        }
		//transform.position = new Vector3 (transform.position.x, transform.position.y,  -1f * (GetComponent<Renderer> ().sortingOrder) / 32);
	}

	public void Play(string[] stateNames, bool autoAlign = true)
	{
		foreach (string s in stateNames)
		{
            m_autoAlign = autoAlign;

            if (Play(s, autoAlign)) 
				break;
		}
	}

	public bool Play(string stateName, bool autoAlign = true, bool forceReset = false)
	{
		if (!forceReset && (m_currentAnim == (stateName + suffix) || m_currentAnim == "none"))
			return true;
        m_autoAlign = autoAlign;
        if (autoAlign && suffix.Length > 0)
        {
            string newName = stateName + suffix;
            if (m_states.Contains(newName))
                return SetAndPlay(stateName,suffix);

            if (m_anim.HasState(0, Animator.StringToHash(newName)))
            {
                m_states.Add(newName);
                return SetAndPlay(stateName,suffix);
            }
            //return false;
        }
		if (m_states.Contains(stateName))
			return SetAndPlay(stateName);
		
		if (m_anim.HasState(0, Animator.StringToHash(stateName))) {
			m_states.Add(stateName);
			return SetAndPlay(stateName);
		}
		return false;
	}

	private bool SetAndPlay(string stateName, string suffix = "")
	{
        m_baseAnimName = stateName;
        m_currentAnim = stateName + suffix;
        m_anim.Play(m_currentAnim);
		return true;
	}

	public void SetSpeed(float speed)
	{
		m_anim.speed = speed;
	}
    public void SetDirection(Direction d)
    {
        string lastSuffix = suffix;
        if (d == Direction.DOWN)
        {
            suffix = "_d";
        } else if (d == Direction.UP)
        {
            suffix = "_u";
        } else
        {
            suffix = "";
        }
        if (lastSuffix != suffix)
        {
            Play(m_baseAnimName, m_autoAlign,true);
        }
    }
}