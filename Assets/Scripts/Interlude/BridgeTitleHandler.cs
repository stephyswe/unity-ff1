using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Overworld {
	public class BridgeTitleHandler : MonoBehaviour {

		public Sprite[] sprites;
		public string[] strings;

		public SpriteRenderer sr;

		[FormerlySerializedAs("display_text")] public Text displayText;

		// Start is called before the first frame update
		void Start() {
			displayText.text = "";
			StartCoroutine(Process());
		}

		// Update is called once per frame
		void Update() {}

		IEnumerator Process() {
			for (int i = 0; i < 13; i++) {
				sr.sprite = sprites[i];
				yield return new WaitForSeconds(.25f);
			}

			for (int i = 0; i < strings.Length; i++) {
				if (i > 3)
					displayText.alignment = TextAnchor.UpperCenter;
				if (i == 8)
					displayText.alignment = TextAnchor.UpperLeft;

				string s = strings[i];

				displayText.text = s.Replace("<br>", "\n");
				;
				yield return new WaitForSeconds(8f);
			}

			while (!Input.GetKeyDown(CustomInputManager.Cim.Select))
				yield return null;

			displayText.text = "";

			for (int i = 11; i > -1; i--) {
				sr.sprite = sprites[i];
				yield return new WaitForSeconds(.25f);
			}
			sr.enabled = false;

			SceneManager.LoadSceneAsync("Title Screen");
		}
	}
}
