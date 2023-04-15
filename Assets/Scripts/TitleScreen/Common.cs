using System;
using UnityEngine;
using Utils.SaveGame.Scripts.SaveSystem;

namespace TitleScreen {
	public partial class TitleScreenHandler {
		public static (float[] values, int characterIndex) GetCharacterValues(string pClass) {
			float[] values = null;
			int characterIndex = 0;
			switch (pClass) {
				case "fighter":
					values = new[] {20, 5, 1, 10, 5, 35, .1f, .15f, 0};
					characterIndex = 3;
					break;
				case "black_belt":
					values = new[] {5, 5, 5, 20, 5, 33, .05f, .1f, 0};
					characterIndex = 0;
					break;
				case "red_mage":
					values = new[] {10, 10, 10, 5, 5, 30, .07f, .2f, 10};
					characterIndex = 7;
					break;
				case "thief":
					values = new[] {5, 10, 5, 5, 15, 30, .05f, .15f, 0};
					characterIndex = 9;
					break;
				case "white_mage":
					values = new[] {5, 5, 15, 10, 5, 28, .05f, .2f, 10};
					characterIndex = 10;
					break;
				case "black_mage":
					values = new[] {1, 10, 20, 1, 10, 25, .055f, .2f, 10};
					characterIndex = 1;
					break;
				default:
					Console.WriteLine("Invalid class name.");
					break;
			}
			return (values, characterIndex);
		}
		string pre_setup() {
			title.SetActive(true);
			charSelect.SetActive(false);
			settingsContainer.SetActive(false);
			string binPath = Application.persistentDataPath + "/party.json";
			names = new[] {"Matt", "Alta", "Ivan", "Cora"};
			
			// Set the music from preference
			bool classicMusic = SaveSystem.GetBool("classic_music");
			bool remasterMusic = SaveSystem.GetBool("remaster_music");
			SetMusicVolumes(classicMusic, remasterMusic);
			
			// battle speed
			battleSpeedSlider.value = SaveSystem.GetFloat("battle_speed");

			// show cursor
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			return binPath;
		}
	}
}
