using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIList : MonoBehaviour {
	private static UIList m_instance;
	public GameObject HealthBarPrefab;

	public AudioClip SFXSelect;
	public AudioClip SFXAccept;
	public AudioClip SFXDeny;
	public AudioClip SFXDialogueClick;
	public AudioClip SFXDialogueStatic;
	public AudioClip SFXDialogueSpeak;
	public AudioClip SFXMenuOpen;

	public static UIList Instance
	{
		get { return m_instance; }
		set { m_instance = value; }
	}

	void Awake()
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
	}
}
