using System.Collections;
using System.Collections.Generic;
using Overworld;
using Refactor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils.SaveGame.Scripts.SaveSystem;

namespace Battling {
	public class PartyMember : Battler {
		public int index;

		[FormerlySerializedAs("maxHP")] public int maxHp;
		public string job;
		public string weapon;
		public int strength;
		public int agility;
		public int intelligence;
		public int vitality;
		public int luck;

		public int experience;
		public int level;

		[FormerlySerializedAs("can_run")] public bool canRun;

		public BattleHandler bh;

		[FormerlySerializedAs("move_point")] public Vector3 movePoint;

		public BattleSpriteController bsc;

		[FormerlySerializedAs("weapon_sprite")]
		public WeaponSprite weaponSprite;
		[FormerlySerializedAs("magic_sprite")] public MagicSprite magicSprite;

		public string action;
		public GameObject target;

		[FormerlySerializedAs("done_showing")] public bool doneShowing = true;

		[FormerlySerializedAs("done_set_up")] public bool doneSetUp;

		bool choosing;

		string drink_chosen;
		CursorController menu_cursor;

		CursorController monster_cursor;

		float timer = -1;
		List<float> times;

		// Start is called before the first frame update
		void Start() {
			doneSetUp = false;

			movePoint = transform.position;
			if (!GlobalControl.Instance.bossmode)
				monster_cursor = bh.monsterCursor;
			menu_cursor = bh.menuCursor;

			menu_cursor.gameObject.SetActive(false);
			if (!GlobalControl.Instance.bossmode)
				monster_cursor.gameObject.SetActive(false);

			bsc = GetComponent<BattleSpriteController>();

			weapon = SaveSystem.GetString("player" + (index + 1) + "_weapon");
			weaponSprite.set_sprite(bh.mwsh.get_weapon(weapon));

			doneSetUp = true;

			doneShowing = true;
		}

		// Update is called once per frame
		void Update() {
			if (hp <= 0 && bsc.get_state() != "dead")
				bsc.change_state("dead");

			if (transform.position == movePoint && bsc.get_state() != "idle" && bsc.get_state() != "victory" && hp > 0)
				bsc.change_state("idle");

			if (bh.partySelecting && GlobalControl.Instance.bossmode)
				menu_cursor.gameObject.SetActive(true);

			transform.position = Vector3.MoveTowards(transform.position, movePoint, 8 * Time.deltaTime);
			/*
		if (timer >= 0)
		{
			timer += Time.deltaTime;
			if (transform.position == move_point)
			{
			    stopTimer();
			}
		}
		*/
		}

