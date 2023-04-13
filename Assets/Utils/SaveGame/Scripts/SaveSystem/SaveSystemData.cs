using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json;

[System.Serializable]
public class SaveData {

	public string Key {get;set;}
	public string Value {get;set;}

	public SaveData(){}

	public SaveData(string key, string value)
	{
		this.Key = key;
		this.Value = value;
	}
}

[System.Serializable]
public class DataState {

	public List<SaveData> items = new List<SaveData>();

	public DataState(){}

	public void AddItem(SaveData item)
	{
		items.Add(item);
	}
}

public class SerializatorBinary
{

    public static void SaveBinary(DataState state, string dataPath)
    {
        string jsonData = JsonConvert.SerializeObject(state);
        File.WriteAllText(dataPath, jsonData);
    }

    public static DataState LoadBinary(string dataPath)
    {
        string jsonData = File.ReadAllText(dataPath);
        DataState state = JsonConvert.DeserializeObject<DataState>(jsonData);


        return state;
    }
}

