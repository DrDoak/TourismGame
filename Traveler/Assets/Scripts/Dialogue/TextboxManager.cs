using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Luminosity.IO;

public class TextboxManager : MonoBehaviour {

	private static TextboxManager m_instance;

	public static TextboxManager Instance
	{
		get { return m_instance; }
		set { m_instance = value; }
	}
	//public delegate void optionResponse(int r);
	public GameObject textboxPrefab;
	public GameObject textboxStaticPrefab;
	public GameObject textboxFullPrefab;
	public GameObject SkipTextPrefab;

	public GameObject DialogueBoxPrefab;
	public GameObject DialogueOptionPrefab;

	public DialogueSound nextSoundType;
	List<DialogueSequence> m_currentSequences;

	//Color TextboxColor;
	float timeAfter = 2f;
	float textSpeed = 0.03f;

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
		m_currentSequences = new List<DialogueSequence> ();
	}

	public static void StartSequence(string text,GameObject speaker = null, bool Skippable = false) {
		DialogueSequence ds = Instance.parseSequence (text);
		ds.Speaker = speaker;
		ds.AdvanceSequence ();
		if (Skippable == true) {
			GameObject go = Instantiate (Instance.SkipTextPrefab);
			go.GetComponent<SkipText> ().SingleSequence = true;
			go.GetComponent<SkipText> ().toSkip = ds.allDUnits [0];
		}
		Instance.m_currentSequences.Add (ds);
	}

	public DialogueSequence parseSequence(string text,int startingChar = 0,int indLevel = 0,DialogueSequence parentSeq = null) {
		DialogueSequence newSeq = new DialogueSequence ();
		newSeq.parentSequence = parentSeq;
		List<DialogueUnit> subDS = new List<DialogueUnit> ();

		DialogueUnit du = new DialogueUnit {};
		subDS.Add (du);
		string lastText = "";
		string lastAnim = "none";
		int i = startingChar;
		bool full = false;
		int specialSequenceDepth = 0;
		while (i < text.Length) {
			char lastC = text.ToCharArray () [i];
			newSeq.rawText += lastC;
			du.RawText += lastC;
			if (lastC == '<')
				specialSequenceDepth++;
			else if (lastC == '>')
				specialSequenceDepth--;
			if (lastText.Length == 0 && lastC == ' ') {
			} else if (lastText.Length == 0 && lastC == '-') {
				DialogueSequence newS = parseSequence (text, i, indLevel + 1, newSeq);
				i += newS.numChars;
			} else if (lastC == '~') {
				full = true;
			} else if (specialSequenceDepth == 0 && (lastC == '\n' || lastC == '|')) {
				if (lastText.Length > 0) {
					if (lastAnim == "none") {
						du.addTextbox (lastText,full);
					} else {
						du.addTextbox (lastText, lastAnim,full);
					}
					full = false;
				}
				lastText = "";
			} else {
				lastText += lastC;
			}
			newSeq.numChars += 1;
			i += 1;
		}
		if (lastAnim == "none") {
			du.addTextbox (lastText,full);
		} else {
			du.addTextbox (lastText,lastAnim,full);
		}
		subDS.Add (du);
		newSeq.allDUnits = subDS;
		return newSeq;
	}

	public static string TrimSpecialSequences(string s) {
		bool inSpecial = false;
		string trimmedStr = "";
		for (int i = 0; i < s.Length; i++) {
			char c = s.ToCharArray () [i];
			if (c == '<')
				inSpecial = true;
			else if (c == '>')
				inSpecial = false;
			else if (!inSpecial)
				trimmedStr += c;
		}
		return trimmedStr;
	}

	public static void SetSoundType(DialogueSound ds) {
		m_instance.nextSoundType = ds;
	}
	public static Textbox addTextbox(string text,GameObject targetObj,bool full = false) {
		return m_instance.addTextbox (text, targetObj, true, m_instance.textSpeed,Color.black,full);
	}
	public Textbox addTextbox(string text,GameObject targetObj,bool typeText,float textSpeed, Color tbColor, bool full) {
		Vector2 newPos = new Vector2();
		GameObject newTextbox;
		if (full) {
			newTextbox = Instantiate (textboxFullPrefab);
		} else if (targetObj != null) {
			newPos = findPosition (targetObj.transform.position);
			newTextbox = Instantiate (textboxPrefab, newPos, Quaternion.identity);
		} else {
			newTextbox = Instantiate (textboxStaticPrefab);
		}

		Textbox tb = newTextbox.GetComponent<Textbox> ();
		tb.TypingSound = nextSoundType;
		/*if (!type) {
			//Debug.Log ("displaying Textbox: " + text);
			newTextbox.GetComponent<DestroyAfterTime> ().duration = textSpeed * 1.2f * text.Length + timeAfter;
			newTextbox.GetComponent<DestroyAfterTime> ().toDisappear = true;
		}*/

		tb.SetTypeMode (typeText);			
		tb.setText(text);
		//tb.transform.position = newPos;
		tb.SetTargetObj (targetObj);
		tb.PauseAfterTextboxDone = timeAfter;
		tb.TimeBetweenType = textSpeed;
		RectTransform[] transforms = newTextbox.GetComponentsInChildren<RectTransform> ();
		if (text.Length > 200) {
			Vector2 v = new Vector2 ();
			foreach (RectTransform r in transforms) {
				v.y = r.sizeDelta.y * 2f;
				v.x = r.sizeDelta.x;
				if (text.Length > 300) {
					v.x = r.sizeDelta.x * 1.5f;
				}
				r.sizeDelta = v;
			}
		}

		//textboxes.Add (newTextbox);
		//tb.setColor (tbColor);
		return tb;
	}

	public Vector2 findPosition(Vector2 startLocation) {
		float targetY = startLocation.y + 5f;
		return new Vector2 (startLocation.x, targetY);
	}
	public void setPauseAfterTextboxDone(float time) {
		timeAfter = time;
	}
	public void setTextSpeed(float time ){
		textSpeed = time;
	}
	public static void ClearAllSequences() {
		//Debug.Log ("Clearing: " + Instance.m_currentSequences.Count);
		foreach (DialogueSequence ds in Instance.m_currentSequences) {
			if (ds != null) {
				ds.CloseSequence ();
			}
		}
		Instance.m_currentSequences.Clear ();
	}
	public static string GetKeyString(string action) {
        return "D"; // InputManager.GetAction ("Default", action).Bindings [0].Positive.ToString ();
	}

    public static void StartDialogueOptions(DialogueSelectionInitializer initializer)
    {
        GameObject go = GameObject.Instantiate(GameObject.FindObjectOfType<TextboxManager>().DialogueBoxPrefab);
        go.GetComponent<DialogueOptionBox>().Prompt = initializer.prompt;
        //go.GetComponent<DialogueOptionBox>().MasterSequence = originTextbox.MasterSequence;
        Debug.Log("Add options: " + initializer.options.Count);
        foreach (DialogueOptionInitializer dop in initializer.options)
        {
            go.GetComponent<DialogueOptionBox>().AddDialogueOption(dop);
        }
        //originTextbox.MasterSequence.closeSequence();
    }
}

