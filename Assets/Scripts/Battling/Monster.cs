using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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

		void ChoosePlayerTarget() {
			PartyMember[] party = bh.party;
			GameObject targetObject = FindValidTargetOptimal(party);
			target = targetObject;
		}

		// Find a valid target for monster
		static GameObject FindValidTargetOptimal(IReadOnlyList<PartyMember> party) {
			int randomIndex = Random.Range(0, 100);
			int maxIndex = party.Count;

			switch (randomIndex) {
				// Determine the target based on the random index
				case < 50 when party[0].hp > 0:
					return party[0].gameObject;
				case < 75 when party[1].hp > 0:
					return party[1].gameObject;
				case < 85 when party[2].hp > 0:
					return party[2].gameObject;
				default: {
					if (party[3].hp > 0) {
						return party[3].gameObject;
					}
					break;
				}
			}

			// If no valid target is found, target the next available party member
			for (int i = 0; i < maxIndex; i++) {
				int index = (randomIndex + i) % maxIndex;
				if (party[index].hp > 0) {
					return party[index].gameObject;
				}
			}
			throw new InvalidOperationException("FindValidTargetOptimal - No valid target found.");
		}


		public void Turn() {
			List<string> actions = new List<string> {
				"fight"
			};

			if (spells.Length > 0)
				actions.Add("magic");

			if (specials.Length > 0)
				actions.Add("special");

			action = actions[Random.Range(0, actions.Count)];

			PartyMember leader = bh.party[0];
			foreach (PartyMember t in bh.party) {
				if (t.hp <= 0)
					continue;
				leader = t;
				break;
			}

			// calculate morale
			float monsterMorale = morale - 2 * leader.level + Random.Range(0, 50);

			if (monsterMorale < 80)
				action = "run";

			if (action == "fight")
				ChoosePlayerTarget();
		}
	}
}
