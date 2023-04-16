using System.Collections;
using System.Drawing;
using System.Linq;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Utils.SaveGame.Scripts.SaveSystem;
using Tests.InputControl;
using UnityEditor;
using UnityEngine;

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
			int[][] positions = {
				new[] {900, 500},
				new[] {1100, 600},
			};
			
			Config.LoadScene(sceneName);
			// Wait for one frame to ensure the scene has loaded
			yield return null;
			// Check that the active scene is "Title Screen"
			Assert.AreEqual(sceneName, SceneManager.GetActiveScene().name, "Should load the Title Screen scene");
			Config.FindObject(findObj);

			// Go from New Game to Start
			yield return Mouse.MultipleClicks(positions);
			
			// Load save data
			DataState dataState = SaveSystem.LoadAndReturn();
			DataState dataState2 = Config.LoadBinary(testFilePath);

			// Assert
			Assert.IsTrue(dataState != null, "should find the 'dataState");
			Assert.IsTrue(dataState2 != null, "should find the 'dataState2");

			// Convert to List, remove the searchKey, and compare
			Config.CompareSaveDataLengthAndContent(dataState, dataState2, randomKey);
		}
		
		[UnityTest]
		public IEnumerator Test_TitleScreen_02_Quit() {
			// variables
			const string sceneName = "Title Screen";
			const string findObj = "Title";
			// Arrange
			bool isPlaying = true;
			Config.LoadScene(sceneName);
			yield return null;
			Assert.AreEqual(sceneName, SceneManager.GetActiveScene().name, "Should load the Title Screen scene");
			Config.FindObject(findObj);
			yield return new WaitForSeconds(2);
			Mouse.LeftMouseClick(1200,600);
			isPlaying = EditorApplication.isPlaying;
			Assert.IsFalse(isPlaying);
		}
		
		[UnityTest]
		public IEnumerator Test_TitleScreen_03_Settings() {
			// variables
			const string sceneName = "Title Screen";
			const string findObj = "Title";
			// Arrange
			bool isPlaying = true;
			Config.LoadScene(sceneName);
			yield return null;
			Assert.AreEqual(sceneName, SceneManager.GetActiveScene().name, "Should load the Title Screen scene");
			Config.FindObject(findObj);
			
			// Before Setting is Loaded
			const string findObjSettings = "Settings";
			GameObject objSettings = GameObject.Find(findObjSettings);
			
			// Assert objSetting isnt visible
			Assert.IsTrue(objSettings.activeSelf);
			
			yield return new WaitForSeconds(2);
			Mouse.LeftMouseClick(1200,500);
			yield return new WaitForSeconds(2);
		}
	}
}
