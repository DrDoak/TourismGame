using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAction  {

	public virtual bool IsExecutionString(string actionString) {return false;}

	public virtual void PerformAction(string remainder, Textbox originTextbox) {}

	protected bool MatchStart(string actionString, string key) {
		if (key.Length > actionString.Length)
			return false;
		for (int i = 0; i < key.Length; i++) {
			if (actionString.ToCharArray () [i] != key.ToCharArray () [i])
				return false;
		}
		return true;
	}

	protected List<string> ExtractArgs(string actionString, string key) {
		List<string> allArgs = new List<string> ();
		if (key.Length > actionString.Length)
			return allArgs;
		string lastArg = "";
		int numSpecials = 0;
		for (int i = key.Length; i < actionString.Length; i++) {
			char nextChar = actionString.ToCharArray () [i];
			if (nextChar == '>') {
				numSpecials--;
			} else if (nextChar == '<') {
				numSpecials++;
			} else if (nextChar != ' ' || numSpecials > 0) {
				lastArg += actionString.ToCharArray () [i];
			} else if (lastArg.Length > 0) {
				allArgs.Add (lastArg);
				lastArg = "";
			}
		}
		allArgs.Add (lastArg);
		return allArgs;
	}
}
