using System;
using System.Collections;
using System.Collections.Generic;
using Overworld.Controller;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.SaveGame.Scripts.SaveSystem;
using Random = UnityEngine.Random;

namespace Overworld {
	public class Npc : Interactable {

		[FormerlySerializedAs("move_point")] public Transform movePoint;

		public FlagCheck[] flags;

		[FormerlySerializedAs("give_item")] public bool giveItem;
		[FormerlySerializedAs("item_to_give")] public string itemToGive;

		[FormerlySerializedAs("map_to_teleport_to")]
		public GameObject mapToTeleportTo;
		public Vector3 overworldPosTeleport;

		public LayerMask collision;
		public LayerMask water;
		public LayerMask river;

		[FormerlySerializedAs("move_speed")] public float moveSpeed;

		public SpriteController sc;

		[FormerlySerializedAs("immobile_npc")] public bool immobileNpc;
		bool can_move;

		SpriteRenderer child_sr;

		BoxCollider2D cld;
		int frame_count;

		int frames_until_move;

		bool interacting;

		float last_direction;

		int move_away_frames;

		void Start() {
			foreach (FlagCheck fc in flags) {
				if (!fc.Check())
					gameObject.SetActive(false);
			}
			movePoint.parent = transform.parent;

			cld = GetComponent<BoxCollider2D>();

			frames_until_move = Random.Range(30, 300);
			frame_count = 0;
			last_direction = 1f;

			child_sr = GetComponent<SpriteRenderer>();

			can_move = true;
		}

		// Update is called once per frame
		void Update() {
			frame_count += 1;
			move_away_frames += 1;

			float multiplier = 2f;
			float hor = 0f;
			float ver = 0f;

			if (immobileNpc) {
				if (!sc.walkAnimation && can_move && !interacting)
					StartCoroutine(sc.Walk());
			}

			if (frame_count >= frames_until_move && can_move && !immobileNpc) {
				if (!is_player_within_radius(5f)) {
					float direction = Random.Range(0, 3);

					float continueFromLastDirection = Random.Range(0, 3);
					if (continueFromLastDirection == 1f)
						direction = last_direction;

					Vector3 dirvec = new Vector3(0, 0, 0);

					switch (direction) {
						case 0:
							ver = 1f;
							dirvec = new Vector3(0, 1, 0);
							break;
						case 1:
							ver = -1f;
							dirvec = new Vector3(0, -1, 0);
							break;
						case 2:
							hor = 1f;
							dirvec = new Vector3(1, 0, 0);
							break;
						case 3:
							hor = -1f;
							dirvec = new Vector3(-1, 0, 0);
							break;
					}

					while (is_layer_in_direction("Warp", dirvec) && is_layer_in_direction("Npc", dirvec)) {
						switch (direction) {
							case 0:
								ver = 1f;
								dirvec = new Vector3(0, 1, 0);
								break;
							case 1:
								ver = -1f;
								dirvec = new Vector3(0, -1, 0);
								break;
							case 2:
								hor = 1f;
								dirvec = new Vector3(1, 0, 0);
								break;
							case 3:
								hor = -1f;
								dirvec = new Vector3(-1, 0, 0);
								break;
						}
					}
					frame_count = 0;
					frames_until_move = Random.Range(120, 300);

					last_direction = direction;
				}
			}

			hor *= multiplier;
			ver *= multiplier;

			if (Mathf.Abs(hor) == multiplier) {
				RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector3(hor, 0f, 0f), multiplier, collision | water | river);
				if (hor == multiplier) {
					sc.change_direction("right");
					StartCoroutine(sc.Walk());
				}
				if (hor == -1f * multiplier) {
					sc.change_direction("left");
					StartCoroutine(sc.Walk());
				}
				if (hit.collider == null || hit.collider.gameObject.layer == 0)
					movePoint.position += new Vector3(hor, 0f, 0f);
				else
					last_direction = Random.Range(0, 3);
			}

