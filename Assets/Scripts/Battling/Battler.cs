using System;
using System.Collections.Generic;
using Overworld;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.SaveGame.Scripts.SaveSystem;
using Random = UnityEngine.Random;

namespace Battling {
	public class Battler : MonoBehaviour {
		[FormerlySerializedAs("HP")] public int hp;
		public float hit;
		[FormerlySerializedAs("magic_defense")]
		public float magicDefense;
		public List<string> conditions;

		Equips equip;

		// Start is called before the first frame update
		void Start() {
			equip = new Equips();
		}

		// Update is called once per frame
		void Update() {}

		public int Fight(Monster attack, PartyMember defend) {

			if (equip == null)
				equip = new Equips();

			string[] conditionsArray = conditions.ToArray();

			float damageRating = Random.Range(attack.damageLow, (float)attack.damageHigh);

			int absorbRating = 0;
			if (defend.job == "black_belt" || defend.job == "master")
				absorbRating = defend.level;
			else
				absorbRating = equip.sum_armor(defend.index);

			float damage = 0f;

			if ((int)Random.Range(0f, 100f) <= (int)(100f * attack.critPercent)) {
				Debug.Log("Critical hit");
				float range = Random.Range(damageRating, 2f * damageRating);
				damage = range + range - absorbRating;
			}
			else
				damage = Random.Range(damageRating, 2f * damageRating) - absorbRating;

			float chanceToHit = 168f + attack.hit - (48 + defend.agility);

			if (Array.Exists(conditionsArray, condition => condition == "blind"))
				chanceToHit -= 40f;
			if (Array.Exists(conditionsArray, condition => condition == "blind"))
				chanceToHit += 40f;

			if (damage < 0f)
				damage = 1f;

			if (Random.Range(0f, 200f) <= chanceToHit) {
				defend.hp -= (int)damage;

				if (defend.hp < 0)
					defend.hp = 0;

				return (int)damage;
			}
			return -1;
		}

		public int Fight(PartyMember attack, Monster defend) {

			if (equip == null)
				equip = new Equips();

			string[] conditionsArray = conditions.ToArray();

			float damageRating = 0;
			if (attack.job == "black_mage" || attack.job == "black_wizard")
				damageRating = attack.strength / 2 + 1f + weapon_damage(attack.weapon);
			else if (attack.job == "black_belt" || attack.job == "master") {
				if (weapon_damage(attack.weapon) == 0)
					damageRating = attack.level * 2;
				else
					damageRating = attack.strength / 2 + 1f + weapon_damage(attack.weapon);
			}
			else
				damageRating = attack.strength / 2 + (float)weapon_damage(attack.weapon);

			float damage = 0f;

			float chanceToHit = 168f + attack.hit - defend.evade;

			if (Array.Exists(conditionsArray, condition => condition == "blind"))
				chanceToHit -= 40f;

			if (damage < 1f)
				damage = 1f;

			float critHitChance = Random.Range(0f, 200f);


			if (critHitChance <= chanceToHit) {

				bool crit = false;

				if (Random.Range(0f, 100f) <= weapon_crit(attack.weapon)) {
					float range = Random.Range(damageRating, 2f * damageRating);
					damage = range + range - defend.absorb;
					crit = true;
				}
				else
					damage = Random.Range(damageRating, 2f * damageRating) - defend.absorb;

				if (damage < 1f)
					damage = 1f;

				defend.hp -= (int)damage;

				if (defend.hp < 0)
					defend.hp = 0;

				if (crit)
					return -(int)damage;

				return (int)damage;
			}
			return -9999999;
		}

		int weapon_damage(string weapon) {
			if (weapon == "")
				return 0;
			return equip.get_weapon(weapon).Damage;
		}

		float weapon_crit(string weapon) {
			if (weapon == "")
				return 0f;
			return equip.get_weapon(weapon).Crit;
		}

		string weapon_type(PartyMember member) {
			return equip.get_weapon(SaveSystem.GetString("player" + (member.index + 1) + "_weapon")).Element;
		}
	}
}
