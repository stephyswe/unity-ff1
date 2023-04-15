using Utils.SaveGame.Scripts.SaveSystem;


namespace Overworld.PauseMenu {
	public class DataSave {
		readonly PauseMenuHandler pmh;

		public DataSave(PauseMenuHandler pmh) {
			this.pmh = pmh;
		}
		public void StatusTextCharacter(string playerN) {
			pmh.statusName.text = SaveSystem.GetString(playerN + "name");
			pmh.statusLevel.text = "LVL " + pmh.get_level_from_exp(SaveSystem.GetInt(playerN + "exp"));
			pmh.statusExp.text = "" + SaveSystem.GetInt(playerN + "exp");
			pmh.statusToLevelUp.text = "" + pmh.exp_till_level(SaveSystem.GetInt(playerN + "exp"));

			pmh.statusStr.text = "" + SaveSystem.GetInt(playerN + "strength");
			pmh.statusAgl.text = "" + SaveSystem.GetInt(playerN + "agility");
			pmh.statusINT.text = "" + SaveSystem.GetInt(playerN + "intelligence");
			pmh.statusVit.text = "" + SaveSystem.GetInt(playerN + "vitality");
			pmh.statusLuck.text = "" + SaveSystem.GetInt(playerN + "luck");

			pmh.statusDmg.text = "NA";
			pmh.statusHit.text = "" + (int)(100 * SaveSystem.GetFloat(playerN + "hit_percent"));
			pmh.statusAbs.text = "NA";
			pmh.statusEvade.text = "" + (48 + SaveSystem.GetInt(playerN + "agility"));
		}
	}
}
