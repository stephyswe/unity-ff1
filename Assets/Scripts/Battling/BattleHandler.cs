using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Overworld;
using Refactor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils.SaveGame.Scripts.SaveSystem;
using Random = UnityEngine.Random;

namespace Battling {
	public class BattleHandler : MonoBehaviour {
		public PartyMember[] party;

		[FormerlySerializedAs("party_placement")]
		public Transform[] partyPlacement;

		[FormerlySerializedAs("battle_text")] public Text battleText;
		[FormerlySerializedAs("party_names")] public Text[] partyNames;
		[FormerlySerializedAs("party_HP")] public Text[] partyHp;

		public MagicWeaponSpriteHandler mwsh;

		public GameObject medicineContainer;
		[FormerlySerializedAs("monster_cursor")]
		public CursorController monsterCursor;
		[FormerlySerializedAs("menu_cursor")] public CursorController menuCursor;
		[FormerlySerializedAs("es")] public EventSystem eventSystem;
		public Canvas c;

		[FormerlySerializedAs("battle_music")] public MusicHandler battleMusic;
		[FormerlySerializedAs("victory_music")]
		public MusicHandler victoryMusic;
		[FormerlySerializedAs("death_music")] public MusicHandler deathMusic;
		[FormerlySerializedAs("boss_music")] public MusicHandler bossMusic;

		[FormerlySerializedAs("active_party_member")]
		public PartyMember activePartyMember;

		[FormerlySerializedAs("battle_complete")]
		public bool battleComplete;
		public bool win;
		public bool lose;
		public bool stalemate;

		[FormerlySerializedAs("party_selecting")]
		public bool partySelecting;

		public string drk = "";

		[FormerlySerializedAs("medicine_buttons")]
		public GameObject[] medicineButtons;

		[FormerlySerializedAs("monster_party")]
		public GameObject monsterParty;

		bool accept_input;

		List<GameObject> battlers;
		Dictionary<int, int> level_up_chart;

		LevelChart levelChart;
		Monster[] monsters;
		PartyHandler partyHandler;

		bool setting_battle_text;

		// Start is called before the first frame update
		void Start() {
			
			// Load the party from the GlobalControl instance
			load_party();
			
			// Set the active party member to the first party member
			activePartyMember = party[0];

			// Call the GetLevelChart() method using the class name
			level_up_chart = LevelChart.GetLevelChart();
			
			// If the player is fighting a boss, play the boss music
			if (GlobalControl.Instance.bossmode)
				battleMusic.gameObject.SetActive(false);
			else
				bossMusic.gameObject.SetActive(false);

			// If the player is not fighting a boss
			if (!GlobalControl.Instance.bossmode) {
				// Instantiate the monster party
				monsterParty = Instantiate(GlobalControl.Instance.monsterParty, new Vector3(0f, 0f, 1f), Quaternion.identity);
				monsterParty.SetActive(true);
				
				// Get the cursor controller component from the monster party
				monsterCursor = monsterParty.GetComponentInChildren<CursorController>();
				monsterCursor.eventSystem = eventSystem;
				monsterCursor.gameObject.SetActive(false);

				monsters = (from m in monsterCursor.monsters select m.GetComponent<Monster>()).ToArray();
			}

			else {
				monsterParty = Instantiate(GlobalControl.Instance.monsterParty, new Vector3(-10.415f, 2.1f, 1f), Quaternion.identity);
				monsterParty.SetActive(true);

				monsters = new[] {monsterParty.GetComponent<Monster>()};
			}

			menuCursor.gameObject.SetActive(false);

			battleComplete = false;
			win = false;
			lose = false;

			StartCoroutine(Battle());
		}

		// Update is called once per frame
		void Update() {
			if (monsters.Length == 0)
				battleComplete = true;
			for (int i = 0; i < 4; i++)
				partyHp[i].text = "HP: " + party[i].hp;
		}

		static void remove_from_array<T>(ref T[] arr, int index) {
			for (int a = index; a < arr.Length - 1; a++)
				// moving elements downwards, to fill the gap at [index]
				arr[a] = arr[a + 1];
			// finally, let's decrement Array's size by one
			Array.Resize(ref arr, arr.Length - 1);
		}

		IEnumerator set_battle_text(string t, float wait, bool waitForInput, bool clearOnFinish) {
			setting_battle_text = true;
			battleText.text = t;

			yield return new WaitForSeconds(wait);
			if (waitForInput) {
				while (!Input.GetKey(CustomInputManager.Cim.Select))
					yield return null;
			}

			if (clearOnFinish)
				battleText.text = "";

			setting_battle_text = false;
		}

