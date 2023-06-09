using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Utils.SaveGame.Scripts.SaveSystem {
	public static class SaveSystem {
	
		private static string _file;
		private static bool _loaded;
		private static DataState _data;

		public static void Initialize(string fileName) // initialization (used once, after the application starts)
		{
			if(!_loaded)
			{
				_file = fileName;
				if(File.Exists(GetPath())) Load(); else _data = new DataState();
				_loaded = true;
			}
		}

		static string GetPath()
		{
			return Application.persistentDataPath + "/" + _file;
		}

		static void Load()
		{
			_data = SerializatorBinary.LoadBinary(GetPath());
			Debug.Log("[SaveGame] --> Loading the save file: " + GetPath());
		}

		public static DataState LoadAndReturn()
		{
			_data = SerializatorBinary.LoadBinary(GetPath());
			return _data;
		}

		static void ReplaceItem(string name, string item)
		{
			bool j = false;
			for(int i = 0; i < _data.items.Count; i++)
			{
				if(string.Compare(name, _data.items[i].Key) == 0)
				{
					_data.items[i].Value = Crypt(item);
					j = true;
					break;
				}
			}

			if(!j) _data.AddItem(new SaveData(name, Crypt(item)));
		}

		public static bool HasKey(string name) // check for a key
		{
			if(string.IsNullOrEmpty(name)) return false;

			foreach(SaveData k in _data.items)
			{
				if(string.Compare(name, k.Key) == 0)
				{
					return true;
				}
			}

			return false;
		}

		public static void SetStringIntDict(string name, Dictionary<string, int> dict)
		{
			string s = string.Join(";", dict.Select(x => x.Key + "=" + x.Value).ToArray());
			SetString(name, s);
		}

		public static void SetStringList(string name, List<string> strings)
		{
			if (string.IsNullOrEmpty(name)) return;
			SetString(name, string.Join(",", strings.ToArray()));
		}

		public static void SetIntList(string name, List<int> ints)
		{
			if (string.IsNullOrEmpty(name)) return;

			List<string> strings = new List<string>();
			foreach (int i in ints)
				strings.Add("" + i);

			SetStringList(name, strings);
		}

		public static void SetVector3(string name, Vector3 val)
		{
			if(string.IsNullOrEmpty(name)) return;
			SetString(name, val.x + "|" + val.y + "|" + val.z);
		}

		public static void SetVector2(string name, Vector2 val)
		{
			if(string.IsNullOrEmpty(name)) return;
			SetString(name, val.x + "|" + val.y);
		}

		public static void SetColor(string name, Color val)
		{
			if(string.IsNullOrEmpty(name)) return;
			SetString(name, val.r + "|" + val.g + "|" + val.b + "|" + val.a);
		}

		public static void SetBool(string name, bool val) // set the key and value
		{
			if(string.IsNullOrEmpty(name)) return;
			string tmp = string.Empty;
			if(val) tmp = "1"; else tmp = "0";
			ReplaceItem(name, tmp);
		}

		public static void SetFloat(string name, float val)
		{
			if(string.IsNullOrEmpty(name)) return;
			ReplaceItem(name, val.ToString());
		}

		public static void SetInt(string name, int val)
		{
			if(string.IsNullOrEmpty(name)) return;
			ReplaceItem(name, val.ToString());
		}

		public static void SetString(string name, string val)
		{
			if(string.IsNullOrEmpty(name)) return;
			ReplaceItem(name, val);
		}

		public static void SaveToDisk() // write data to file
		{
			if(_data.items.Count == 0) return;
			SerializatorBinary.SaveBinary(_data, GetPath());
			Debug.Log("[SaveGame] --> Save game data: " + GetPath());
		}

		public static Dictionary<string, int> GetStringIntDict(string name)
		{
			if (string.IsNullOrEmpty(name)) return new Dictionary<string, int>();

			string s = GetString(name);

			if(s.Length < 3)
			{
				return new Dictionary<string, int>();
			}

			string[] strings = s.Split(';');

			Dictionary<string, int> output = new Dictionary<string, int>();

			foreach(string str in strings)
			{
				string key = str.Substring(0, str.IndexOf("="));
				string preValue = str.Substring(str.IndexOf("=") + 1, str.Length - str.IndexOf("=") - 1);
				int value = Int32.Parse(preValue);
				output.Add(key, value);
			}

			return output;
		}

		public static List<int> GetIntList(string name)
		{
			if (string.IsNullOrEmpty(name)) return new List<int>();

			string s = GetString(name);
			string[] strings = s.Split(',');

			List<int> output = new List<int>();
			foreach (string str in strings)
				output.Add(Int32.Parse(str));

			return output;
		}

		public static List<string> GetStringList(string name)
		{
			if (string.IsNullOrEmpty(name)) return new List<string>();

			string s = GetString(name);
			string[] strings = s.Split(',');
			return new List<string>(strings);
		}

		public static Vector3 GetVector3(string name)
		{
			if(string.IsNullOrEmpty(name)) return Vector3.zero;
			return IVector3(name, Vector3.zero);
		}

		public static Vector3 GetVector3(string name, Vector3 defaultValue)
		{
			if(string.IsNullOrEmpty(name)) return defaultValue;
			return IVector3(name, defaultValue);
		}

		static Vector3 IVector3(string name, Vector3 defaultValue)
		{
			Vector3 vector = Vector3.zero;

			for(int i = 0; i < _data.items.Count; i++)
			{
				if(string.Compare(name, _data.items[i].Key) == 0)
				{
					string[] t = Crypt(_data.items[i].Value).Split(new char[]{'|'});
					if(t.Length == 3)
					{
						vector.x = FloatParse(t[0]);
						vector.y = FloatParse(t[1]);
						vector.z = FloatParse(t[2]);
						return vector;
					}
					break;
				}
			}

			return defaultValue;
		}

		public static Vector2 GetVector2(string name)
		{
			if(string.IsNullOrEmpty(name)) return Vector2.zero;
			return IVector2(name, Vector2.zero);
		}

		public static Vector2 GetVector2(string name, Vector2 defaultValue)
		{
			if(string.IsNullOrEmpty(name)) return defaultValue;
			return IVector2(name, defaultValue);
		}

		static Vector2 IVector2(string name, Vector2 defaultValue)
		{
			Vector2 vector = Vector2.zero;

			for(int i = 0; i < _data.items.Count; i++)
			{
				if(string.Compare(name, _data.items[i].Key) == 0)
				{
					string[] t = Crypt(_data.items[i].Value).Split(new char[]{'|'});
					if(t.Length == 2)
					{
						vector.x = FloatParse(t[0]);
						vector.y = FloatParse(t[1]);
						return vector;
					}
					break;
				}
			}

			return defaultValue;
		}

		public static Color GetColor(string name)
		{
			if(string.IsNullOrEmpty(name)) return Color.white;
			return IColor(name, Color.white);
		}

		public static Color GetColor(string name, Color defaultValue)
		{
			if(string.IsNullOrEmpty(name)) return defaultValue;
			return IColor(name, defaultValue);
		}

		static Color IColor(string name, Color defaultValue)
		{
			Color color = Color.clear;

			for(int i = 0; i < _data.items.Count; i++)
			{
				if(string.Compare(name, _data.items[i].Key) == 0)
				{
					string[] t = Crypt(_data.items[i].Value).Split(new char[]{'|'});
					if(t.Length == 4)
					{
						color.r = FloatParse(t[0]);
						color.g = FloatParse(t[1]);
						color.b = FloatParse(t[2]);
						color.a = FloatParse(t[3]);
						return color;
					}
					break;
				}
			}

			return defaultValue;
		}

		public static bool GetBool(string name) // get value by key
		{
			if(string.IsNullOrEmpty(name)) return false;
			return IBool(name, false);
		}

		public static bool GetBool(string name, bool defaultValue) // with the default setting
		{
			if(string.IsNullOrEmpty(name)) return defaultValue;
			return IBool(name, defaultValue);
		}

		static bool IBool(string name, bool defaultValue)
		{
			for(int i = 0; i < _data.items.Count; i++)
			{
				if(string.Compare(name, _data.items[i].Key) == 0)
				{
					if(string.Compare(Crypt(_data.items[i].Value), "1") == 0) return true; else return false;
				}
			}

			return defaultValue;
		}

		public static float GetFloat(string name)
		{
			if(string.IsNullOrEmpty(name)) return 0;
			return IFloat(name, 0);
		}

		public static float GetFloat(string name, float defaultValue)
		{
			if(string.IsNullOrEmpty(name)) return defaultValue;
			return IFloat(name, defaultValue);
		}

		static float IFloat(string name, float defaultValue)
		{
			for(int i = 0; i < _data.items.Count; i++)
			{
				if(string.Compare(name, _data.items[i].Key) == 0)
				{
					return FloatParse(Crypt(_data.items[i].Value));
				}
			}

			return defaultValue;
		}

		public static int GetInt(string name)
		{
			if(string.IsNullOrEmpty(name)) return 0;
			return IInt(name, 0);
		}

		public static int GetInt(string name, int defaultValue)
		{
			if(string.IsNullOrEmpty(name)) return defaultValue;
			return IInt(name, defaultValue);
		}

		static int IInt(string name, int defaultValue)
		{
			if (_data != null)
			{
				for (int i = 0; i < _data.items.Count; i++)
				{
					if (string.Compare(name, _data.items[i].Key) == 0)
					{
						return INTParse(Crypt(_data.items[i].Value));
					}
				}
			}

			return defaultValue;
		}

		public static string GetString(string name)
		{
			if(string.IsNullOrEmpty(name)) return string.Empty;
			return IString(name, string.Empty);
		}

		public static string GetString(string name, string defaultValue)
		{
			if(string.IsNullOrEmpty(name)) return defaultValue;
			return IString(name, defaultValue);
		}

		static string IString(string name, string defaultValue)
		{
			for(int i = 0; i < _data.items.Count; i++)
			{
				if(string.Compare(name, _data.items[i].Key) == 0)
				{
					return Crypt(_data.items[i].Value);
				}
			}

			return defaultValue;
		}

		static int INTParse(string val)
		{
			if(int.TryParse(val, out int value)) return value;
			return 0;
		}

		static float FloatParse(string val)
		{
			if(float.TryParse(val, out float value)) return value;
			return 0;
		}

		static string Crypt(string text) {
			return text;
			string result = string.Empty;
			foreach(char j in text) result += (char)((int)j ^ 42);
			return result;
		}
	}
}
