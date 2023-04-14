using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Battling;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils.SaveGame.Scripts.SaveSystem;

namespace Overworld {
	public class Shop : MonoBehaviour {

		public Transform[] party;
		[FormerlySerializedAs("party_clones")] public List<GameObject> partyClones;

		[FormerlySerializedAs("party_names")] public string[] partyNames;

		public string shopmode;

		[FormerlySerializedAs("prompt_text")] public Text promptText;
		[FormerlySerializedAs("gil_display_text")]
		public Text gilDisplayText;
		[FormerlySerializedAs("shop_type_text")]
		public Text shopTypeText;

		[FormerlySerializedAs("buy_container")]
		public GameObject buyContainer;
		[FormerlySerializedAs("buy_cursor")] public GameObject buyCursor;
		[FormerlySerializedAs("product_Texts")]
		public ProductText[] productTexts;

		[FormerlySerializedAs("learn_spell_container")]
		public GameObject learnSpellContainer;
		[FormerlySerializedAs("learn_cursor")] public GameObject learnCursor;
		[FormerlySerializedAs("spell_party_names")]
		public Text[] spellPartyNames;

		[FormerlySerializedAs("yes_no")] public GameObject yesNo;
		[FormerlySerializedAs("buy_sell_quit")]
		public GameObject buySellQuit;
		[FormerlySerializedAs("only_quit")] public GameObject onlyQuit;

		public FadeOut fadeOut;
		[FormerlySerializedAs("shop_music")] public MusicHandler shopMusic;
		[FormerlySerializedAs("inn_music")] public MusicHandler innMusic;

		int buy_index;

		List<int> dead_players;

		int inn_clinic_price;

		int learn_index = -1;
		bool no;
		int player_gil;

		SpriteController sc;
		bool sell;

		bool yes;

		// Start is called before the first frame update
		void Start() {
			partyClones = new List<GameObject>();
			partyNames = new string[4];

			buyContainer.SetActive(false);

			for (int i = 0; i < 4; i++) {
				partyNames[i] = SaveSystem.GetString("player" + (i + 1) + "_name");
				spellPartyNames[i].text = SaveSystem.GetString("player" + (i + 1) + "_name");
			}

			//if (shopmode == null)
			shopmode = GlobalControl.Instance.shopmode;

			if (shopmode != "inn" && shopmode != "clinic")
				Setup();
			else {
				yesNo.SetActive(true);
				buySellQuit.SetActive(false);
				onlyQuit.SetActive(false);
				inn_clinic_price = GlobalControl.Instance.innClinicPrice;

				if (shopmode == "inn")
					promptText.text = "Stay to heal and save your data? A room will be " + inn_clinic_price + "G per night.";
				if (shopmode == "clinic")
					clinic_setup();
			}

			sc = GetComponentInChildren<SpriteController>();
			switch (shopmode) {
				case "armor":
					sc.set_character(0);
					shopTypeText.text = "ARMOR";
					break;
				case "b_magic":
					sc.set_character(1);
					shopTypeText.text = "BLACK MAGIC";
					break;
				case "clinic":
					sc.set_character(2);
					shopTypeText.text = "CLINIC";
					break;
				case "inn":
					sc.set_character(3);
					shopTypeText.text = "INN";
					break;
				case "item":
					sc.set_character(4);
					shopTypeText.text = "ITEM";
					break;
				case "oasis":
					sc.set_character(5);
					shopTypeText.text = "OASIS";
					break;
				case "w_magic":
					sc.set_character(6);
					shopTypeText.text = "WHITE MAGIC";
					break;
				case "weapon":
					sc.set_character(7);
					shopTypeText.text = "WEAPON";
					break;
			}
			sc.change_direction("up");

			for (int i = 0; i < 4; i++) {
				string playerN = "player" + (i + 1) + "_";
				string job = SaveSystem.GetString("player" + (i + 1) + "_class");
				switch (job) {
					case "fighter":
						GameObject fighter = Instantiate(Resources.Load<GameObject>("party/fighter"), party[i].position, Quaternion.identity);
						partyClones.Add(fighter);
						fighter.transform.localScale = party[i].localScale;
						Destroy(fighter.GetComponent<PartyMember>());
						break;
					case "black_belt":
						GameObject bb = Instantiate(Resources.Load<GameObject>("party/black_belt"), party[i].position, Quaternion.identity);
						partyClones.Add(bb);
						bb.transform.localScale = party[i].localScale;
						Destroy(bb.GetComponent<PartyMember>());
						break;
					case "red_mage":
						GameObject redMage = Instantiate(Resources.Load<GameObject>("party/red_mage"), party[i].position, Quaternion.identity);
						partyClones.Add(redMage);
						redMage.transform.localScale = party[i].localScale;
						Destroy(redMage.GetComponent<PartyMember>());
						break;
					case "thief":
						GameObject thief = Instantiate(Resources.Load<GameObject>("party/thief"), party[i].position, Quaternion.identity);
						partyClones.Add(thief);
						thief.transform.localScale = party[i].localScale;
						Destroy(thief.GetComponent<PartyMember>());
						break;
					case "white_mage":
						GameObject whiteMage = Instantiate(Resources.Load<GameObject>("party/white_mage"), party[i].position, Quaternion.identity);
						partyClones.Add(whiteMage);
						whiteMage.transform.localScale = party[i].localScale;
						Destroy(whiteMage.GetComponent<PartyMember>());
						break;
					case "black_mage":
						GameObject blackMage = Instantiate(Resources.Load<GameObject>("party/black_mage"), party[i].position, Quaternion.identity);
						partyClones.Add(blackMage);
						blackMage.transform.localScale = party[i].localScale;
						Destroy(blackMage.GetComponent<PartyMember>());
						break;
				}
			}

			player_gil = SaveSystem.GetInt("gil");

			gilDisplayText.text = "G: " + player_gil;
		}

