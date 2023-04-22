using UnityEngine;

namespace Overworld {
	public class NpcHandler : MonoBehaviour {
		// Start is called before the first frame update
		void OnEnable() {
			foreach (Transform c in transform)
				c.gameObject.SetActive(true);
		}
	}
}
