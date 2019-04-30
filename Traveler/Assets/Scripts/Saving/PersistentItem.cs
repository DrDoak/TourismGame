using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PersistentItem : MonoBehaviour {
	public CharData data = new CharData();

	public bool recreated = false;
	public Vector3 StartPosition;

	public delegate void LoadFunction(CharData d);
	private event LoadFunction m_onLoad;
	private event LoadFunction m_onSave;

	protected bool m_registryChecked = false;
	void Awake() {
		if (data.regID == "") {
			data.regID = SaveObjManager.Instance.GenerateID (gameObject,data.prefabPath);
		}
//		saveID = data.regID;
		InitializeSaveLoadFuncs (saveBasic, loadBasic);
		StartPosition = transform.position;
	}
	public void registryCheck() {
		m_registryChecked = true;
		if (data.regID == "") {
			data.regID = "Not Assigned";
		}
		if (recreated)
			LoadData ();
		if (SaveObjManager.CheckRegistered(gameObject)) {
			//Debug.Log (gameObject + " Already registered, deleting duplicate ID: " + data.regID);
			Destroy(gameObject);
		}
	}
	void Update () {
		if (!m_registryChecked) {
			registryCheck ();
		}
	}

	private void saveBasic(CharData d) {
		d.name = gameObject.name;
		d.pos = transform.position;
		d.zRot = transform.rotation.eulerAngles.z;
		if (d.prefabPath == "")
			d.prefabPath = getProperName ();
	}
	public void StoreData() {
		m_onSave (data);
		/*
		if (GetComponent<Turret> ())
			data.TurretDefaultFace = GetComponent<Turret> ().DefaultFaceLeft;
		if (GetComponent<PropertyHolder> ()) {
			Property[] pL = GetComponents<Property> ();
			string[] allPs = new string[pL.Length];
			string[] allDs = new string[pL.Length];
			float[] allVs = new float[pL.Length];
			for (int i = 0; i < pL.Length; i++) {
				allPs [i] = pL [i].GetType ().ToString ();
				allDs [i] = pL [i].Description;
				allVs [i] = pL [i].value;
			}
			data.propertyList = allPs;
			data.propertyDescriptions = allDs;
			data.propertyValues = allVs;
			data.transfers = GetComponent<PropertyHolder> ().NumTransfers;
			data.slots = GetComponent<PropertyHolder> ().MaxSlots;
		}*/
	}

	private void loadBasic(CharData d) {
		Quaternion q = Quaternion.Euler(new Vector3 (0f, 0f, d.zRot));
		transform.localRotation = q;
		gameObject.name = d.name;
		gameObject.name = getProperName ();
	}
	public void LoadData() {
		m_onLoad (data);
		/*
		if (GetComponent<Turret> ())
			GetComponent<Turret> ().DefaultFaceLeft = data.TurretDefaultFace;
		if (GetComponent<PropertyHolder> ()) {
			GetComponent<PropertyHolder> ().NumTransfers = data.transfers;
			GetComponent<PropertyHolder> ().MaxSlots = data.slots;
			GetComponent<PropertyHolder> ().ClearProperties ();
			Debug.Log ("Readding properties: " + data.propertyList.Length);
			for (int i = 0; i < data.propertyList.Length; i++) {
				Type t = Type.GetType (data.propertyList [i]);
				GetComponent<PropertyHolder> ().AddProperty (data.propertyList [i]);
				Property p = (Property)gameObject.GetComponent (t);
				p.Description = data.propertyDescriptions [i];
				p.value = data.propertyValues [i];
			}
		}*/
	}
	public void InitializeSaveLoadFuncs(LoadFunction onSave, LoadFunction onLoad) {
		m_onSave += onSave;
		m_onLoad += onLoad;
	}
	private string getProperName() {
		string properName = "";
		foreach (char c in gameObject.name) {
			if (!c.Equals ('(') && !c.Equals(' ')) {
				properName += c;
			} else {
				break;
			}
		}
		return properName;
	}
	void OnEnable() {
		SaveObjManager.OnLoaded += LoadData;
		SaveObjManager.OnBeforeSave += StoreData;
	}

	void OnDisable() {
		SaveObjManager.OnLoaded -= LoadData;
		SaveObjManager.OnBeforeSave -= StoreData;
	}
}