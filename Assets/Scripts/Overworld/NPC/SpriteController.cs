using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.SaveGame.Scripts.SaveSystem;

namespace Overworld {
	public class SpriteController : MonoBehaviour {

		public SpriteGroup[] characters;

		[FormerlySerializedAs("character_index")]
		public int characterIndex;
		[FormerlySerializedAs("active_character")]
		public SpriteGroup activeCharacter;

		public SpriteRenderer sr;

		[FormerlySerializedAs("title_screen_mode")]
		public bool titleScreenMode;

		public bool display;

		[FormerlySerializedAs("walk_animation")]
		public bool walkAnimation;

		string direction;

		int frames_since_last_increment;

		// Start is called before the first frame update
		void Start() {
			if (!titleScreenMode)
				characterIndex = SaveSystem.GetInt("character_index");

			activeCharacter = characters[characterIndex];

			direction = "down";

			sr = GetComponent<SpriteRenderer>();
			sr.sprite = activeCharacter.down1;

			frames_since_last_increment = 15;

			display = true;
		}

		// Update is called once per frame
		void Update() {

			if (display && sr.enabled == false)
				sr.enabled = true;
			else if (!display)
				sr.enabled = false;

			frames_since_last_increment += 1;

			if (characters != null && activeCharacter != characters[characterIndex])
				activeCharacter = characters[characterIndex];
		}

		public void set_character(int index) {
			characterIndex = index;
			activeCharacter = characters[characterIndex];
			sr.sprite = activeCharacter.down1;
		}

		public void increment_character() {
			if (frames_since_last_increment < 15)
				return;

			characterIndex += 1;
			if (characterIndex >= characters.Length)
				characterIndex = 0;

			frames_since_last_increment = 0;

			if (!titleScreenMode)
				SaveSystem.SetInt("character_index", characterIndex);

			sr.sprite = characters[characterIndex].down1;
		}

		public void decrement_character() {
			if (frames_since_last_increment < 15)
				return;

			characterIndex -= 1;
			if (characterIndex < 0)
				characterIndex = characters.Length - 1;

			frames_since_last_increment = 0;

			if (!titleScreenMode)
				SaveSystem.SetInt("character_index", characterIndex);

			sr.sprite = characters[characterIndex].down1;
		}

		public void change_direction(string dir) {
			direction = dir;
			switch (direction) {
				case "up":
					sr.sprite = activeCharacter.up1;
					break;
				case "down":
					sr.sprite = activeCharacter.down1;
					break;
				case "left":
					sr.flipX = false;
					sr.sprite = activeCharacter.step1;
					break;
				case "right":
					sr.flipX = true;
					sr.sprite = activeCharacter.step1;
					break;
			}
		}

		public string get_direction() {
			return direction;
		}

		public string get_class() {
			return activeCharacter.name;
		}

		public IEnumerator Walk() {
			float wait = 0.13189315f;
			walkAnimation = true;
			switch (direction) {
				case "up":
					sr.sprite = activeCharacter.up1;
					yield return new WaitForSeconds(wait);
					sr.sprite = activeCharacter.up2;
					yield return new WaitForSeconds(wait);
					break;
				case "down":
					sr.sprite = activeCharacter.down1;
					yield return new WaitForSeconds(wait);
					sr.sprite = activeCharacter.down2;
					yield return new WaitForSeconds(wait);
					break;
				case "left":
					sr.flipX = false;
					sr.sprite = activeCharacter.step1;
					yield return new WaitForSeconds(wait);
					sr.flipX = false;
					sr.sprite = activeCharacter.step2;
					yield return new WaitForSeconds(wait);
					break;
				case "right":
					sr.flipX = true;
					sr.sprite = activeCharacter.step1;
					yield return new WaitForSeconds(wait);
					sr.sprite = activeCharacter.step2;
					yield return new WaitForSeconds(wait);
					break;
			}
			walkAnimation = false;
		}

		[Serializable]
		public class SpriteGroup {

			public string name;

			public Sprite up1;
			public Sprite up2;
			public Sprite down1;
			public Sprite down2;
			public Sprite step1;
			public Sprite step2;
		}
	}
}
