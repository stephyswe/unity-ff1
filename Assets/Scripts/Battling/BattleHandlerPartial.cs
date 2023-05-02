using System;
using System.Collections;
using UnityEngine;
using Utils.SaveGame.Scripts.SaveSystem;

namespace Battling {
	public partial class BattleHandler {
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
		
		public void magic_choose() {
			if (!accept_input)
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
	}
}