			else if (Mathf.Abs(ver) == multiplier) {
				RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector3(0f, ver, 0f), 1f * multiplier, collision | water | river);
				if (ver == multiplier) {
					sc.change_direction("up");
					StartCoroutine(sc.Walk());
				}
				if (ver == -1f * multiplier) {
					//anim.SetTrigger("down");
					sc.change_direction("down");
					StartCoroutine(sc.Walk());
				}
				if (hit.collider == null || hit.collider.gameObject.layer == 0)
					movePoint.position += new Vector3(0f, ver, 0f);
				else
					last_direction = Random.Range(0, 3);
			}
			transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
		}

		void OnEnable() {
			if (immobileNpc) {
				can_move = true;
				interacting = false;
				sc.walkAnimation = false;
			}
			foreach (FlagCheck fc in flags) {
				if (!fc.Check())
					gameObject.SetActive(false);
			}
		}

		void OnCollisionEnter2D(Collision2D c) {
			GetComponent<BoxCollider2D>().enabled = false;
			can_move = false;
		}

		void OnCollisionExit2D(Collision2D c) {
			can_move = true;
		}

		public IEnumerator Interact(PlayerController p) {
			interacting = true;

			if (is_player_within_radius(2.5f) && transform.position == movePoint.position || immobileNpc) {
				can_move = false;
				p.canMove = false;

				Vector3 pPos = p.gameObject.transform.position;

				Vector3 location = new Vector3(pPos.x, pPos.y - 7.5f, pPos.z);

				string directionToLook = "";

				if (p.sc.get_direction() == "down")
					directionToLook = "up";
				else if (p.sc.get_direction() == "up")
					directionToLook = "down";
				else if (p.sc.get_direction() == "left")
					directionToLook = "right";
				else if (p.sc.get_direction() == "right")
					directionToLook = "left";

				sc.change_direction(directionToLook);
				StartCoroutine(look_in_direction(directionToLook));

				display_textbox(location);

				yield return new WaitForSeconds(.6f);
				while (!Input.GetKey(CustomInputManager.Cim.Select))
					yield return null;

				hide_textbox();

				if (giveItem && !SaveSystem.GetBool(gameObject.name + "_item_give_" + itemToGive)) {
					Dictionary<string, int> items = SaveSystem.GetStringIntDict("items");
					if (items.ContainsKey(itemToGive))
						items[itemToGive] = items[itemToGive] + 1;
					else
						items.Add(itemToGive, 1);
					SaveSystem.SetStringIntDict("items", items);

					SaveSystem.SetBool(gameObject.name + "_item_give_" + itemToGive, true);
				}

				if (gameObject.name == "Princess_TOF")
					princess_in_TOF(p);
				if (gameObject.name == "KingAfterRescue")
					KingAfterRescue();
				if (gameObject.name == "Princess_Castle")
					Princess_Castle();

				p.framesSinceLastInteract = 0;
			}
			interacting = false;
			if (immobileNpc) {
				can_move = true;
				sc.walkAnimation = false;
			}
			p.canMove = true;
			can_move = true;
		}

		void princess_in_TOF(PlayerController p) {
			SaveSystem.SetBool("princess_in_temple_of_fiends", true);
			p.mapHandler.overworldX = overworldPosTeleport.x;
			p.mapHandler.overworldY = overworldPosTeleport.y;
			StartCoroutine(change_map(mapToTeleportTo, p));
		}

		void KingAfterRescue() {
			SaveSystem.SetBool("king_mentioned_bridge", true);
			flags[1].replacementNpc.SetActive(true);
			Destroy(gameObject);
		}

		void Princess_Castle() {
			SaveSystem.SetBool("princess_gave_lute", true);
			flags[1].replacementNpc.SetActive(true);
			Destroy(gameObject);
		}

		IEnumerator change_map(GameObject map, PlayerController p) {
			p.canMove = false;

			p.pauseMenuContainer.SetActive(false);

			while (p.transform.position != p.movePoint.position)
				yield return null;

			p.mapHandler.change_maps(map);

			while (!p.mapHandler.doneChanging)
				yield return null;

			p.travelMode = p.mapHandler.activeMap.GetComponent<Map>().travelMode;

			p.canMove = true;

			p.pauseMenuContainer.SetActive(true);
		}

		IEnumerator look_in_direction(string dir) {
			while (interacting) {
				switch (dir) {
					case "up":
						if (sc.sr.sprite != sc.activeCharacter.up1)
							sc.change_direction("up");
						break;
					case "down":
						if (sc.sr.sprite != sc.activeCharacter.down1)
							sc.change_direction("down");
						break;
					case "left":
						if (sc.sr.sprite != sc.activeCharacter.step1)
							sc.change_direction("left");
						break;
					case "right":
						if (sc.sr.sprite != sc.activeCharacter.step1)
							sc.change_direction("right");
						break;
				}
				yield return null;
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

		bool is_layer_in_direction(string layerName, Vector3 direction) {
			float radius = .1f;
			Collider2D[] layer = Physics2D.OverlapCircleAll(transform.position + direction, radius, 1 << LayerMask.NameToLayer(layerName));
			if (layerName == "NPC")
				return layer.Length - 1 > 0;
			return layer.Length > 0;
		}
		// Start is called before the first frame update

		[Serializable]
		public class FlagCheck {
			public string name;
			public bool flagValToShow;
			[FormerlySerializedAs("replacementNPC")]
			public GameObject replacementNpc;

			public bool Check() {
				return SaveSystem.GetBool(name) == flagValToShow;
			}
		}
	}
}
