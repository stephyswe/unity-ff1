﻿using System;
using UnityEngine;

namespace TitleScreen {
	public partial class GameManager {
		public static void ContainerToggle(GameObject container, GameObject title) {
			if (container == null || title == null) {
				Debug.LogError("Container and title GameObjects cannot be null.");
				return;
			}	
			if (!container.activeSelf) {
				title.SetActive(false);
				container.SetActive(true);
			}
			else {
				title.SetActive(true);
				container.SetActive(false);
			}
		}
		
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
		void pre_setup() {
			title.SetActive(true);
			charSelect.SetActive(false);
			settingsContainer.SetActive(false);
			
			names = new[] {"Matt", "Alta", "Ivan", "Cora"};

			// show cursor
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
}
