using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Overworld;
using Refactor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.SaveGame.Scripts.SaveSystem;
using Random = UnityEngine.Random;

namespace Battling {
	public partial class BattleHandler {
		
		IEnumerator ProcessPartyMemberTurns()
		{
			string txt = GetMonsterNames();
			partySelecting = true;
			yield return StartCoroutine(PartyMemberTurns(txt));
			partySelecting = false;
		}
		
		void InitializeMonsterParty()
		{
			if (!GlobalControl.Instance.bossmode)
			{
				monsterParty = InstantiateMonsterParty(new Vector3(0f, 0f, 1f));
				InitializeMonsterCursor();
			}
			else
			{
				monsterParty = InstantiateMonsterParty(new Vector3(-10.415f, 2.1f, 1f));
				_monsters = new[] { monsterParty.GetComponent<Monster>() };
			}

			monsterParty.SetActive(true);
		}


		static GameObject InstantiateMonsterParty(Vector3 position)
		{
			return Instantiate(GlobalControl.Instance.monsterParty, position, Quaternion.identity);
		}

		
		void DeactivateMusic(bool isBattleMusic)
		{
			if (isBattleMusic)
				battleMusic.gameObject.SetActive(false);
			else
				bossMusic.gameObject.SetActive(false);
		}
		void LoadParty() {
			for (int i = 0; i < 4; i++) {
				string playerN = "player" + (i + 1) + "_";
				string job = SaveSystem.GetString("player" + (i + 1) + "_class");

				try {
					GameObject playerPrefab = GetPlayerPrefab(job);
					if (playerPrefab != null) {
						PartyMember player = Instantiate(playerPrefab, partyPlacement[i].position, Quaternion.identity).GetComponent<PartyMember>();
						player.gameObject.name = SaveSystem.GetString(playerN + "name");
						player.bh = this;
						player.index = i;
						player.load_player();
						party[i] = player;
					}
					else {
						throw new Exception("Player prefab not found for job: " + job);
					}
				} catch (Exception e) {
					Debug.LogError("Error loading party member " + (i + 1) + ": " + e.Message);
					party[i] = null;
				}
			}
			// set active party member 
			activePartyMember = party[0];
		}


		static GameObject GetPlayerPrefab(string job) {
			return job switch {
				"fighter" => Resources.Load<GameObject>("party/fighter"),
				"black_belt" => Resources.Load<GameObject>("party/black_belt"),
				"red_mage" => Resources.Load<GameObject>("party/red_mage"),
				"thief" => Resources.Load<GameObject>("party/thief"),
				"white_mage" => Resources.Load<GameObject>("party/white_mage"),
				"black_mage" => Resources.Load<GameObject>("party/black_mage"),
				_ => null
			};
		}

		static void remove_from_array<T>(ref T[] arr, int index) {
			for (int a = index; a < arr.Length - 1; a++)
				// moving elements downwards, to fill the gap at [index]
				arr[a] = arr[a + 1];
			// finally, let's decrement Array's size by one
			Array.Resize(ref arr, arr.Length - 1);
		}

		IEnumerator set_battle_text(string t, float wait, bool waitForInput, bool clearOnFinish) {
			battleText.text = t;

			yield return new WaitForSeconds(wait);
			if (waitForInput) {
				while (!Input.GetKey(CustomInputManager.Cim.Select))
					yield return null;
			}

			if (clearOnFinish)
				battleText.text = "";
		}

		void enableBattleText(string t) {
			battleText.text = t;
		}

		static IEnumerator waitForInput() {
			while (!Input.GetKey(CustomInputManager.Cim.Select))
				yield return null;
		}

		IEnumerator setBattleText(string t) {
			enableBattleText(t);
			yield return new WaitForSeconds(_battleSpeed);
			yield return StartCoroutine(waitForInput());
		}

		IEnumerator setBattleTextClear(string t) {
			enableBattleText(t);
			yield return new WaitForSeconds(_battleSpeed);
			yield return StartCoroutine(waitForInput());
			battleText.text = "";
		}

