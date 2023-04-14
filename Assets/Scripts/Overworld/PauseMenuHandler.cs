using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils.SaveGame.Scripts.SaveSystem;

namespace Overworld {
	public class PauseMenuHandler : MonoBehaviour {
		public PlayerController player;
		[FormerlySerializedAs("overworld_scene_container")]
		public GameObject overworldSceneContainer;
		[FormerlySerializedAs("pausemenu_container")]
		public GameObject pausemenuContainer;
		[FormerlySerializedAs("status_container")]
		public GameObject statusContainer;
		[FormerlySerializedAs("status_bag")] public GameObject statusBag;
		[FormerlySerializedAs("status_bag_category")]
		public Text statusBagCategory;
		[FormerlySerializedAs("status_bag_items")]
		public Text[] statusBagItems;
		[FormerlySerializedAs("music_container")]
		public GameObject musicContainer;

		public SpriteController[] spriteControllers;
		public Text[] names;
		public Text[] levels;
		[FormerlySerializedAs("HPs")] public Text[] hPs;
		[FormerlySerializedAs("MPs")] public Text[] mPs;

		[FormerlySerializedAs("button_hover")] public AudioSource buttonHover;

		[FormerlySerializedAs("bag_obj")] public GameObject bagObj;
		[FormerlySerializedAs("bag_btn")] public GameObject bagBtn;
		[FormerlySerializedAs("bag_items")] public Text[] bagItems;

		[FormerlySerializedAs("give_names")] public Text[] giveNames;
		[FormerlySerializedAs("use_names")] public Text[] useNames;
		public GameObject givedrop;
		public GameObject usedrop;
		public GameObject equipparty;

		public GameObject give;
		[FormerlySerializedAs("use_on")] public GameObject useOn;

		public GameObject areyousure;
		public Text areyousuretext;

		public Text gold;

		public GameObject earth;
		public GameObject fire;
		public GameObject water;
		public GameObject wind;

		public SpriteController sprite;
		[FormerlySerializedAs("status_class")] public Text statusClass;
		[FormerlySerializedAs("status_name")] public Text statusName;
		[FormerlySerializedAs("status_level")] public Text statusLevel;
		[FormerlySerializedAs("status_exp")] public Text statusExp;
		[FormerlySerializedAs("status_to_level_up")]
		public Text statusToLevelUp;
		[FormerlySerializedAs("status_str")] public Text statusStr;
		[FormerlySerializedAs("status_agl")] public Text statusAgl;
		[FormerlySerializedAs("status_int")] public Text statusINT;
		[FormerlySerializedAs("status_vit")] public Text statusVit;
		[FormerlySerializedAs("status_luck")] public Text statusLuck;
		[FormerlySerializedAs("status_dmg")] public Text statusDmg;
		[FormerlySerializedAs("status_abs")] public Text statusAbs;
		[FormerlySerializedAs("status_hit")] public Text statusHit;
		[FormerlySerializedAs("status_evade")] public Text statusEvade;

		[FormerlySerializedAs("hover_text")] public Text hoverText;

		[FormerlySerializedAs("use_index")] public int useIndex = -1;

		[FormerlySerializedAs("give_index")] public int giveIndex = -1;
		bool areyousure_no;

		bool areyousure_yes;

		Equips equips;

		int item_select_index;

		int item_select_status_index;

		string status_player_n;

		void Start() {
			overworldSceneContainer.SetActive(true);
			pausemenuContainer.SetActive(false);
			statusContainer.SetActive(false);
			bagObj.SetActive(false);
			give.SetActive(false);
			givedrop.SetActive(false);
			equipparty.SetActive(false);
			usedrop.SetActive(false);
			useOn.SetActive(false);

			equips = new Equips();
		}

		// Update is called once per frame
		void Update() {
			if (Input.GetKeyDown("escape")) {
				if (SceneManager.sceneCount == 1 && player.canMove)
					Setup();
				On();
			}
		}

		void OnEnable() {
			statusContainer.SetActive(false);
			bagObj.SetActive(false);
			givedrop.SetActive(false);
			equipparty.SetActive(false);
		}

		public void hover_sound() {
			if (bagObj.active) {}
			buttonHover.Play();
		}

