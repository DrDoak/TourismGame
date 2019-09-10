using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIList : MonoBehaviour {
	private static UIList m_instance;

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
