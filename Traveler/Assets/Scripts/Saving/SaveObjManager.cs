using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveObjManager : MonoBehaviour{
	private static SaveObjManager m_instance;

	static SceneTrigger [] sceneTriggers;
	string curRoom;
	public static string saveBase = "SaveData/";
	static string savePath = "SaveData/AutoSave/";
	static string saveFolder = "AutoSave/";
	public static SaveProfileContainer CurrentSaveInfo = new SaveProfileContainer();
	List<string> registeredPermItems;

	public bool SetDirectory(string directory) {
		if (IsAlphaNum (directory) && directory.Length < 16) {
			saveFolder = directory + "/";
			savePath = saveBase + saveFolder;
			if (!Directory.Exists (savePath)) {
				Directory.CreateDirectory (savePath);
			}
			return true;
		}
		return false;
	}
	public bool DeleteProfile(string profileName) {
		string p = saveBase + profileName;
		if (!Directory.Exists (p))
			return false;
		foreach (string file in Directory.GetFiles(p)) {
			//File.Copy (file, savePath + file);
			file.Substring (p.Length);
			File.Delete (file); 
		}
		Directory.Delete (p);
		return true;
	}
	public bool ProfileExists(string profileName) {
		string f = profileName + "/";
		string p = saveBase + f;
		return Directory.Exists (p);
	}
	public bool LoadProfile(string profileName) {
		string f = profileName + "/";
		string p = saveBase + f;
		if (!Directory.Exists (p)) {
			return false;
		}
		if (profileName + "/" != saveFolder) {
			foreach (string file in Directory.GetFiles(savePath)) {
				File.Delete (file);
			}
			foreach (string file in Directory.GetFiles(saveBase + profileName)) {
				//File.Copy (file, savePath + file);
				string s = file.Substring (p.Length);
				File.Copy (file, savePath + s); 
				//Debug.Log ("Copying: " + file + " to " + savePath + s);
			}
		}
		string json = File.ReadAllText(savePath+ "base.txt");
		CurrentSaveInfo = JsonUtility.FromJson<SaveProfileContainer> (json);
		registeredPermItems.Clear ();
		registeredPermItems = CurrentSaveInfo.RegisteredIDs;			
		SceneManager.LoadScene (CurrentSaveInfo.LastRoomName, LoadSceneMode.Single);
		return true;
	}
	public bool SaveProfile(string profileName) {
		string f = profileName + "/";
		string p = saveBase + f;
		if (!Directory.Exists (p)) {
			Directory.CreateDirectory (p);
		}
		refreshPersItems ();
		ResaveRoom ();
		foreach (string file in Directory.GetFiles(p))
		{
			File.Delete (file);
		}

		string name = SceneManager.GetActiveScene().name;
		foreach (string file in Directory.GetFiles(savePath))
		{
			string s = file.Substring(savePath.Length);
			File.Copy (file, p + s); 
		}
		return true;
	}
	public static SaveObjManager Instance
	{
		get { return m_instance; }
		set { m_instance = value; }
	}

	public static CharData PublicVars()
	{
		return charContainer.PublicVars;
	}
	
	bool IsAlphaNum(string str)
	{
		if (string.IsNullOrEmpty(str))
			return false;

		for (int i = 0; i < str.Length; i++)
		{
			if (!(char.IsLetter(str[i])) && (!(char.IsNumber(str[i]))))
				return false;
		}
		return true;
	}

	void Awake()
	{
		if (m_instance == null) {
			m_instance = this;
		} else if (m_instance != this) {
			Destroy (gameObject);
			return;
		}
		registeredPermItems = new List<string> ();
		SceneManager.sceneLoaded += onRoomLoad;
		curRoom = SceneManager.GetActiveScene ().name;
	}
	public void saveCurrentRoom() {}
	public void resetRoomData() {
		foreach (string file in Directory.GetFiles(savePath))
		{
			File.Delete(file);      
		}
		registeredPermItems = new List<string> ();
	}
	public List<string> loadRegisteredIDs() {
		List<string> ids = new List<string> ();
		foreach (string file in Directory.GetFiles(savePath))
		{
			//File.Delete(file);      
		}
		return ids;
	}
	public void onRoomLoad(Scene scene, LoadSceneMode mode) {
		//curRoomInfo = getRoom(roomName);
		curRoom = SceneManager.GetActiveScene ().name;;
		sceneTriggers = GameObject.FindObjectsOfType<SceneTrigger> ();
		recreateItems (curRoom);
		//GameManager.Instance.FindPlayer ();
		ResaveRoom ();
	}

	public void recreateItems(string RoomName) {
		LoadRoom (savePath + RoomName);
	}

	public static void MoveItem(GameObject go,string newRoom, Vector3 newPos) {
		Instance.m_moveItem (go,newRoom,newPos);
	}
	void m_moveItem(GameObject go,string newRoom, Vector3 newPos) {
		PersistentItem item = go.GetComponent<PersistentItem> ();
		item.StoreData ();
		refreshPersItems();
		DelCharData (item.data);
		CharacterSaveContainer cc = LoadChars(savePath + newRoom);
//		item.pos = newPos;
		JsonUtility.ToJson(new CharacterSaveContainer());
		cc.actors.Add (item.data);
		Save (savePath + newRoom, cc);
		ResaveRoom ();
	}

	public static void MoveItem(GameObject gm,string newRoom, string newID,RoomDirection newDir) {
		Instance.m_moveItem (gm, newRoom, newID, newDir);
	}

	void m_moveItem(GameObject go,string newRoom, string newID,RoomDirection newDir) {
		//Debug.Log ("Moving " + go.name + " to " + newRoom + " at position: " + newID + " direction: " + newDir);
		PersistentItem item = go.GetComponent<PersistentItem> ();
		//Remove the current data.
		item.StoreData ();
		refreshPersItems();
		DelCharData (item.data);

		//load the list of things in the new room and add the current character to it
		CharacterSaveContainer cc = LoadChars(savePath + newRoom);
		item.data.targetID = newID;
		item.data.targetDir = newDir;
		cc.actors.Add (item.data);
		//Debug.Log ("What is this anyways?: " + item.data.targetID + " : " + item.data.targetDir);
		Save (savePath+newRoom, cc);
		ResaveRoom ();
		LoadChars (savePath + curRoom);
	}

	public static bool CheckRegistered(GameObject go) {
		return m_instance.m_checkRegister (go);
	}


	public string GenerateID (GameObject go, string prefabName) {
		string xID = (Mathf.CeilToInt(go.GetComponent<PersistentItem>().StartPosition.x/1f)*1).ToString ();
		string yID = (Mathf.CeilToInt(go.GetComponent<PersistentItem>().StartPosition.y/1f)*1).ToString ();
		string id = "";
		foreach (char c in go.name) {
			if (!c.Equals ('(') && !c.Equals(' ')) {
				id += c;
			} else {
				break;
			}
		}
		string pref = (prefabName == "") ? prefabName : go.name;
		if (id != pref) {
			id = pref + "-" + SceneManager.GetActiveScene ().name + "-" + go.name;
		} else {
			id += "-" + SceneManager.GetActiveScene ().name + xID + "_" + yID;
		}
		return id;
	}
	bool m_checkRegister(GameObject go) {
		PersistentItem c = go.GetComponent<PersistentItem> ();
		string id = GenerateID (go, c.data.prefabPath);
		//Debug.Log ("Checking if character: " + c + " registered id is: " + c.data.regID);
		//Debug.Log ("incoming ID: " + c.data.regID);
		if (c.data.regID != "Not Assigned") {
			id = c.data.regID;
		}
		if (registeredPermItems.Contains(id) ){
			if (c.recreated) {
				//Debug.Log ("Recreated entity: " + c.data.regID);
				c.recreated = false;
				return false;
			} else {
				//Debug.Log ("already registered, removing");
				return true;
			}
		}
		//Debug.Log ("new entity. " + id + " Adding to registry");
		registeredPermItems.Add(id);
		c.data.regID = id;
		//Debug.Log ("saved ID is: " + c.data.regID);
		//Debug.Log ("Length of registry is: " + registeredPermItems.Count);
		return false;
	}

	public void refreshPersItems() {
		PersistentItem [] cList = Object.FindObjectsOfType<PersistentItem>();
		charContainer.actors.Clear ();
		foreach (PersistentItem c in cList) {
			c.StoreData ();
			charContainer.actors.Add(c.data);
		}
	}
	public void ResaveRoom() {
		Save (savePath + curRoom, charContainer);
		if (File.Exists (savePath + "base.txt"))
			File.Delete (savePath + "base.txt");
		CurrentSaveInfo.LastRoomName = curRoom;
		CurrentSaveInfo.RegisteredIDs = registeredPermItems;
		string json = JsonUtility.ToJson(CurrentSaveInfo);
		StreamWriter sw = File.CreateText(savePath + "base.txt");
		sw.Close();
		File.WriteAllText(savePath + "base.txt", json);
	}
	//-----------------------------------------------------------

	public static CharacterSaveContainer charContainer = new CharacterSaveContainer();

	public delegate void SerializeAction();
	public static event SerializeAction OnLoaded;
	public static event SerializeAction OnBeforeSave;

	//public const string playerPath = "Prefabs/Player";

	//Loading---------------
	public static void LoadRoom(string path) {		
		charContainer = LoadChars(path);	
		//Debug.Log ("items to recreate: " + charContainer.actors.Count + " from: " + path);
		foreach (CharData data in charContainer.actors) {
			RecreatePersistentItem (data, data.prefabPath,
				data.pos, Quaternion.identity);
			//PersistentItem pi = RecreatePersistentItem
			//pi.registryCheck ();
		}
		//OnLoaded();
		//ClearActorList();
	}
	public static void ClearActorList() {
		charContainer.actors.Clear();
	}
	private static CharacterSaveContainer LoadChars(string path) {
		if (File.Exists(path+ ".txt"))
		{
			string json = File.ReadAllText(path+ ".txt");
			//Debug.Log ("Chars from path: " + path + " : " + json);
			return JsonUtility.FromJson<CharacterSaveContainer>(json);
		} else {
			//Debug.Log("no save data found, creating new file");
			CharacterSaveContainer cc = new CharacterSaveContainer();
			SaveActors(path,cc);
			return cc;
		} 

	}
	public static PersistentItem RecreatePersistentItem(string path, Vector3 position, Quaternion rotation) {
		
		GameObject prefab = Resources.Load<GameObject>(path);
		if (prefab == null) {
			Debug.Log ("ERROR: Could not re-instantiate object: " + path);
			return null;
		}
		//Debug.Log (prefab);
		GameObject go = GameObject.Instantiate(prefab, position, rotation) as GameObject;
		PersistentItem actor = go.GetComponent<PersistentItem>() ?? go.AddComponent<PersistentItem>();
		actor.recreated = true;
		return actor;
	}
	public static PersistentItem RecreatePersistentItem(CharData data, string path, Vector3 position, Quaternion rotation) {
		PersistentItem actor = null;
		if (data.targetID != null) {
			Vector3 nv = data.pos;
			bool found = false;
			foreach (SceneTrigger rm in sceneTriggers) {
				//Debug.Log ("COMPARE: " + rm.TriggerID + " : " + data.targetID);
				if (rm.TriggerID == data.targetID) {
					if (data.targetDir == RoomDirection.LEFT) {
							nv = rm.transform.position - new Vector3 (rm.GetComponent<BoxCollider2D> ().size.x + 3f, 0f);
					} else if (data.targetDir == RoomDirection.RIGHT) {
							nv = rm.transform.position + new Vector3 (rm.GetComponent<BoxCollider2D> ().size.x + 3f, 0f);
					} else if (data.targetDir == RoomDirection.UP) {
							nv = rm.transform.position + new Vector3 (0f, rm.GetComponent<BoxCollider2D> ().size.y + 3f, 0f);
					} else if (data.targetDir == RoomDirection.DOWN) {
							nv = rm.transform.position - new Vector3 (0f, rm.GetComponent<BoxCollider2D> ().size.x + 3f, 0f);
					}
					found = true;
					break;
				}
			}
			if (found) {
				actor = RecreatePersistentItem(path, nv, rotation);
			} else {
				actor = RecreatePersistentItem(path, nv, rotation);
			}
		} else {
			actor = RecreatePersistentItem(path, data.pos, rotation);
		}
		actor.data = data;
		return actor;
	}
	public static void AddCharData(CharData data) {
		//Debug.Log ("Adding character");
		charContainer.actors.Add(data);
	}
	public static void DelCharData(CharData data) {
		charContainer.actors.Remove (data);
		////Debug.Log (charContainer.actors.Count);
	}

	//Saving --------------------
	public static void Save(string path, CharacterSaveContainer actors) {
		SaveActors(path, actors);
	}

	private static void SaveActors(string path, CharacterSaveContainer actors) {
		string json = JsonUtility.ToJson(actors);
		//Debug.Log ("jsoN: " + json);
		//Debug.Log ("save to path: " + path+ ".txt");
		//Debug.Log("Saving: " + json.ToString() + " to path: " + path);
		StreamWriter sw = File.CreateText(path + ".txt");
		sw.Close();
		File.WriteAllText(path+ ".txt", json);
	}
}