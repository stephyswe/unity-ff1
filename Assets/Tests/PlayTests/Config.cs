using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Utils.SaveGame.Scripts.SaveSystem;
using Assert = NUnit.Framework.Assert;

namespace Tests.PlayTests {
	public abstract class Config {
		// ReSharper disable once UnusedMethodReturnValue.Local
		public static GameObject FindObject(string objName) {
			GameObject gameObj = GameObject.Find(objName);
			Assert.IsTrue(gameObj != null, "should find the "+objName+" object in the scene");
			return gameObj;
		}

		public static void LoadScene(string sceneName = null) {
			string path = "Assets/Scenes/" + sceneName + ".unity";
			int buildIndex = SceneUtility.GetBuildIndexByScenePath(path);
			if (buildIndex == -1) {
				throw new Exception($"Failed to load scene {sceneName}. Scene not found in build settings.");
			}
			SceneManager.LoadScene(buildIndex, LoadSceneMode.Single);
			/*if (SceneManager.GetActiveScene().name != sceneName) {
				throw new Exception($"Failed to load scene {sceneName}. Loaded scene name is {SceneManager.GetActiveScene().name}.");
			}*/
		}
		
		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
		public static void CompareSaveDataLengthAndContent(DataState dataState,  DataState dataState2, string searchKey) {
			IEnumerable<KeyValuePair<string, string>> actual = ListRemoveOneKey(dataState, searchKey);
			IEnumerable<KeyValuePair<string, string>> expected = ListRemoveOneKey(dataState2, searchKey);

			// Assert
			Assert.AreEqual(expected.Count(), actual.Count(), "The lists should have the same length");
			Assert.IsTrue(expected.SequenceEqual(actual, new KeyValuePairComparer()), "The lists should be equal");
		}
		static IEnumerable<KeyValuePair<string, string>> ListRemoveOneKey(DataState dataState, string searchKey) {
			List<KeyValuePair<string, string>> list = dataState.items
				.Where(item => item.Key != searchKey)
				.Select(item => new KeyValuePair<string, string>(item.Key, item.Value))
				.ToList();
			return list;
		}
		class KeyValuePairComparer : IEqualityComparer<KeyValuePair<string, string>> {
			public bool Equals(KeyValuePair<string, string> x, KeyValuePair<string, string> y) {
				return x.Key == y.Key && x.Value == y.Value;
			}

			public int GetHashCode(KeyValuePair<string, string> obj) {
				return obj.Key.GetHashCode() ^ obj.Value.GetHashCode();
			}
		}
		
		public static void CompareKeyValuePairArrays(IEnumerable<KeyValuePair<string, string>> expected, IEnumerable<KeyValuePair<string, string>> actual) {
			Debug.Log(expected.SequenceEqual(actual) ? "The arrays are equal." : "The arrays are not equal.");
		}
		public static DataState LoadBinary(string dataPath) {
			string jsonData = File.ReadAllText(dataPath);
			return JsonConvert.DeserializeObject<DataState>(jsonData);
		}
	}
}