		Dictionary<int, int> get_level_chart() {
			Dictionary<int, int> levelChart = new Dictionary<int, int>();

			levelChart.Add(2, 40);
			levelChart.Add(3, 196);
			levelChart.Add(4, 547);
			levelChart.Add(5, 1171);
			levelChart.Add(6, 2146);
			levelChart.Add(7, 3550);
			levelChart.Add(8, 5461);
			levelChart.Add(9, 7957);
			levelChart.Add(10, 11116);
			levelChart.Add(11, 15016);
			levelChart.Add(12, 19753);
			levelChart.Add(13, 25351);
			levelChart.Add(14, 31942);
			levelChart.Add(15, 39586);
			levelChart.Add(16, 48361);
			levelChart.Add(17, 58345);
			levelChart.Add(18, 69617);
			levelChart.Add(19, 82253);
			levelChart.Add(20, 96332);
			levelChart.Add(21, 111932);
			levelChart.Add(22, 129131);
			levelChart.Add(23, 148008);
			levelChart.Add(24, 168639);
			levelChart.Add(25, 191103);
			levelChart.Add(26, 215479);
			levelChart.Add(27, 241843);
			levelChart.Add(28, 270275);
			levelChart.Add(29, 300851);
			levelChart.Add(30, 333651);
			levelChart.Add(31, 366450);
			levelChart.Add(32, 399250);
			levelChart.Add(33, 432049);
			levelChart.Add(34, 464849);
			levelChart.Add(35, 497648);
			levelChart.Add(36, 530448);
			levelChart.Add(37, 563247);
			levelChart.Add(38, 596047);
			levelChart.Add(39, 628846);
			levelChart.Add(40, 661646);
			levelChart.Add(41, 694445);
			levelChart.Add(42, 727245);
			levelChart.Add(43, 760044);
			levelChart.Add(44, 792844);
			levelChart.Add(45, 825643);
			levelChart.Add(46, 858443);
			levelChart.Add(47, 891242);
			levelChart.Add(48, 924042);
			levelChart.Add(49, 956841);
			levelChart.Add(50, 989641);

			return levelChart;
		}

		int get_level_from_exp(int exp) {
			Dictionary<int, int> levelChart = get_level_chart();
			int level = 1;

			foreach (KeyValuePair<int, int> entry in levelChart) {
				if (entry.Value > exp)
					break;
				level = entry.Key;
			}
			return level;
		}

		int exp_till_level(int exp) {
			Dictionary<int, int> levelChart = get_level_chart();
			int level = get_level_from_exp(exp);

			return levelChart[level + 1] - exp;
		}

		// Start is called before the first frame update
		void Setup() {
			gold.text = "" + SaveSystem.GetInt("gil");

			for (int i = 0; i < 4; i++) {
				string playerN = "player" + (i + 1) + "_";

				names[i].text = SaveSystem.GetString(playerN + "name");

				levels[i].text = "L " + get_level_from_exp(SaveSystem.GetInt(playerN + "exp"));

				hPs[i].text = "HP\n" + SaveSystem.GetInt(playerN + "HP") + "-" + SaveSystem.GetInt(playerN + "maxHP");

				mPs[i].text = "0-0-0-0\n0-0-0-0";

				string job = SaveSystem.GetString(playerN + "class");

				switch (job) {
					case "fighter":
						spriteControllers[i].set_character(0);
						break;
					case "knight":
						spriteControllers[i].set_character(1);
						break;
					case "thief":
						spriteControllers[i].set_character(2);
						break;
					case "ninja":
						spriteControllers[i].set_character(3);
						break;
					case "black_belt":
						spriteControllers[i].set_character(4);
						break;
					case "master":
						spriteControllers[i].set_character(5);
						break;
					case "black_mage":
						spriteControllers[i].set_character(6);
						break;
					case "black_wizard":
						spriteControllers[i].set_character(7);
						break;
					case "white_mage":
						spriteControllers[i].set_character(8);
						break;
					case "white_wizard":
						spriteControllers[i].set_character(9);
						break;
					case "red_mage":
						spriteControllers[i].set_character(10);
						break;
					case "red_wizard":
						spriteControllers[i].set_character(11);
						break;
				}
			}



			if (!SaveSystem.GetBool("earth_orb"))
				earth.GetComponent<SpriteController>().change_direction("down");
			else
				earth.GetComponent<SpriteController>().change_direction("up");

			if (!SaveSystem.GetBool("fire_orb"))
				fire.GetComponent<SpriteController>().change_direction("down");
			else
				fire.GetComponent<SpriteController>().change_direction("up");

			if (!SaveSystem.GetBool("water_orb"))
				water.GetComponent<SpriteController>().change_direction("down");
			else
				water.GetComponent<SpriteController>().change_direction("up");

			if (!SaveSystem.GetBool("wind_orb"))
				wind.GetComponent<SpriteController>().change_direction("down");
			else
				wind.GetComponent<SpriteController>().change_direction("up");
		}

