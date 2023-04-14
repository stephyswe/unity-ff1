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

		[FormerlySerializedAs("input_allowed")]
		public bool inputAllowed;

		int frames_since_music_switch = 15;

		string[] names;

		[FormerlySerializedAs("new_save")] [SerializeField] bool newSave;

		// Start is called before the first frame update
		void Start() {

			title.SetActive(true);
			charSelect.SetActive(false);
			settingsContainer.SetActive(false);

			string binPath = Application.persistentDataPath + "/party.json";

			inputAllowed = true;

			names = new[] {"Matt", "Alta", "Ivan", "Cora"};

			if (!File.Exists(binPath)) {
				init_save_file();
				newSave = true;
			}

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

			battleSpeedSlider.value = SaveSystem.GetFloat("battle_speed");

			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		// Update is called once per frame
		void Update() {

			frames_since_music_switch += 1;

			if (inputAllowed) {
				/*
			if(Input.GetKeyDown("h") && names.Length > 0){
				foreach(string name in names){
				    SaveSystem.SetInt(name + "_maxHP", 100);
				    SaveSystem.SetInt(name + "_HP", 100);
				}
				Debug.Log("Healed your party");
			}
			*/
			}

		}

		void Flags() {
			SaveSystem.SetBool("earth_orb", false);
			SaveSystem.SetBool("fire_orb", false);
			SaveSystem.SetBool("water_orb", false);
			SaveSystem.SetBool("wind_orb", false);

			SaveSystem.SetBool("garland_battle", false);
			SaveSystem.SetBool("princess_in_temple_of_fiends", false);
			SaveSystem.SetBool("king_mentioned_bridge", false);
			SaveSystem.SetBool("princess_gave_lute", false);
		}

		public void init_save_file() {
			SaveSystem.SetBool("in_submap", false);
			SaveSystem.SetBool("inside_of_room", false);

			RandomEncounterHandler reh = gameObject.AddComponent<RandomEncounterHandler>();
			reh.gen_seed();

			SaveSystem.SetInt("reh_seed", reh.seed);

			SaveSystem.SetInt("gil", 400);

			SaveSystem.SetFloat("overworldX", -1f);
			SaveSystem.SetFloat("overworldY", -5f);

			SaveSystem.SetString("player1_name", names[0]);
			SaveSystem.SetString("player2_name", names[1]);
			SaveSystem.SetString("player3_name", names[2]);
			SaveSystem.SetString("player4_name", names[3]);

			for (int i = 0; i < 4; i++) {
				string playerN = "player" + (i + 1) + "_";

				SaveSystem.SetInt(playerN + "magic_level", 1);

				List<string> empty = new List<string>();

				SaveSystem.SetStringList(playerN + "level_1_spells", empty);
				SaveSystem.SetStringList(playerN + "level_2_spells", empty);
				SaveSystem.SetStringList(playerN + "level_3_spells", empty);
				SaveSystem.SetStringList(playerN + "level_4_spells", empty);
				SaveSystem.SetStringList(playerN + "level_5_spells", empty);
				SaveSystem.SetStringList(playerN + "level_6_spells", empty);
				SaveSystem.SetStringList(playerN + "level_7_spells", empty);
				SaveSystem.SetStringList(playerN + "level_8_spells", empty);
			}

			SaveSystem.SetStringIntDict("items", new Dictionary<string, int>());

			Flags();

			SaveSystem.SaveToDisk();
			Debug.Log("Done initializing");
		}

		public void hover_sound() {
			buttonHover.Play();
		}

		public void set_classic_music() {
			classic.volume = 1f;
			remaster.volume = 0f;
			gba.volume = 0f;

			SaveSystem.SetBool("classic_music", true);
			SaveSystem.SetBool("remaster_music", false);

			classic.Play();

			SaveSystem.SaveToDisk();
		}

		public void set_gba_music() {
			classic.volume = 0f;
			remaster.volume = 0f;
			gba.volume = 1f;

			SaveSystem.SetBool("classic_music", false);
			SaveSystem.SetBool("remaster_music", false);

			gba.Play();

			SaveSystem.SaveToDisk();
		}

		public void set_remastered_music() {
			classic.volume = 0f;
			remaster.volume = 1f;
			gba.volume = 0f;

			SaveSystem.SetBool("classic_music", false);
			SaveSystem.SetBool("remaster_music", true);

			remaster.Play();

			SaveSystem.SaveToDisk();
		}

		public void Exit() {
        #if UNITY_EDITOR
			// Application.Quit() does not work in the editor so
			// UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
			EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
		}

		public void continue_game() {
			inputAllowed = false;
			StartCoroutine(Load());
		}

		public void new_game() {
			inputAllowed = false;
			title.SetActive(false);
			charSelect.SetActive(true);
		}

		public void Settings() {
			if (!settingsContainer.active) {
				inputAllowed = false;
				title.SetActive(false);
				settingsContainer.SetActive(true);
			}
			else {
				inputAllowed = true;
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

		public void new_game_start() {
			for (int i = 0; i < fields.Length; i++)
				names[i] = fields[i].text;
			for (int i = 0; i < spriteControllers.Length; i++) {

				int characterIndex = 0;

				string playerN = "player" + (i + 1) + "_";

				string pClass = spriteControllers[i].get_class();
				SaveSystem.SetString(playerN + "class", pClass);

				switch (pClass) {
					case "fighter":
						SaveSystem.SetInt(playerN + "strength", 20);
						SaveSystem.SetInt(playerN + "agility", 5);
						SaveSystem.SetInt(playerN + "intelligence", 1);
						SaveSystem.SetInt(playerN + "vitality", 10);
						SaveSystem.SetInt(playerN + "luck", 5);
						SaveSystem.SetInt(playerN + "HP", 35);
						SaveSystem.SetFloat(playerN + "hit_percent", .1f);
						SaveSystem.SetFloat(playerN + "magic_defense", .15f);

						SaveSystem.SetInt(playerN + "MP", 0);

						characterIndex = 3;

						break;
					case "black_belt":
						SaveSystem.SetInt(playerN + "strength", 5);
						SaveSystem.SetInt(playerN + "agility", 5);
						SaveSystem.SetInt(playerN + "intelligence", 5);
						SaveSystem.SetInt(playerN + "vitality", 20);
						SaveSystem.SetInt(playerN + "luck", 5);
						SaveSystem.SetInt(playerN + "HP", 33);
						SaveSystem.SetFloat(playerN + "hit_percent", .05f);
						SaveSystem.SetFloat(playerN + "magic_defense", .1f);

						SaveSystem.SetInt(playerN + "MP", 0);

						characterIndex = 0;

						break;
					case "red_mage":
						SaveSystem.SetInt(playerN + "strength", 10);
						SaveSystem.SetInt(playerN + "agility", 10);
						SaveSystem.SetInt(playerN + "intelligence", 10);
						SaveSystem.SetInt(playerN + "vitality", 5);
						SaveSystem.SetInt(playerN + "luck", 5);
						SaveSystem.SetInt(playerN + "HP", 30);
						SaveSystem.SetFloat(playerN + "hit_percent", .07f);
						SaveSystem.SetFloat(playerN + "magic_defense", .2f);

						SaveSystem.SetInt(playerN + "MP", 10);

						characterIndex = 7;

						break;
					case "thief":
						SaveSystem.SetInt(playerN + "strength", 5);
						SaveSystem.SetInt(playerN + "agility", 10);
						SaveSystem.SetInt(playerN + "intelligence", 5);
						SaveSystem.SetInt(playerN + "vitality", 5);
						SaveSystem.SetInt(playerN + "luck", 15);
						SaveSystem.SetInt(playerN + "HP", 30);
						SaveSystem.SetFloat(playerN + "hit_percent", .05f);
						SaveSystem.SetFloat(playerN + "magic_defense", .15f);

						SaveSystem.SetInt(playerN + "MP", 0);

						characterIndex = 9;

						break;
					case "white_mage":
						SaveSystem.SetInt(playerN + "strength", 5);
						SaveSystem.SetInt(playerN + "agility", 5);
						SaveSystem.SetInt(playerN + "intelligence", 15);
						SaveSystem.SetInt(playerN + "vitality", 10);
						SaveSystem.SetInt(playerN + "luck", 5);
						SaveSystem.SetInt(playerN + "HP", 28);
						SaveSystem.SetFloat(playerN + "hit_percent", .05f);
						SaveSystem.SetFloat(playerN + "magic_defense", .2f);

						SaveSystem.SetInt(playerN + "MP", 10);

						characterIndex = 10;

						break;
					case "black_mage":
						SaveSystem.SetInt(playerN + "strength", 1);
						SaveSystem.SetInt(playerN + "agility", 10);
						SaveSystem.SetInt(playerN + "intelligence", 20);
						SaveSystem.SetInt(playerN + "vitality", 1);
						SaveSystem.SetInt(playerN + "luck", 10);
						SaveSystem.SetInt(playerN + "HP", 25);
						SaveSystem.SetFloat(playerN + "hit_percent", .055f);
						SaveSystem.SetFloat(playerN + "magic_defense", .2f);

						SaveSystem.SetInt(playerN + "MP", 10);

						characterIndex = 1;

						break;
				}

				SaveSystem.SetStringList(playerN + "armor_inventory", new List<string>());
				SaveSystem.SetStringList(playerN + "weapons_inventory", new List<string>());

				SaveSystem.SetString(playerN + "shield", "");
				SaveSystem.SetString(playerN + "helmet", "");
				SaveSystem.SetString(playerN + "armor", "");
				SaveSystem.SetString(playerN + "glove", "");

				SaveSystem.SetString(playerN + "weapon", "");

				SaveSystem.SetInt(playerN + "exp", 0);
				SaveSystem.SetBool(playerN + "poison", false);
				SaveSystem.SetBool(playerN + "stone", false);
				SaveSystem.SetInt(playerN + "maxHP", SaveSystem.GetInt(playerN + "HP"));

				if (i == 0)
					SaveSystem.SetInt("character_index", characterIndex);
			}
			init_save_file();
			StartCoroutine(Load());
		}

		IEnumerator Load() {
			loadingCircle.gameObject.SetActive(true);
			loadingCircle.start_loading_circle();
			yield return new WaitForSeconds(.5f);
			Cursor.visible = false;
			SceneManager.LoadSceneAsync("Overworld");
		}
	}
}
