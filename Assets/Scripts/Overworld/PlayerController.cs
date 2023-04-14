using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Utils.SaveGame.Scripts.SaveSystem;

namespace Overworld {
	public class PlayerController : MonoBehaviour {

		[FormerlySerializedAs("move_speed")] public float moveSpeed;
		[FormerlySerializedAs("move_point")] public Transform movePoint;
		[FormerlySerializedAs("can_move")] public bool canMove;

		[FormerlySerializedAs("travel_mode")] public string travelMode;

		public LayerMask collision;
		public LayerMask water;
		public LayerMask river;
		[FormerlySerializedAs("NPC")] public LayerMask npc;

		[FormerlySerializedAs("pause_menu_container")]
		public GameObject pauseMenuContainer;

		public SpriteController sc;

		[FormerlySerializedAs("map_handler")] public MapHandler mapHandler;

		[FormerlySerializedAs("map_just_changed")]
		public bool mapJustChanged;

		[FormerlySerializedAs("warp_sound")] public AudioSource warpSound;

		public RandomEncounterHandler reh;

		public OverworldGrid og;

		[FormerlySerializedAs("frames_since_last_interact")]
		public int framesSinceLastInteract;

		public float multiplier = 2f;

		float timer = -1;
		List<float> times;

		// Start is called before the first frame update
		void Start() {

			movePoint.parent = transform.parent;
			movePoint.transform.position = transform.position;
			canMove = true;
			mapJustChanged = false;

			framesSinceLastInteract = 0;

			reh.gameObject.SetActive(true);
			reh.seed = SaveSystem.GetInt("reh_seed");
		}

		// Update is called once per frame
		void Update() {

			if (reh.gameObject.activeSelf == false)
				reh.gameObject.SetActive(true);

			//Movement
			Transform transform1 = transform;
			transform1.rotation = Quaternion.identity;

			transform.position = Vector3.MoveTowards(transform1.position, movePoint.position, moveSpeed * Time.deltaTime);

			if (canMove) {
				if (Vector3.Distance(transform.position, movePoint.position) <= .025f) {

					bool up = Input.GetKey(CustomInputManager.Cim.Up);
					bool down = Input.GetKey(CustomInputManager.Cim.Down);
					bool left = Input.GetKey(CustomInputManager.Cim.Left);
					bool right = Input.GetKey(CustomInputManager.Cim.Right);

					float ver = 0f;
					float hor = 0f;

					if (up)
						ver = 1f;
					else if (down)
						ver = -1f;
					else if (left)
						hor = -1f;
					else if (right)
						hor = 1f;

					hor *= multiplier;
					ver *= multiplier;

					if (!up && !down && !left && !right && transform.position == movePoint.position) {
						switch (sc.get_direction()) {
							case "up":
								sc.change_direction("up");
								break;
							case "down":
								sc.change_direction("down");
								break;
							case "left":
								sc.change_direction("left");
								break;
							case "right":
								sc.change_direction("right");
								break;
						}
					}

					else if (Mathf.Abs(hor) == 1f * multiplier) {
						RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector3(hor, 0f, 0f), 1f * multiplier, collision | water | river);
						if (hor == 1f * multiplier)
							sc.change_direction("right");
						if (hor == -1f * multiplier)
							sc.change_direction("left");
						if (hit.collider == null || hit.collider.gameObject.layer == 0 || hit.collider.isTrigger) {
							movePoint.position += new Vector3(hor, 0f, 0f);
							StartCoroutine(reh_decrement());
							StartCoroutine(sc.Walk());
						}
					}

					else if (Mathf.Abs(ver) == 1f * multiplier) {
						RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector3(0f, ver, 0f), 1f * multiplier, collision | water | river);
						if (ver == 1f * multiplier)
							sc.change_direction("up");
						if (ver == -1f * multiplier)
							sc.change_direction("down");
						if (hit.collider == null || hit.collider.gameObject.layer == 0) {
							movePoint.position += new Vector3(0f, ver, 0f);
							StartCoroutine(reh_decrement());
							StartCoroutine(sc.Walk());
						}
					}
				}
			}

