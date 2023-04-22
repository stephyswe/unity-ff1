using System.Collections;
using Overworld.Controller;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.BattleTransitions;

namespace Overworld {
	public class BridgeTrigger : MonoBehaviour {

		public ScreenTransition st;
		public MusicHandler mh;
		public PlayerController p;

		// Start is called before the first frame update
		void Start() {
			GetComponent<SpriteRenderer>().enabled = false;
		}

		// Update is called once per frame
		void Update() {}

		void OnTriggerEnter2D(Collider2D collision) {
			StartCoroutine(Loadbridge());
		}

		IEnumerator Loadbridge() {
			p.canMove = false;
			Transform t = p.movePoint;
			while (t.position.y > p.transform.position.y)
				yield return null;
			Destroy(p);
			mh.gameObject.SetActive(false);
			st.Transition();
			while (st.filling)
				yield return null;
			SceneManager.LoadSceneAsync("BridgeTitle");
		}
	}
}
