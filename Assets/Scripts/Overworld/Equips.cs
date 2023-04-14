using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.SaveGame.Scripts.SaveSystem;

namespace Overworld {
	public class Equips {

		static List<Armor> _armor;
		static List<Weapon> _weapons;
		static List<Item> _items;
		static List<Spell> _spells;

		public Equips() {
			_armor = new List<Armor>();
			_weapons = new List<Weapon>();
			_items = new List<Item>();
			_spells = new List<Spell>();

			setup_armor();
			setup_weapons();
			setup_items();
			setup_spells();
		}

		void setup_armor() {
			_armor.Add(new Armor("Cloth", "armor", 1, "", .2f, new List<string>(), new[] {"fighter", "knight", "thief", "ninja", "black_belt", "master", "black_mage", "black_wizard", "red_mage", "red_wizard", "white_mage", "white_wizard"}, 10));
			_armor.Add(new Armor("Cap", "helmet", 1, "", .1f, new List<string>(), new[] {"fighter", "knight", "thief", "ninja", "black_belt", "master", "black_mage", "black_wizard", "red_mage", "red_wizard", "white_mage", "white_wizard"}, 80));
			_armor.Add(new Armor("Wooden Armor", "armor", 4, "", .8f, new List<string>(), new[] {"fighter", "knight", "thief", "ninja", "black_belt", "master", "red_mage", "red_wizard"}, 50));
			_armor.Add(new Armor("Chain Armor", "armor", 15, "", .15f, new List<string>(), new[] {"fighter", "knight", "ninja", "red_mage", "red_wizard"}, 80));
			_armor.Add(new Armor("Iron Armor", "armor", 24, "", .23f, new List<string>(), new[] {"fighter", "knight", "ninja"}, 800));
		}

		void setup_weapons() {
			string[] equipNunchuck = {"black_belt", "ninja", "master"};
			string[] equipDagger = {"fighter", "knight", "thief", "ninja", "black_mage", "black_wizard", "red_mage", "red_wizard"};
			string[] equipStaff = {"fighter", "knight", "ninja", "black_belt", "master", "black_mage", "black_wizard", "red_mage", "red_wizard", "white_mage", "white_wizard"};
			string[] equipSpecialSword = {"fighter", "knight", "thief", "ninja", "red_mage", "red_wizard"};
			string[] equipHammer = {"fighter", "knight", "ninja", "white_mage", "white_wizard"};

			_weapons.Add(new Weapon("Wooden Nunchuck", 12, 0f, .01f, "", "", equipNunchuck, 10));
			_weapons.Add(new Weapon("Small Dagger", 5, .1f, .015f, "", "", equipDagger, 5));
			_weapons.Add(new Weapon("Wooden Staff", 6, 0f, .02f, "", "", equipStaff, 5));
			_weapons.Add(new Weapon("Rapier", 9, .05f, .052f, "", "", equipSpecialSword, 10));
			_weapons.Add(new Weapon("Iron Hammer", 9, 0f, .03f, "", "", equipHammer, 10));
		}

		void setup_items() {
			_items.Add(new Item("Potion", 60, false, true, true));
			_items.Add(new Item("Antidote", 75, false, true, true));
			_items.Add(new Item("Tent", 75, false, false, false));
			_items.Add(new Item("Lute", 0, true, false, false));
		}