		public void status_n(int n) {
			Status(n);
		}

		public void Bag() {

			givedrop.SetActive(false);
			usedrop.SetActive(false);

			Dictionary<string, int> items = SaveSystem.GetStringIntDict("items");
			foreach (Text t in bagItems) {
				t.text = "";
				t.gameObject.transform.parent.gameObject.SetActive(false);
			}

			int i = 0;

			foreach (KeyValuePair<string, int> kvp in items) {
				bagItems[i].gameObject.transform.parent.gameObject.SetActive(true);
				bagItems[i].text = kvp.Key + " x" + kvp.Value;
				i += 1;
			}

			bagObj.SetActive(true);
			bagBtn.SetActive(false);
		}

		public void Status(int index) {
			string playerN = "player" + (index + 1) + "_";
			status_player_n = playerN;
			pausemenuContainer.SetActive(false);
			bagObj.SetActive(false);
			statusBag.SetActive(false);

			string job = SaveSystem.GetString(playerN + "class");

			switch (job) {
				case "fighter":
					sprite.set_character(0);
					statusClass.text = "FIGHTER";
					break;
				case "knight":
					sprite.set_character(1);
					statusClass.text = "KNIGHT";
					break;
				case "thief":
					sprite.set_character(2);
					statusClass.text = "THIEF";
					break;
				case "ninja":
					sprite.set_character(3);
					statusClass.text = "NINJA";
					break;
				case "black_belt":
					sprite.set_character(4);
					statusClass.text = "BLACK BELT";
					break;
				case "master":
					sprite.set_character(5);
					statusClass.text = "MASTER";
					break;
				case "black_mage":
					sprite.set_character(6);
					statusClass.text = "BLACK MAGE";
					break;
				case "black_wizard":
					sprite.set_character(7);
					statusClass.text = "BLACK WIZARD";
					break;
				case "white_mage":
					sprite.set_character(8);
					statusClass.text = "WHITE MAGE";
					break;
				case "white_wizard":
					sprite.set_character(9);
					statusClass.text = "WHITE WIZARD";
					break;
				case "red_mage":
					sprite.set_character(10);
					statusClass.text = "RED MAGE";
					break;
				case "red_wizard":
					sprite.set_character(11);
					statusClass.text = "RED WIZARD";
					break;
			}

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

			statusContainer.SetActive(true);
		}

		public void select_item_n(int n) {
			if (bagItems[n].text == "")
				return;
			item_select_index = n;
			Select();
		}

		public void Select() {
			for (int i = 0; i < 4; i++) {
				giveNames[i].text = SaveSystem.GetString("player" + (i + 1) + "_name");
				useNames[i].text = SaveSystem.GetString("player" + (i + 1) + "_name");
			}

			string itemName = bagItems[item_select_index].text.Substring(0, bagItems[item_select_index].text.IndexOf(" x"));

			string category = equips.item_category(itemName);

			Debug.Log(itemName);

			if (category == "weapon" || category == "armor")
				givedrop.SetActive(true);
			else if (category == "item")
				usedrop.SetActive(true);
		}

		public void Drop() {
			areyousuretext.text = "Are you sure you want to drop this?";
			areyousure.SetActive(true);
			StartCoroutine(drop_item());
		}

		public void Areyousureyes() {
			areyousure_yes = true;
		}

		public void Areyousureno() {
			areyousure_no = true;
		}