		IEnumerator Battle() {

			float textDelay = SaveSystem.GetFloat("battle_speed");

			battleText.text = "";

			int goldWon = 0;
			int expWon = 0;

			foreach (PartyMember p in party) {
				while (!p.doneSetUp)
					yield return null;
			}

			for (int i = 0; i < 4; i++) {
				partyNames[i].text = party[i].name;
				partyHp[i].text = "HP: " + party[i].hp;
			}

			battlers = new List<GameObject>();

			foreach (Monster m in monsters)
				battlers.Add(m.gameObject);

			foreach (PartyMember p in party)
				battlers.Add(p.gameObject);

			List<string> monstersEncountered = new List<string>();
			foreach (Monster m in monsters) {
				if (!monstersEncountered.Contains(MonsterHandler.ProcessMonsterName(m.gameObject.name)))
					monstersEncountered.Add(MonsterHandler.ProcessMonsterName(m.gameObject.name));
			}
			string encounterText = monstersEncountered.Aggregate("Encountered ", (current, s) => current + (s + ", "));
			encounterText = encounterText[..^2] + "!";

			yield return StartCoroutine(set_battle_text(encounterText, textDelay, true, true));

			accept_input = false;

			yield return new WaitForSeconds(.3f);

			// menu cursor is active
			menuCursor.gameObject.SetActive(false);
			if (!GlobalControl.Instance.bossmode)
				monsterCursor.gameObject.SetActive(false);

			// while the player is selecting a party member
			while (Input.GetKey(CustomInputManager.Cim.Select)) {
				menuCursor.gameObject.SetActive(false);
				if (!GlobalControl.Instance.bossmode)
					monsterCursor.gameObject.SetActive(false);
				yield return null;
			}

			accept_input = true;

			while (!battleComplete) {

				//Check if players won
				int living = 0;
				foreach (Monster m in monsters) {
					if (m.hp > 0)
						living += 1;
					else
						m.gameObject.SetActive(false);
				}

				if (living == 0) {
					if (monsters.Length == 0)
						stalemate = true;
					else
						win = true;
					break;
				}

				List<string> ms = new List<string>();
				foreach (Monster mo in monsters) {
					if (!ms.Contains(MonsterHandler.ProcessMonsterName(mo.gameObject.name)) && mo.hp > 0)
						ms.Add(MonsterHandler.ProcessMonsterName(mo.gameObject.name));
				}
				string txt = "";
				foreach (string s in ms)
					txt += s + "  ";

				//Party selection
				partySelecting = true;
				for (int i = 0; i < party.Length; i++) {

					PartyMember p = party[i];

					if (p.hp > 0) {
						activePartyMember = p;

						yield return StartCoroutine(set_battle_text(txt, .05f, false, false));

						p.Turn();

						menuCursor.gameObject.SetActive(true);

						while (p.action == "" || p.target == null) {

							if (Input.GetKeyDown(CustomInputManager.Cim.Back)) {
								if (i > 0) {
									i -= 2;
									yield return StartCoroutine(p.end_turn());
									break;
								}
								i = -1;
								yield return StartCoroutine(p.end_turn());
								break;
							}

							if (p.action == "run")
								break;
							yield return null;
						}

						yield return StartCoroutine(p.end_turn());
					}
				}
				partySelecting = false;
				if (!GlobalControl.Instance.bossmode)
					monsterCursor.gameObject.SetActive(false);
				menuCursor.gameObject.SetActive(false);

				//Monster selection
				foreach (Monster m in monsters) {
					if (m.hp > 0) {
						m.Turn();
						/*
					if(m.target == null){
						lose = true;
						break;
					}
					*/
					}
				}

				/*
			if (lose)
			{
				battle_complete = true;
				break;
			}
			*/

				//Scheduling
				//Debug.Log("Scheduling...");
				List<int> schedule = new List<int>();

				foreach (Monster m in monsters)
					schedule.Add(schedule.Count);
				int addedPartyMembers = 0;
				foreach (PartyMember p in party) {
					schedule.Add(80 + addedPartyMembers);
					addedPartyMembers += 1;
				}
				for (int i = 0; i < 17; i++) {
					int idx1 = Random.Range(0, battlers.Count);
					int idx2 = Random.Range(0, battlers.Count);

					int temp = schedule[idx1];
					schedule[idx1] = schedule[idx2];
					schedule[idx2] = temp;
				}

				foreach (PartyMember p in party) {
					while (p.is_moving())
						yield return null;
				}

				living = 0;
				foreach (Monster m in monsters) {
					if (m.hp > 0)
						living += 1;
					else
						m.gameObject.SetActive(false);
				}

				if (living == 0) {
					stalemate = true;
					break;
				}

				//Display battle
				foreach (int x in schedule) {
					if (x >= 80) {
						PartyMember p = party[x - 80];

						if (p.hp <= 0)
							continue;

						if (p.action == "fight") {

							while (p.target.GetComponent<Monster>().hp <= 0)
								p.target = monsters[Random.Range(0, monsters.Length)].gameObject;

							StartCoroutine(p.show_battle());
							while (!p.doneShowing)
								yield return null;

							while (p.target == null)
								p.target = monsters[Random.Range(0, monsters.Length)].gameObject;

							int damage = p.GetComponent<Battler>().Fight(p, p.target.GetComponent<Monster>());
							if (damage == -9999999)
								yield return StartCoroutine(set_battle_text(p.gameObject.name + " missed", textDelay, true, true));
							else if (damage > 0)
								yield return StartCoroutine(set_battle_text(p.gameObject.name + " does " + damage + " damage to " + MonsterHandler.ProcessMonsterName(p.target.gameObject.name), textDelay, true, true));
							else {
								yield return StartCoroutine(set_battle_text("Critical hit!", textDelay, true, true));
								yield return StartCoroutine(set_battle_text(p.gameObject.name + " does " + -damage + " damage to " + MonsterHandler.ProcessMonsterName(p.target.gameObject.name), textDelay, true, true));
							}

							while (setting_battle_text)
								yield return null;

							if (p.target.GetComponent<Monster>().hp <= 0) {
								goldWon += p.target.GetComponent<Monster>().gold;
								expWon += p.target.GetComponent<Monster>().exp;

								yield return StartCoroutine(set_battle_text(MonsterHandler.ProcessMonsterName(p.target.gameObject.name) + " was slain", textDelay, true, true));

								while (setting_battle_text)
									yield return null;
							}
						}

						else if (p.action == "drink") {
							string drinkText = p.drink_action();

							yield return StartCoroutine(set_battle_text(drinkText, textDelay, true, true));
						}

						else if (p.action == "run") {
							/*
						if(!p.can_run){
							Debug.Log("Can't run!");
						}
						else{
						*/
							int runSeed = Random.Range(0, p.level + 15);
							if (p.luck > runSeed && p.canRun) {

								yield return StartCoroutine(set_battle_text(p.gameObject.name + " ran away", textDelay, true, true));

								while (setting_battle_text)
									yield return null;

								foreach (PartyMember pm in party) {
									if (pm.hp > 0)
										pm.bsc.change_state("run");
									yield return new WaitForSeconds(.26f);
								}

								battleComplete = true;
								stalemate = true;
								break;
							}
							yield return StartCoroutine(set_battle_text(p.name + " couldn't run", textDelay, true, true));

							while (setting_battle_text)
								yield return null;
							//}
						}
					}
					else {
						GameObject b = battlers[x];
						Monster m = b.GetComponent<Monster>();

						if (m.hp > 0 && m.target != null) {
							if (m.action == "fight") {

								while (m.target.GetComponent<PartyMember>().hp <= 0)
									m.target = party[Random.Range(0, party.Length)].gameObject;

								int damage = m.GetComponent<Battler>().Fight(m, m.target.GetComponent<PartyMember>());
								if (damage == -1)
									yield return StartCoroutine(set_battle_text(MonsterHandler.ProcessMonsterName(m.gameObject.name) + " missed", textDelay, true, true));
								else
									yield return StartCoroutine(set_battle_text(MonsterHandler.ProcessMonsterName(m.gameObject.name) + " does " + damage + " damage to " + m.target.gameObject.name, textDelay, true, true));

								while (setting_battle_text)
									yield return null;
							}

							else if (m.action == "run") {
								yield return StartCoroutine(set_battle_text(MonsterHandler.ProcessMonsterName(m.gameObject.name) + " ran away", textDelay, true, true));

								while (setting_battle_text)
									yield return null;

								Destroy(m.gameObject);
								remove_from_array(ref monsters, x);
							}

							if (m.target.GetComponent<PartyMember>().hp <= 0) {
								m.target.GetComponent<PartyMember>().bsc.change_state("dead");
								yield return StartCoroutine(set_battle_text(m.target.gameObject.name + " was slain", textDelay, true, true));

								while (setting_battle_text)
									yield return null;
							}
						}
					}
					//Check if players won
					living = 0;
					foreach (Monster m in monsters) {
						if (m.hp > 0)
							living += 1;
						else
							m.gameObject.SetActive(false);
					}

					if (living == 0) {
						if (monsters.Length == 0)
							stalemate = true;
						else
							win = true;
						break;
					}

					//Check if players lost
					lose = true;
					foreach (PartyMember p in party) {
						if (p.hp > 0)
							lose = false;
					}

					if (win || lose || stalemate) {
						battleComplete = true;
						break;
					}
				}


			}

			if (win) {
				foreach (PartyMember p in party) {
					if (p.hp > 0)
						p.bsc.change_state("victory");
					if (GlobalControl.Instance.bossmode)
						p.canRun = false;
					p.save_player();
				}

				if (GlobalControl.Instance.bossmode)
					bossMusic.get_active().Stop();
				else
					battleMusic.get_active().Stop();
				victoryMusic.gameObject.SetActive(true);
				victoryMusic.get_active().Play();

				yield return StartCoroutine(set_battle_text("Victory!", textDelay, true, false));

				while (victoryMusic.get_active().time <= victoryMusic.get_active().gameObject.GetComponent<IntroLoop>().loopStartSeconds / 2f)
					yield return null;

				yield return StartCoroutine(set_battle_text("Obtained " + goldWon + " gold", textDelay, true, true));
				SaveSystem.SetInt("gil", SaveSystem.GetInt("gil") + goldWon);

				int living = 0;
				foreach (PartyMember m in party) {
					if (m.hp > 0)
						living += 1;
				}

				int expEach = expWon / living;
				yield return StartCoroutine(set_battle_text("Obtained " + expEach + " exp", textDelay, true, false));

				foreach (PartyMember m in party) {
					if (m.hp > 0) {
						m.experience += expEach;
						while (LevelChart.GetLevelFromExp(m.experience) > m.level) {
							List<string> stats = m.level_up();

							yield return StartCoroutine(set_battle_text(m.gameObject.name + " leveled up!", textDelay, true, true));

							foreach (string s in stats)
								yield return StartCoroutine(set_battle_text(s + " up", textDelay, true, false));
						}
					}
				}

				while (!Input.GetKey(CustomInputManager.Cim.Select))
					yield return null;


			}
			else if (lose) {
				battleMusic.get_active().Stop();
				deathMusic.gameObject.SetActive(true);
				deathMusic.get_active().Play();

				foreach (PartyMember p in party)
					p.bsc.change_state("dead");

				yield return StartCoroutine(set_battle_text("Game over...", textDelay * 2f, true, false));

				SceneManager.UnloadScene("Overworld");
				SceneManager.LoadSceneAsync("Title Screen");
			}

			Destroy(monsterParty);

			GlobalControl.Instance.monsterParty = null;

			foreach (PartyMember p in party) {
				p.save_player();
				Destroy(p.gameObject);
			}

			if (win && GlobalControl.Instance.bossmode)
				GlobalControl.Instance.bossvictory = true;
			SceneManager.UnloadScene("Battle");

		}

