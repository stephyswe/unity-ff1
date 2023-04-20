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

public class EventSystemTests {
	GameObject eventSystemPrefab;
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
		eventSystemPrefab = ((GameObject)Resources.Load("TestsReferences", typeof(GameObject))).GetComponent<TestsReferences>().eventSystemPrefab;
	}

	[Test]
	public void _01_EventSystemPrefabExists() {
		Assert.NotNull(eventSystemPrefab);
	}

	[Test]
	[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
	public void _02_EventSystemPrefabHasRequiredComponents() {
		EventSystem eventComponent = eventSystemPrefab.GetComponent<EventSystem>();

		// Get the components
		EventSystem es = eventComponent.GetComponent<EventSystem>();
		Transform transform = eventComponent.GetComponent<Transform>();
		StandaloneInputModule sim = eventComponent.GetComponent<StandaloneInputModule>();
		
		// Unknown
		Assert.IsTrue(eventComponent.currentInputModule == null, "The EventSystem currentInputModule is incorrect");     

		// Check the name, tag, and clear flag of the Camera component
		Assert.IsTrue(eventComponent.name == "EventSystem", "The name is incorrect");
		Assert.IsTrue(eventComponent.CompareTag("Untagged"), "The EventSystem is not tagged as Untagged");
		
		// Transform
		Assert.IsTrue(transform.position == Vector3.zero, "The position is incorrect");
		Assert.IsTrue(transform.rotation == Quaternion.identity, "The rotation is incorrect");
		Assert.IsTrue(transform.localScale == Vector3.one, "The scale is incorrect");
		
		// EventSystem
		Assert.IsTrue(es.currentSelectedGameObject == null, "The currentSelectedGameObject is incorrect");
		Assert.IsTrue(es.sendNavigationEvents, "The sendNavigationEvents is incorrect");
		Assert.IsTrue(es.pixelDragThreshold == 10, "The pixelDragThreshold is incorrect");
		// can you check there is only the above options available like length 
		Assert.IsTrue(es.GetComponents<Component>().Length == 3, "There are more than three components on the camera prefab");

		// StandaloneInputModule
		Assert.IsTrue(eventComponent.currentInputModule == null, "The EventSystem currentInputModule is incorrect");
		Assert.IsTrue(sim.horizontalAxis == "Horizontal", "The horizontalAxis is incorrect");
		Assert.IsTrue(sim.verticalAxis == "Vertical", "The verticalAxis is incorrect");
		Assert.IsTrue(sim.submitButton == "Submit", "The submitButton is incorrect");
		Assert.IsTrue(sim.cancelButton == "Cancel", "The cancelButton is incorrect");
		Assert.IsTrue(sim.inputActionsPerSecond == 10, "The inputActionsPerSecond is incorrect");
		Assert.IsTrue(sim.repeatDelay == 0.5f, "The repeatDelay is incorrect");
		Debug.Log(sim.GetComponents<Component>().Length);
		Assert.IsTrue(sim.GetComponents<Component>().Length == 3, "There are more than three components on the sim prefab");  
	}
	
	[Test]
	public void _02_B_EventSystemPrefabHasRequiredComponent() {
		Assert.IsNotNull(eventSystemPrefab.GetComponent<Transform>(), "The Transform component is missing");
		Assert.IsNotNull(eventSystemPrefab.GetComponent<EventSystem>(), "The EventSystem component is missing");
		Assert.IsNotNull(eventSystemPrefab.GetComponent<StandaloneInputModule>(), "The StandaloneInputModule component is missing");

		// Check that the camera prefab only has three components (the three required components)
		Assert.IsTrue(eventSystemPrefab.GetComponents<Component>().Length == 3, "There are more than three components on the camera prefab");
	}

	[UnityTest]
	public IEnumerator _03_EventSystemExistsInScene() {
#if UNITY_EDITOR
		EditorSceneManager.LoadSceneInPlayMode(menuScenePath, loadSceneParameters);
		yield return null;

		Assert.IsTrue(Object.FindObjectOfType<EventSystem>().name == "EventSystem", "The EventSystem is not in the scene");
#else
        yield return null;

        Assert.Pass();
#endif
	}
}
