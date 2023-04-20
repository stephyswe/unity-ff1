using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using TitleScreen;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class MenuMusicTests {
	GameObject musicPrefab;
	LoadSceneParameters loadSceneParameters;

#if UNITY_EDITOR
	string menuScenePath;
#endif

	[SetUp]
	public void Setup() {
		GameManager.InitializeTestingEnvironment(true, true, true, false, false);

		loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None);

		Object menuScene = ((GameObject)Resources.Load("TestsReferences")).GetComponent<TestsReferences>().menuScene;

#if UNITY_EDITOR
		menuScenePath = AssetDatabase.GetAssetPath(menuScene);
#endif
		musicPrefab = ((GameObject)Resources.Load("TestsReferences", typeof(GameObject))).GetComponent<TestsReferences>().musicPrefab;
	}

	[Test]
	public void _01_PrefabExists() {
		Assert.NotNull(musicPrefab);
	}

	[Test]
	public void _02_PrefabHasRequiredComponents() {
		// Get the components
		Transform transform = musicPrefab.GetComponent<Transform>();
		
		// General
		Assert.IsTrue(musicPrefab.name == "MenuMusic", "The name is incorrect");
		Assert.IsTrue(musicPrefab.CompareTag("Untagged"), "Is not tagged as Untagged");
		
		// Transform
		Assert.IsTrue(transform.position == Vector3.zero, "The position is incorrect");
		Assert.IsTrue(transform.rotation == Quaternion.identity, "The rotation is incorrect");
		Assert.IsTrue(transform.localScale == Vector3.one, "The scale is incorrect");
	}
	
	[Test]
	public void _02_B_PrefabHasRequiredComponent() {
		Assert.IsNotNull(musicPrefab.GetComponent<Transform>(), "The Transform component is missing");
		Assert.IsTrue(musicPrefab.GetComponents<Component>().Length == 1, "There are more than one component on the music prefab");
	}

	[UnityTest]
	public IEnumerator _03_ExistsInScene() {
#if UNITY_EDITOR
		EditorSceneManager.LoadSceneInPlayMode(menuScenePath, loadSceneParameters);
		yield return null;
		
		var musicHandler = Object.FindObjectOfType<MusicHandler>();
		Assert.IsNotNull(musicHandler, "No instance of MenuMusic was found in the scene");
		Assert.AreEqual("MenuMusic", musicHandler.name, "The MenuMusic object has an incorrect name");

		//TODO: Cannot find Object without specific Component. Find a way to do this.
		Assert.IsTrue(Object.FindObjectOfType<MusicHandler>().name == "MenuMusic", "The Music is not in the scene");
#else
        yield return null;

        Assert.Pass();
#endif
	}
}
