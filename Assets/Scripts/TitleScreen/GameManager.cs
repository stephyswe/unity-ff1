using System.Collections;
using System.IO;
using Overworld;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils.SaveGame.Scripts.SaveSystem;

namespace TitleScreen {
	public partial class GameManager : MonoBehaviour {

		[FormerlySerializedAs("loading_circle")]
		public LoadingCircle loadingCircle;

		public AudioSource classic;
		public AudioSource remaster;
		[FormerlySerializedAs("GBA")] public AudioSource gba;

		[FormerlySerializedAs("button_hover")] public AudioSource buttonHover;

		public GameObject title;
		[FormerlySerializedAs("char_select")] public GameObject charSelect;
		[FormerlySerializedAs("settings_container")]
		public GameObject settingsContainer;

		public InputField[] fields;
		[FormerlySerializedAs("sprite_controllers")]
		public SpriteController[] spriteControllers;

		[FormerlySerializedAs("battle_speed_slider")]
		public Slider battleSpeedSlider;
		[FormerlySerializedAs("bss_text")] public Text bssText;

		public string[] names;
		
		
		// Values and methods for testing
		public static bool startEnabled = true;
		public static bool updateEnabled = true;

		public static void InitializeTestingEnvironment(bool start, bool update)
		{
			
			startEnabled = start;
			updateEnabled = update;
		}
		
		public static GameManager instance;

		void Start() {
			pre_setup();
			string binPath = Application.persistentDataPath + "/party.json";
			// If the save file doesn't exist, create it
			if (!File.Exists(binPath)) {
				// first-time values 
				SaveSystem.SetBool("gameStarted", false);
				SaveSystem.SetBool("classic_music", true);
				SaveSystem.SetBool("remaster_music", false);
				SaveSystem.SetBool("gba_music", false);
			}
			
			if (!SaveSystem.GetBool("gameStarted")) {
				GameObject.Find("Continue").GetComponent<Button>().interactable = false;
			}
			battleSpeedSlider.value = SaveSystem.GetFloat("battle_speed");
			
			GetMusic();
		}
		void GetMusic() {
			bool classicMusic = SaveSystem.GetBool("classic_music");
			bool remasterMusic = SaveSystem.GetBool("remaster_music");
			SetMusicVolumes(classicMusic, remasterMusic);
		}

		// Load (start game & continue)
		IEnumerator Load() {
			gameObject.SetActive(true);
			loadingCircle.start_loading_circle();
			yield return new WaitForSeconds(.5f);
			Cursor.visible = false;
			SceneManager.LoadSceneAsync("Overworld");
		}

		// Misc - Hover Sound
		public void hover_sound() {
			buttonHover.Play();
		}

		// Title Screen Options:

		// New game & back
		public void new_game() {
			ContainerToggle(charSelect, title);
		}

		// Continue
		public void continue_game() {
			StartCoroutine(Load());
		}
		
		// setting & back
		public void Settings() {
			ContainerToggle(settingsContainer, title);
		}

		// Quit
		public void Exit() {
        #if UNITY_EDITOR
			EditorApplication.isPlaying = false;
        #endif
		}

		// Setting Options:
		public void battle_speed_set() {
			SaveSystem.SetFloat("battle_speed", battleSpeedSlider.value);
			bssText.text = "" + (int)(battleSpeedSlider.value * 1000f);
			SaveSystem.SaveToDisk();
		}

		// Music Choice
		void set_classic_music() {
			PlayMusicTrack(MusicTrack.Classic, .3f);
		}

		void set_gba_music() {
			PlayMusicTrack(MusicTrack.Gba, .3f);
		}

		void set_remastered_music() {
			PlayMusicTrack(MusicTrack.Remastered, .3f);
		}

		// Character Selection Screen Options:

		// Start Game
		public void new_game_start() {

			// set name if exist
			if (fields[0].text != "") {
				for (int j = 0; j < fields.Length; j++)
					names[j] = fields[j].text;
			}

			for (int spriteIndex = 0; spriteIndex < spriteControllers.Length; spriteIndex++) {
				string playerName = "player" + (spriteIndex + 1) + "_";
				string pClass = spriteControllers[spriteIndex].get_class();
				SaveSystem.SetString(playerName + "class", pClass);

				// Set player data
				(float[] values, int characterIndex) = GetCharacterValues(pClass);

				// Save the character attributes
				SaveCharacterValues(playerName, values);
				SaveCharacterInventory(playerName);

				// Set player model based on first character selected
				if (spriteIndex == 0)
					SaveSystem.SetInt("character_index", characterIndex);
				SaveSystem.SetBool("gameStarted", true);
			}

			// more save data
			init_save_file();

			// Load the overworld
			StartCoroutine(Load());
		}
	}
}