		// Update is called once per frame
		void Update() {

			if (Input.GetKeyDown(CustomInputManager.Cim.Back)) {
				if (buySellQuit.activeSelf || shopmode == "clinic" || shopmode == "inn")
					exit_shop();
				else if (buyContainer.activeSelf)
					Setup();
			}

			if (player_gil != SaveSystem.GetInt("gil")) {
				player_gil = SaveSystem.GetInt("gil");

				gilDisplayText.text = "G: " + player_gil;
			}

			if ((shopmode == "inn" || shopmode == "clinic") && (yes || no)) {
				if (yes && player_gil < inn_clinic_price)
					Debug.Log("You don't have enough money!");
				else if (yes) {
					SaveSystem.SetInt("gil", player_gil - inn_clinic_price);
					if (shopmode == "inn") {
						for (int i = 0; i < 4; i++) {
							if (SaveSystem.GetInt("player" + (i + 1) + "_HP") > 0)
								SaveSystem.SetInt("player" + (i + 1) + "_HP", SaveSystem.GetInt("player" + (i + 1) + "_maxHP"));
						}

						GlobalControl.Instance.player.mapHandler.save_inn();

						SaveSystem.SaveToDisk();

						promptText.text = "Thank you for staying the night! Sleep well!";

						yesNo.SetActive(false);

						StartCoroutine(inn_sleep());

						yes = false;
						no = false;
					}
					else {
						SaveSystem.SetInt("player" + (dead_players[0] + 1) + "_HP", 1);
						yes = false;
						no = false;
						clinic_setup();
					}
				}
			}
		}

		void Setup() {
			foreach (ProductText t in productTexts)
				t.name.transform.parent.gameObject.SetActive(false);

			yesNo.SetActive(false);
			buySellQuit.SetActive(true);
			onlyQuit.SetActive(false);
			buyContainer.SetActive(false);

			learnSpellContainer.SetActive(false);
			learnCursor.SetActive(false);

			for (int i = 0; i < GlobalControl.Instance.ShopProducts.Count; i++) {
				string prodName = GlobalControl.Instance.ShopProducts.ElementAt(i).Key;
				int prodCost = GlobalControl.Instance.ShopProducts.ElementAt(i).Value;

				productTexts[i].name.text = prodName;
				productTexts[i].cost.text = "" + prodCost;

				if (prodName.Length > 1)
					productTexts[i].name.transform.parent.gameObject.SetActive(true);
			}
			promptText.text = "What do you need?";
		}

		void clinic_setup() {
			dead_players = new List<int>();

			for (int i = 0; i < 4; i++) {
				if (SaveSystem.GetInt("player" + (i + 1) + "_HP") <= 0)
					dead_players.Add(i);
			}

			if (dead_players.Count > 0) {
				promptText.text = "It seems " + SaveSystem.GetString("player" + (dead_players[0] + 1) + "_name") + " has fallen. Would you like me to revive them for " + inn_clinic_price + "G?";
				yesNo.SetActive(true);
			}
			else {
				promptText.text = "You do not need my help right now.";
				yesNo.SetActive(false);
				onlyQuit.SetActive(true);
			}
		}

		public void yes_true() {
			yes = true;
			no = false;
		}

		public void no_true() {
			yes = false;
			no = true;
		}

		public void Buy() {
			buyContainer.SetActive(true);
			buyCursor.SetActive(true);
			buySellQuit.SetActive(false);

			if (shopmode == "b_magic" || shopmode == "w_magic")
				promptText.text = "Which spell would you like to learn?";
			else
				promptText.text = "What would you like?";
		}

		public void sell_true() {
			sell = true;
		}

		public void exit_shop() {
			foreach (GameObject t in partyClones)
				Destroy(t);
			// close music
			shopMusic.get_active().Stop();
			SceneManager.UnloadSceneAsync("Shop");
			GlobalControl.Instance.overworldSceneContainer.SetActive(true);
		}

