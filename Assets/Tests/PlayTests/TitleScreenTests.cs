using System.Collections;
using System.Drawing;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Utils.SaveGame.Scripts.SaveSystem;
using Tests.InputControl;

// ReSharper disable UnusedMember.Local

namespace Tests.PlayTests {
	public class TitleScreenTests {
		[UnityTest]
		public IEnumerator Test_TitleScreen_01_SaveData() {
			// variables
			const string testFilePath = "./Assets/Tests/Files/const-party.json";
			const string sceneName = "Title Screen";
			const string findObj = "Title";
			const string randomKey = "reh_seed";
			
			Config.LoadScene(sceneName);
			// Wait for one frame to ensure the scene has loaded
			yield return null;
			// Check that the active scene is "Title Screen"
			Assert.AreEqual(sceneName, SceneManager.GetActiveScene().name, "Should load the Title Screen scene");
			Config.FindObject(findObj);

			// Go from New Game to Start
			// ReSharper disable once IteratorMethodResultIsIgnored
			Mouse.MultipleClicks(new[] {
				new[] {900, 500},
				new[] {1000, 600},
			});

			// Load save data
			DataState dataState = SaveSystem.LoadAndReturn();
			DataState dataState2 = Config.LoadBinary(testFilePath);

			// Assert
			Assert.IsTrue(dataState != null, "should find the 'dataState");
			Assert.IsTrue(dataState2 != null, "should find the 'dataState2");

			// Convert to List, remove the searchKey, and compare
			Config.CompareSaveDataLengthAndContent(dataState, dataState2, randomKey);
		}
		
	}

}
