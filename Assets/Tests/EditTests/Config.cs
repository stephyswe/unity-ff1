using UnityEditor.SceneManagement;

namespace Tests.EditTests {
	public abstract class Config {
		public static void LoadEditScene(string sceneName = null) {
			string path = "Assets/Scenes/" + sceneName + ".unity";
			EditorSceneManager.OpenScene(path);
		}
	}
}
