using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Overworld {
	public class OverworldGrid : MonoBehaviour {

		[FormerlySerializedAs("monster_encounters")]
		public MonsterEncounter[] monsterEncounters;

		// Start is called before the first frame update
		void Start() {}

		// Update is called once per frame
		void Update() {}

		public GameObject get_monster_party() {
			int index = Random.Range(0, 99);

			List<float> bounds = new List<float> {
				0f
			};

			float highest = 0f;

			for (int i = 1; i < monsterEncounters.Length; i++) {
				MonsterEncounter current = monsterEncounters[i - 1];
				MonsterEncounter next = monsterEncounters[i];

				float firstRate = current.encounterRate * 100f + highest;
				float secondRate = current.encounterRate * 100f + firstRate;
				highest = firstRate;

				if (index <= firstRate)
					return current.monsterParty;

				if (index >= firstRate && index <= secondRate)
					return next.monsterParty;

				highest += current.encounterRate;
			}

			Debug.Log("broken");
			return monsterEncounters[0].monsterParty;
		}

		[Serializable]
		public class MonsterEncounter {
			[FormerlySerializedAs("monster_party")]
			public GameObject monsterParty;
			[FormerlySerializedAs("encounter_rate")]
			public float encounterRate;
		}
	}
}
