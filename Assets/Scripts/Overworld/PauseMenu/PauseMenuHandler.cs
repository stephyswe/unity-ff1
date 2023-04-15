using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Overworld.Controller;
using Refactor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils.SaveGame.Scripts.SaveSystem;

namespace Overworld.PauseMenu {
	public partial class PauseMenuHandler : MonoBehaviour {
		public PlayerController player;
		[FormerlySerializedAs("overworld_scene_container")]
		public GameObject overworldSceneContainer;
		[FormerlySerializedAs("pausemenu_container")]
		// ReSharper disable once IdentifierTypo
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
		[FormerlySerializedAs("givedrop")] public GameObject giveDrop;
		[FormerlySerializedAs("usedrop")] public GameObject useDrop;
		[FormerlySerializedAs("equipparty")] public GameObject equipParty;

		public GameObject give;
		[FormerlySerializedAs("use_on")] public GameObject useOn;

		[FormerlySerializedAs("areyousure")] public GameObject areYouSure;
		[FormerlySerializedAs("areyousuretext")] public Text areYouSureText;

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
		bool areYouSureNo;
		bool areYouSureYes;
		Equips equips;
		int item_select_index;
		int item_select_status_index;
		string status_player_n;


		public PauseMenuHandler() {
			Common = new Common(this);
			DataSave = new DataSave(this);

		}
		public Common Common { get; }
		DataSave DataSave { get; }


		void Start() {
			// setup
			Common.Setup();
			
			// overworld, pause menu & status scene container
			overworldSceneContainer.SetActive(true);
			pausemenuContainer.SetActive(false);
			statusContainer.SetActive(false);
			
			// activate buttons
			bagObj.SetActive(false);
			give.SetActive(false);
			giveDrop.SetActive(false);
			equipParty.SetActive(false);
			useDrop.SetActive(false);
			useOn.SetActive(false);

			// setup equips
			equips = new Equips();
		}

		// Update is called once per frame
		void Update() {
			// open on escape key
			if (!Input.GetKeyDown("escape"))
				return;
			
			// 
			if (SceneManager.sceneCount == 1 && player.canMove) {}
			On();
		}

		void OnEnable() {
			// de-activate second pause menu
			statusContainer.SetActive(false);
			bagObj.SetActive(false);
			giveDrop.SetActive(false);
			equipParty.SetActive(false);
		}

		public void hover_sound() {
			// play sound on hover
			if (bagObj.activeSelf) {}
			buttonHover.Play();
		}



		public int get_level_from_exp(int exp) {
			Dictionary<int, int> levelChart = LevelChart.GetLevelChart();
			int level = 1;

			foreach (KeyValuePair<int, int> entry in levelChart.TakeWhile(entry => entry.Value <= exp)) {
				level = entry.Key;
			}
			return level;
		}

		public int exp_till_level(int exp) {
			Dictionary<int, int> levelChart = LevelChart.GetLevelChart();
			int level = get_level_from_exp(exp);

			return levelChart[level + 1] - exp;
		}

		// Start is called before the first frame update

		public void status_n(int n) {
			Status(n);
		}

		public void Bag() {

			giveDrop.SetActive(false);
			useDrop.SetActive(false);

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

			// set sprite
			Common.SwitchStatusCharacter(job);

			// set name and save
			DataSave.StatusTextCharacter(playerN);

			statusContainer.SetActive(true);
		}

		public void select_item_n(int n) {
			if (bagItems[n].text == "")
				return;
			item_select_index = n;
			Select();
		}

		void Select() {
			for (int i = 0; i < 4; i++) {
				giveNames[i].text = SaveSystem.GetString("player" + (i + 1) + "_name");
				useNames[i].text = SaveSystem.GetString("player" + (i + 1) + "_name");
			}

			string itemName = bagItems[item_select_index].text[..bagItems[item_select_index].text.IndexOf(" x", StringComparison.Ordinal)];

			string category = equips.item_category(itemName);

			Debug.Log(itemName);

			switch (category) {
				case "weapon" or "armor":
					giveDrop.SetActive(true);
					break;
				case "item":
					useDrop.SetActive(true);
					break;
			}
		}

		public void Drop() {
			areYouSureText.text = "Are you sure you want to drop this?";
			areYouSure.SetActive(true);
			StartCoroutine(drop_item());
		}

		// ReSharper disable once IdentifierTypo
		public void Areyousureyes() {
			areYouSureYes = true;
		}

		// ReSharper disable once IdentifierTypo
		public void Areyousureno() {
			areYouSureNo = true;
		}