		IEnumerator setBattleTextGameOver(string t) {
			enableBattleText(t);
			yield return new WaitForSeconds(_battleSpeed * 2f);
			yield return StartCoroutine(waitForInput());
		}

		public void medicine_choose() {
			if (!_acceptInput)
				return;
			menuCursor.gameObject.SetActive(false);
			activePartyMember.choose_drink();
		}

		public void select_drink(string dr) {
			drk = dr;
		}

		public void fight_choose() {
			if (_acceptInput)
				StartCoroutine(activePartyMember.choose_monster("fight"));
		}

		public void magic_choose() {
			if (!_acceptInput)
				return;
			Debug.Log("magic_choose");
			// ENABLE WINDOW WITH MAGIC.
			// LIST OF MAGIC.
			// CHOOSE ONE MAGIC
			// CHOOSE ENEMY
			// DONE.
			// TODO: Open window with magic.
			StartCoroutine(activePartyMember.choose_monster("magic"));
		}

		public void player_run() {
			activePartyMember.action = "run";
			activePartyMember.walk_back();
		}

		void GetBattlers(List<GameObject> battlers) {
			battlers.AddRange(_monsters.Select(m => m.gameObject));
			battlers.AddRange(party.Select(p => p.gameObject));
		}

		IEnumerable<string> GetMonstersEncountered()
		{
			HashSet<string> encounteredMonsters = new HashSet<string>();
			foreach (Monster m in _monsters)
			{
				string processedName = MonsterHandler.ProcessMonsterName(m.gameObject.name);
				encounteredMonsters.Add(processedName);
			}
			return new List<string>(encounteredMonsters);
		}

		static string GetEncounterText(IEnumerable<string> monstersEncountered)
		{
			string encounterText = "Encountered ";
			encounterText += string.Join(", ", monstersEncountered);
			encounterText += "!";
			return encounterText;
		}

		void DisableCursors() {
			menuCursor.gameObject.SetActive(false);
			if (!GlobalControl.Instance.bossmode)
				monsterCursor.gameObject.SetActive(false);
		}

		IEnumerator WaitForPartyMemberSelection() {
			while (Input.GetKey(CustomInputManager.Cim.Select)) {
				menuCursor.gameObject.SetActive(false);
				if (!GlobalControl.Instance.bossmode)
					monsterCursor.gameObject.SetActive(false);
				yield return null;
			}
		}

		string GetMonsterNames() {
			List<string> monsterNames = new List<string>();

			foreach (Monster m in _monsters) {
				if (!monsterNames.Contains(MonsterHandler.ProcessMonsterName(m.gameObject.name)) && m.hp > 0)
					monsterNames.Add(MonsterHandler.ProcessMonsterName(m.gameObject.name));
			}

			string txt = string.Join("  ", monsterNames.ToArray());
			return txt;
		}

