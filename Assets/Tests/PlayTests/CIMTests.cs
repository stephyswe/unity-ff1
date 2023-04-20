using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using TitleScreen;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class CIMTests {
	GameObject cimPrefab;
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
		cimPrefab = ((GameObject)Resources.Load("TestsReferences", typeof(GameObject))).GetComponent<TestsReferences>().cimPrefab;
	}

	[Test]
	public void _01_PrefabExists() {
		Assert.NotNull(cimPrefab);
	}

	[Test]
	[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
	public void _02_PrefabHasRequiredComponents() {
		CustomInputManager cimComponent = cimPrefab.GetComponent<CustomInputManager>();

		// Get the components
		Transform transform = cimComponent.GetComponent<Transform>();
		CustomInputManager cim = cimComponent.GetComponent<CustomInputManager>();
		
		// General
		Assert.IsTrue(cimPrefab.name == "CIM", "The name is incorrect");
		Assert.IsTrue(cimPrefab.CompareTag("Untagged"), "Is not tagged as Untagged");
		
		// Transform
		Assert.IsTrue(transform.position == Vector3.zero, "The position is incorrect");
		Assert.IsTrue(transform.rotation == Quaternion.identity, "The rotation is incorrect");
		Assert.IsTrue(transform.localScale == Vector3.one, "The scale is incorrect");
		
		// CustomInputManager
		// ...
	}
	
	[Test]
	public void _02_B_PrefabHasRequiredComponent() {
		Assert.IsNotNull(cimPrefab.GetComponent<Transform>(), "The Transform component is missing");
		Assert.IsNotNull(cimPrefab.GetComponent<CustomInputManager>(), "The CustomInputManager component is missing");
		Assert.IsTrue(cimPrefab.GetComponents<Component>().Length == 2, "There are more than two components on the cim prefab");
	}

	[UnityTest]
	public IEnumerator _03_ExistsInScene() {
#if UNITY_EDITOR
		EditorSceneManager.LoadSceneInPlayMode(menuScenePath, loadSceneParameters);
		yield return null;

		Assert.IsTrue(Object.FindObjectOfType<CustomInputManager>().name == "CIM", "The CIM is not in the scene");
#else
        yield return null;

        Assert.Pass();
#endif
	}
}