		public List<string> level_up() {

			level += 1;

			int seed = 0;

			List<string> statsIncreased = new List<string>();

			int sOld = strength;
			int aOld = agility;
			int iOld = intelligence;
			int vOld = vitality;
			int lOld = luck;

			switch (SaveSystem.GetString("player" + (index + 1) + "_class")) {
				case "fighter":
					seed = Random.Range(1, 8);
					if (seed > 0)
						strength += 1;
					seed = Random.Range(1, 8);
					if (seed > 2)
						agility += 1;
					seed = Random.Range(1, 8);
					if (seed > 4)
						intelligence += 1;
					seed = Random.Range(1, 8);
					if (seed > 3)
						vitality += 1;
					seed = Random.Range(1, 8);
					if (seed > 2)
						luck += 1;

					seed = Random.Range(1, 100);
					if (seed > 50)
						maxHp += 20 + vitality / 4 + Random.Range(1, 6);
					else
						maxHp += vitality / 4 + 1;

					hit += .03f;
					magicDefense += .03f;

					break;
				case "black_belt":
					seed = Random.Range(1, 8);
					if (seed > 3)
						strength += 1;
					seed = Random.Range(1, 8);
					if (seed > 3)
						agility += 1;
					seed = Random.Range(1, 8);
					if (seed > 3)
						intelligence += 1;
					seed = Random.Range(1, 8);
					if (seed > 0)
						vitality += 1;
					seed = Random.Range(1, 8);
					if (seed > 3)
						luck += 1;

					seed = Random.Range(1, 100);
					if (seed > 60)
						maxHp += 20 + vitality / 4 + Random.Range(1, 6);
					else
						maxHp += vitality / 4 + 1;

					hit += .03f;
					magicDefense += .04f;

					break;
				case "red_mage":
					seed = Random.Range(1, 8);
					if (seed > 3)
						strength += 1;
					seed = Random.Range(1, 8);
					if (seed > 3)
						agility += 1;
					seed = Random.Range(1, 8);
					if (seed > 3)
						intelligence += 1;
					seed = Random.Range(1, 8);
					if (seed > 3)
						vitality += 1;
					seed = Random.Range(1, 8);
					if (seed > 3)
						luck += 1;

					seed = Random.Range(1, 100);
					if (seed > 75)
						maxHp += 20 + vitality / 4 + Random.Range(1, 6);
					else
						maxHp += vitality / 4 + 1;

					hit += .02f;
					magicDefense += .02f;

					break;
				case "thief":
					seed = Random.Range(1, 8);
					if (seed > 1)
						strength += 1;
					seed = Random.Range(1, 8);
					if (seed > 2)
						agility += 1;
					seed = Random.Range(1, 8);
					if (seed > 4)
						intelligence += 1;
					seed = Random.Range(1, 8);
					if (seed > 4)
						vitality += 1;
					seed = Random.Range(1, 8);
					if (seed > 0)
						luck += 1;

					seed = Random.Range(1, 100);
					if (seed > 65)
						maxHp += 20 + vitality / 4 + Random.Range(1, 6);
					else
						maxHp += vitality / 4 + 1;

					hit += .02f;
					magicDefense += .02f;

					break;
				case "white_mage":
					seed = Random.Range(1, 8);
					if (seed > 4)
						strength += 1;
					seed = Random.Range(1, 8);
					if (seed > 4)
						agility += 1;
					seed = Random.Range(1, 8);
					if (seed > 3)
						intelligence += 1;
					seed = Random.Range(1, 8);
					if (seed > 4)
						vitality += 1;
					seed = Random.Range(1, 8);
					if (seed > 3)
						luck += 1;

					seed = Random.Range(1, 100);
					if (seed > 70)
						maxHp += 20 + vitality / 4 + Random.Range(1, 6);
					else
						maxHp += vitality / 4 + 1;

					hit += .01f;
					magicDefense += .02f;

					break;
				case "black_mage":
					seed = Random.Range(1, 8);
					if (seed > 4)
						strength += 1;
					seed = Random.Range(1, 8);
					if (seed > 5)
						agility += 1;
					seed = Random.Range(1, 8);
					if (seed > 0)
						intelligence += 1;
					seed = Random.Range(1, 8);
					if (seed > 4)
						vitality += 1;
					seed = Random.Range(1, 8);
					if (seed > 4)
						luck += 1;

					seed = Random.Range(1, 100);
					if (seed > 75)
						maxHp += 20 + vitality / 4 + Random.Range(1, 6);
					else
						maxHp += vitality / 4 + 1;

					hit += .01f;
					magicDefense += .02f;

					break;
			}

			if (sOld != strength)
				statsIncreased.Add("Strength");
			if (aOld != strength)
				statsIncreased.Add("Agility");
			if (iOld != strength)
				statsIncreased.Add("Intelligence");
			if (vOld != strength)
				statsIncreased.Add("Vitality");
			if (lOld != strength)
				statsIncreased.Add("Luck");
			statsIncreased.Add("HP");

			save_player();

			return statsIncreased;
		}