		IEnumerator drop_item() {
			while (!areyousure_yes && !areyousure_no)
				yield return null;

			if (areyousure_yes && !equips.get_item(bagItems[item_select_index].text.Substring(0, bagItems[item_select_index].text.IndexOf(" x"))).KeyItem) {
				string name = bagItems[item_select_index].text.Substring(0, bagItems[item_select_index].text.IndexOf(" x"));

				Dictionary<string, int> items = SaveSystem.GetStringIntDict("items");
				if (items[name] == 1)
					items.Remove(name);
				else
					items[name] = items[name] - 1;
				SaveSystem.SetStringIntDict("items", items);
			}

			areyousure.SetActive(false);
			givedrop.SetActive(false);

			areyousure_yes = false;
			areyousure_no = false;

			Bag();
		}

		public void give_button() {
			give.SetActive(true);
			StartCoroutine(give_to_player());
		}

		public void use_button() {
			string itemName = bagItems[item_select_index].text.Substring(0, bagItems[item_select_index].text.IndexOf(" x"));

			if (equips.get_item(itemName).SingleUse) {
				useOn.SetActive(true);
				StartCoroutine(use_on_player());
			}
			else
				use_on_party();
		}

		public void select_use_index(int n) {
			useIndex = n;
		}

		IEnumerator use_on_player() {
			usedrop.SetActive(false);

			while (useIndex == -1)
				yield return null;

			string itemName = bagItems[item_select_index].text.Substring(0, bagItems[item_select_index].text.IndexOf(" x"));

			if (equips.use_item(itemName, useIndex)) {
				Dictionary<string, int> partyItems = SaveSystem.GetStringIntDict("items");
				int count = partyItems[itemName];
				if (count == 1)
					partyItems.Remove(itemName);
				else
					partyItems[itemName] = partyItems[itemName] - 1;
				SaveSystem.SetStringIntDict("items", partyItems);

				Setup();
			}

			useOn.SetActive(false);
			usedrop.SetActive(false);

			Bag();

			giveIndex = -1;
		}

		void use_on_party() {
			usedrop.SetActive(false);

			string itemName = bagItems[item_select_index].text.Substring(0, bagItems[item_select_index].text.IndexOf(" x"));

			if (equips.use_item(itemName, -1)) {
				Dictionary<string, int> partyItems = SaveSystem.GetStringIntDict("items");
				int count = partyItems[itemName];
				if (count == 1)
					partyItems.Remove(itemName);
				else
					partyItems[itemName] = partyItems[itemName] - 1;
				SaveSystem.SetStringIntDict("items", partyItems);
			}

			useOn.SetActive(false);
			usedrop.SetActive(false);

			Bag();
		}

		public void select_give_index(int i) {
			giveIndex = i;
		}

		IEnumerator give_to_player() {
			givedrop.SetActive(false);

			while (giveIndex == -1)
				yield return null;

			string itemName = bagItems[item_select_index].text.Substring(0, bagItems[item_select_index].text.IndexOf(" x"));

			string category = equips.item_category(itemName);

			switch (category) {
				case "weapon":
					List<string> weapons = SaveSystem.GetStringList("player" + (giveIndex + 1) + "_weapons_inventory");
					weapons.Add(itemName);
					SaveSystem.SetStringList("player" + (giveIndex + 1) + "_weapons_inventory", weapons);

					Dictionary<string, int> partyItems = SaveSystem.GetStringIntDict("items");
					int count = partyItems[itemName];
					if (count == 1)
						partyItems.Remove(itemName);
					else
						partyItems[itemName] = partyItems[itemName] - 1;

					SaveSystem.SetStringIntDict("items", partyItems);

					break;
				case "armor":
					List<string> armor = SaveSystem.GetStringList("player" + (giveIndex + 1) + "_armor_inventory");
					armor.Add(itemName);
					SaveSystem.SetStringList("player" + (giveIndex + 1) + "_armor_inventory", armor);

					Dictionary<string, int> partyItems1 = SaveSystem.GetStringIntDict("items");
					int count1 = partyItems1[itemName];
					if (count1 == 1)
						partyItems1.Remove(itemName);
					else
						partyItems1[itemName] = partyItems1[itemName] - 1;

					SaveSystem.SetStringIntDict("items", partyItems1);

					break;
			}

			give.SetActive(false);
			givedrop.SetActive(false);

			Bag();

			giveIndex = -1;
		}