		void setup_spells() {
			string[] blackBroad = {"black_mage", "black_wizard", "ninja", "red_mage", "red_wizard"};
			string[] whiteBroadSmall = {"white_mage", "white_wizard", "red_wizard", "knight"};
			string[] whiteBroadLarge = {"white_mage", "white_wizard", "red_mage", "red_wizard", "knight"};
			string[] whiteOnly = {"white_mage", "white_wizard"};

			_spells.Add(new Spell("LIT", "black", 4, 1, 20, .96f, false, false, false, "", "lightning", 100, blackBroad));
			_spells.Add(new Spell("FIRE", "black", 3, 1, 20, .96f, false, false, false, "", "fire", 100, blackBroad));
			_spells.Add(new Spell("LOCK", "black", 8, 1, 0, 1.15f, false, false, false, "", "", 100, blackBroad));
			_spells.Add(new Spell("SLEP", "black", 5, 1, 0, .96f, true, false, false, "Sleep", "status", 100, blackBroad));

			_spells.Add(new Spell("RUSE", "white", 3, 1, 0, -1f, false, false, true, "", "", 100, whiteBroadSmall));
			_spells.Add(new Spell("CURE", "white", 5, 1, 0, -1f, false, true, false, "", "", 100, whiteBroadLarge));
			_spells.Add(new Spell("HARM", "white", 8, 1, 40, .96f, true, false, false, "", "", 100, whiteOnly));
			_spells.Add(new Spell("FOG", "white", 5, 1, 0, -1f, false, false, true, "", "", 100, whiteBroadLarge));
		}

		public void communal_to_personal(string type, string equipName, int pIndex) {
			List<string> typeInventory = SaveSystem.GetStringList(type);
			typeInventory.Remove(equipName);
			SaveSystem.SetStringList(type, typeInventory);

			List<string> pInventory = SaveSystem.GetStringList("player" + (pIndex + 1) + "_" + type + "_inventory");
			pInventory.Add(equipName);
			SaveSystem.SetStringList("player" + (pIndex + 1) + "_" + type + "_inventory", pInventory);
		}

		public Armor get_armor(string name) {
			foreach (Armor a in _armor) {
				if (a.Name == name)
					return a;
			}
			return null;
		}

		public Weapon get_weapon(string name) {
			foreach (Weapon w in _weapons) {
				if (w.Name == name)
					return w;
			}
			return null;
		}

		public Item get_item(string name) {
			foreach (Item i in _items) {
				if (i.Name == name)
					return i;
			}
			return null;
		}

		public Spell get_spell(string name) {
			foreach (Spell s in _spells) {
				if (s.Name == name)
					return s;
			}
			return null;
		}

		public KeyValuePair<string, int> name_price(string name) {
			foreach (Weapon w in _weapons) {
				if (w.Name == name)
					return new KeyValuePair<string, int>(w.Name, w.Cost);
			}
			foreach (Armor a in _armor) {
				if (a.Name == name)
					return new KeyValuePair<string, int>(a.Name, a.Cost);
			}
			foreach (Item i in _items) {
				if (i.Name == name)
					return new KeyValuePair<string, int>(i.Name, i.Cost);
			}
			foreach (Spell s in _spells) {
				if (s.Name == name)
					return new KeyValuePair<string, int>(s.Name, s.Cost);
			}
			return new KeyValuePair<string, int>("", 0);
		}

		public string item_category(string name) {
			foreach (Weapon w in _weapons) {
				if (w.Name == name)
					return "weapon";
			}
			foreach (Armor a in _armor) {
				if (a.Name == name)
					return "armor";
			}
			foreach (Item i in _items) {
				if (i.Name == name)
					return "item";
			}
			foreach (Spell s in _spells) {
				if (s.Name == name)
					return "spell";
			}
			return "idk";
		}

		public int sum_armor(int index) {
			string playerN = "player" + (index + 1) + "_";

			Armor shield = get_armor(SaveSystem.GetString(playerN + "shield"));
			Armor helmet = get_armor(SaveSystem.GetString(playerN + "helmet"));
			Armor arm = get_armor(SaveSystem.GetString(playerN + "armor"));
			Armor glove = get_armor(SaveSystem.GetString(playerN + "glove"));

			int total = 0;

			if (shield != null)
				total += shield.Absorb;
			if (helmet != null)
				total += helmet.Absorb;
			if (arm != null)
				total += arm.Absorb;
			if (glove != null)
				total += glove.Absorb;

			return total;
		}