public class DialogueSelectionInitializer
{
    public string prompt;
    public List<DialogueOptionInitializer> options = new List<DialogueOptionInitializer>();
    public DialogueSelectionInitializer(string initialPrompt)
    {
        prompt = initialPrompt;
    }
    public void AddDialogueOption(DialogueOptionInitializer doption)
    {
        options.Add(doption);
    }
    public void AddDialogueOption(string promptText, DialogueOption.SelectFunction function)
    {
        DialogueOptionInitializer dop = new DialogueOptionInitializer();
        dop.SelectionText = promptText;
        dop.OnSelect = function;
        options.Add(dop);
    }
    public void AddDialogueOption(string promptText, string remainingSequenceText)
    {
        DialogueOptionInitializer dop = new DialogueOptionInitializer();
        dop.SelectionText = promptText;
        dop.OnSelect = SelectionFunction;
        dop.remainderText = remainingSequenceText;
        options.Add(dop);
    }
    private void SelectionFunction(DialogueOption dop)
    {
        Debug.Log("Starting seqeunce: " + dop.remainderText);
        TextboxManager.StartSequence(dop.remainderText);
        GameObject.Destroy(dop.MasterBox.gameObject);
    }
}

public class DialogueOptionInitializer
{
    public string SelectionText = "";
    public string remainderText = "";
    public DialogueOption.SelectFunction OnSelect;
    public DialogueOptionBox MasterBox;
}