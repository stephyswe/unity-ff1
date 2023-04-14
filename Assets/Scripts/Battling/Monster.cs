using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Battling {
	public class Monster : Battler {

		public int gold;
		public int exp;
		public int absorb;
		public float evade;
		[FormerlySerializedAs("damage_low")] public int damageLow;
		[FormerlySerializedAs("damage_high")] public int damageHigh;
		public int multihit;
		[FormerlySerializedAs("crit_percent")] public float critPercent;
		public string[] family;
		public int morale;
		[FormerlySerializedAs("attack_status")]
		public string attackStatus;
		[FormerlySerializedAs("attack_element")]
		public string attackElement;
		public string[] weaknesses;
		public string[] resistances;

		public string[] spells;
		public string[] specials;

		public string action;
		public GameObject target;

		public BattleHandler bh;

		// Start is called before the first frame update
		void Start() {
			if (!bh)
				bh = FindObjectsOfType<BattleHandler>()[0];
		}

		// Update is called once per frame
		void Update() {}

		void choose_player() {
			PartyMember[] party = bh.party;

			PartyMember t = party[0];

			int n = Random.Range(1, 8);
			int originalN = n;

			if (n >= 1 && n <= 4) {
				t = party[0];
				target = party[0].gameObject;
			}
			if (n == 5 || n == 6) {
				t = party[1];
				target = party[1].gameObject;
			}
			if (n == 7) {
				t = party[2];
				target = party[2].gameObject;
			}
			if (n == 8) {
				t = party[3];
				target = party[3].gameObject;
			}

			while (t.hp <= 0) {
				n++;

				if (n >= 1 && n <= 4) {
					t = party[0];
					target = party[0].gameObject;
				}
				if (n == 5 || n == 6) {
					t = party[1];
					target = party[1].gameObject;
				}
				if (n == 7) {
					t = party[2];
					target = party[2].gameObject;
				}
				if (n == 8) {
					t = party[3];
					target = party[3].gameObject;
				}

				if (n == originalN) {
					target = null;
					break;
				}

				if (n > 8)
					n = 0;
			}
		}

		public void Turn() {
			List<string> actions = new List<string>();

			actions.Add("fight");

			if (spells.Length > 0)
				actions.Add("magic");

			if (specials.Length > 0)
				actions.Add("special");

			action = actions[Random.Range(0, actions.Count)];

			PartyMember leader = bh.party[0];
			for (int i = 0; i < bh.party.Length; i++) {
				if (bh.party[i].hp > 0) {
					leader = bh.party[i];
					break;
				}
			}

			if (morale - 2 * leader.level + Random.Range(0, 50) < 80)
				action = "run";

			if (action == "fight")
				choose_player();
		}
	}
}
