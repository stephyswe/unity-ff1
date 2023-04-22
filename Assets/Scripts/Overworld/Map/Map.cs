using Overworld.Controller;
using UnityEngine;
using UnityEngine.Serialization;

namespace Overworld {
	public class Map : MonoBehaviour {

		[FormerlySerializedAs("entry_position")]
		public Transform entryPosition;

		[FormerlySerializedAs("travel_mode")] public string travelMode;

		[FormerlySerializedAs("use_stairs")] public bool useStairs;

		[FormerlySerializedAs("stair_entry_position")]
		public Transform stairEntryPosition;

		[FormerlySerializedAs("suppress_map_just_changed")]
		public bool suppressMapJustChanged;

		public bool encounters;

		[FormerlySerializedAs("play_music")] public bool playMusic;

		MusicHandler a_s;

		PlayerController p;

		void Awake() {
			// Get the player controller
			p = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerController>();

			// If the map is active, play the music
			if (suppressMapJustChanged)
				p.mapJustChanged = false;

			// Get the music handler
			a_s = GetComponentInChildren<MusicHandler>();
		}

		// Start is called before the first frame update
		void Start() {}

		// Update is called once per frame
		void Update() {
			// If the map is active, play the music
			AudioSource active = a_s.get_active();
			if (!active.isPlaying && playMusic) {
				active.enabled = true;
				active.volume = 1f;
				active.Play();
			}

			// If the map is not active, stop the music
			if (suppressMapJustChanged)
				p.mapJustChanged = false;
		}
	}
}
