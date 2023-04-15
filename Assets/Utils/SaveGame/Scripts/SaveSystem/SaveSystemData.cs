using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Utils.SaveGame.Scripts.SaveSystem {
	[System.Serializable]
	public class SaveData {
		public string Key { get; }
		public string Value {get;set;}
		
		public SaveData(){}
		public SaveData(string key, string value)
		{
			Key = key;
			Value = value;
		}
	}

	[System.Serializable]
	public class DataState {
		public List<SaveData> items = new List<SaveData>();
		public void AddItem(SaveData item)
		{
			items.Add(item);
		}
	}

	// ReSharper disable once IdentifierTypo
	public abstract class SerializatorBinary
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
}