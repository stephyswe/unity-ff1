using UnityEngine;
using UnityEngine.UI;

namespace TitleScreen {
	public class BlinkText : MonoBehaviour {
		float counter;
		Text text;

		// Start is called before the first frame update
		void Start() {
			text = GetComponent<Text>();
		}

		// Update is called once per frame
		void Update() {
			counter += Time.deltaTime;
			if (counter < 0.2f)
				return;
			text.enabled = !text.enabled;
			counter = 0;
		}
	}
}
