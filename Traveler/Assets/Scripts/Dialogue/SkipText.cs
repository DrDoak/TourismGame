using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipText : MonoBehaviour {
	public string SceneName = "";	
	public bool DisablePause = false;
	public DialogueUnit toSkip;
	public bool SingleSequence = false;
	void Start() {
		//PauseGame.CanPause = !DisablePause;
	}

	void Update () {
		if (SingleSequence) {
			if (toSkip == null || toSkip.finished)
				Destroy (gameObject);
			else if (Input.GetKey (KeyCode.Escape))
				toSkip.closeSequence();
		} else if (Input.GetKey (KeyCode.Escape)) {
			TextboxManager.ClearAllSequences ();
			SceneManager.LoadScene (SceneName);
			TextboxManager.ClearAllSequences ();
		}
	}
}
