using Utils.SaveGame.Scripts.SaveSystem;

namespace Overworld {
	public class CommonWrapper {
		readonly PauseMenuHandler pauseMenuHandler;

		public CommonWrapper(PauseMenuHandler pauseMenuHandler) {
			this.pauseMenuHandler = pauseMenuHandler;
		}
		public void Setup() {
			// get gold
			pauseMenuHandler.gold.text = "" + SaveSystem.GetInt("gil");

			// get all characters and their stats
			for (int i = 0; i < 4; i++) {
				string playerN = "player" + (i + 1) + "_";

				pauseMenuHandler.names[i].text = SaveSystem.GetString(playerN + "name");
				pauseMenuHandler.levels[i].text = "L " + pauseMenuHandler.get_level_from_exp(SaveSystem.GetInt(playerN + "exp"));
				pauseMenuHandler.hPs[i].text = "HP\n" + SaveSystem.GetInt(playerN + "HP") + "-" + SaveSystem.GetInt(playerN + "maxHP");
				pauseMenuHandler.mPs[i].text = "0-0-0-0\n0-0-0-0";

				string job = SaveSystem.GetString(playerN + "class");

				// setup character
				pauseMenuHandler.Common.SetupCharacter(job, i);
			}

			pauseMenuHandler.earth.GetComponent<SpriteController>().change_direction(!SaveSystem.GetBool("earth_orb") ? "down" : "up");
			pauseMenuHandler.fire.GetComponent<SpriteController>().change_direction(!SaveSystem.GetBool("fire_orb") ? "down" : "up");
			pauseMenuHandler.water.GetComponent<SpriteController>().change_direction(!SaveSystem.GetBool("water_orb") ? "down" : "up");
			pauseMenuHandler.wind.GetComponent<SpriteController>().change_direction(!SaveSystem.GetBool("wind_orb") ? "down" : "up");
		}
	}
}
