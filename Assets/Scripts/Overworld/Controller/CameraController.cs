using UnityEngine;
using UnityEngine.SceneManagement;

namespace Overworld.Controller {
	public class CameraController : MonoBehaviour {

		public Transform player;
		public Transform reh;

		int last_loaded;
		Camera camera1;
		Camera camera2;

		// Start is called before the first frame update
		void Start() {
			camera2 = GetComponent<Camera>();
			camera1 = GetComponent<Camera>();
		}

		void Awake() {
			transform.position = new Vector3(0, 0, -1f);
		}

		// Update is called once per frame
		void Update() {
			int countLoaded = SceneManager.sceneCount;
			
			// if last 
			if (last_loaded != countLoaded) {
				if (countLoaded == 1) {
					Transform transform1 = transform;
					transform1.parent = player;
					transform1.localPosition = new Vector3(0, 0, -1f);
					camera1.orthographicSize = 12f;
				}
				else {
					Transform transform1 = transform;
					transform1.parent = reh;
					transform1.localPosition = new Vector3(-1f, 0, -1f);
					camera2.orthographicSize = 16f;
				}
			}
			last_loaded = countLoaded;
		}
	}
}
