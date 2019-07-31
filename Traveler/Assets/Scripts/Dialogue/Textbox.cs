using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public enum DialogueSound { TYPED, SPOKEN, RECORDED};

public class Textbox : MonoBehaviour {

	public DialogueUnit MasterSequence;
	public string FullText;
	public string CurrentText;
	public float TimeBetweenType = 0.01f;
	public float PauseAfterTextboxDone = 2f;
	public Color TextboxColor;
	public Dictionary<MovementBase, bool> FrozenCharacters;
	public DialogueSound TypingSound;

	TextMeshProUGUI m_Text;
	private float m_sinceLastChar = 0f;
	private float m_sinceLastSound = 0f;
	private float m_pauseTime = 0f;
	private float m_timeSinceStop = 0f;
	private Vector3 m_lastPos;
	private int m_lastCharacterIndex;
	private bool m_isTyping;

	private List<DialogueAction> m_potentialActions;
	private GameObject m_targetedObj;


	// Use this for initialization
	void Start () {
		m_Text = GetComponentInChildren<TextMeshProUGUI> ();
		if (!m_isTyping)
			m_Text.text = FullText;
		
		m_potentialActions = new List<DialogueAction> ();
		m_potentialActions.Add (new DAPause());
		m_potentialActions.Add (new DATextSpeed ());
		m_potentialActions.Add (new DAWalkTo ());
		m_potentialActions.Add (new DAControl ());
		m_potentialActions.Add (new DAFacingDirection ());
		m_potentialActions.Add (new DAKeyname ());
		m_potentialActions.Add (new DACameraFocus ());
		m_potentialActions.Add (new DASceneChange ());
		m_potentialActions.Add (new DAAnimation ());

		m_potentialActions.Add (new DAEnd ());
		m_potentialActions.Add (new DAJump ());
		m_potentialActions.Add (new DAQuestion ());

		m_potentialActions.Add (new DAVarSet ());
		m_potentialActions.Add (new DAVarCheck ());
	}

	void OnDestroy() {
		if (MasterSequence != null) {
			MasterSequence.parseNextElement ();
		}
		//TextboxManager.Instance.removeTextbox (gameObject);
		/*if (m_targetedObj.GetComponent<Character> ()) {
			m_targetedObj.GetComponent<Character> ().onTBComplete ();
		}*/
	}
		
	public void SetPause(float pt) {
		m_pauseTime = pt;
	}
	public void SetTargetObj(GameObject gameObj) {
		m_targetedObj = gameObj;
		if (m_targetedObj != null)
			m_lastPos = gameObj.transform.position;
	}
	public void SetTypeMode(bool type) {
		m_isTyping = type;
		if (type) {
			CurrentText = "";
			m_lastCharacterIndex = 0;
		} else {
			CurrentText = FullText;
		}
	}
	public void setText(string text) {
		FullText = text;
	}
	public void FreezeCharacter(MovementBase bm, bool isFrozen = true) {
		if (!FrozenCharacters.ContainsKey (bm))
			FrozenCharacters.Add (bm, bm.IsPlayerControl);
		bm.GetComponent<CharacterBase>().SetAutonomy(!isFrozen);
	}
		
	/* Cutscene scripting guide:
	 *  Normal text is shown as dialogue for the starting character.
	 * 	Using the '|' character or the newline character will create a new textbox.
	 *  At the start of a new textbox if the colon character is found within the first 18 characters, the game will attempt to search
	 *  For the character and make the dialogue come from that character instead.
	 *  
	 *  The characters < and > surrounds a special block. This can call commands in a simple lisp like language
	 * 
	 * 
	 * A number will result in a pause for a certain amount of time.
	 * $ will change the text speed
	 * 
	 * */

