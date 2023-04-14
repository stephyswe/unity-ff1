using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Overworld {
	public class WarpTiles : MonoBehaviour {

		[FormerlySerializedAs("warp_to")] public GameObject warpTo;

		// Start is called before the first frame update
		void Start() {
			GetComponent<TilemapRenderer>().enabled = false;
		}

		// Update is called once per frame
		void Update() {}
	}
}
