using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils.SaveGame.Scripts.SaveSystem;

namespace TitleScreen {
	public class SaveGameFabricator : MonoBehaviour {

		public InputField input;

		public Toggle flagToggle;

		bool do_all;
		string field;

		string flag;

		int index;

		// Start is called before the first frame update
		void Start() {
			index = 1;
			input.gameObject.SetActive(false);
		}

		// Update is called once per frame
		void Update() {}

		public void Flag(Text t) {
			flag = t.text;
		}

		public void Toggle() {
			SaveSystem.SetBool(flag, flagToggle.isOn);
		}

		public void player_index(Text t) {

			if (t.text == "All")
				do_all = true;

			int val;

			bool success = int.TryParse(t.text, out val);

			if (success)
				index = val;

			if (field != null)
				input.gameObject.SetActive(true);
		}

		public void s_field(Text t) {
			field = t.text;

			switch (field) {
				case "Class":
					input.contentType = InputField.ContentType.Standard;
					break;
				case "HP":
					input.contentType = InputField.ContentType.IntegerNumber;
					break;
				case "Experience":
					input.contentType = InputField.ContentType.IntegerNumber;
					break;
				case "Strength":
					input.contentType = InputField.ContentType.IntegerNumber;
					break;
				case "Intelligence":
					input.contentType = InputField.ContentType.IntegerNumber;
					break;
				case "Vitality":
					input.contentType = InputField.ContentType.IntegerNumber;
					break;
				case "Agility":
					input.contentType = InputField.ContentType.IntegerNumber;
					break;
				case "Luck":
					input.contentType = InputField.ContentType.IntegerNumber;
					break;
				case "MaxHP":
					input.contentType = InputField.ContentType.IntegerNumber;
					break;
				case "Weapon":
					input.contentType = InputField.ContentType.Standard;
					break;
			}

			input.gameObject.SetActive(true);
		}

		public void save_value(Text t) {

			string type = "";
			if (input.contentType == InputField.ContentType.IntegerNumber)
				type = "int";
			else
				type = "string";

			if (do_all) {
				for (int i = 1; i < 5; i++) {

					Debug.Log(i);

					if (type == "int") {
						int val;

						bool success = int.TryParse(t.text, out val);

						if (success)
							SaveSystem.SetInt("player" + i + "_" + field, val);
					}
					else
						SaveSystem.SetString("player" + i + "_" + field, t.text);
				}

				do_all = false;
			}
			else {
				if (type == "int") {
					int val;

					bool success = int.TryParse(t.text, out val);

					if (success)
						SaveSystem.SetInt("player" + index + "_" + field, val);
				}
				else
					SaveSystem.SetString("player" + index + "_" + field, t.text);
			}

			input.gameObject.SetActive(false);
		}

		public void title_screen() {
			SceneManager.LoadSceneAsync("Title Screen");
			SceneManager.UnloadScene("Save Game Fabricator");
		}

		public void Save() {
			SaveSystem.SaveToDisk();
		}

		public void delete_save_game() {
			File.Delete(Application.persistentDataPath + "/party.bin");
		}

		public void Gold(Text t) {
			int val;

			bool success = int.TryParse(t.text, out val);

			if (success)
				SaveSystem.SetInt("gil", val);
		}
	}
}