		IEnumerator drop_item() {
			while (!areYouSureYes && !areYouSureNo)
				yield return null;

			if (areYouSureYes && !equips.get_item(bagItems[item_select_index].text[..bagItems[item_select_index].text.IndexOf(" x", StringComparison.Ordinal)]).KeyItem) {
				string bagItem = bagItems[item_select_index].text.Substring(0, bagItems[item_select_index].text.IndexOf(" x", StringComparison.Ordinal));

				Dictionary<string, int> items = SaveSystem.GetStringIntDict("items");
				if (items[bagItem] == 1)
					items.Remove(bagItem);
				else
					items[bagItem] -= 1;
				SaveSystem.SetStringIntDict("items", items);
			}

			areYouSure.SetActive(false);
			giveDrop.SetActive(false);

			areYouSureYes = false;
			areYouSureNo = false;

			Bag();
		}

		public void give_button() {
			give.SetActive(true);
			StartCoroutine(give_to_player());
		}

		public void use_button() {
			string itemName = bagItems[item_select_index].text[..bagItems[item_select_index].text.IndexOf(" x", StringComparison.Ordinal)];

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

		// ReSharper disable Unity.PerformanceAnalysis
		IEnumerator use_on_player() {
			useDrop.SetActive(false);

			while (useIndex == -1)
				yield return null;

			string itemName = bagItems[item_select_index].text[..bagItems[item_select_index].text.IndexOf(" x", StringComparison.Ordinal)];

			if (equips.use_item(itemName, useIndex)) {
				Dictionary<string, int> partyItems = SaveSystem.GetStringIntDict("items");
				int count = partyItems[itemName];
				if (count == 1)
					partyItems.Remove(itemName);
				else
					partyItems[itemName] -= 1;
				SaveSystem.SetStringIntDict("items", partyItems);

				Common.Setup();
			}

			useOn.SetActive(false);
			useDrop.SetActive(false);

			Bag();

			giveIndex = -1;
		}

		void use_on_party() {
			useDrop.SetActive(false);

			string itemName = bagItems[item_select_index].text.Substring(0, bagItems[item_select_index].text.IndexOf(" x"));

			if (equips.use_item(itemName, -1)) {
				Dictionary<string, int> partyItems = SaveSystem.GetStringIntDict("items");
				int count = partyItems[itemName];
				if (count == 1)
					partyItems.Remove(itemName);
				else
					partyItems[itemName] -= 1;
				SaveSystem.SetStringIntDict("items", partyItems);
			}

			useOn.SetActive(false);
			useDrop.SetActive(false);

			Bag();
		}

		public void select_give_index(int i) {
			giveIndex = i;
		}

		IEnumerator give_to_player() {
			giveDrop.SetActive(false);

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
						partyItems[itemName] -= 1;

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
						partyItems1[itemName] -= 1;

					SaveSystem.SetStringIntDict("items", partyItems1);

					break;
			}

			give.SetActive(false);
			giveDrop.SetActive(false);

			Bag();

			giveIndex = -1;
		}

		public void status_weapon() {
			equipParty.SetActive(false);
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
			equipParty.SetActive(false);
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
			string statusItem = statusBagItems[n].text;
			if (statusItem == "")
				return;
			equipParty.SetActive(true);
		}

