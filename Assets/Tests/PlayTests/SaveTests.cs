using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using TitleScreen;
using UnityEngine.SceneManagement;
using Utils.SaveGame.Scripts;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class SaveTests {
	GameObject savePrefab;
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
		savePrefab = ((GameObject)Resources.Load("TestsReferences", typeof(GameObject))).GetComponent<TestsReferences>().savePrefab;
	}

	[Test]
	public void _01_PrefabExists() {
		Assert.NotNull(savePrefab);
	}

	[Test]
	[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
	public void _02_PrefabHasRequiredComponents() {
		// Get the components
		SaveSystemSetup save = savePrefab.GetComponent<SaveSystemSetup>();
		Transform transform = savePrefab.GetComponent<Transform>();

		// General
		Assert.IsTrue(savePrefab.name == "Save", "The name is incorrect");
		Assert.IsTrue(savePrefab.CompareTag("Untagged"), "Is not tagged as Untagged");
		
		// Transform
		Assert.IsTrue(transform.position == Vector3.zero, "The position is incorrect");
		Assert.IsTrue(transform.rotation == Quaternion.identity, "The rotation is incorrect");
		Assert.IsTrue(transform.localScale == Vector3.one, "The scale is incorrect");
		
		// SaveSystemSetup
		//Assert.IsTrue(save.. == "save.json", "The save path is incorrect");
		// ...
	}
	
	[Test]
	public void _02_B_PrefabHasRequiredComponent() {
		Assert.IsNotNull(savePrefab.GetComponent<Transform>(), "The Transform component is missing");
		Assert.IsNotNull(savePrefab.GetComponent<SaveSystemSetup>(), "The SaveSystemSetup component is missing");
		Assert.IsTrue(savePrefab.GetComponents<Component>().Length == 2, "There are more than two components on the cim prefab");
	}

	[UnityTest]
	public IEnumerator _03_ExistsInScene() {
#if UNITY_EDITOR
		EditorSceneManager.LoadSceneInPlayMode(menuScenePath, loadSceneParameters);
		yield return null;

		Assert.IsTrue(Object.FindObjectOfType<SaveSystemSetup>().name == "Save", "The Save is not in the scene");
#else
        yield return null;

        Assert.Pass();
#endif
	}
}