		public bool can_equip_armor(Armor ar, string playerClass) {
			foreach (string c in ar.EquipBy) {
				if (c == playerClass)
					return true;
			}

			return false;
		}

		public bool can_equip_weapon(Weapon w, string playerClass) {
			foreach (string c in w.EquipBy) {
				if (c == playerClass)
					return true;
			}

			return false;
		}

		public bool use_item(string name, int i) {
			bool success = true;

			switch (name) {
				case "Potion":
					SaveSystem.SetInt("player" + (i + 1) + "_HP", Mathf.Min(SaveSystem.GetInt("player" + (i + 1) + "_HP") + 30, SaveSystem.GetInt("player" + (i + 1) + "_maxHP")));
					break;
				case "Antidote":
					if (SaveSystem.GetBool("player" + (i + 1) + "poison"))
						SaveSystem.SetBool("player" + (i + 1) + "poison", false);
					else
						success = false;
					break;
				case "Tent":

					if (GlobalControl.Instance.mh.activeMap.name != "Overworld") {
						success = false;
						break;
					}

					for (int n = 0; n < 4; n++)
						SaveSystem.SetInt("player" + (n + 1) + "_HP", Mathf.Min(SaveSystem.GetInt("player" + (n + 1) + "_HP") + 30, SaveSystem.GetInt("player" + (n + 1) + "_maxHP")));

					Dictionary<string, int> partyItems = SaveSystem.GetStringIntDict("items");
					int count = partyItems["Tent"];
					if (count == 1)
						partyItems.Remove("Tent");
					else
						partyItems["Tent"] = partyItems["Tent"] - 1;
					SaveSystem.SetStringIntDict("items", partyItems);

					GlobalControl.Instance.mh.save_position();

					SaveSystem.SaveToDisk();
					SceneManager.LoadScene("Title Screen");

					break;
				case "Lute":
					success = false;
					break;
			}

			return success;
		}

		public class Armor {
			public int Absorb;
			public string Category;
			public int Cost;
			public string[] EquipBy;
			public float EvadeCost;
			public string Name;
			public List<string> Resistances;
			public string Spell;

			public Armor(string n, string cat, int d, string s, float e, List<string> r, string[] eq, int c) {
				Name = n;
				Category = cat;
				Absorb = d;
				Spell = s;
				EvadeCost = e;
				Resistances = r;
				EquipBy = eq;
				Cost = c;
			}
		}

		public class Weapon {
			public int Cost;
			public float Crit;
			public int Damage;
			public string Element;
			public string[] EquipBy;
			public float Hit;
			public string Name;
			public string Spell;

			public Weapon(string n, int d, float h, float cr, string s, string el, string[] eq, int c) {
				Name = n;
				Damage = d;
				Hit = h;
				Crit = c;
				Spell = s;
				Element = el;
				EquipBy = eq;
				Cost = c;
			}
		}

		public class Item {
			public int Cost;
			public bool IsDrink;
			public bool KeyItem;
			public string Name;
			public bool SingleUse;

			public Item(string n, int c, bool k, bool s, bool i) {
				Name = n;
				Cost = c;
				KeyItem = k;
				SingleUse = s;
				IsDrink = i;
			}
		}

		public class Spell {
			public int Cost;
			public string Element;
			public float Hit;
			public bool HitAllies;
			public string[] LearnBy;
			public int Level;
			public int Mp;
			public bool MultiTarget;
			public string Name;
			public int Power;
			public bool Self;
			public string Status;
			public string Type;

			public Spell(string n, string t, int mP, int l, int p, float h, bool m, bool ha, bool sf, string s, string e, int cst, string[] lb) {
				Name = n;
				Type = t;
				Level = l;
				Mp = mP;
				Power = p;
				Hit = h;
				MultiTarget = m;
				HitAllies = ha;
				Self = sf;
				Status = s;
				Element = e;
				Cost = cst;
				LearnBy = lb;
			}
		}
	}
}
