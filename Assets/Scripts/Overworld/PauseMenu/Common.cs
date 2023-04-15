using Refactor;
using Utils.SaveGame.Scripts.SaveSystem;

namespace Overworld.PauseMenu {
	public class Common {
		readonly PauseMenuHandler pmh;

		public Common(PauseMenuHandler pmh) {
			this.pmh = pmh;
		}

		public void Setup() {
			// get gold
			pmh.gold.text = "" + SaveSystem.GetInt("gil");

			// get all characters and their stats
			for (int i = 0; i < 4; i++) {
				string playerN = "player" + (i + 1) + "_";

				pmh.names[i].text = SaveSystem.GetString(playerN + "name");
				pmh.levels[i].text = "L " + pmh.get_level_from_exp(SaveSystem.GetInt(playerN + "exp"));
				pmh.hPs[i].text = "HP\n" + SaveSystem.GetInt(playerN + "HP") + "-" + SaveSystem.GetInt(playerN + "maxHP");
				pmh.mPs[i].text = "0-0-0-0\n0-0-0-0";

				string job = SaveSystem.GetString(playerN + "class");

				// setup character
				pmh.Common.SetupCharacter(job, i);
			}

			pmh.earth.GetComponent<SpriteController>().change_direction(!SaveSystem.GetBool("earth_orb") ? "down" : "up");
			pmh.fire.GetComponent<SpriteController>().change_direction(!SaveSystem.GetBool("fire_orb") ? "down" : "up");
			pmh.water.GetComponent<SpriteController>().change_direction(!SaveSystem.GetBool("water_orb") ? "down" : "up");
			pmh.wind.GetComponent<SpriteController>().change_direction(!SaveSystem.GetBool("wind_orb") ? "down" : "up");
		}

		public void SwitchStatusCharacter(string job) {
			switch (job) {
				case "fighter":
					pmh.sprite.set_character(0);
					pmh.statusClass.text = "FIGHTER";
					break;
				case "knight":
					pmh.sprite.set_character(1);
					pmh.statusClass.text = "KNIGHT";
					break;
				case "thief":
					pmh.sprite.set_character(2);
					pmh.statusClass.text = "THIEF";
					break;
				case "ninja":
					pmh.sprite.set_character(3);
					pmh.statusClass.text = "NINJA";
					break;
				case "black_belt":
					pmh.sprite.set_character(4);
					pmh.statusClass.text = "BLACK BELT";
					break;
				case "master":
					pmh.sprite.set_character(5);
					pmh.statusClass.text = "MASTER";
					break;
				case "black_mage":
					pmh.sprite.set_character(6);
					pmh.statusClass.text = "BLACK MAGE";
					break;
				case "black_wizard":
					pmh.sprite.set_character(7);
					pmh.statusClass.text = "BLACK WIZARD";
					break;
				case "white_mage":
					pmh.sprite.set_character(8);
					pmh.statusClass.text = "WHITE MAGE";
					break;
				case "white_wizard":
					pmh.sprite.set_character(9);
					pmh.statusClass.text = "WHITE WIZARD";
					break;
				case "red_mage":
					pmh.sprite.set_character(10);
					pmh.statusClass.text = "RED MAGE";
					break;
				case "red_wizard":
					pmh.sprite.set_character(11);
					pmh.statusClass.text = "RED WIZARD";
					break;
			}
		}


		void SetupCharacter(string job, int i) {
			switch (job) {
				case "fighter":
					pmh.spriteControllers[i].set_character(0);
					break;
				case "knight":
					pmh.spriteControllers[i].set_character(1);
					break;
				case "thief":
					pmh.spriteControllers[i].set_character(2);
					break;
				case "ninja":
					pmh.spriteControllers[i].set_character(3);
					break;
				case "black_belt":
					pmh.spriteControllers[i].set_character(4);
					break;
				case "master":
					pmh.spriteControllers[i].set_character(5);
					break;
				case "black_mage":
					pmh.spriteControllers[i].set_character(6);
					break;
				case "black_wizard":
					pmh.spriteControllers[i].set_character(7);
					break;
				case "white_mage":
					pmh.spriteControllers[i].set_character(8);
					break;
				case "white_wizard":
					pmh.spriteControllers[i].set_character(9);
					break;
				case "red_mage":
					pmh.spriteControllers[i].set_character(10);
					break;
				case "red_wizard":
					pmh.spriteControllers[i].set_character(11);
					break;
			}
		}
	}
}