		public void medicine_choose() {
			if (!accept_input)
				return;
			menuCursor.gameObject.SetActive(false);
			activePartyMember.choose_drink();
		}

		public void select_drink(string dr) {
			drk = dr;
		}

		public void fight_choose() {
			if (accept_input)
				StartCoroutine(activePartyMember.choose_monster("fight"));
		}

		public void player_run() {
			activePartyMember.action = "run";
			activePartyMember.walk_back();
		}

		void load_party() {
			for (int i = 0; i < 4; i++) {
				string playerN = "player" + (i + 1) + "_";
				string job = SaveSystem.GetString("player" + (i + 1) + "_class");
				party[i] = job switch {
					"fighter" => Instantiate(Resources.Load<GameObject>("party/fighter"), partyPlacement[i].position, Quaternion.identity).GetComponent<PartyMember>(),
					"black_belt" => Instantiate(Resources.Load<GameObject>("party/black_belt"), partyPlacement[i].position, Quaternion.identity).GetComponent<PartyMember>(),
					"red_mage" => Instantiate(Resources.Load<GameObject>("party/red_mage"), partyPlacement[i].position, Quaternion.identity).GetComponent<PartyMember>(),
					"thief" => Instantiate(Resources.Load<GameObject>("party/thief"), partyPlacement[i].position, Quaternion.identity).GetComponent<PartyMember>(),
					"white_mage" => Instantiate(Resources.Load<GameObject>("party/white_mage"), partyPlacement[i].position, Quaternion.identity).GetComponent<PartyMember>(),
					"black_mage" => Instantiate(Resources.Load<GameObject>("party/black_mage"), partyPlacement[i].position, Quaternion.identity).GetComponent<PartyMember>(),
					_ => party[i]
				};

				party[i].gameObject.name = SaveSystem.GetString(playerN + "name");
				party[i].bh = this;
				party[i].index = i;
				party[i].load_player();
			}
		}
	}
}
