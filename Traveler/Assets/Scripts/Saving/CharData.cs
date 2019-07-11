using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<TKey> keys = new List<TKey>();

	[SerializeField]
	private List<TValue> values = new List<TValue>();

	// save the dictionary to lists
	public void OnBeforeSerialize()
	{
		keys.Clear();
		values.Clear();
		foreach(KeyValuePair<TKey, TValue> pair in this)
		{
			keys.Add(pair.Key);
			values.Add(pair.Value);
		}
	}

	// load dictionary from lists
	public void OnAfterDeserialize()
	{
		this.Clear();

		if(keys.Count != values.Count)
			throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

		for(int i = 0; i < keys.Count; i++)
			this.Add(keys[i], values[i]);
	}
}

[Serializable]
public class StringDictionary : SerializableDictionary<string, string> { }

[Serializable]
public class CharData {

	public string regID = "Not Assigned";
	public string prefabPath;
	public string name;
	public Vector3 pos;
	public float zRot;

	[Serializable] public class DictionaryOfStringAndInt : SerializableDictionary<string, int> {}
	public DictionaryOfStringAndInt PersistentInt;

	[Serializable] public class DictionaryOfStringAndFloat : SerializableDictionary<string, float> {}
    public DictionaryOfStringAndFloat PersistentFloats;

	[Serializable] public class DictionaryOfStringAndString : SerializableDictionary<string, string> {}
    public DictionaryOfStringAndString PersistentStrings;

	[Serializable] public class DictionaryOfStringAndBool : SerializableDictionary<string, bool> {}
    public DictionaryOfStringAndBool PersistentBools;

	public RoomDirection targetDir;
	public string targetID;

    public void SetInt(string key, int value) { PersistentInt[key] = value; }
    public int GetInt(string key, int defaultValue = 0)
    {
        if (PersistentInt.ContainsKey(key))
            return PersistentInt[key];
        else
            return defaultValue;
    }

    public void SetFloat(string key, float value) { PersistentFloats[key] = value; }
    public float GetFloat(string key, float defaultValue = 0f)
    {
        if (PersistentFloats.ContainsKey(key))
            return PersistentFloats[key];
        else
            return defaultValue;
    }

    public void SetString(string key, string value) { PersistentStrings [key] = value; }
    public string GetString(string key, string defaultValue = "")
    {
        if (PersistentStrings.ContainsKey(key))
            return PersistentStrings[key];
        else
            return defaultValue;
    }

    public void SetBool(string key, bool value) { PersistentBools[key] = value; }
    public bool GetBool(string key, bool defaultValue = false)
    {
        if (PersistentBools.ContainsKey(key))
            return PersistentBools[key];
        else
            return defaultValue;
    }
    public void ClearString(string key)
    {
        if (PersistentStrings.ContainsKey(key))
            PersistentStrings.Remove(key);
    }
}