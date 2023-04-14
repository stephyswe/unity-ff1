using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Overworld;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils.SaveGame.Scripts.SaveSystem;

namespace TitleScreen {
	public class TitleScreenHandler : MonoBehaviour {

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
		
		void Start() {
			// Setup config
			title.SetActive(true);
			charSelect.SetActive(false);
			settingsContainer.SetActive(false);
			string binPath = Application.persistentDataPath + "/party.json";

			// enable keyboard in character select

			// Set up the names array
			names = new[] {"Matt", "Alta", "Ivan", "Cora"};

			// If the save file doesn't exist, create it
			if (!File.Exists(binPath)) {
				init_save_file();
			}

			// music selection
			title_selection();

			// battle speed
			battleSpeedSlider.value = SaveSystem.GetFloat("battle_speed");

			// show cursor
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		// used in the character select screen
		public void new_game_start() {
			for (int i = 0; i < fields.Length; i++)
				names[i] = fields[i].text;

			for (int i = 0; i < spriteControllers.Length; i++) {
				const int characterIndex = 0;
				string playerN = "player" + (i + 1) + "_";
				string pClass = spriteControllers[i].get_class();
				SaveSystem.SetString(playerN + "class", pClass);
				before_init_save(pClass, playerN, characterIndex, i);
			}
			init_save_file();
			StartCoroutine(Load());
		}

		// used in the title screen
		public void continue_game() {
			StartCoroutine(Load());
		}

		IEnumerator Load() {
			gameObject.SetActive(true);
			loadingCircle.start_loading_circle();
			yield return new WaitForSeconds(.5f);
			Cursor.visible = false;
			SceneManager.LoadSceneAsync("Overworld");
		}

		void init_save_file() {
			// Random encounter handler
			RandomEncounterHandler reh = gameObject.AddComponent<RandomEncounterHandler>();
			reh.gen_seed();
			SaveSystem.SetInt("reh_seed", reh.seed);

			// World data
			SaveSystem.SetInt("gil", 400);
			SaveSystem.SetBool("in_submap", false);
			SaveSystem.SetBool("inside_of_room", false);
			SaveSystem.SetFloat("overworldX", -1f);
			SaveSystem.SetFloat("overworldY", -5f);

			// Items
			SaveSystem.SetStringIntDict("items", new Dictionary<string, int>());

			// Player data
			for (int i = 0; i < 4; i++) {
				string playerN = "player" + (i + 1) + "_";
				// Player names
				SaveSystem.SetString(playerN + "name", names[i]);
				// Player magic level
				SaveSystem.SetInt(playerN + "magic_level", 1);
				List<string> empty = new List<string>();
				// Player spells
				for (int j = 1; j < 9; j++) {
					SaveSystem.SetStringList(playerN + "level_" + j + "_spells", empty);
				}
			}
			WorldFlags();
			SaveSystem.SaveToDisk();
			Debug.Log("Done initializing");
		}

		static void WorldFlags() {
			SaveSystem.SetBool("earth_orb", false);
			SaveSystem.SetBool("fire_orb", false);
			SaveSystem.SetBool("water_orb", false);
			SaveSystem.SetBool("wind_orb", false);

			SaveSystem.SetBool("garland_battle", false);
			SaveSystem.SetBool("princess_in_temple_of_fiends", false);
			SaveSystem.SetBool("king_mentioned_bridge", false);
			SaveSystem.SetBool("princess_gave_lute", false);
		}

		void hover_sound() {
			buttonHover.Play();
		}

		void set_classic_music() {
			classic.volume = 1f;
			remaster.volume = 0f;
			gba.volume = 0f;

			SaveSystem.SetBool("classic_music", true);
			SaveSystem.SetBool("remaster_music", false);

			classic.Play();

			SaveSystem.SaveToDisk();
		}

		void set_gba_music() {
			classic.volume = 0f;
			remaster.volume = 0f;
			gba.volume = 1f;

			SaveSystem.SetBool("classic_music", false);
			SaveSystem.SetBool("remaster_music", false);

			gba.Play();

			SaveSystem.SaveToDisk();
		}

		void set_remastered_music() {
			classic.volume = 0f;
			remaster.volume = 1f;
			gba.volume = 0f;

			SaveSystem.SetBool("classic_music", false);
			SaveSystem.SetBool("remaster_music", true);

			remaster.Play();

			SaveSystem.SaveToDisk();
		}

		void title_selection() {
			Debug.Log("music selection");
			if (SaveSystem.GetBool("classic_music")) {
				classic.volume = 1f;
				remaster.volume = 0f;
				gba.volume = 0f;
			}
			else if (SaveSystem.GetBool("remaster_music")) {
				remaster.volume = 1f;
				classic.volume = 0f;
				gba.volume = 0f;
			}
			else {
				gba.volume = 1f;
				classic.volume = 0f;
				remaster.volume = 0f;
			}
		}

		public void Exit() {
        #if UNITY_EDITOR
			// Application.Quit() does not work in the editor so
			// UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
			EditorApplication.isPlaying = false;
        #endif
		}

		public void new_game() {
			title.SetActive(false);
			charSelect.SetActive(true);
		}

		public void Settings() {
			if (!settingsContainer.activeSelf) {
				title.SetActive(false);
				settingsContainer.SetActive(true);
			}
			else {
				title.SetActive(true);
				settingsContainer.SetActive(false);
			}
		}

		public void battle_speed_set() {
			SaveSystem.SetFloat("battle_speed", battleSpeedSlider.value);
			bssText.text = "" + (int)(battleSpeedSlider.value * 1000f);
			SaveSystem.SaveToDisk();
		}

		public void new_game_back() {
			title.SetActive(true);
			charSelect.SetActive(false);
		}

		static void before_init_save(string pClass, string player, int characterIndex, int i) {
			float[] values = null;
			switch (pClass) {
				case "fighter":
					values = new[] { 20f, 5f, 1f, 10f, 5f, 35f, 0.1f, 0.15f, 0f };
					characterIndex = 3;
					break;
				case "black_belt":
					values = new[] { 5f, 5f, 5f, 20f, 5f, 33f, 0.05f, 0.1f, 0f };
					characterIndex = 0;
					break;
				case "red_mage":
					values = new[] { 10f, 10f, 10f, 5f, 5f, 30f, 0.07f, 0.2f, 10f };
					characterIndex = 7;
					break;
				case "thief":
					values = new[] { 5f, 15f, 5f, 5f, 10f, 30f, 0.05f, 0.15f, 0f };
					characterIndex = 9;
					break;
				case "white_mage":
					values = new[] { 5f, 5f, 15f, 10f, 5f, 28f, 0.05f, 0.2f, 10f };
					characterIndex = 10;
					break;
				case "black_mage":
					values = new[] { 1f, 10f, 20f, 1f, 10f, 25f, 0.055f, 0.2f, 10f };
					characterIndex = 1;
					break;
				default:
					Console.WriteLine("Invalid class name.");
					break;
			}
			
			// Save the character attributes
			if (values != null && characterIndex != -1) {
				SaveCharacterValues(player, characterIndex, values);
			}

			void SaveCharacterValues(string playerName, int chIndex, IReadOnlyList<float> attributes) {
				SaveSystem.SetInt(playerName + "character_index", chIndex);
				SaveSystem.SetInt(playerName + "strength", (int)attributes[0]);
				SaveSystem.SetInt(playerName + "agility", (int)attributes[1]);
				SaveSystem.SetInt(playerName + "intelligence", (int)attributes[2]);
				SaveSystem.SetInt(playerName + "vitality", (int)attributes[3]);
				SaveSystem.SetInt(playerName + "luck", (int)attributes[4]);
				SaveSystem.SetInt(playerName + "HP", (int)attributes[5]);
				SaveSystem.SetFloat(playerName + "hit_percent", attributes[6]);
				SaveSystem.SetFloat(playerName + "magic_defense", attributes[7]);
				SaveSystem.SetInt(playerName + "MP", (int)attributes[8]);
			}

			SaveSystem.SetStringList(player + "armor_inventory", new List<string>());
			SaveSystem.SetStringList(player + "weapons_inventory", new List<string>());

			SaveSystem.SetString(player + "shield", "");
			SaveSystem.SetString(player + "helmet", "");
			SaveSystem.SetString(player + "armor", "");
			SaveSystem.SetString(player + "glove", "");

			SaveSystem.SetString(player + "weapon", "");

			SaveSystem.SetInt(player + "exp", 0);
			SaveSystem.SetBool(player + "poison", false);
			SaveSystem.SetBool(player + "stone", false);
			SaveSystem.SetInt(player + "maxHP", SaveSystem.GetInt(player + "HP"));

			if (i == 0)
				SaveSystem.SetInt("character_index", characterIndex);
		}


	}
}