	public void PerformSpecialAction(string section) {
		string actStr = "";
		int charNum = 0;
		char nextChar = section.ToCharArray () [charNum];
		actStr += nextChar;
		int numSpecials = 1;
		while (numSpecials > 0 && charNum < section.Length - 1) {
			charNum++;
			nextChar = section.ToCharArray () [charNum];
			if (nextChar == '>') {
				numSpecials--;
				continue;
			} else if (nextChar == '<') {
				numSpecials++;
				continue;
			}
			actStr += nextChar;
		}
			
		List<DialogueAction> executedActions = new List<DialogueAction> ();
		foreach (DialogueAction da in m_potentialActions) {
			if (da.IsExecutionString (actStr))
				executedActions.Add (da);
		}
		foreach (DialogueAction da in executedActions)
			da.PerformAction (actStr, this);
	}


	private void processSpecialSection() {
		string actStr = "";
		char nextChar = FullText.ToCharArray () [m_lastCharacterIndex];
		int numSpecials = 1;
		while (numSpecials > 0 && m_lastCharacterIndex < FullText.Length - 1) {
			actStr += nextChar;
			m_lastCharacterIndex++;
			nextChar = FullText.ToCharArray () [m_lastCharacterIndex];
			if (nextChar == '>')
				numSpecials--;
			else if (nextChar == '<')
				numSpecials++;
		}
		List<DialogueAction> executedActions = new List<DialogueAction> ();
		foreach (DialogueAction da in m_potentialActions) {
			if (da.IsExecutionString (actStr))
				executedActions.Add (da);
		}
		foreach (DialogueAction da in executedActions)
			da.PerformAction (actStr, this);
		m_lastCharacterIndex++;
	}

	private void processNormalChar(char nextChar) {
		if (m_sinceLastSound > 0.15f) {
			m_sinceLastSound = 0f;
			playSound ();
		}
		CurrentText += nextChar;
		m_Text.text = CurrentText;
		m_sinceLastChar = 0f;
	}

	private void processChar() {
		m_lastCharacterIndex++;
		char nextChar = FullText.ToCharArray () [m_lastCharacterIndex - 1];
		if (nextChar == '<') {
			processSpecialSection ();
		} else {
			processNormalChar (nextChar);
		}
	}

	void Update () {
		if (m_targetedObj != null) {
			transform.position += m_targetedObj.transform.position-m_lastPos;
			m_lastPos = m_targetedObj.transform.position;
		}
		if (m_isTyping ) {
			if (m_lastCharacterIndex < FullText.Length) { 
				m_sinceLastChar += Time.deltaTime;
				m_sinceLastSound += Time.deltaTime;
				if (m_pauseTime > 0f) {
					m_pauseTime -= Time.deltaTime;
				} else if (m_sinceLastChar > TimeBetweenType) {
					processChar ();
				}
			} else {
				m_timeSinceStop += Time.deltaTime;
				if (m_timeSinceStop > PauseAfterTextboxDone) {
					Destroy (gameObject);
				}
			}

		}
	}
	private void playSound() {
		/*switch (TypingSound) {
		case DialogueSound.RECORDED:
			FindObjectOfType<AudioManager> ().PlayClipAtPos (UIList.Instance.SFXDialogueStatic, FindObjectOfType<CameraFollow> ().transform.position, 0.15f, 0f, 0.1f);
			break;
		case DialogueSound.TYPED:
			FindObjectOfType<AudioManager> ().PlayClipAtPos (UIList.Instance.SFXDialogueClick, FindObjectOfType<CameraFollow> ().transform.position, 0.2f, 0f, 0.25f);
			break;
		case DialogueSound.SPOKEN:
			FindObjectOfType<AudioManager> ().PlayClipAtPos (UIList.Instance.SFXDialogueSpeak, FindObjectOfType<CameraFollow> ().transform.position, 0.2f, 0f, 0.25f);
			break;
		default:
			break;
		}*/
	}
}

/*
private void playAnimation(string targetChar) {
		string[] chars = targetChar.Split(',');
		if (chars.Length < 2)
			return;
		GameObject character = GameObject.Find (chars [0]);
		string anim = chars [1];
		if (character != null && character.GetComponent<AnimatorSprite>()) {
			bool res = character.GetComponent<AnimatorSprite> ().Play (anim);
		}
	}
*/