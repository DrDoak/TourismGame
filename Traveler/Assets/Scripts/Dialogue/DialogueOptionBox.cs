using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class DialogueOptionBox : Textbox {

	public string Prompt = "";
	List<DialogueOption> m_options;

	// Use this for initialization
	void Start () {
		transform.Find("text").GetComponent<TextMeshProUGUI> ().text = Prompt;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddDialogueOption(DialogueOption dop) {
		GameObject newOption = Instantiate (FindObjectOfType<TextboxManager> ().DialogueOptionPrefab,transform.Find("Options"));
		newOption.GetComponent<DialogueOption> ().SelectionText = dop.SelectionText;
		newOption.GetComponent<DialogueOption> ().OnSelect = dop.OnSelect;
		newOption.GetComponent<DialogueOption> ().MasterBox = this;
		newOption.GetComponent<DialogueOption> ().remainderText = dop.remainderText;
		newOption.GetComponentInChildren<TextMeshProUGUI> ().text = TextboxManager.TrimSpecialSequences (dop.SelectionText);
		EventSystem.current.SetSelectedGameObject(newOption);
		Debug.Log ("added DialogueOption: " + dop.SelectionText);
	}

	public void AddDialogueOption(string optionText, DialogueOption.SelectFunction func) {
		GameObject newOption = Instantiate (FindObjectOfType<TextboxManager> ().DialogueOptionPrefab,transform.Find("Options"));
		newOption.GetComponent<DialogueOption> ().SelectionText = optionText;
		newOption.GetComponentInChildren<TextMeshProUGUI> ().text = TextboxManager.TrimSpecialSequences (optionText);
		newOption.GetComponent<DialogueOption> ().MasterBox = this;
		newOption.GetComponent<DialogueOption> ().OnSelect = func;
		EventSystem.current.SetSelectedGameObject(newOption);
	}
}
