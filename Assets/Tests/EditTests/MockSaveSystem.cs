using System.Collections.Generic;
using TitleScreen;

namespace Tests.EditTests {
	public class MockSaveSystem : ISaveSystem {
		private readonly Dictionary<string, bool> boolValues = new Dictionary<string, bool>();

		public bool GetBool(string key) {
			return boolValues.TryGetValue(key, out bool value) && value;
		}

		public void SetInt(string key, int value) {
			throw new System.NotImplementedException();
		}

		public int GetInt(string key) {
			throw new System.NotImplementedException();
		}

		public void SetFloat(string key, float value) {
			throw new System.NotImplementedException();
		}

		public float GetFloat(string key) {
			throw new System.NotImplementedException();
		}

		public void SetString(string key, string value) {
			throw new System.NotImplementedException();
		}

		public string GetString(string key) {
			throw new System.NotImplementedException();
		}

		public void SetStringList(string key, List<string> value) {
			throw new System.NotImplementedException();
		}

		public List<string> GetStringList(string key) {
			throw new System.NotImplementedException();
		}

		public void SetStringIntDict(string key, Dictionary<string, int> value) {
			throw new System.NotImplementedException();
		}

		public Dictionary<string, int> GetStringIntDict(string key) {
			throw new System.NotImplementedException();
		}

		public void SetStringStringDict(string key, Dictionary<string, string> value) {
			throw new System.NotImplementedException();
		}

		public Dictionary<string, string> GetStringStringDict(string key) {
			throw new System.NotImplementedException();
		}

		public void SetBool(string key, bool value) {
			boolValues[key] = value;
		}

		public void SaveToDisk() {
			// No need to implement in mock version
		}

		public void LoadFromDisk() {
			// No need to implement in mock version
		}
	}

	public interface ISaveSystem {}
}
