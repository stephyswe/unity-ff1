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
	public partial class BattleHandler : MonoBehaviour {
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

		bool _acceptInput;

		List<GameObject> _battlers;

		LevelChart _levelChart;
		Monster[] _monsters;
		PartyHandler _partyHandler;

		float _battleSpeed;

		// Start is called before the first frame update
		void Start() {
			LoadParty();
			DeactivateMusic(GlobalControl.Instance.bossmode);
			InitializeMonsterParty();
			ResetBattleState();
			StartCoroutine(Battle());
		}

		void Update() {
			// UpdatePartyHp();
		}

		IEnumerator Battle() {

			// set battleSpeed into reward
			battleText.text = "";
			int goldWon = 0;
			int expWon = 0;

			yield return StartCoroutine(SetupPartyMembers());
			yield return StartCoroutine(DisplayPartyInfo());

			_battlers = new List<GameObject>();
			GetBattlers(_battlers);

			// execute the encounter
			yield return StartCoroutine(ExecuteEncounter());

			_acceptInput = false;
			yield return new WaitForSeconds(.3f);

			// menu cursor is active
			DisableCursors();

			// while the player is selecting a party member
			yield return StartCoroutine(WaitForPartyMemberSelection());

			_acceptInput = true;

			while (!win && !lose) {
				// Party selection - skip to make faster win / lose.
				// yield return StartCoroutine(ProcessPartyMemberTurns());

				DisableCursors();

				List<int> schedule = GetSchedule();

				yield return StartCoroutine(WaitForPartyMembersToStopMoving());

				//Display battle
				bool hasEscaped = false;
				BattleRewards rewards = new BattleRewards();

				foreach (int x in schedule) {
					if (x >= 80) {
						PartyMember p = party[x - 80];

						if (p.hp <= 0)
							continue;
						// ReSharper disable once ConvertIfStatementToSwitchStatement
						if (p.action == "fight") {
							yield return StartCoroutine(ExecuteFightAction(p, rewards));
						}
						else if (p.action == "drink") {
							string drinkText = p.drink_action();
							yield return StartCoroutine(set_battle_text(drinkText, _battleSpeed, true, true));
						}
						else if (p.action == "run") {
							yield return StartCoroutine(ExecutePartyRunAction(p, rewards, _battleSpeed));
						}

						goldWon += rewards.GoldWon;
						expWon += rewards.ExpWon;
						hasEscaped = rewards.HasRun;
					}
					else if (!hasEscaped) {
						//Monster selection
						MonstersGetTarget(_monsters);
						GameObject b = _battlers[x];
						Monster m = b.GetComponent<Monster>();

						if (m.hp <= 0 || m.target == null)
							continue;
						// ReSharper disable once ConvertIfStatementToSwitchStatement
						if (m.action == "fight") {
							yield return StartCoroutine(ExecuteFightActionMonster(m));
						}
						else if (m.action == "run") {
							yield return StartCoroutine(ExecuteRunActionMonster(m, x));
						}
					}
					UpdatePartyHp();
					if (!CheckWinOrLose())
						continue;
					break;
				}
			}

			if (win) {
				yield return StartCoroutine(ExecuteVictorySequence(goldWon, expWon));
			}
			else if (lose) {
				yield return StartCoroutine(ExecuteLossSequence());
			}

			Destroy(monsterParty);

			GlobalControl.Instance.monsterParty = null;

			foreach (PartyMember p in party) {
				p.save_player();
				Destroy(p.gameObject);
			}

			if (win && GlobalControl.Instance.bossmode)
				GlobalControl.Instance.bossvictory = true;
			//SceneManager.UnloadSceneAsync("Battle");
		}

		bool CheckWinOrLose() {
			int alivePartyMembers = party.Count(p => p.hp > 0);
			int aliveMonsters = _monsters.Count(m => m.hp > 0);
					
			if (alivePartyMembers == 0) {
				lose = true;
				return true;
			}
			if (aliveMonsters == 0) {
				win = true;
				return true;
			}
			return false;
		}
	}
}
