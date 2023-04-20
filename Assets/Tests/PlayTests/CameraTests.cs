using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using TitleScreen;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class CameraTests {
	GameObject cameraPrefab;
	LoadSceneParameters loadSceneParameters;

#if UNITY_EDITOR
	string menuScenePath;
#endif

	[SetUp]
	public void Setup() {
		GameManager.InitializeTestingEnvironment(true, true, true, false, false);

		loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None);

		Object asteroidsScene = ((GameObject)Resources.Load("TestsReferences")).GetComponent<TestsReferences>().menuScene;

#if UNITY_EDITOR
		menuScenePath = AssetDatabase.GetAssetPath(asteroidsScene);
#endif
		cameraPrefab = ((GameObject)Resources.Load("TestsReferences", typeof(GameObject))).GetComponent<TestsReferences>().cameraPrefab;
	}

	[Test]
	public void _01_CameraPrefabExists() {
		Assert.NotNull(cameraPrefab);
	}

	[Test]
	[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
	public void _02_CameraPrefabHasRequiredComponents() {
		Camera cameraComponent = cameraPrefab.GetComponent<Camera>();

		// Check the name, tag, and clear flag of the Camera component
		Assert.IsTrue(cameraComponent.name == "Camera", "The name is incorrect");
		Assert.IsTrue(cameraComponent.CompareTag("MainCamera"), "The is not tagged as MainCamera");
		Assert.IsTrue(cameraComponent.clearFlags == CameraClearFlags.Skybox, "The clear flags are incorrect");

		// Check the background color and orthographic projection
		Assert.IsTrue(cameraComponent.backgroundColor == Color.white, "The background color is incorrect");
		Assert.IsTrue(cameraComponent.orthographic, "The is not set to orthographic projection");
		Assert.IsTrue(cameraComponent.orthographicSize == 2.2f, "The orthographic size is incorrect");

		// Check the near and far clip planes, depth, and rendering path
		Assert.IsTrue(cameraComponent.nearClipPlane == 0.3f, "The near clip plane is incorrect");
		Assert.IsTrue(cameraComponent.farClipPlane == 1000f, "The far clip plane is incorrect");
		Assert.IsTrue(cameraComponent.depth == -1, "The depth is incorrect");
		Assert.IsTrue(cameraComponent.renderingPath == RenderingPath.UsePlayerSettings, "The rendering path is incorrect");

		// Check the target texture, occlusion culling, HDR, MSAA, dynamic resolution, and render texture properties
		Assert.IsTrue(cameraComponent.targetTexture == null, "The target texture is incorrect");
		Assert.IsTrue(cameraComponent.useOcclusionCulling, "The is not set to use occlusion culling");
		Assert.IsTrue(cameraComponent.allowHDR, "The is not allowing HDR");
		Assert.IsTrue(cameraComponent.allowMSAA, "The is not allowing MSAA");
		Assert.IsFalse(cameraComponent.allowDynamicResolution, "The is allowing dynamic resolution");
		Assert.IsFalse(cameraComponent.forceIntoRenderTexture, "The is forcing into render texture");
		Assert.IsTrue(cameraComponent.targetDisplay == 0, "The target display is incorrect");
		Assert.IsFalse(cameraComponent.usePhysicalProperties, "The use physical properties is incorrect");

		// Check the rect, lens shift, sensor size, and gate fit properties
		Assert.IsTrue(cameraComponent.rect == new Rect(0f, 0f, 1f, 1f), "The rect is incorrect");
		Assert.IsTrue(cameraComponent.lensShift == Vector2.zero, "The lens shift is incorrect");
		Assert.IsTrue(cameraComponent.sensorSize == new Vector2(36f, 24f), "The sensor size is incorrect");
		Assert.IsTrue(cameraComponent.gateFit == Camera.GateFitMode.Horizontal, "The gate fit is incorrect");

		// Check the pixel width, height, and rect properties using a specific display mode
		Assert.IsTrue(cameraComponent.pixelWidth == 1092, "The pixel width is incorrect");
		Assert.IsTrue(cameraComponent.pixelHeight == 516, "The pixel height is incorrect");
		Assert.IsTrue(cameraComponent.pixelRect == new Rect(0f, 0f, 1092f, 516f), "The Camera pixel rect is incorrect");

		Assert.IsTrue(cameraComponent.stereoSeparation == 0.022f, "The stereo separation is incorrect");
		Assert.IsTrue(cameraComponent.stereoConvergence == 10f, "The stereo convergence is incorrect");

		// Check the AudioListener component
		Assert.IsNotNull(cameraPrefab.GetComponent<AudioListener>(), "Does not have an AudioListener component");
		
		// Check the culling mask, layer cull distances, and layer cull Spherical distances properties
		Assert.IsTrue(cameraComponent.cullingMask == -1, "The culling mask is incorrect");
		Assert.IsTrue(cameraComponent.layerCullDistances[0] == 0f, "The layer cull distances are incorrect");
		Assert.IsTrue(cameraComponent.layerCullSpherical == false, "The layer cull spherical is incorrect");
		
		// Check the event mask, event mask 2, and event mask 3 properties
		Assert.IsTrue(cameraComponent.eventMask == -1, "The event mask is incorrect");
		
		// Check the target display, target eye, and stereo target eye properties
		Assert.IsTrue(cameraComponent.targetDisplay == 0, "The target display is incorrect");
		Assert.IsTrue(cameraComponent.stereoTargetEye == StereoTargetEyeMask.Both, "The stereo target eye is incorrect");
	}

	[Test]
	public void _02_B_CameraPrefabHasRequiredComponent() {
		Assert.IsNotNull(cameraPrefab.GetComponent<AudioListener>(), "The AudioListener component is missing");
		Assert.IsNotNull(cameraPrefab.GetComponent<Transform>(), "The Transform component is missing");
		Assert.IsNotNull(cameraPrefab.GetComponent<Camera>(), "The Camera component is missing");

		// Check that the camera prefab only has three components (the three required components)
		Assert.IsTrue(cameraPrefab.GetComponents<Component>().Length == 3, "There are more than three components on the camera prefab");
	}

	[UnityTest]
	public IEnumerator _03_CameraExistsInScene() {
#if UNITY_EDITOR
		EditorSceneManager.LoadSceneInPlayMode(menuScenePath, loadSceneParameters);
		yield return null;

		Assert.IsTrue(Object.FindObjectOfType<Camera>().name == "Camera");
#else
        yield return null;

        Assert.Pass();
#endif

	}
}
