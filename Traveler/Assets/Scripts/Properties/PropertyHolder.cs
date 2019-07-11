using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PropertyHolder : MonoBehaviour {

	public string HolderName = "SameAsObject";

	public int MaxSlots = 4;
	public int NumTransfers = 99;

	private List<Property> m_properties;
	private bool m_currentPlayer;
	private List<string> m_toRemove;

	// Use this for initialization
	void Awake () {
		m_properties = new List<Property> ();
		m_toRemove = new List<string> ();
		if (GetComponent<PersistentItem> () != null)
			GetComponent<PersistentItem> ().InitializeSaveLoadFuncs (storeData,loadData);
	}

	void Start() {
		Property[] prList = GetComponents<Property> ();
		foreach (Property p in prList) {
			if (p != null) {
				p.InitPropertyData ();
				m_properties.Add (p);
				p.OnAddProperty ();
			}
		}
		ExecuteEvents.Execute<ICustomMessageTarget> (gameObject, null, (x, y) => x.OnCreation ());
	}

	void Update() {
		ExecuteEvents.Execute<ICustomMessageTarget> (gameObject, null, (x, y) => x.OnUpdate ());
	}

	void LateUpdate() {
		foreach (string s in m_toRemove ) {
			Property toRemove = null;
			foreach (Property p in m_properties) {
				if (p.GetType().ToString() == s) {
					toRemove = p;
					break;
				}
			}
			if (toRemove != null) {
				RemoveProperty (toRemove);
			}
		}
		m_toRemove.Clear ();
	}

	public virtual List<Property> GetVisibleProperties() {
		List<Property> lp = new List<Property> ();
		foreach (Property p in m_properties) {
			if (p != null && p.Viewable && !m_toRemove.Contains(p.GetType().ToString())) {
				lp.Add (p);
			}
		}
		return lp;
	}

	public virtual void AddProperty(Property originalP) {
		if (originalP.GetType() == null)
			return;
		Type t = originalP.GetType();
		Property p = (Property)gameObject.AddComponent (t);

		p.CopyPropInfo (originalP);
		m_properties.Add (p);
		p.OnAddProperty ();
	}

	public virtual void AddProperty(string pName) {
		if (Type.GetType (pName) == null)
			return;
		//Property p = (Property)(System.Activator.CreateInstance (Type.GetType (pName)));
		Type t = Type.GetType (pName);
		Property p = (Property)gameObject.AddComponent (t);
		m_properties.Add (p);
		p.OnAddProperty ();
	}
	public virtual void ClearProperties() {
		Property[] prList = GetComponents<Property> ();
		foreach (Property p in prList) {
			if (m_properties.Contains (p)) {
				m_properties.Remove (p);
			}
			p.OnRemoveProperty();
			Destroy (p);
//			if (m_currentPlayer) {
//				GUIHandler.Instance.RemovePropIcon (p);
//			}
		}
		m_properties.Clear();
	}
	public virtual void RequestRemoveProperty(string pName) {
		m_toRemove.Add (pName);
	}

	public virtual void RemoveProperty(Property p) {
		if (m_properties.Contains(p)) {
			m_properties.Remove (p);
			p.OnRemoveProperty ();
			Destroy(p);
//			if (m_currentPlayer) {
//				GUIHandler.Instance.RemovePropIcon (p);
//			}
		} else if (HasProperty(p)) {
			Type pType = p.GetType();
			Property mp = (Property)gameObject.GetComponent(pType);
			m_properties.Remove (mp);
			mp.OnRemoveProperty ();
			Destroy(mp);
		}
	}

	public virtual bool HasProperty(Property p) {
		foreach (Property mp in m_properties) {
			if (mp != null && mp.GetType() == p.GetType())
				return true;
		}
		return false;
	}

	public virtual bool HasProperty(string pName) {
		foreach (Property p in m_properties) {
			if (p != null && p.PropertyName == pName)
				return true;
		}
		return false;
	}

	public virtual void TransferProperty( Property p, PropertyHolder other) {
		RemoveProperty (p);
		other.AddProperty (p);
	}

	public virtual GameObject AddBodyEffect(GameObject go) {
		GameObject newGO = Instantiate (go, transform.position, Quaternion.identity);
		newGO.transform.parent = transform;
		Vector3 newS = BodyScale ();
		//newGO.transform.localScale = newS;
		float z =  transform.rotation.eulerAngles.z;
		for (int i = 0; i < newGO.transform.childCount; i++) {
			ParticleSystem.ShapeModule s = newGO.transform.GetChild (i).GetComponent<ParticleSystem>().shape;
			s.scale = newS;
			Vector3 v = newGO.transform.GetChild (i).rotation.eulerAngles;
			newGO.transform.GetChild (i).rotation = Quaternion.Euler(new Vector3(v.x,v.y, z));
		}
		return newGO;
	}
	public virtual void RemoveBodyEffect(GameObject go) {
		Destroy (go);
	}
	public virtual Vector3 BodyScale() {
		Vector3 sc = transform.localScale;
		Vector2 v2 =  GetComponent<BoxCollider2D> ().size;
		return new Vector3(sc.x * v2.x,sc.y *v2.y,1f);
	}/*
	public void AddAmbient(AudioClip ac) {
		AmbientNoise am = GetComponent<AmbientNoise> ();
		if (am == null)
			am = gameObject.AddComponent<AmbientNoise> ();
		am.AddSound (ac);
	}
	public void RemoveAmbient(AudioClip ac) {
		AmbientNoise am = GetComponent<AmbientNoise> ();
		if (am == null)
			return;
		am.RemoveSound (ac);
	}*/


	private void storeData(CharData d) {
		Property[] pL = GetComponents<Property> ();
		string allPs = "";
		for (int i = 0; i < pL.Length; i++) {
			pL [i].OnSave (d);
			allPs += pL [i].GetType ().ToString ();
			allPs += ",";
		}
		d.SetString("Properties", allPs);
		d.SetInt("NumTransfers", NumTransfers);
		d.SetInt("MaxSlots", MaxSlots);
		//Debug.Log ("Saving: "); //d.PersistentStrings["Properties"]);
	}

	private void loadData(CharData d) {
		//Debug.Log ("Loading: " + d.PersistentStrings["Properties"]);

		NumTransfers = d.GetInt("NumTransfers");

		MaxSlots = d.GetInt("MaxSlots");
		GetComponent<PropertyHolder> ().ClearProperties ();
		string lastProp = "";
		for (int i = 0; i < d.GetString("Properties").Length; i++) {
			char l = d.GetString("Properties").ToCharArray () [i];
			if (l == ',') {
				Type t = Type.GetType (lastProp);
				AddProperty (lastProp);
				Property p = (Property)gameObject.GetComponent (t);
				p.OnLoad (d);
				lastProp = "";
			} else {
				lastProp += l;
			}
		}
	}
}