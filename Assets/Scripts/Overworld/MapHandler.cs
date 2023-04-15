using System.Collections;
using Overworld.Controller;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.BattleTransitions;
using Utils.SaveGame.Scripts.SaveSystem;

namespace Overworld {
	public class MapHandler : MonoBehaviour {

		public PlayerController player;
		public ScreenTransition st;
		public GameObject grids;
		[FormerlySerializedAs("active_map")] public GameObject activeMap;

		public float overworldX;
		public float overworldY;

		[FormerlySerializedAs("done_changing")]
		public bool doneChanging;

		float submapX;
		float submapY;

		// Start is called before the first frame update
		void Start() {
			overworldX = SaveSystem.GetFloat("overworldX");
			overworldY = SaveSystem.GetFloat("overworldY");

			submapX = SaveSystem.GetFloat("submapX");
			submapY = SaveSystem.GetFloat("submapY");

			if (SaveSystem.GetBool("in_submap")) {
				Map[] maps = grids.GetComponentsInChildren<Map>(true);
				foreach (Map m in maps) {
					if (m.gameObject.name == SaveSystem.GetString("submap_name")) {
						activeMap = m.gameObject;
						break;
					}
				}

				player.movePoint.position = new Vector3(submapX, submapY, 0f);
				player.gameObject.transform.position = new Vector3(submapX, submapY, 0f);
			}
			else {
				player.movePoint.position = new Vector3(overworldX, overworldY, 0f);
				player.gameObject.transform.position = new Vector3(overworldX, overworldY, 0f);
			}

			deactivate_maps_except(activeMap);
			doneChanging = true;
			activeMap.SetActive(true);
			activeMap.GetComponent<Map>().playMusic = true;

			player.reh.encounters = activeMap.GetComponent<Map>().encounters;
		}

		// Update is called once per frame
		void Update() {
			if (activeMap.name == "Overworld") {
				overworldX = player.transform.position.x;
				overworldY = player.transform.position.y;
				if (!activeMap.GetComponent<Map>().playMusic && doneChanging)
					activeMap.GetComponent<Map>().playMusic = true;
			}
			else {
				submapX = player.transform.position.x;
				submapY = player.transform.position.y;
			}
		}

		public void save_position() {
			SaveSystem.SetFloat("overworldX", overworldX);
			SaveSystem.SetFloat("overworldY", overworldY);
			SaveSystem.SetFloat("submapX", submapX);
			SaveSystem.SetFloat("submapY", submapY);

			if (activeMap.name == "Overworld") {
				SaveSystem.SetBool("in_submap", false);
				SaveSystem.SetBool("inside_of_room", false);
			}
			else {
				SaveSystem.SetBool("in_submap", true);
				SaveSystem.SetString("submap_name", activeMap.name);

				RoomHandler rh = activeMap.GetComponentInChildren<RoomHandler>();

				if (rh)
					SaveSystem.SetBool("inside_of_room", rh.rooms.activeSelf);
			}
		}

		public void save_inn() {
			SaveSystem.SetFloat("overworldX", overworldX);
			SaveSystem.SetFloat("overworldY", overworldY);

			SaveSystem.SetBool("in_submap", false);
			SaveSystem.SetBool("inside_of_room", false);
		}

		void deactivate_maps_except(GameObject map) {
			Map[] maps = grids.GetComponentsInChildren<Map>();
			foreach (Map m in maps) {
				if (m.enabled == false) {
					m.playMusic = false;
					continue;
				}
				if (m.gameObject.GetInstanceID() != map.GetInstanceID()) {
					m.playMusic = false;
					m.gameObject.SetActive(false);
				}
			}

			activeMap = map;
		}

		public void change_maps(GameObject map) {
			if (doneChanging)
				StartCoroutine(Change(map));
		}

		IEnumerator Change(GameObject map) {
			doneChanging = false;

			player.canMove = false;

			Map active = activeMap.GetComponent<Map>();
			active.playMusic = false;

			bool useStairs = active.useStairs;

			AudioSource amAs = activeMap.GetComponentInChildren<MusicHandler>().get_active();
			amAs.Stop();
			amAs.volume = 0f;

			player.warpSound.Play();

			st.Transition();
			while (st.filling)
				yield return null;

			deactivate_maps_except(map);
			active = activeMap.GetComponent<Map>();

			if (active.name != "Overworld") {
				if (useStairs) {
					player.transform.position = active.stairEntryPosition.position;
					player.sc.change_direction("down");
				}
				else {
					player.transform.position = active.entryPosition.position;
					player.sc.change_direction("up");
				}
			}
			else {
				player.transform.position = new Vector3(overworldX, overworldY, 0f);
				player.sc.change_direction("down");
			}

			player.movePoint.position = player.transform.position;
			player.mapJustChanged = true;

			activeMap.SetActive(true);

			st.unfilling = true;

			while (player.warpSound.isPlaying || st.unfilling)
				yield return null;

			active.playMusic = true;

			player.canMove = true;

			doneChanging = true;
		}

		GameObject get_map(string mapName) {
			Transform[] children = grids.transform.GetComponentsInChildren<Transform>();
			foreach (Transform child in children) {
				if (child.GetComponent<Map>() && child.name == mapName)
					return child.gameObject;
			}
			return null;
		}
	}
}
