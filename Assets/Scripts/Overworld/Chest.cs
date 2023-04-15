using System.Collections;
using System.Collections.Generic;
using Overworld.Controller;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.SaveGame.Scripts.SaveSystem;

namespace Overworld {
	public class Chest : Interactable {

		[FormerlySerializedAs("ID")] public string id;

		public bool gold;
		[FormerlySerializedAs("gold_val")] public int goldVal;

		public bool item;
		[FormerlySerializedAs("item_val")] public string itemVal;

		public bool weapon;
		[FormerlySerializedAs("weapon_val")] public string weaponVal;

		public bool armor;
		[FormerlySerializedAs("armor_val")] public string armorVal;
		bool obtained;

		// Update is called once per frame
		void Update() {}

		// Start is called before the first frame update
		void OnEnable() {
			obtained = SaveSystem.GetBool("chest_" + id);
		}

		public IEnumerator Interact(PlayerController p) {
			p.canMove = false;
			p.pauseMenuContainer.SetActive(false);

			Vector3 pPos = Vector3.zero;
			Vector3 location = Vector3.zero;

			if (!obtained) {
				pPos = p.gameObject.transform.position;

				location = new Vector3(pPos.x, pPos.y - 7.5f, pPos.z);

				if (gold) {
					dialogue = "Obtained " + goldVal + " G";
					SaveSystem.SetInt("gil", SaveSystem.GetInt("gil") + goldVal);
				}
				else if (item || weapon || armor) {
					dialogue = "Obtained " + itemVal;

					Dictionary<string, int> items = SaveSystem.GetStringIntDict("items");
					if (items.ContainsKey(itemVal))
						items[itemVal] = items[itemVal] + 1;
					else
						items.Add(itemVal, 1);

					SaveSystem.SetStringIntDict("items", items);
				}

				SaveSystem.SetBool("chest_" + id, true);
				obtained = true;
			}
			else {

				pPos = p.gameObject.transform.position;

				location = new Vector3(pPos.x, pPos.y - 7.5f, pPos.z);

				dialogue = "Nothing";
			}

			display_textbox(location);

			yield return new WaitForSeconds(.8f);
			while (!Input.GetKey(CustomInputManager.Cim.Select))
				yield return null;

			hide_textbox();

			p.canMove = true;
			p.pauseMenuContainer.SetActive(true);

			p.framesSinceLastInteract = 0;
		}
	}
}