			//Interaction
			if (Input.GetKey(CustomInputManager.Cim.Select) && canMove && framesSinceLastInteract > 30) {

				Vector3 direction = get_direction_facing();

				RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1.5f, npc);
				if (hit.collider) {
					GameObject obj = hit.collider.gameObject;
					Npc interNpc = obj.GetComponent<Npc>();
					Chest interChest = obj.GetComponent<Chest>();
					LockedDoor lockedDoor = obj.GetComponent<LockedDoor>();
					Boss interBoss = obj.GetComponent<Boss>();

					if (interNpc) {
						if (interNpc.movePoint.position == interNpc.transform.position)
							StartCoroutine(interNpc.Interact(this));
					}
					else if (interChest)
						StartCoroutine(interChest.Interact(this));
					else if (lockedDoor)
						StartCoroutine(lockedDoor.Interact(this));
					else if (interBoss)
						StartCoroutine(interBoss.Interact(this));
				}
				else
					canMove = true;
			}

			framesSinceLastInteract += 1;
		}

		void OnEnable() {
			Cursor.visible = false;
			if (canMove)
				pauseMenuContainer.SetActive(true);
		}

		void OnTriggerEnter2D(Collider2D c) {

			if (transform.position == movePoint.position)
				return;
			if (c.gameObject.GetComponent<RoomHandler>())
				c.gameObject.GetComponent<RoomHandler>().Change();
			else if (c.gameObject.GetComponent<TilemapRenderer>() && !mapJustChanged) {
				reh.gameObject.SetActive(false);
				GameObject map = c.gameObject.GetComponent<WarpTiles>().warpTo;
				reh.set_encounters(map.GetComponent<Map>().encounters);
				StartCoroutine(change_map(map));
			}
			else if (c.gameObject.GetComponent<OverworldGrid>()) {
				og = c.gameObject.GetComponent<OverworldGrid>();
				//SaveSystem.SetString("player_og", og.gameObject.name);
			}
			else if (c.gameObject.GetComponent<ShopWarp>() && movePoint.position != transform.position) {
				mapJustChanged = true;
				StartCoroutine(shop_warp(c.gameObject.GetComponent<ShopWarp>()));
			}
			else if (mapHandler.doneChanging)
				canMove = true;
		}

		void OnTriggerExit2D(Collider2D c) {
			mapJustChanged = false;
			reh.gameObject.SetActive(true);
			if (mapHandler.doneChanging)
				canMove = true;
		}

		void StartTimer() {
			if (times == null)
				times = new List<float>();
			timer = 0;
		}

		void StopTimer() {
			Debug.Log(timer);
			times.Add(timer);

			float total = 0f;
			foreach (float f in times)
				total += f;
			Debug.Log("Average: " + total / times.Count);

			timer = -1f;
		}

		public Vector3 get_direction_facing() {
			Vector3 direction = Vector3.zero;
			if (sc.get_direction() == "up")
				direction = new Vector3(0f, 1f, 0f);
			else if (sc.get_direction() == "down")
				direction = new Vector3(0f, -1f, 0f);
			else if (sc.get_direction() == "left")
				direction = new Vector3(-1f, 0f, 0f);
			else if (sc.get_direction() == "right")
				direction = new Vector3(1f, 0f, 0f);
			return direction;
		}

		IEnumerator reh_decrement() {

			yield return new WaitForSeconds(.2f);
			switch (travelMode) {
				case "walking":
					reh.Decrement(6);
					break;
				case "walking_dungeon":
					reh.Decrement(5);
					break;
				case "sailing":
					reh.Decrement(2);
					break;
				case "none":
					break;
			}
		}

		IEnumerator shop_warp(ShopWarp warp) {
			canMove = false;

			while (transform.position != movePoint.position)
				yield return null;

			StartCoroutine(warp.Warp());

			while (warp.shopping)
				yield return null;

			canMove = true;
		}

		IEnumerator change_map(GameObject map) {
			canMove = false;

			pauseMenuContainer.SetActive(false);

			while (transform.position != movePoint.position)
				yield return null;

			mapHandler.change_maps(map);

			while (!mapHandler.doneChanging || mapHandler.st.unfilling)
				yield return null;



			travelMode = mapHandler.activeMap.GetComponent<Map>().travelMode;

			canMove = true;

			pauseMenuContainer.SetActive(true);
		}
	}
}
