using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueUnit  {

	public DialogueUnit Previous;
	List<DialogueSubunit> elements;
	Textbox currentTB;

	public bool finished = false;
	int currentElement = 0;
	public string RawText;
	//List<Fighter> modifiedAnims;
	public GameObject Speaker;

	public Dictionary<MovementBase, bool> FrozenCharacters;

	// Use this for initialization
	public DialogueUnit () {
		//modifiedAnims = new List<Fighter> ();
		elements = new List<DialogueSubunit> ();
		FrozenCharacters = new Dictionary<MovementBase, bool>();
	}

	public void RestartSequence() {
		var du = new DialogueUnit { Previous = Previous, elements = elements};
		du.startSequence ();
	}
	public void startSequence() {
		parseNextElement ();
	}
	public void parseNextElement() {
		if (finished || currentElement >= elements.Count) {
			closeSequence ();
		} else {
			if (currentTB) {
				GameObject.Destroy (currentTB.gameObject);
				FrozenCharacters = currentTB.FrozenCharacters;
			}
			DialogueSubunit ne = elements [currentElement];
			currentTB = TextboxManager.addTextbox (ne.text, Speaker,ne.isFullScreen);
			currentTB.MasterSequence = this;
			currentTB.FrozenCharacters = FrozenCharacters;
			currentElement += 1;
		}
	}

	public void addTextbox(string s, bool full = false) {
		DialogueSubunit ne = new DialogueSubunit ();
		ne.text = s;
		ne.isFullScreen = full;
		elements.Add (ne);
	}
	public void addTextbox(string s,string animation, bool full = false) {
		DialogueSubunit ne = new DialogueSubunit ();
		ne.text = s;
		ne.animation = animation;
		ne.isFullScreen = full;
		elements.Add (ne);
	}

	public void goToElement(int i) {
		currentElement = i;
	}
	public void initiateDialogue() {
		currentElement = 0;
		parseNextElement ();
	}

	public void closeSequence() {
		if (currentTB)
			GameObject.Destroy (currentTB.gameObject);
		finished = true;
		foreach (MovementBase bm in FrozenCharacters.Keys) {
			bm.IsPlayerControl = FrozenCharacters [bm];
			/*if (bm.GetComponent<CharacterBase> () != null)
				bm.GetComponent<CharacterBase> ().SetPause (false); */
		}
	}

	void runEvent() {}
}