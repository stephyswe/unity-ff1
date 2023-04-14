using UnityEngine;
using UnityEngine.Serialization;

namespace Battling {
	public class WeaponSprite : MonoBehaviour {

		[FormerlySerializedAs("weapon_sprite")]
		public Sprite weaponSprite;
		public SpriteRenderer sr;
		public Transform forward;
		public Transform back;
		public bool display;

		// Start is called before the first frame update
		void Start() {
			sr.enabled = false;
			display = false;
		}

		public void set_sprite(Sprite n) {
			weaponSprite = n;
			sr.sprite = n;
		}

		public void Show() {
			display = true;
			sr.enabled = true;
		}

		public void Hide() {
			display = false;
			sr.enabled = false;
		}

		public void go_forward() {
			sr.gameObject.transform.position = forward.position;
			sr.gameObject.transform.rotation = forward.rotation;
		}

		public void go_back() {
			sr.gameObject.transform.position = back.position;
			sr.gameObject.transform.rotation = back.rotation;
		}
	}
}