		public void save_player() {
			string playerN = "player" + (index + 1) + "_";
			SaveSystem.SetInt(playerN + "strength", strength);
			SaveSystem.SetInt(playerN + "agility", agility);
			SaveSystem.SetInt(playerN + "intelligence", intelligence);
			SaveSystem.SetInt(playerN + "vitality", vitality);
			SaveSystem.SetInt(playerN + "luck", luck);
			SaveSystem.SetFloat(playerN + "hit_percent", hit);
			SaveSystem.SetFloat(playerN + "magic_defense", magicDefense);
			SaveSystem.SetInt(playerN + "HP", hp);
			SaveSystem.SetInt(playerN + "maxHP", maxHp);
			SaveSystem.SetInt(playerN + "exp", experience);
			SaveSystem.SetBool(playerN + "poison", conditions.Contains("poison"));
			SaveSystem.SetBool(playerN + "stone", conditions.Contains("stone"));
		}

		public void load_player() {
			string playerN = "player" + (index + 1) + "_";
			strength = SaveSystem.GetInt(playerN + "strength");
			agility = SaveSystem.GetInt(playerN + "agility");
			intelligence = SaveSystem.GetInt(playerN + "intelligence");
			vitality = SaveSystem.GetInt(playerN + "vitality");
			luck = SaveSystem.GetInt(playerN + "luck");
			hit = SaveSystem.GetInt(playerN + "hit_percent");
			magicDefense = SaveSystem.GetInt(playerN + "magic_defense");
			hp = SaveSystem.GetInt(playerN + "HP");
			maxHp = SaveSystem.GetInt(playerN + "maxHP");
			experience = SaveSystem.GetInt(playerN + "exp");
			level = LevelChart.GetLevelFromExp(experience);

			if (SaveSystem.GetBool(playerN + "stone"))
				conditions.Add("stone");
			if (SaveSystem.GetBool(playerN + "poison"))
				conditions.Add("poison");
		}

		public IEnumerator choose_monster(string act) {

			if (GlobalControl.Instance.bossmode) {
				target = bh.monsterParty;
				menu_cursor.gameObject.SetActive(true);
				action = act;

				yield return StartCoroutine(end_turn());
			}
			else {
				monster_cursor.gameObject.SetActive(true);
				monster_cursor.GetComponent<SpriteRenderer>().enabled = false;
				menu_cursor.gameObject.SetActive(false);
				monster_cursor.GetComponent<SpriteRenderer>().enabled = true;

				yield return new WaitForSeconds(.2f);

				while (!Input.GetKey(CustomInputManager.Cim.Select))
					yield return null;

				yield return StartCoroutine(end_turn());

				target = monster_cursor.get_monster().gameObject;

				action = act;
			}

			yield return null;
		}

		public void choose_drink() {
			foreach (GameObject g in bh.medicineButtons)
				g.SetActive(false);

			Dictionary<string, int> items = SaveSystem.GetStringIntDict("items");

			if (items.ContainsKey("Potion")) {
				bh.medicineButtons[0].SetActive(true);
				bh.medicineButtons[0].GetComponentInChildren<Text>().text = "Potion x" + items["Potion"];
			}
			if (items.ContainsKey("Antidote")) {
				bh.medicineButtons[1].SetActive(true);
				bh.medicineButtons[1].GetComponentInChildren<Text>().text = "Antidote x" + items["Antidote"];
			}
			if (items.ContainsKey("Gold Needle")) {
				bh.medicineButtons[2].SetActive(true);
				bh.medicineButtons[2].GetComponentInChildren<Text>().text = "G. Needle x" + items["Gold Needle"];
			}

			bh.medicineContainer.SetActive(true);
			StartCoroutine(Drink());
		}

