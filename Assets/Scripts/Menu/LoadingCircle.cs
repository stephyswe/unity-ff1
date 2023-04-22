using UnityEngine;

namespace TitleScreen {
	public class LoadingCircle : MonoBehaviour {
		public GameObject progress;
		public float rotateSpeed = 200f;
		RectTransform rectComponent;
		bool turning;

		void Start() {
			turning = false;
			rectComponent = progress.GetComponent<RectTransform>();
			progress.SetActive(false);
		}

		void Update() {
			if (turning)
				rectComponent.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
		}

		public void start_loading_circle() {
			progress.SetActive(true);
			turning = true;
		}

		public void stop_loading_circle() {
			progress.SetActive(false);
			turning = false;
		}
	}
}
