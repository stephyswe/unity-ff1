using Utils.SaveGame.Scripts.SaveSystem;

namespace Overworld {
	public class Common {
		readonly PauseMenuHandler pauseMenuHandler;

		public Common(PauseMenuHandler pauseMenuHandler) {
			this.pauseMenuHandler = pauseMenuHandler;
		}

		public void SwitchStatusCharacter(string job) {
			switch (job) {
				case "fighter":
					pauseMenuHandler.sprite.set_character(0);
					pauseMenuHandler.statusClass.text = "FIGHTER";
					break;
				case "knight":
					pauseMenuHandler.sprite.set_character(1);
					pauseMenuHandler.statusClass.text = "KNIGHT";
					break;
				case "thief":
					pauseMenuHandler.sprite.set_character(2);
					pauseMenuHandler.statusClass.text = "THIEF";
					break;
				case "ninja":
					pauseMenuHandler.sprite.set_character(3);
					pauseMenuHandler.statusClass.text = "NINJA";
					break;
				case "black_belt":
					pauseMenuHandler.sprite.set_character(4);
					pauseMenuHandler.statusClass.text = "BLACK BELT";
					break;
				case "master":
					pauseMenuHandler.sprite.set_character(5);
					pauseMenuHandler.statusClass.text = "MASTER";
					break;
				case "black_mage":
					pauseMenuHandler.sprite.set_character(6);
					pauseMenuHandler.statusClass.text = "BLACK MAGE";
					break;
				case "black_wizard":
					pauseMenuHandler.sprite.set_character(7);
					pauseMenuHandler.statusClass.text = "BLACK WIZARD";
					break;
				case "white_mage":
					pauseMenuHandler.sprite.set_character(8);
					pauseMenuHandler.statusClass.text = "WHITE MAGE";
					break;
				case "white_wizard":
					pauseMenuHandler.sprite.set_character(9);
					pauseMenuHandler.statusClass.text = "WHITE WIZARD";
					break;
				case "red_mage":
					pauseMenuHandler.sprite.set_character(10);
					pauseMenuHandler.statusClass.text = "RED MAGE";
					break;
				case "red_wizard":
					pauseMenuHandler.sprite.set_character(11);
					pauseMenuHandler.statusClass.text = "RED WIZARD";
					break;
			}
		}


		public void SetupCharacter(string job, int i) {
			switch (job) {
				case "fighter":
					pauseMenuHandler.spriteControllers[i].set_character(0);
					break;
				case "knight":
					pauseMenuHandler.spriteControllers[i].set_character(1);
					break;
				case "thief":
					pauseMenuHandler.spriteControllers[i].set_character(2);
					break;
				case "ninja":
					pauseMenuHandler.spriteControllers[i].set_character(3);
					break;
				case "black_belt":
					pauseMenuHandler.spriteControllers[i].set_character(4);
					break;
				case "master":
					pauseMenuHandler.spriteControllers[i].set_character(5);
					break;
				case "black_mage":
					pauseMenuHandler.spriteControllers[i].set_character(6);
					break;
				case "black_wizard":
					pauseMenuHandler.spriteControllers[i].set_character(7);
					break;
				case "white_mage":
					pauseMenuHandler.spriteControllers[i].set_character(8);
					break;
				case "white_wizard":
					pauseMenuHandler.spriteControllers[i].set_character(9);
					break;
				case "red_mage":
					pauseMenuHandler.spriteControllers[i].set_character(10);
					break;
				case "red_wizard":
					pauseMenuHandler.spriteControllers[i].set_character(11);
					break;
			}
		}
	}

	public partial class PauseMenuHandler {
		void StatusTextCharacter(string playerN) {
			statusName.text = SaveSystem.GetString(playerN + "name");
			statusLevel.text = "LVL " + get_level_from_exp(SaveSystem.GetInt(playerN + "exp"));
			statusExp.text = "" + SaveSystem.GetInt(playerN + "exp");
			statusToLevelUp.text = "" + exp_till_level(SaveSystem.GetInt(playerN + "exp"));

			statusStr.text = "" + SaveSystem.GetInt(playerN + "strength");
			statusAgl.text = "" + SaveSystem.GetInt(playerN + "agility");
			statusINT.text = "" + SaveSystem.GetInt(playerN + "intelligence");
			statusVit.text = "" + SaveSystem.GetInt(playerN + "vitality");
			statusLuck.text = "" + SaveSystem.GetInt(playerN + "luck");

			statusDmg.text = "NA";
			statusHit.text = "" + (int)(100 * SaveSystem.GetFloat(playerN + "hit_percent"));
			statusAbs.text = "NA";
			statusEvade.text = "" + (48 + SaveSystem.GetInt(playerN + "agility"));
		}
	}
}
