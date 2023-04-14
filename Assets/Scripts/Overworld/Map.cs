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
			p = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerController>();
			if (suppressMapJustChanged)
				p.mapJustChanged = false;
			a_s = GetComponentInChildren<MusicHandler>();
		}

		// Start is called before the first frame update
		void Start() {}

		// Update is called once per frame
		void Update() {
			AudioSource active = a_s.get_active();
			if (!active.isPlaying && playMusic) {
				active.enabled = true;
				active.volume = 1f;
				active.Play();
			}

			if (suppressMapJustChanged)
				p.mapJustChanged = false;
		}
	}
}