		public void Equip() {
			string itemName = statusBagItems[item_select_status_index].text;
			string category = equips.item_category(itemName);

			if (itemName.Contains("E- ")) {
				category = equips.item_category(itemName.Substring(3));
				switch (category) {
					case "armor":
						string armorType = equips.get_armor(itemName).Category;

						SaveSystem.SetString(status_player_n + armorType, "");

						statusBagItems[item_select_status_index].text = itemName.Substring(3);

						break;
					case "weapon":
						SaveSystem.SetString(status_player_n + "weapon", "");
						statusBagItems[item_select_status_index].text = itemName.Substring(3);
						break;
				}
			}
			else {
				switch (category) {
					case "armor":
						string armorType = equips.get_armor(itemName).Category;

						string playerClass = SaveSystem.GetString(status_player_n + "class");

						if (equips.can_equip_armor(equips.get_armor(itemName), playerClass)) {

							if (SaveSystem.GetString(status_player_n + armorType) != "") {
								string alreadyEquipped = SaveSystem.GetString(status_player_n + armorType);
								foreach (Text t in statusBagItems) {
									if (t.text != "E- " + alreadyEquipped)
										continue;
									t.text = t.text.Substring(3, t.text.Length - 3);
									break;
								}
							}

							switch (armorType) {
								case "armor":
									SaveSystem.SetString(status_player_n + "armor", itemName);
									break;
								case "shield":
									SaveSystem.SetString(status_player_n + "shield", itemName);
									break;
								case "helmet":
									SaveSystem.SetString(status_player_n + "helmet", itemName);
									break;
								case "glove":
									SaveSystem.SetString(status_player_n + "glove", itemName);
									break;
							}

							statusBagItems[item_select_status_index].text = "E- " + itemName;
						}
						break;
					case "weapon":
						string playerClass1 = SaveSystem.GetString(status_player_n + "class");
						if (equips.can_equip_weapon(equips.get_weapon(itemName), playerClass1)) {
							if (SaveSystem.GetString(status_player_n + "weapon") != "") {
								string alreadyEquipped = SaveSystem.GetString(status_player_n + "weapon");
								foreach (Text t in statusBagItems) {
									if (t.text != "E- " + alreadyEquipped)
										continue;
									t.text = t.text.Substring(3, t.text.Length - 3);
									break;
								}
							}
							SaveSystem.SetString(status_player_n + "weapon", statusBagItems[item_select_status_index].text);
							statusBagItems[item_select_status_index].text = "E- " + statusBagItems[item_select_status_index].text;
						}
						break;
				}
			}

			equipParty.SetActive(false);
		}

		public void send_to_party() {
			string partyItem = statusBagItems[item_select_status_index].text;
			if (partyItem == "")
				return;
			if (partyItem.Contains("E- "))
				partyItem = partyItem.Substring(3);
			string category = equips.item_category(partyItem);

			//Unequip
			switch (category) {
				case "weapon":
					if (partyItem == SaveSystem.GetString(status_player_n + "weapon"))
						SaveSystem.SetString(status_player_n + "weapon", "");
					break;
				case "armor":
					if (partyItem == SaveSystem.GetString(status_player_n + "armor"))
						SaveSystem.SetString(status_player_n + "armor", "");
					if (partyItem == SaveSystem.GetString(status_player_n + "helmet"))
						SaveSystem.SetString(status_player_n + "helmet", "");
					if (partyItem == SaveSystem.GetString(status_player_n + "shield"))
						SaveSystem.SetString(status_player_n + "shield", "");
					if (partyItem == SaveSystem.GetString(status_player_n + "glove"))
						SaveSystem.SetString(status_player_n + "weapon", "");
					break;
			}

			Dictionary<string, int> items = SaveSystem.GetStringIntDict("items");
			if (items.ContainsKey(partyItem))
				items[partyItem] += 1;
			else
				items.Add(partyItem, 1);

			SaveSystem.SetStringIntDict("items", items);

			equipParty.SetActive(false);

			switch (category) {
				case "armor":
					List<string> armor = SaveSystem.GetStringList(status_player_n + "armor_inventory");
					armor.Remove(partyItem);
					SaveSystem.SetStringList(status_player_n + "armor_inventory", armor);
					status_armor();
					break;
				case "weapon":
					List<string> weapons = SaveSystem.GetStringList(status_player_n + "weapons_inventory");
					weapons.Remove(partyItem);
					SaveSystem.SetStringList(status_player_n + "weapons_inventory", weapons);
					status_weapon();
					break;
			}

			statusBagItems[item_select_status_index].text = "";
		}

		public void status_off() {
			if (equipParty.activeSelf)
				equipParty.SetActive(false);
			if (statusBag.activeSelf)
				statusBag.SetActive(false);
			else {
				pausemenuContainer.SetActive(true);
				bagObj.SetActive(false);
				bagBtn.SetActive(true);
				give.SetActive(false);
				giveDrop.SetActive(false);
				statusContainer.SetActive(false);
			}
		}

		public void Off() {
			if (give.activeSelf)
				give.SetActive(false);
			else if (giveDrop.activeSelf)
				giveDrop.SetActive(false);
			else if (bagObj.activeSelf) {
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
				giveDrop.SetActive(false);
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
			areYouSureText.text = "Do you want to quit? Any unsaved progress will be lost.";
			areYouSure.SetActive(true);
			StartCoroutine(quit_coroutine());
		}

		IEnumerator quit_coroutine() {
			while (!areYouSureYes && !areYouSureNo)
				yield return null;

			if (areYouSureYes)
				actually_quit_application();
			else
				areYouSure.SetActive(false);

			areYouSureYes = false;
			areYouSureNo = false;

			yield return null;
		}

		static void actually_quit_application() {
		#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
		#endif
		}
	}
}
