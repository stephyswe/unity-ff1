using System.Collections;
using UnityEngine;
using Utils.SaveGame.Scripts.SaveSystem;

namespace Overworld {
	public class Boss : Interactable {

		public GameObject boss;

		public Transform cam;

		public string flag;
		public bool flagval;

		public RandomEncounterHandler reh;

		public bool interacting;

		SpriteController sc;

		// Update is called once per frame
		void Update() {
			if (!sc.walkAnimation && !interacting)
				StartCoroutine(sc.Walk());
		}

		// Start is called before the first frame update
		void OnEnable() {
			if (SaveSystem.GetBool(flag) == flagval)
				Destroy(gameObject);
			sc = GetComponentInChildren<SpriteController>();
		}

		public IEnumerator Interact(PlayerController p) {
			interacting = true;

			if (is_player_within_radius(2.5f)) {
				p.canMove = false;

				Vector3 pPos = p.gameObject.transform.position;

				Vector3 location = new Vector3(pPos.x, pPos.y - 7.5f, pPos.z);

				display_textbox(location);

				yield return new WaitForSeconds(.6f);
				while (!Input.GetKey(CustomInputManager.Cim.Select))
					yield return null;

				hide_textbox();

				GlobalControl.Instance.boss = this;

				if (!reh.battling) {
					yield return StartCoroutine(reh.start_boss_battle(p, boss, flag, flagval, gameObject));

					interacting = false;

					p.framesSinceLastInteract = 0;
				}
			}

			yield return null;
		}

		bool is_player_within_radius(float radius) {
			Collider2D[] player = Physics2D.OverlapCircleAll(transform.position, radius, 1 << LayerMask.NameToLayer("Player"));
			/*
		Collider2D[] NPCs = Physics2D.OverlapCircleAll(transform.position, radius, 1 << LayerMask.NameToLayer("NPC"));
		Collider2D[] warps = Physics2D.OverlapCircleAll(transform.position, radius, 1 << LayerMask.NameToLayer("Warp"));
		return player.Length + (NPCs.Length - 1) + warps.Length > 0;
		*/
			return player.Length > 0;
		}
	}
}
