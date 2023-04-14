using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Utils.SaveGame.Scripts.SaveSystem;

namespace Overworld {
	public class RoomHandler : MonoBehaviour {

		public GameObject rooms;
		[FormerlySerializedAs("outside_collision")]
		public GameObject outsideCollision;
		[FormerlySerializedAs("outside_NPCs")] public GameObject outsideNpCs;

		// Start is called before the first frame update
		void Start() {
			if (SaveSystem.GetBool("inside_of_room")) {
				Debug.Log(SaveSystem.GetBool("inside_of_room"));
				rooms.SetActive(true);
				outsideCollision.SetActive(false);
			}
			else {
				rooms.SetActive(false);
				outsideCollision.SetActive(true);
				outsideNpCs.SetActive(true);
			}
			GetComponent<TilemapRenderer>().enabled = false;
		}

		// Update is called once per frame
		void Update() {}

		public void Change() {
			rooms.SetActive(!rooms.activeSelf);
			outsideCollision.SetActive(!outsideCollision.activeSelf);
			outsideNpCs.SetActive(!outsideNpCs.activeSelf);
		}
	}
}
