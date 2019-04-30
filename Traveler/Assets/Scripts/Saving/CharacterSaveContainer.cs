using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class CharacterSaveContainer {
	public List<CharData> actors = new List<CharData>();
	public CharData PublicVars = new CharData();
}