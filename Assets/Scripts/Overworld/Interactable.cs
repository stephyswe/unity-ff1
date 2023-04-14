using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Overworld {
	public class Interactable : MonoBehaviour {

		public string dialogue;

		[FormerlySerializedAs("text_box")] public GameObject textBox;

		GameObject tb_instance;

		// Start is called before the first frame update
		void Start() {}

		// Update is called once per frame
		void Update() {}

		public void display_textbox(Vector3 pos) {
			tb_instance = Instantiate(textBox, pos, Quaternion.identity);
			if (dialogue.Length >= 91)
				tb_instance.GetComponentInChildren<Text>().fontSize = 26;
			tb_instance.GetComponentInChildren<Text>().text = dialogue;
			tb_instance.SetActive(true);
		}

		public void hide_textbox() {
			Destroy(tb_instance);
		}
	}
}
