using System.Collections;
using System.Drawing;
using System.Linq;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Utils.SaveGame.Scripts.SaveSystem;
using Tests.InputControl;
using TitleScreen;
using UnityEditor;
using UnityEngine;

// ReSharper disable UnusedMember.Local

namespace Tests.PlayTests {
	public class TitleHandlerTests {
		[UnityTest]
		public IEnumerator _01_SaveData() {
			Setup.Setup_01(out string testFilePath, out string sceneName, out string findObj, out string randomKey, out int[][] positions);
			Config.LoadScene(sceneName);
			yield return null;
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
		

		/*[UnityTest]
		public IEnumerator _02_Quit_TODO() {
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
			Mouse.LeftMouseClick(1200, 600);
			isPlaying = EditorApplication.isPlaying;
			Assert.IsFalse(isPlaying);
		}*/

		[UnityTest]
		public IEnumerator _03_Settings() {
			// variables
			const string sceneName = "Title Screen";
			const string findObj = "Title";
			// Arrange
			Config.LoadScene(sceneName);
			yield return null;
			Assert.AreEqual(sceneName, SceneManager.GetActiveScene().name, "Should load the Title Screen scene");
			Config.FindObject(findObj);
			// Get the loaded scene
			Scene loadedScene = SceneManager.GetActiveScene();

			GameObject settingsObject = null;

			// Get all root game objects in the loaded scene
			GameObject[] rootObjects = loadedScene.GetRootGameObjects();

			// Loop through each root game object and get its children
			foreach (GameObject rootObject in rootObjects) {
				if (rootObject.name != "Settings")
					continue;
				Assert.IsFalse(rootObject.gameObject.activeSelf, "Settings should not be active");

				// Save the Settings object found to the variable
				settingsObject = rootObject;
			}
			Mouse.LeftMouseClick(1200, 500);
			yield return new WaitForSeconds(2);

			// Assert Settings is visible
			Assert.IsTrue(settingsObject != null && settingsObject.gameObject.activeSelf, "Settings should be active");

			yield return new WaitForSeconds(2);
		}

		[UnityTest]
		public IEnumerator _04_ContainerToggle_TODO() {
			// Create a new scene
			Scene testScene = SceneManager.CreateScene("Test Scene");

			// Create the two objects
			GameObject container = new GameObject("Container");
			GameObject title = new GameObject("Title");

			// Add the objects to the scene
			SceneManager.MoveGameObjectToScene(container, testScene);
			SceneManager.MoveGameObjectToScene(title, testScene);

			// Test your code here
			TitleScreenHandler.ContainerToggle(container, title);

			// Assert
			Assert.IsTrue(container.activeSelf);
			Assert.IsFalse(title.activeSelf);

			// Act
			container.SetActive(false);

			// Assert
			Assert.IsFalse(container.activeSelf);
			Assert.IsTrue(title.activeSelf);

			// Unload the test scene
			SceneManager.UnloadSceneAsync(testScene);

			yield return null;
		}
		
		
	}
}
