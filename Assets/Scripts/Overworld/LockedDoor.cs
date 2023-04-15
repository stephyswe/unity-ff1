using System.Collections;
using System.Collections.Generic;
using Overworld.Controller;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.SaveGame.Scripts.SaveSystem;

namespace Overworld {
	public class LockedDoor : Interactable {

		[FormerlySerializedAs("ID")] public string id;

		public new BoxCollider2D collider;
		bool unlocked;

		void OnEnable() {
			unlocked = SaveSystem.GetBool("door_" + id);
		}

		// ReSharper disable Unity.PerformanceAnalysis
		
		// summmary //
		// This is the method that is called when the player interacts with the object
		public IEnumerator Interact(PlayerController p) {
			p.canMove = false;
			p.pauseMenuContainer.SetActive(false);

			Dictionary<string, int> items = SaveSystem.GetStringIntDict("items");

			Vector3 pPos = p.gameObject.transform.position;

			Vector3 location = new Vector3(pPos.x, pPos.y - 7.5f, pPos.z);

			if (items.ContainsKey("MYSTIC KEY") && !unlocked) {
				dialogue = "Unlocked the door with the MYSTIC KEY";
				Destroy(collider);
				unlocked = true;

				SaveSystem.SetBool("door_" + id, true);
			}
			else if (!items.ContainsKey("MYSTIC KEY") && !unlocked)
				dialogue = "This door is locked";

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