		IEnumerator Drink() {
			choosing = true;
			while (bh.drk == "") {
				if (Input.GetKeyDown(CustomInputManager.Cim.Back)) {
					action = "";
					target = null;

					bh.medicineContainer.SetActive(false);
					menu_cursor.gameObject.SetActive(true);

					choosing = false;
				}
				yield return null;
			}

			if (choosing) {
				bool success = false;

				switch (bh.drk) {
					case "Potion":
						if (maxHp != hp)
							success = true;
						drink_chosen = "Potion";
						break;
					case "Antidote":
						if (conditions.Contains("poison"))
							success = true;
						drink_chosen = "Antidote";
						break;
					case "Gold Needle":
						if (conditions.Contains("stone"))
							success = true;
						drink_chosen = "Gold Needle";
						break;
				}

				if (success) {

					action = "drink";
					target = gameObject;

					bh.medicineContainer.SetActive(false);
					menu_cursor.gameObject.SetActive(true);
				}
				else {
					drink_chosen = "";
					target = null;

					bh.medicineContainer.SetActive(false);
					menu_cursor.gameObject.SetActive(true);
				}
			}

			choosing = false;
			end_turn();

			bh.drk = "";
		}

		public string drink_action() {

			string output = "";

			Dictionary<string, int> items = SaveSystem.GetStringIntDict("items");

			switch (drink_chosen) {
				case "Potion":
					if (items["Potion"] == 1)
						items.Remove("Potion");
					else
						items["Potion"] = items["Potion"] - 1;

					int healed = (int)Random.Range(16f, 32f);

					hp = Mathf.Min(hp + healed, maxHp);

					output = name + " drank a potion and regained " + healed + " HP.";
					break;
				case "Antidote":
					if (items["Antidote"] == 1)
						items.Remove("Antidote");
					else
						items["Antidote"] = items["Antidote"] - 1;
					conditions.Remove("poison");
					output = name + " recovered from poison";
					break;
				case "Gold Needle":
					if (items["Gold Needle"] == 1)
						items.Remove("Gold Needle");
					else
						items["Gold Needle"] = items["Gold Needle"] - 1;
					conditions.Remove("stone");
					output = name + " recovered from stone";
					break;
			}

			SaveSystem.SetStringIntDict("items", items);

			return output;
		}

		public IEnumerator end_turn() {
			bsc.change_state("walk");
			walk_back();
			while (is_moving())
				yield return null;
			bsc.change_state("idle");

			if (!GlobalControl.Instance.bossmode)
				monster_cursor.gameObject.SetActive(false);
			menu_cursor.gameObject.SetActive(false);
		}

		public void walk_back() {
			movePoint = new Vector3(3.66f, transform.position.y, transform.position.z);
		}

		public bool is_moving() {
			return transform.position != movePoint;
		}

		public bool is_playing_animation() {
			return bsc.isCasting || bsc.isFighting || bsc.isWalking;
		}

		public IEnumerator show_battle() {
			doneShowing = false;

			bsc.change_state("walk");
			movePoint = new Vector3(1.66f, transform.position.y, transform.position.z);
			while (is_moving() && is_playing_animation())
				yield return null;

			if (action == "fight") {
				bsc.change_state("fight", weaponSprite);
				while (is_playing_animation())
					yield return null;
			}

			bsc.change_state("walk");
			movePoint = new Vector3(3.66f, transform.position.y, transform.position.z);
			while (is_playing_animation() || is_moving())
				yield return null;
			bsc.change_state("idle");

			doneShowing = true;
		}

		void check_load() {
			if (!monster_cursor && !GlobalControl.Instance.bossmode)
				monster_cursor = bh.monsterCursor;
			if (!bsc)
				bsc = GetComponent<BattleSpriteController>();
			if (!menu_cursor)
				menu_cursor = bh.menuCursor;
		}

		public void Turn() {
			check_load();

			menu_cursor.gameObject.SetActive(true);

			action = "";
			target = null;

			if (!GlobalControl.Instance.bossmode)
				monster_cursor.gameObject.SetActive(false);

			bsc.change_state("walk");

			movePoint = new Vector3(1.66f, transform.position.y, transform.position.z);

			menu_cursor.gameObject.SetActive(true);
		}

		void StartTimer() {
			if (times == null)
				times = new List<float>();
			timer = 0;
		}

		void StopTimer() {
			Debug.Log(timer);
			times.Add(timer);

			float total = 0f;
			foreach (float f in times)
				total += f;
			Debug.Log("Average: " + total / times.Count);

			timer = -1f;
		}
	}
}
