using UnityEngine;
using UnityEngine.UI;

namespace TitleScreen {
	public class BlinkText : MonoBehaviour {

		int counter;

		Text text;

		// Start is called before the first frame update
		void Start() {
			text = GetComponent<Text>();
		}

		// Update is called once per frame
		void Update() {
			counter += 1;

			if (counter == 30) {
				text.enabled = !text.enabled;
				counter = 0;
			}
		}
	}
}
