using UnityEngine;
using UnityEngine.Serialization;

namespace Battling {
	public class MagicSprite : MonoBehaviour {

		[FormerlySerializedAs("magic_sprite")] public Sprite magicSprite;
		public SpriteRenderer sr;
		public bool display;

		// Start is called before the first frame update
		void Start() {
			sr.enabled = false;
			display = false;
		}

		public void set_sprite(Sprite n) {
			magicSprite = n;
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
	}
}
