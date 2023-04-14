using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battling {
	public class MagicWeaponSpriteHandler : MonoBehaviour {

		public Weapon[] weapons;
		public Spell[] spells;

		public Sprite get_weapon(string name) {
			return (from w in weapons where w.name == name select w.sprite).FirstOrDefault();
		}

		public KeyValuePair<Sprite, Sprite> get_spell(string name) {
			foreach (Spell s in spells) {
				if (s.name == name)
					return new KeyValuePair<Sprite, Sprite>(s.sprite1, s.sprite2);
			}
			return new KeyValuePair<Sprite, Sprite>(null, null);
		}

		[Serializable]
		public class Weapon {
			public string name;
			public Sprite sprite;
		}

		[Serializable]
		public class Spell {
			public string name;
			public Sprite sprite1;
			public Sprite sprite2;
		}
	}
}
