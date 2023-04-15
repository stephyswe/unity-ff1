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
	public partial class TitleScreenHandler : MonoBehaviour {

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
				string playerName = "player" + (i + 1) + "_";
				string pClass = spriteControllers[i].get_class();
				SaveSystem.SetString(playerName + "class", pClass);
				
				// Set player data
				before_init_save(pClass, playerName, i);
				if (i == 0)
					SaveSystem.SetInt("character_index", i);
			}
			
			// Set the battle speed
			init_save_file();
			
			// Load the overworld
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

		void hover_sound() {
			buttonHover.Play();
		}

		void set_classic_music() {
			
			PlayMusicTrack(MusicTrack.Classic, 1f);
		}

		void set_gba_music() {
			PlayMusicTrack(MusicTrack.Gba, 1f);
		}

		void set_remastered_music() {
			PlayMusicTrack(MusicTrack.Remastered, 1f);
		}

		void title_selection() {
			// Set the music from preference
			bool classicMusic = SaveSystem.GetBool("classic_music");
			bool remasterMusic = SaveSystem.GetBool("remaster_music");
			SetMusicVolumes(classicMusic, remasterMusic);
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

		static void before_init_save(string pClass, string player, int i) {
			
			// get the character attributes
			(float[] values, int characterIndex) = GetCharacterValues(pClass);

			// Save the character attributes
			SaveCharacterValues(player, characterIndex, values);
			SaveCharacterInventory(player);
		}


	}
}