		public void status_weapon() {
			equipparty.SetActive(false);
			statusBagCategory.text = "WEAPONS";

			foreach (Text t in statusBagItems) {
				t.text = "";
				t.gameObject.transform.parent.gameObject.SetActive(false);
			}

			List<string> weapons = SaveSystem.GetStringList(status_player_n + "weapons_inventory");

			for (int i = 1; i < weapons.Count; i++) {
				string prefix = "";
				if (SaveSystem.GetString(status_player_n + "weapon") == weapons[i])
					prefix = "E- ";
				statusBagItems[i - 1].text = prefix + weapons[i];
				statusBagItems[i - 1].gameObject.transform.parent.gameObject.SetActive(true);
			}

			statusBag.SetActive(true);
		}

		public void status_armor() {
			equipparty.SetActive(false);
			statusBagCategory.text = "ARMOR";

			foreach (Text t in statusBagItems)
				t.text = "";

			List<string> armor = SaveSystem.GetStringList(status_player_n + "armor_inventory");

			for (int i = 1; i < armor.Count; i++) {
				string category = equips.get_armor(armor[i]).Category;
				string prefix = "";

				if (SaveSystem.GetString(status_player_n + category) == armor[i])
					prefix = "E- ";

				statusBagItems[i - 1].text = prefix + armor[i];
			}

			statusBag.SetActive(true);
		}

		public void select_status_item_n(int n) {
			item_select_status_index = n;
			string name = statusBagItems[n].text;
			if (name == "")
				return;
			equipparty.SetActive(true);
		}

		public void Equip() {
			string name = statusBagItems[item_select_status_index].text;
			string category = equips.item_category(name);

			if (name.Contains("E- ")) {
				category = equips.item_category(name.Substring(3));
				switch (category) {
					case "armor":
						string armorType = equips.get_armor(name).Category;

						switch (armorType) {
							case "armor":
								SaveSystem.SetString(status_player_n + "armor", "");
								break;
							case "shield":
								SaveSystem.SetString(status_player_n + "shield", "");
								break;
							case "helmet":
								SaveSystem.SetString(status_player_n + "helmet", "");
								break;
							case "glove":
								SaveSystem.SetString(status_player_n + "glove", "");
								break;
						}

						statusBagItems[item_select_status_index].text = name.Substring(3);

						break;
					case "weapon":
						SaveSystem.SetString(status_player_n + "weapon", "");
						statusBagItems[item_select_status_index].text = name.Substring(3);
						break;
				}
			}
			else {
				switch (category) {
					case "armor":
						string armorType = equips.get_armor(name).Category;

						string playerClass = SaveSystem.GetString(status_player_n + "class");

						if (equips.can_equip_armor(equips.get_armor(name), playerClass)) {

							if (SaveSystem.GetString(status_player_n + armorType) != "") {
								string alreadyEquipped = SaveSystem.GetString(status_player_n + armorType);
								foreach (Text t in statusBagItems) {
									if (t.text == "E- " + alreadyEquipped) {
										t.text = t.text.Substring(3, t.text.Length - 3);
										break;
									}
								}
							}

							switch (armorType) {
								case "armor":
									SaveSystem.SetString(status_player_n + "armor", name);
									break;
								case "shield":
									SaveSystem.SetString(status_player_n + "shield", name);
									break;
								case "helmet":
									SaveSystem.SetString(status_player_n + "helmet", name);
									break;
								case "glove":
									SaveSystem.SetString(status_player_n + "glove", name);
									break;
							}

							statusBagItems[item_select_status_index].text = "E- " + name;
						}
						break;
					case "weapon":
						string playerClass1 = SaveSystem.GetString(status_player_n + "class");
						if (equips.can_equip_weapon(equips.get_weapon(name), playerClass1)) {
							if (SaveSystem.GetString(status_player_n + "weapon") != "") {
								string alreadyEquipped = SaveSystem.GetString(status_player_n + "weapon");
								foreach (Text t in statusBagItems) {
									if (t.text == "E- " + alreadyEquipped) {
										t.text = t.text.Substring(3, t.text.Length - 3);
										break;
									}
								}
							}
							SaveSystem.SetString(status_player_n + "weapon", statusBagItems[item_select_status_index].text);
							statusBagItems[item_select_status_index].text = "E- " + statusBagItems[item_select_status_index].text;
						}
						break;
				}
			}

			equipparty.SetActive(false);
		}

