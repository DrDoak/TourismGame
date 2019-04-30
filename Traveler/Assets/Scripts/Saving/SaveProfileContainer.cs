using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SaveProfileContainer {
	public string LastRoomName = "LB_BottomPoint";
	public int Level = 1;
	public int NextLevel = 200;
	public List<string> RegisteredIDs = new List<string>();
}