		IEnumerator inn_sleep() {

			shopMusic.get_active().Stop();
			innMusic.get_active().Play();

			fadeOut.start_fade(true);

			yield return new WaitForSeconds(.5f);

			while (fadeOut.is_fading())
				yield return null;

			fadeOut.start_fade(false);

			yield return new WaitForSeconds(.5f);

			while (innMusic.get_active().isPlaying)
				yield return null;

			exit_shop();
		}

		public void buy_n(int n) {
			if (shopmode == "w_magic" || shopmode == "b_magic")
				StartCoroutine(learn_select(n));
			else
				StartCoroutine(buy_select(n));
		}

		IEnumerator buy_select(int index) {

			buyCursor.SetActive(false);

			yes = false;
			no = false;

			string pName = productTexts[index].name.text;
			int pCost = int.Parse(productTexts[index].cost.text);

			promptText.text = "Is " + pCost + "G for a " + pName + " okay?";
			yesNo.SetActive(true);

			while (!yes && !no)
				yield return null;

			if (yes) {
				if (player_gil >= pCost) {
					switch (shopmode) {
						case "item":
							Dictionary<string, int> items = SaveSystem.GetStringIntDict("items");
							if (items.ContainsKey(pName))
								items[pName] = items[pName] + 1;
							else
								items.Add(pName, 1);
							SaveSystem.SetStringIntDict("items", items);
							SaveSystem.SetInt("gil", player_gil - pCost);
							break;
						case "weapon":
							Dictionary<string, int> items1 = SaveSystem.GetStringIntDict("items");
							if (items1.ContainsKey(pName))
								items1[pName] = items1[pName] + 1;
							else
								items1.Add(pName, 1);
							SaveSystem.SetStringIntDict("items", items1);
							SaveSystem.SetInt("gil", player_gil - pCost);
							break;
						case "armor":
							Dictionary<string, int> items2 = SaveSystem.GetStringIntDict("items");
							if (items2.ContainsKey(pName))
								items2[pName] = items2[pName] + 1;
							else
								items2.Add(pName, 1);
							SaveSystem.SetStringIntDict("items", items2);
							SaveSystem.SetInt("gil", player_gil - pCost);
							break;
					}
				}
				else {
					promptText.text = "You don't have enough money";
					yesNo.SetActive(false);
					yield return new WaitForSeconds(1.5f);
				}
			}

			Setup();
		}

		public void learn_n(int n) {
			learn_index = n;
		}

		IEnumerator learn_select(int index) {

			promptText.text = "Who will learn this spell?";

			buyContainer.SetActive(false);
			buyCursor.SetActive(false);

			learnSpellContainer.SetActive(true);
			learnCursor.SetActive(true);

			while (learn_index == -1)
				yield return null;

			if (!new Equips().get_spell(productTexts[index].name.text).LearnBy.Contains(SaveSystem.GetString("player" + (learn_index + 1) + "_class"))) {
				promptText.text = "They can't learn this spell!";
				yield return new WaitForSeconds(1f);
			}
			else {
				buyCursor.SetActive(false);

				yes = false;
				no = false;

				string pName = productTexts[index].name.text;
				int pCost = int.Parse(productTexts[index].cost.text);

				promptText.text = "Is " + pCost + "G for " + partyNames[index] + " to learn " + pName + " okay?";
				yesNo.SetActive(true);

				while (!yes && !no)
					yield return null;

				yesNo.SetActive(false);

				if (yes) {
					if (player_gil >= pCost) {
						int level = new Equips().get_spell(productTexts[index].name.text).Level;
						List<string> levelNSpells = SaveSystem.GetStringList("player" + (learn_index + 1) + "_level_" + level + "_spells");


						if (levelNSpells.Count > 3) {
							promptText.text = "You have the maximum amount of level " + level + " spells";
							yield return new WaitForSeconds(2f);
						}
						else if (levelNSpells.Contains(pName)) {
							promptText.text = "You already know this spell!";
							yield return new WaitForSeconds(2f);
						}
						else if (level > SaveSystem.GetInt("player" + (learn_index + 1) + "_magic_level")) {
							promptText.text = "Your magic level is not yet high enough to learn this spell.";
							yield return new WaitForSeconds(2f);
						}
						else {
							levelNSpells.Add(pName);
							SaveSystem.SetStringList("player" + (learn_index + 1) + "_level_" + level + "_spells", levelNSpells);
							SaveSystem.SetInt("gil", player_gil - pCost);

							promptText.text = "Thank you!";
							yield return new WaitForSeconds(2f);
						}
					}
					else {
						promptText.text = "You don't have enough money";
						yield return new WaitForSeconds(2f);
					}
				}
			}

			Setup();
			learn_index = -1;

			yield return null;
		}

		[Serializable]
		public class ProductText {
			public Text name;
			public Text cost;
		}
	}
}