		public void send_to_party() {
			string name = statusBagItems[item_select_status_index].text;
			if (name == "")
				return;
			if (name.Contains("E- "))
				name = name.Substring(3);
			string category = equips.item_category(name);

			//Unequip
			switch (category) {
				case "weapon":
					if (name == SaveSystem.GetString(status_player_n + "weapon"))
						SaveSystem.SetString(status_player_n + "weapon", "");
					break;
				case "armor":
					if (name == SaveSystem.GetString(status_player_n + "armor"))
						SaveSystem.SetString(status_player_n + "armor", "");
					if (name == SaveSystem.GetString(status_player_n + "helmet"))
						SaveSystem.SetString(status_player_n + "helmet", "");
					if (name == SaveSystem.GetString(status_player_n + "shield"))
						SaveSystem.SetString(status_player_n + "shield", "");
					if (name == SaveSystem.GetString(status_player_n + "glove"))
						SaveSystem.SetString(status_player_n + "weapon", "");
					break;
			}

			Dictionary<string, int> items = SaveSystem.GetStringIntDict("items");
			if (items.ContainsKey(name))
				items[name] = items[name] + 1;
			else
				items.Add(name, 1);

			SaveSystem.SetStringIntDict("items", items);

			equipparty.SetActive(false);

			switch (category) {
				case "armor":
					List<string> armor = SaveSystem.GetStringList(status_player_n + "armor_inventory");
					armor.Remove(name);
					SaveSystem.SetStringList(status_player_n + "armor_inventory", armor);
					status_armor();
					break;
				case "weapon":
					List<string> weapons = SaveSystem.GetStringList(status_player_n + "weapons_inventory");
					weapons.Remove(name);
					SaveSystem.SetStringList(status_player_n + "weapons_inventory", weapons);
					status_weapon();
					break;
			}

			statusBagItems[item_select_status_index].text = "";
		}

		public void status_off() {
			if (equipparty.active)
				equipparty.SetActive(false);
			if (statusBag.active)
				statusBag.SetActive(false);
			else {
				pausemenuContainer.SetActive(true);
				bagObj.SetActive(false);
				bagBtn.SetActive(true);
				give.SetActive(false);
				givedrop.SetActive(false);
				statusContainer.SetActive(false);
			}
		}

		public void Off() {
			if (give.active)
				give.SetActive(false);
			else if (givedrop.active)
				givedrop.SetActive(false);
			else if (bagObj.active) {
				bagObj.SetActive(false);
				bagBtn.SetActive(true);
			}
			else {
				Cursor.visible = false;
				pausemenuContainer.SetActive(false);
				statusContainer.SetActive(false);
				musicContainer.SetActive(false);
				overworldSceneContainer.SetActive(true);
				bagObj.SetActive(false);
				givedrop.SetActive(false);
			}
		}

		public void On() {
			overworldSceneContainer.SetActive(false);
			pausemenuContainer.SetActive(true);
			musicContainer.SetActive(true);
			Cursor.visible = true;
		}

		public void Items() {
			Dictionary<string, int> dict = SaveSystem.GetStringIntDict("items");
			foreach (KeyValuePair<string, int> kvp in dict)
				Debug.Log(kvp.Key + " x" + kvp.Value);
		}

		public void Quit() {
			areyousuretext.text = "Do you want to quit? Any unsaved progress will be lost.";
			areyousure.SetActive(true);
			StartCoroutine(quit_coroutine());
		}

		IEnumerator quit_coroutine() {
			while (!areyousure_yes && !areyousure_no)
				yield return null;

			if (areyousure_yes)
				actually_quit_application();
			else
				areyousure.SetActive(false);

			areyousure_yes = false;
			areyousure_no = false;

			yield return null;
		}

		void actually_quit_application() {
#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
		}
	}
}
