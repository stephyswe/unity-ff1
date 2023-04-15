using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Utils.SaveGame.Scripts.SaveSystem {

	[Serializable]
	public abstract class SaveSystem {

		static string _file;
		static bool _loaded;
		static DataState _data;

		public static void Initialize(string fileName) // initialization (used once, after the application starts)
		{
			if (_loaded)
				return;
			_file = fileName;
			if(File.Exists(GetPath())) Load(); else _data = new DataState();
			_loaded = true;
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

		static void ReplaceItem(string name, string item)
		{
			bool j = false;
			foreach (SaveData t in _data.items.Where(t => string.CompareOrdinal(name, t.Key) == 0)) {
				t.Value = Crypt(item);
				j = true;
				break;
			}

			if(!j) _data.AddItem(new SaveData(name, Crypt(item)));
		}

		public static bool HasKey(string name) // check for a key
		{
			return !string.IsNullOrEmpty(name) && _data.items.Any(k => string.CompareOrdinal(name, k.Key) == 0);
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

		public static void SetIntList(string name, IEnumerable<int> ints)
		{
			if (string.IsNullOrEmpty(name)) return;
			List<string> strings = ints.Select(i => "" + i).ToList();
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
			ReplaceItem(name, val ? "1" : "0");
		}

		public static void SetFloat(string name, float val)
		{
			if(string.IsNullOrEmpty(name)) return;
			ReplaceItem(name, val.ToString(CultureInfo.InvariantCulture));
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
				string key = str[..str.IndexOf("=", StringComparison.Ordinal)];
				string preValue = str.Substring(str.IndexOf("=", StringComparison.Ordinal) + 1, str.Length - str.IndexOf("=", StringComparison.Ordinal) - 1);
				int value = int.Parse(preValue);
				output.Add(key, value);
			}
			return output;
		}

		public static List<int> GetIntList(string name)
		{
			if (string.IsNullOrEmpty(name)) return new List<int>();
			string s = GetString(name);
			string[] strings = s.Split(',');
			return strings.Select(int.Parse).ToList();
		}

		public static List<string> GetStringList(string name)
		{
			if (string.IsNullOrEmpty(name)) return new List<string>();

			string s = GetString(name);
			string[] strings = s.Split(',');
			return new List<string>(strings);
		}

		public static Vector3 GetVector3(string name) {
			return string.IsNullOrEmpty(name) ? UnityEngine.Vector3.zero : Vector3(name, UnityEngine.Vector3.zero);
		}

		public static Vector3 GetVector3(string name, Vector3 defaultValue) {
			return string.IsNullOrEmpty(name) ? defaultValue : Vector3(name, defaultValue);
		}

		static Vector3 Vector3(string name, Vector3 defaultValue)
		{
			Vector3 vector = UnityEngine.Vector3.zero;

			foreach (string[] t in from t1 in _data.items where string.CompareOrdinal(name, t1.Key) == 0 select Crypt(t1.Value).Split(new char[]{'|'})) {
				if(t.Length == 3)
				{
					vector.x = FloatParse(t[0]);
					vector.y = FloatParse(t[1]);
					vector.z = FloatParse(t[2]);
					return vector;
				}
				break;
			}

			return defaultValue;
		}

		public static Vector2 GetVector2(string name) {
			return string.IsNullOrEmpty(name) ? UnityEngine.Vector2.zero : Vector2(name, UnityEngine.Vector2.zero);
		}

		public static Vector2 GetVector2(string name, Vector2 defaultValue) {
			return string.IsNullOrEmpty(name) ? defaultValue : Vector2(name, defaultValue);
		}

		static Vector2 Vector2(string name, Vector2 defaultValue)
		{
			Vector2 vector = UnityEngine.Vector2.zero;

			foreach (string[] t in from t1 in _data.items where string.CompareOrdinal(name, t1.Key) == 0 select Crypt(t1.Value).Split(new char[]{'|'})) {
				if(t.Length == 2)
				{
					vector.x = FloatParse(t[0]);
					vector.y = FloatParse(t[1]);
					return vector;
				}
				break;
			}

			return defaultValue;
		}

		public static Color GetColor(string name) {
			return string.IsNullOrEmpty(name) ? UnityEngine.Color.white : Color(name, UnityEngine.Color.white);
		}

		public static Color GetColor(string name, Color defaultValue) {
			return string.IsNullOrEmpty(name) ? defaultValue : Color(name, defaultValue);
		}

		static Color Color(string name, Color defaultValue)
		{
			Color color = UnityEngine.Color.clear;
			foreach (string[] t in from t1 in _data.items where string.CompareOrdinal(name, t1.Key) == 0 select Crypt(t1.Value).Split(new char[]{'|'})) {
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
			return defaultValue;
		}

		public static bool GetBool(string name) // get value by key
		{
			return !string.IsNullOrEmpty(name) && Bool(name, false);
		}

		public bool GetBool(string name, bool defaultValue) // with the default setting
		{
			return string.IsNullOrEmpty(name) ? defaultValue : Bool(name, defaultValue);
		}

		static bool Bool(string name, bool defaultValue) {
			foreach (SaveData t in _data.items.Where(t => string.CompareOrdinal(name, t.Key) == 0)) {
				return string.CompareOrdinal(Crypt(t.Value), "1") == 0;
			}
			return defaultValue;
		}

		public static float GetFloat(string name)
		{
			if(string.IsNullOrEmpty(name)) return 0;
			return Float(name, 0);
		}

		public static float GetFloat(string name, float defaultValue)
		{
			if(string.IsNullOrEmpty(name)) return defaultValue;
			return Float(name, defaultValue);
		}

		static float Float(string name, float defaultValue) {
			foreach (SaveData t in _data.items.Where(t => string.CompareOrdinal(name, t.Key) == 0)) {
				return FloatParse(Crypt(t.Value));
			}

			return defaultValue;
		}

		public static int GetInt(string name) {
			return string.IsNullOrEmpty(name) ? 0 : Int(name, 0);
		}

		public static int GetInt(string name, int defaultValue) {
			return string.IsNullOrEmpty(name) ? defaultValue : Int(name, defaultValue);
		}

		static int Int(string name, int defaultValue)
		{
			if (_data == null)
				return defaultValue;
			foreach (SaveData t in _data.items.Where(t => string.CompareOrdinal(name, t.Key) == 0)) {
				return INTParse(Crypt(t.Value));
			}
			return defaultValue;
		}

		public static string GetString(string name) {
			return string.IsNullOrEmpty(name) ? string.Empty : String(name, string.Empty);
		}

		public static string GetString(string name, string defaultValue) {
			return string.IsNullOrEmpty(name) ? defaultValue : String(name, defaultValue);
		}

		static string String(string name, string defaultValue) {
			foreach (SaveData t in _data.items.Where(t => string.CompareOrdinal(name, t.Key) == 0)) {
				return Crypt(t.Value);
			}
			return defaultValue;
		}

		static int INTParse(string val) {
			return int.TryParse(val, out int value) ? value : 0;
		}

		static float FloatParse(string val)
		{
			if(float.TryParse(val, out float value)) return value;
			return 0;
		}

		static string Crypt(string text) {
			return text.Aggregate(string.Empty, (current, j) => current + (char)((int)j ^ 42));
		}
	}
}