		IEnumerator PartyMemberTurns(string txt) {
			for (int i = 0; i < party.Length; i++) {

				PartyMember p = party[i];

				if (p.hp <= 0)
					continue;
				activePartyMember = p;
				
				Debug.Log("What is this?: " + txt);
				// yield return StartCoroutine(set_battle_text(txt, .05f, false, false));

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

		IEnumerator SetupPartyMembers() {
			foreach (PartyMember p in party) {
				while (!p.doneSetUp)
					yield return null;
			}
		}

		IEnumerator DisplayPartyInfo() {
			for (int i = 0; i < 4; i++) {
				PartyMember p = party[i];
				partyNames[i].text = p.name;
				partyHp[i].text = "HP: " + p.hp;
			}
			yield return null;
		}

		List<int> GetSchedule() {
			// Create an empty schedule list
			List<int> schedule = new List<int>();

			// Add monsters to the schedule
			foreach (Monster m in _monsters) {
				schedule.Add(schedule.Count);
			}

			// Add party members to the schedule
			int addedPartyMembers = 0;
			foreach (PartyMember p in party) {
				// Party members are represented by values starting from 80 in the schedule
				schedule.Add(80 + addedPartyMembers);
				addedPartyMembers += 1;
			}

			// Shuffle the schedule using random swapping
			for (int i = 0; i < 17; i++) {
				// Get random indices within the battler count
				int idx1 = Random.Range(0, _battlers.Count);
				int idx2 = Random.Range(0, _battlers.Count);

				// Swap the values at the random indices
				int temp = schedule[idx1];
				schedule[idx1] = schedule[idx2];
				schedule[idx2] = temp;
			}

			// Return the generated schedule
			return schedule;
		}


		IEnumerator WaitForPartyMembersToStopMoving() {
			foreach (PartyMember p in party) {
				while (p.is_moving())
					yield return null;
			}
		}

		public class BattleRewards {
			public int GoldWon { get; set; }
			public int ExpWon { get; set; }
			public bool HasRun { get; set; }
		}

		IEnumerator ExecutePartyRunAction(PartyMember p, BattleRewards rewards, float textDelay) {
			int runSeed = Random.Range(0, p.level + 15);
			if (p.luck > runSeed && p.canRun) {

				yield return StartCoroutine(set_battle_text(p.gameObject.name + " ran away", textDelay, true, true));

				foreach (PartyMember pm in party) {
					if (pm.hp > 0)
						pm.bsc.change_state("run");
					yield return new WaitForSeconds(.26f);
				}
				stalemate = true;
				rewards.HasRun = true;
			}
			else {
				yield return StartCoroutine(set_battle_text(p.name + " couldn't run", textDelay, true, true));
			}
		}

		IEnumerator ExecuteFightAction(PartyMember p, BattleRewards rewards) {
			while (p.target.GetComponent<Monster>().hp <= 0)
				p.target = _monsters[Random.Range(0, _monsters.Length)].gameObject;

			StartCoroutine(p.show_battle());
			while (!p.doneShowing)
				yield return null;

			while (p.target == null)
				p.target = _monsters[Random.Range(0, _monsters.Length)].gameObject;

			int damage = p.GetComponent<Battler>().Fight(p, p.target.GetComponent<Monster>());

			// get party member name
			string partyMemberName = p.gameObject.name;

			// Get monster name
			string monsterName = MonsterHandler.ProcessMonsterName(p.target.gameObject.name);

			if (damage == -9999999)
				yield return StartCoroutine(setBattleTextClear(partyMemberName + " missed"));
			else if (damage > 0)
				yield return StartCoroutine(setBattleTextClear(partyMemberName + " does " + damage + " damage to " + monsterName));
			else {
				yield return StartCoroutine(setBattleTextClear("Critical hit!"));
				yield return StartCoroutine(setBattleTextClear(partyMemberName + " does " + -damage + " damage to " + monsterName));
			}

			if (p.target.GetComponent<Monster>().hp > 0)
				yield break;

			incReward(p, rewards);
			yield return StartCoroutine(setBattleTextClear(monsterName + " was slain"));
		}

		IEnumerator ExecuteFightActionMonster(Monster m) {
			while (m.target.GetComponent<PartyMember>().hp <= 0)
				m.target = party[Random.Range(0, party.Length)].gameObject;

			int damage = m.GetComponent<Battler>().Fight(m, m.target.GetComponent<PartyMember>());
			if (damage == -1)
				yield return StartCoroutine(set_battle_text(MonsterHandler.ProcessMonsterName(m.gameObject.name) + " missed", _battleSpeed, true, true));
			else {
				yield return StartCoroutine(set_battle_text(MonsterHandler.ProcessMonsterName(m.gameObject.name) + " does " + damage + " damage to " + m.target.gameObject.name, _battleSpeed, true, true));
			}
		}

		IEnumerator ExecuteRunActionMonster(Monster m, int x) {
			yield return StartCoroutine(set_battle_text(MonsterHandler.ProcessMonsterName(m.gameObject.name) + " ran away", _battleSpeed, true, true));
			Destroy(m.gameObject);
			remove_from_array(ref _monsters, x);
		}

		static void incReward(PartyMember p, BattleRewards rewards) {
			rewards.GoldWon += p.target.GetComponent<Monster>().gold;
			rewards.ExpWon += p.target.GetComponent<Monster>().exp;
		}

		static float GetBattleSpeed() {
			return SaveSystem.GetFloat("battle_speed");
		}

		IEnumerator ExecuteEncounter() {
			IEnumerable<string> monstersEncountered = GetMonstersEncountered();
			string encounterText = GetEncounterText(monstersEncountered);
			yield return StartCoroutine(setBattleTextClear(encounterText));
		}

		static int CalculateExperiencePerPartyMember(int expWon, IEnumerable<PartyMember> allPartyMembers) {
			int living = allPartyMembers.Count(m => m.hp > 0);
			return expWon / living;
		}

		IEnumerator ObtainGold(int goldWon) {
			yield return StartCoroutine(setBattleTextClear("Obtained " + goldWon + " gold"));
			int currentGold = SaveSystem.GetInt("gil");
			SaveSystem.SetInt("gil", currentGold + goldWon);
		}

		IEnumerator ExecuteLossSequence() {
			SwapMusic(battleMusic, deathMusic);
			yield return StartCoroutine(setBattleTextGameOver("Game over..."));
			SceneManager.LoadSceneAsync("Menu");
		}

		static void SwapMusic(MusicHandler prevMusic, MusicHandler currentMusic)
		{
			prevMusic.get_active().Stop();
			currentMusic.gameObject.SetActive(true);
			currentMusic.get_active().Play();
		}

		IEnumerator ExecuteVictorySequence(int goldWon, int expWon) {
			foreach (PartyMember p in party) {
				if (p.hp > 0)
					p.bsc.change_state("victory");
				if (GlobalControl.Instance.bossmode)
					p.canRun = false;

				// save the player
				p.save_player();
			}

			if (GlobalControl.Instance.bossmode)
				bossMusic.get_active().Stop();
			else
				battleMusic.get_active().Stop();
			victoryMusic.gameObject.SetActive(true);
			victoryMusic.get_active().Play();

			yield return StartCoroutine(setBattleText("Victory!"));

			while (victoryMusic.get_active().time <= victoryMusic.get_active().gameObject.GetComponent<IntroLoop>().loopStartSeconds / 2f)
				yield return null;

			yield return StartCoroutine(ObtainGold(goldWon));

			int expEach = CalculateExperiencePerPartyMember(expWon, party);

			yield return StartCoroutine(setBattleText("Obtained " + expEach + " exp"));

			foreach (PartyMember partyMember in party) {
				if (partyMember.hp <= 0)
					continue;

				partyMember.experience += expEach;

				// Level up check
				while (LevelChart.GetLevelFromExp(partyMember.experience) > partyMember.level) {
					List<string> stats = partyMember.level_up();
					yield return StartCoroutine(setBattleTextClear(partyMember.gameObject.name + " leveled up!"));
					foreach (string s in stats)
						yield return StartCoroutine(setBattleText(s + " up"));
				}
			}

			while (!Input.GetKey(CustomInputManager.Cim.Select))
				yield return null;
		}

		static void MonstersGetTarget(IEnumerable<Monster> monsters) {
			foreach (Monster mo in monsters)
				if (mo.hp > 0)
					mo.Turn();
		}
		
		void ResetBattleState()
		{
			menuCursor.gameObject.SetActive(false);
			win = false;
			lose = false;
			_battleSpeed = GetBattleSpeed();
		}
		
		void InitializeMonsterCursor()
		{
			monsterCursor = monsterParty.GetComponentInChildren<CursorController>();
			monsterCursor.eventSystem = eventSystem;
			monsterCursor.gameObject.SetActive(false);
			_monsters = monsterCursor.monsters.Select(m => m.GetComponent<Monster>()).ToArray();
		}

		void UpdatePartyHp()
		{
			for (int i = 0; i < 4; i++)
			{
				partyHp[i].text = "HP: " + party[i].hp;
			}
		}
	}
}
