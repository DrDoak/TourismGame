using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SaveLoadMenu : MonoBehaviour {

	public bool MainMenu = false;
	TMP_Dropdown DropDown;
	TMP_InputField MyInputField;
	List<string> m_savedProfiles;
	TextMeshProUGUI m_message;
	int m_selectedIndex = 0;
	string m_deleteProfile;
	public string DefaultMessage = "Please Select a Profile";
	// Use this for initialization
	void Awake () {
		DropDown = transform.Find ("Dropdown").GetComponent<TMP_Dropdown>();
		m_message = transform.Find ("Message").GetComponent<TextMeshProUGUI> ();
		if (transform.Find ("ProfileInput") != null) {
			MyInputField = transform.Find ("ProfileInput").gameObject.GetComponent<TMP_InputField> ();
		}
		if (m_selectedIndex != 0 && m_selectedIndex < DropDown.options.Count) {
			DropDown.value = m_selectedIndex;
		}
		Refresh ();
	}
	void Update() {
		if (DropDown.IsExpanded || (MyInputField != null && MyInputField.isFocused)) {
			PauseGame.CanPause = false;
		}
	}
	public void Reset() {
		m_message.SetText (DefaultMessage);
		Debug.Log (EventSystem.current);
		EventSystem.current.SetSelectedGameObject(transform.Find("Return").gameObject);
	}
	public void Refresh() {
		m_savedProfiles = new List<string> ();
		foreach (string dir in Directory.GetDirectories(SaveObjManager.saveBase))
		{
			string s = dir.Substring (SaveObjManager.saveBase.Length);
			//Debug.Log (dir + " : " + s);
			m_savedProfiles.Add (s);
		}
		if (MainMenu)
			m_savedProfiles.Remove ("AutoSave");
		DropDown.ClearOptions ();
		DropDown.AddOptions (m_savedProfiles);
	}
	public void OnProfileSelect(int index) {
		if (MyInputField != null) {
			MyInputField.text = m_savedProfiles [DropDown.value];
		}
		m_selectedIndex = DropDown.value;
	}
	public void OnSave() {
		if (MyInputField.text == "") {
			m_message.text = "Please enter a profile name";
			return;
		}
		if (m_savedProfiles.Contains (MyInputField.text)) {
			string w = "Overwrite Profile: " + MyInputField.text + "?";
			WarningMessage.DisplayWarning (w,gameObject, m_save);
		} else {
			m_save ();
		}
	}
	private void m_save() {
		
		bool result = SaveObjManager.Instance.SaveProfile (MyInputField.text);
		//PauseGame.Instance.ReturnToPause ();
		if (result == true) {
			m_message.text = "Current Progress saved as " + MyInputField.text;
		} else {
			m_message.text = "ERROR: Could not save profile: " + MyInputField.text;
		}
		Refresh ();
		PauseGame.CanPause = true;
	}
	public void OnDelete () {
		m_deleteProfile = "";
		if (MyInputField != null) { 
			m_deleteProfile = MyInputField.text;
			MyInputField.text = "";
		} else {
			m_deleteProfile = m_savedProfiles [m_selectedIndex];
		}
		if (m_deleteProfile == "" ) {
			m_message.text = "No profile selected to delete";
			return;
		}
		if (!SaveObjManager.Instance.ProfileExists(m_deleteProfile)) {
			m_message.text = "Profile: " + m_deleteProfile + " does not exist";
			return;
		}
		if (m_deleteProfile == "AutoSave") {
			m_message.text = "AutoSave cannot be deleted.";
			return;
		}
		string w = "Are you sure you want to permanently delete: " + m_deleteProfile + "?";
			w += "\n Deleted saves cannot be recovered.";

		WarningMessage.DisplayWarning (w,gameObject, m_delete);
	}

	private void m_delete() {
		bool result = SaveObjManager.Instance.DeleteProfile (m_deleteProfile);
		Refresh ();
		if (result == true) {
			m_message.text = "Profile Successfully Deleted: " + m_deleteProfile;
		} else {
			m_message.text = "Profile not found: " + m_deleteProfile;
		}
		if (!MainMenu)
			PauseGame.CanPause = true;
	}
	public void OnLoad() {
		string w = "Load Profile: " + m_savedProfiles [m_selectedIndex] + "?";

		if (MainMenu == false) {
			w += "\n All unsaved Progress will be lost.";
            WarningMessage.DisplayWarning (w, gameObject, m_load);
		} else {
            WarningMessage.DisplayWarning (w, gameObject, m_load,"Confirmation");
		}
	}
	private void m_load() {
		//FindObjectOfType<GUIHandler> ().SetHUD (true);
		bool result = SaveObjManager.Instance.LoadProfile (m_savedProfiles[m_selectedIndex]);
		if (result == false) {
			m_message.text = "Profile: " + m_savedProfiles[m_selectedIndex] + " could not be loaded.";
			return;
		}
		PauseGame.Resume ();
		gameObject.SetActive (false);
	}
}
