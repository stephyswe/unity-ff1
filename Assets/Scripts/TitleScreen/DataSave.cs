using System.Collections.Generic;
using Overworld;
using UnityEngine;
using Utils.SaveGame.Scripts.SaveSystem;

namespace TitleScreen {
	public partial class TitleScreenHandler {
		void init_save_file() {
			// Random encounter handler
			RandomEncounterHandler reh = gameObject.AddComponent<RandomEncounterHandler>();
			reh.gen_seed();

			// Initialize the save file
			InitSaveFileStorage(reh);

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
		static void SaveCharacterInventory(string player) {
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
		}
		public static void WorldFlags() {
			SaveSystem.SetBool("earth_orb", false);
			SaveSystem.SetBool("fire_orb", false);
			SaveSystem.SetBool("water_orb", false);
			SaveSystem.SetBool("wind_orb", false);

			SaveSystem.SetBool("garland_battle", false);
			SaveSystem.SetBool("princess_in_temple_of_fiends", false);
			SaveSystem.SetBool("king_mentioned_bridge", false);
			SaveSystem.SetBool("princess_gave_lute", false);
		}
		static void SaveCharacterValues(string playerName, int chIndex, IReadOnlyList<float> attributes) {
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
		
		static void InitSaveFileStorage(RandomEncounterHandler reh) {
			SaveSystem.SetInt("reh_seed", reh.seed);

			// World data
			SaveSystem.SetInt("gil", 400);
			SaveSystem.SetBool("in_submap", false);
			SaveSystem.SetBool("inside_of_room", false);
			SaveSystem.SetFloat("overworldX", -1f);
			SaveSystem.SetFloat("overworldY", -5f);

			// Items
			SaveSystem.SetStringIntDict("items", new Dictionary<string, int>());
		}
	}
}
